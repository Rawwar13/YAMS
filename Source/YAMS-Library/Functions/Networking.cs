using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using NetFwTypeLib;
using YAMS;

namespace YAMS
{
    /// <summary>
    /// Static class that gathers together networking functions for firewall, UPnP and IP addresses.
    /// </summary>
    public static class Networking
    {

        /// <summary>
        /// Get the first IP address on the system
        /// </summary>
        /// <returns>IP Address</returns>
        public static IPAddress GetListenIP()
        {
            IPHostEntry ipListen = Dns.GetHostEntry(Dns.GetHostName());
            return ipListen.AddressList[0];
        }

        /// <summary>
        /// Grab the external IP address using icanhazip.com
        /// </summary>
        /// <returns>IPAddress</returns>
        public static IPAddress GetExternalIP()
        {
            string strExternalIPChecker = "http://icanhazip.com/";
            WebClient wcGetIP = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            string strResponse = "";
            try
            {
                strResponse = utf8.GetString(wcGetIP.DownloadData(strExternalIPChecker));
                strResponse = strResponse.Replace("\n", "");
            }
            catch (WebException e)
            {
                YAMS.Database.AddLog("Unable to determine external IP: " + e.Data, "utils", "warn");
            }

            IPAddress ipExternal = null;
            ipExternal = IPAddress.Parse(strResponse);
            return ipExternal;
        }

        /// <summary>
        /// Check if a port is open to the world using a port checker
        /// </summary>
        /// <param name="externalIP">IP addrress to probe</param>
        /// <param name="intPortNumber">Port number to check</param>
        /// <returns>boolean indicating if the port is accesible from the outside world</returns>
        public static bool CheckExternalPort(IPAddress externalIP, int intPortNumber)
        {
            string strUrl = "http://richardbenson.co.uk/yams/check-port.php?s=" + externalIP.ToString() + "&p=" + intPortNumber.ToString();
            WebClient wcCheckPort = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            string strResponse = "";
            try
            {
                strResponse = utf8.GetString(wcCheckPort.DownloadData(strUrl));
                switch (strResponse)
                {
                    case "open":
                        return true;
                    case "closed":
                        return false;
                    default:
                        return false;
                }
            }
            catch (WebException e)
            {
                YAMS.Database.AddLog("Unable to check open port: " + e.Data, "utils", "warn");
                return false;
            }

        }

        /// <summary>
        /// Open a port on the Windows firewall
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <param name="strFriendlyName">Friendly name for the service</param>
        /// <returns>Boolean indicating whether the rule worked</returns>
        public static bool OpenFirewallPort(int intPortNumber, string strFriendlyName)
        {
            try
            {
                INetFwOpenPorts portList;
                Type TportClass = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
                INetFwOpenPort port = (INetFwOpenPort)Activator.CreateInstance(TportClass);
                port.Name = "[YAMS] " + strFriendlyName;
                port.Port = intPortNumber;
                port.Enabled = true;

                Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);
                portList = (INetFwOpenPorts)mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts;

                portList.Add(port);
                Database.AddLog("Opened port " + intPortNumber + " for " + strFriendlyName, "networking");
                return true;
            }
            catch(Exception e) {
                Database.AddLog("Unable to open firewall port " + intPortNumber + " for " + strFriendlyName + ": Exception - " + e.Message, "networking", "warn");
                return false;
            }
        }

        /// <summary>
        /// Close a previously opened port on the Windows firewall
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <returns>Boolean indicating whether the rule worked</returns>
        public static bool CloseFirewallPort(int intPortNumber)
        {
            try
            {
                INetFwOpenPorts portList;

                Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
                INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);
                portList = (INetFwOpenPorts)mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts;

                portList.Remove(intPortNumber, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);

                Database.AddLog("Close port " + intPortNumber, "networking");
                return true;
            }
            catch (Exception e)
            {
                Database.AddLog("Unable to close firewall port " + intPortNumber + ": Exception - " + e.Message, "networking", "warn");
                return false;
            }
        }

        /// <summary>
        /// Use NATUPNPLib to try and open a port forward
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <param name="strFriendlyName">Friendly name for the service</param>
        /// <returns>Boolean indicating whether the forward worked</returns>
        public static bool OpenUPnP(int intPortNumber, string strFriendlyName)
        {
            try {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                upnpnat.StaticPortMappingCollection.Add(intPortNumber, "TCP", intPortNumber, GetListenIP().ToString(), true, "[YAMS] " + strFriendlyName);
            
                Database.AddLog("Forwarded port " + intPortNumber + " for " + strFriendlyName, "networking");
                return true;
            }
            catch(Exception e) {
                Database.AddLog("Unable foward port " + intPortNumber + " for " + strFriendlyName + ": Exception - " + e.Message, "networking", "warn");
                return false;
            }
        }

        /// <summary>
        /// Use NATUPNPLib to try and close a previously set port forward
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <returns>Boolean indicating whether the forward worked</returns>
        public static bool CloseUPnP(int intPortNumber)
        {
            try 
            {
                NATUPNPLib.UPnPNATClass upnpnat = new NATUPNPLib.UPnPNATClass();
                upnpnat.StaticPortMappingCollection.Remove(intPortNumber, "TCP");
            
                Database.AddLog("Un-forwarded port " + intPortNumber, "networking");
                return true;
            }
            catch (Exception e)
            {
                Database.AddLog("Unable to un-forward port " + intPortNumber + ": Exception - " + e.Message, "networking", "warn");
                return false;
            }
        }

        //### By Matt Brindley: http://www.mattbrindley.com/developing/windows/net/detecting-the-next-available-free-tcp-port/ ###

        /// <summary>
        /// Provides static methods for operations 
        /// commonly required when working with TCP ports
        /// </summary>
        public static class TcpPort
        {
            private const string PortReleaseGuid =
                "8875BD8E-4D5B-11DE-B2F4-691756D89593";

            /// <summary>
            /// Check if startPort is available, incrementing and
            /// checking again if it's in use until a free port is found
            /// </summary>
            /// <param name="startPort">The first port to check</param>
            /// <returns>The first available port</returns>
            public static int FindNextAvailablePort(int startPort)
            {
                int port = startPort;
                bool isAvailable = true;

                var mutex = new Mutex(false,
                    string.Concat("Global/", PortReleaseGuid));
                mutex.WaitOne();
                try
                {
                    IPGlobalProperties ipGlobalProperties =
                        IPGlobalProperties.GetIPGlobalProperties();
                    IPEndPoint[] endPoints =
                        ipGlobalProperties.GetActiveTcpListeners();

                    do
                    {
                        if (!isAvailable)
                        {
                            port++;
                            isAvailable = true;
                        }

                        foreach (IPEndPoint endPoint in endPoints)
                        {
                            if (endPoint.Port != port) continue;
                            isAvailable = false;
                            break;
                        }

                    } while (!isAvailable && port < IPEndPoint.MaxPort);

                    if (!isAvailable)
                        throw new NotImplementedException();

                    return port;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }


}
