using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using NetFwTypeLib;
using YAMS;
using SharpUPnP;

namespace YAMS
{
    public class Networking
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
                port.Name = strFriendlyName;
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
        /// Use SharpUPnPLib to try and open a port forward
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <param name="strFriendlyName">Friendly name for the service</param>
        /// <returns>Boolean indicating whether the forward worked</returns>
        public static bool OpenUPnP(int intPortNumber, string strFriendlyName)
        {
            //First check if there is a upnp device to talk to
            if (UPnP.Discover())
            {
                try {
                    UPnP.CreateForwardingRule(intPortNumber, System.Net.Sockets.ProtocolType.Tcp, strFriendlyName);
                    Database.AddLog("Forwarded port " + intPortNumber + " for " + strFriendlyName, "networking");
                    return true;
                }
                catch (Exception e) {
                    Database.AddLog("Unable to forward port " + intPortNumber + " for " + strFriendlyName + ": Exception - " + e.Message, "networking", "warn");
                    return false;
                }
            }
            else
            {
                Database.AddLog("Unable to forward port " + intPortNumber + " for " + strFriendlyName + ": No UPnP device detected", "networking", "warn");
                return false;
            }
        }

        /// <summary>
        /// Use SharpUPnPLib to try and close a previously set port forward
        /// </summary>
        /// <param name="intPortNumber">The port to open</param>
        /// <returns>Boolean indicating whether the forward worked</returns>
        public static bool CloseUPnP(int intPortNumber)
        {
            //First check if there is a upnp device to talk to
            if (UPnP.Discover())
            {
                try
                {
                    UPnP.DeleteForwardingRule(intPortNumber, System.Net.Sockets.ProtocolType.Tcp);
                    Database.AddLog("Un-forwarded port " + intPortNumber, "networking");
                    return true;
                }
                catch (Exception e)
                {
                    Database.AddLog("Unable to un-forward port " + intPortNumber + ": Exception - " + e.Message, "networking", "warn");
                    return false;
                }
            }
            else
            {
                Database.AddLog("Unable to un-forward port " + intPortNumber + ": No UPnP device detected", "networking", "warn");
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
