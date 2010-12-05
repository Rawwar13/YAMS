﻿using System.IO;
using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace YAMS
{
    public static class Util
    {

        private static string strJRERegKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
        private static string strJDKRegKey = "SOFTWARE\\JavaSoft\\Java Development Kit";

        //Check for the existence to the two JVMs
        public static bool HasJRE()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strJRERegKey);
                if (subKey != null) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool HasJDK()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey subKey = rk.OpenSubKey(strJDKRegKey);
                if (subKey != null) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        
        }

        //Get the Java version from the registry
        public static string JavaVersion(string strType = "jre")
        {
            string strKey = "";
            switch (strType)
            {
                case "jre":
                    strKey = strJRERegKey;
                    break;
                case "jdk":
                    strKey = strJDKRegKey;
                    break;
            }
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey subKey = rk.OpenSubKey(strKey);
            if (subKey != null) return subKey.GetValue("CurrentVersion").ToString();
            else return "";
        }

        //Calculate the path to the Java executable
        public static string JavaPath(string strType = "jre")
        {
            string strKey = "";
            switch (strType)
            {
                case "jre":
                    strKey = strJRERegKey;
                    break;
                case "jdk":
                    strKey = strJDKRegKey;
                    break;
            }
            strKey += "\\" + JavaVersion(strType);
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey subKey = rk.OpenSubKey(strKey);
            if (subKey != null) return subKey.GetValue("JavaHome").ToString() + "\\bin\\";
            else return "";
        }

        //Replaces file 1 with file 2
        public static bool ReplaceFile(string strFileOriginal, string strFileReplacement) {
            try
            {
                if (File.Exists(strFileReplacement))
                {
                    if (File.Exists(strFileOriginal)) File.Delete(strFileOriginal);
                    File.Move(strFileReplacement, strFileOriginal);
                }
                return true;
           }
            catch {
                YAMS.Database.AddLog("Unable to update " + strFileOriginal, "updater", "error");
                return false;
            }
        }

        //Initial Set-up for first run only
        public static void FirstRun()
        {
            //Create directory structure
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\servers\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\servers\");
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\lib\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\lib\");

            //Download and extract latest web files
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\web\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\web\");
            //TODO: Something to grab a zip of web files and expand

            //Grab latest server jar
            YAMS.AutoUpdate.UpdateIfNeeded(YAMS.AutoUpdate.strMCServerURL, YAMS.Core.RootFolder + @"\lib\minecraft_server.jar.UPDATE");

            //Set our MC Defaults in the DB
            var NewServer = new List<KeyValuePair<string, string>>();
            NewServer.Add(new KeyValuePair<string, string>("admin-slot", "true"));
            NewServer.Add(new KeyValuePair<string, string>("enable-health", "true"));
            NewServer.Add(new KeyValuePair<string, string>("hellworld", "false"));
            NewServer.Add(new KeyValuePair<string, string>("level-name", @"..\\world"));
            NewServer.Add(new KeyValuePair<string, string>("max-connections", "1"));
            NewServer.Add(new KeyValuePair<string, string>("max-players", "20"));
            NewServer.Add(new KeyValuePair<string, string>("motd", "Welcome to a YAMS server!"));
            NewServer.Add(new KeyValuePair<string, string>("online-mode", "true"));
            NewServer.Add(new KeyValuePair<string, string>("public", "false"));
            NewServer.Add(new KeyValuePair<string, string>("pvp", "true"));
            NewServer.Add(new KeyValuePair<string, string>("server-ip", ""));
            NewServer.Add(new KeyValuePair<string, string>("server-name", "My YAMS MC Server"));
            NewServer.Add(new KeyValuePair<string, string>("server-port", "25565"));
            NewServer.Add(new KeyValuePair<string, string>("spawn-animals", "true"));
            NewServer.Add(new KeyValuePair<string, string>("spawn-monsters", "true"));
            NewServer.Add(new KeyValuePair<string, string>("verify-names", "true"));
            YAMS.Database.NewServer(NewServer, "My First YAMS Server");

            //Set our YAMS Defaults
            YAMS.Database.SaveSetting("UpdateJAR", "true");
            YAMS.Database.SaveSetting("UpdateSVC", "false");
            YAMS.Database.SaveSetting("UpdateGUI", "true");
            YAMS.Database.SaveSetting("UpdateWeb", "true");
            YAMS.Database.SaveSetting("UpdateAddons", "true");
            YAMS.Database.SaveSetting("RestartOnJarUpdate", "false");
            YAMS.Database.SaveSetting("RestartOnSVCUpdate", "false");
            YAMS.Database.SaveSetting("Memory", "1024");
            YAMS.Database.SaveSetting("EnableJavaOptimisations", "true");
            YAMS.Database.SaveSetting("ListenPort", "56552"); //Use an IANA legal internal port 49152 - 65535
            YAMS.Database.SaveSetting("ExternalIP", GetExternalIP().ToString());
            YAMS.Database.SaveSetting("ListenIP", GetListenIP().ToString());


            //Tell the DB that we've run this
            YAMS.Database.SaveSetting("FirstRun", "true");

        }

        //Uses WhatIsMyIP.com to determine the user's external IP
        public static IPAddress GetExternalIP()
        {
            string strExternalIPChecker = "http://www.whatismyip.com/automation/n09230945.asp"; // See http://forum.whatismyip.com/f14/our-automation-rules-t241/
            WebClient wcGetIP = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            string strResponse = "";
            try
            {
                strResponse = utf8.GetString(wcGetIP.DownloadData(strExternalIPChecker));
            }
            catch (WebException e)
            {
                YAMS.Database.AddLog("Unable to determine external IP: " + e.Data, "utils", "warn");
            }

            IPAddress ipExternal = null;
            ipExternal = IPAddress.Parse(strResponse);
            return ipExternal;
        }

        //Get an IP Address to bind to
        public static IPAddress GetListenIP()
        {
            IPHostEntry ipListen = Dns.GetHostEntry(Dns.GetHostName());
            return ipListen.AddressList[0];
        }

        //What is the bitness of the system
        public static string GetBitness()
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return "x86";
                case 8:
                    return "x64";
                default:
                    return "x86";
            }
        }
    }
}