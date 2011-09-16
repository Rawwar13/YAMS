// From https://github.com/martindevans/SharpUPnP

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace SharpUPnP
{
    /// <summary>
    /// Provides Universal Plug & Play operations for NAT
    /// </summary>
    public class UPnP
    {
        private static long timeoutTicks = new TimeSpan(0, 0, 0, 3).Ticks;
        /// <summary>
        /// The default timeout to use for operations. Threadsafe for reading and writing
        /// </summary>
        public static TimeSpan TimeOut
        {
            get { return new TimeSpan(Interlocked.Read(ref timeoutTicks)); }
            set
            {
                Interlocked.Exchange(ref timeoutTicks, value.Ticks);
            }
        }

        private static string descUrl { get; set; }
        private static string serviceUrl { get; set; }
        private static string eventUrl { get; set; }

        private static object discoveryLock = new object();
        /// <summary>
        /// Gets or sets a value indicating whether an ettempy has been made to Discover UPnP services
        /// </summary>
        /// <value><c>true</c> if discovered; otherwise, <c>false</c>.</value>
        public static bool Discovered
        {
            get;
            private set;
        }

        private static bool uPnPAvailable = false;
        /// <summary>
        /// Gets or sets a value indicating whether any UPnP services are available.
        /// </summary>
        /// <value><c>true</c> if UPnP is available; False if UPnP is unavailable</value>
        public static bool UPnPAvailable
        {
            get
            {
                Discover(false);

                return uPnPAvailable;
            }
        }

        /// <summary>
        /// Discover if UPnP services are available
        /// </summary>
        /// <param name="rediscover">Indicates if discovery should be tried again if it has already been done</param>
        /// <returns>True, if UPnP servivesare available, otherwise false</returns>
        public static bool Discover(bool rediscover)
        {
            if (!Discovered || rediscover)
            {
                lock (discoveryLock)
                {
                    if (!Discovered || rediscover)
                        return Discover();
                }
            }

            return uPnPAvailable;
        }

        /// <summary>
        /// Discover available UPnP services available
        /// </summary>
        /// <returns>True, if UPnP servivesare available, otherwise false</returns>
        public static bool Discover()
        {
            lock (discoveryLock)
            {
                try
                {
                    NetworkInterface nic = NetworkInterface.GetAllNetworkInterfaces()[0];
                    GatewayIPAddressInformation gwInfo = nic.GetIPProperties().GatewayAddresses[0];

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(gwInfo.Address.ToString()), 1900);

                    client.SetSocketOption(SocketOptionLevel.Socket,
                        SocketOptionName.ReceiveTimeout, 5000);

                    string req = "M-SEARCH * HTTP/1.1\r\n" +
                    "HOST: " + gwInfo.Address.ToString() + ":1900\r\n" +
                    "ST:upnp:rootdevice\r\n" +
                    "MAN:\"ssdp:discover\"\r\n" +
                    "MX:3\r\n\r\n";

                    byte[] requestBytes = Encoding.ASCII.GetBytes(req);
                    client.SendTo(requestBytes, requestBytes.Length, SocketFlags.None, endPoint);
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint senderEP = (EndPoint)sender;

                    byte[] data = new byte[1024];
                    int recv = client.ReceiveFrom(data, ref senderEP);
                    string queryResponse = Encoding.ASCII.GetString(data, 0, recv);

                    string resp = queryResponse;
                    if (resp.Contains("upnp:rootdevice"))
                    {
                        resp = resp.Substring(resp.ToLower().IndexOf("location:") + 9);
                        resp = resp.Substring(0, resp.IndexOf("\r")).Trim();
                        if (!string.IsNullOrEmpty(serviceUrl = GetServiceUrl(resp)))
                        {
                            descUrl = resp;
                            uPnPAvailable = true;
                        }
                    }
                    else
                        uPnPAvailable = false;

                    Discovered = true;
                    return UPnPAvailable;
                }
                catch (SocketException e)
                {
                    Discovered = false;
                    uPnPAvailable = false;
                    return uPnPAvailable;
                }
            }
        }

        private static string GetServiceUrl(string resp)
        {
            XmlDocument desc = new XmlDocument();
            try
            {
                desc.Load(WebRequest.Create(resp).GetResponse().GetResponseStream());
            }
            catch (Exception)
            {
                return null;
            }
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(desc.NameTable);
            nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
            XmlNode typen = desc.SelectSingleNode("//tns:device/tns:deviceType/text()", nsMgr);
            if (!typen.Value.Contains("InternetGatewayDevice"))
                return null;
            XmlNode node = desc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", nsMgr);
            if (node == null)
                return null;
            XmlNode eventnode = desc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", nsMgr);
            eventUrl = CombineUrls(resp, eventnode.Value);
            return CombineUrls(resp, node.Value);
        }

        private static string CombineUrls(string resp, string p)
        {
            int n = resp.IndexOf("://");
            n = resp.IndexOf('/', n + 3);
            return resp.Substring(0, n) + p;
        }

        /// <summary>
        /// Forwards an external port to the same internal port
        /// </summary>
        /// <param name="port">The port to map (both external and internal)</param>
        /// <param name="protocol">The protocol type to map</param>
        /// <param name="description">The description of this mapping</param>
        public static void CreateForwardingRule(int port, ProtocolType protocol, string description)
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress addr = ipEntry.AddressList[0];

            XmlDocument xdoc = SOAPRequest(
                "<m:AddPortMapping xmlns:m=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"string\"></NewRemoteHost><NewExternalPort xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"ui2\">" +
                port.ToString() + "</NewExternalPort><NewProtocol xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"string\">" +
                protocol.ToString().ToUpper() + "</NewProtocol><NewInternalPort xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"ui2\">" +
                port.ToString() + "</NewInternalPort><NewInternalClient xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"string\">" +
                addr + "</NewInternalClient><NewEnabled xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"boolean\">1</NewEnabled><NewPortMappingDescription xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"string\">" +
                description + "</NewPortMappingDescription><NewLeaseDuration xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"ui4\">0</NewLeaseDuration></m:AddPortMapping>",
                "AddPortMapping");
        }

        /// <summary>
        /// Deletes the forwarding rule for the specific port
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="protocol">The protocol.</param>
        public static void DeleteForwardingRule(int port, ProtocolType protocol)
        {
            XmlDocument xdoc = SOAPRequest(
            "<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
            "<NewRemoteHost></NewRemoteHost>" +
            "<NewExternalPort>" + port.ToString() + "</NewExternalPort>" +
            "<NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
            "</u:DeletePortMapping>", "DeletePortMapping");
        }

        /// <summary>
        /// Gets the external IP of the router
        /// </summary>
        /// <returns>External IP address of the UPnP device</returns>
        /// <exception cref="UPnPException">Thrown if no UPnP service is available</exception>
        public static IPAddress GetExternalIP()
        {
            XmlDocument xdoc = SOAPRequest("<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"></u:GetExternalIPAddress>", "GetExternalIPAddress");
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
            nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
            string IP = xdoc.SelectSingleNode("//NewExternalIPAddress/text()", nsMgr).Value;
            return IPAddress.Parse(IP);
        }

        /// <summary>
        /// Checks that UPnP services are available
        /// </summary>
        /// <param name="getString">The get string.</param>
        /// <exception cref="UPnPException">Thrown if no UPnP service is available</exception>
        private static void CheckUPnPAvailable()
        {
            Discover(false);

            if (!UPnPAvailable)
                throw new UPnPException("No UPnP Service available");

            Debug.Assert(!string.IsNullOrEmpty(serviceUrl));
        }

        /// <summary>
        /// Sends a SOAP request to the UPnP device
        /// </summary>
        /// <param name="soap">The SOAP Xml request to be sent</param>
        /// <param name="function">The UPnP function to be performed</param>
        /// <returns>The response</returns>
        /// <exception cref="UPnPException">Thrown if no UPnP service is available</exception>
        public static XmlDocument SOAPRequest(string soap, string function)
        {
            CheckUPnPAvailable();

            string req = "<?xml version=\"1.0\"?>" +
            "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
            "<s:Body>" +
            soap +
            "</s:Body>" +
            "</s:Envelope>";
            WebRequest r = HttpWebRequest.Create(serviceUrl);
            r.Timeout = 10000;
            r.Method = "POST";
            byte[] b = Encoding.UTF8.GetBytes(req);
            r.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
            r.ContentType = "text/xml; charset=\"utf-8\"";
            r.ContentLength = b.Length;
            r.GetRequestStream().Write(b, 0, b.Length);
            XmlDocument resp = new XmlDocument();
            WebResponse wres = r.GetResponse();
            Stream ress = wres.GetResponseStream();
            resp.Load(ress);
            return resp;
        }
    }
}