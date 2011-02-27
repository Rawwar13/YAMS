using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using HttpServer;
using HttpServer.Authentication;
using HttpServer.Headers;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Tools;
using Newtonsoft.Json;
using System.Data.SqlServerCe;
using HttpListener = HttpServer.HttpListener;
using YAMS;

namespace YAMS
{
    public static class WebServer
    {
        private static Server adminServer;
        private static Server publicServer;

        private static Thread adminServerThread;
        private static Thread publicServerThread;

        //Control
        public static void Init()
        {
            //See if there is a new version of the web files waiting before we start the server
            if (File.Exists(Core.RootFolder + @"\web.zip"))
            {
                if (Directory.Exists(Core.RootFolder + @"\web\")) Directory.Delete(Core.RootFolder + @"\web\", true);
                Directory.CreateDirectory(YAMS.Core.RootFolder + @"\web\");
                AutoUpdate.ExtractZip(YAMS.Core.RootFolder + @"\web.zip", YAMS.Core.RootFolder + @"\web\");
                File.Delete(Core.RootFolder + @"\web.zip");
            }
            
            adminServer = new Server();
            publicServer = new Server();

            //Handle the requests for static files
            var adminModule = new FileModule();
            adminModule.Resources.Add(new FileResources("/assets/", YAMS.Core.RootFolder + "\\web\\assets\\"));
            adminServer.Add(adminModule);

            //Add any server specific folders
            var publicModule = new FileModule();
            publicModule.Resources.Add(new FileResources("/assets/", YAMS.Core.RootFolder + "\\web\\assets\\"));
            SqlCeDataReader readerServers = YAMS.Database.GetServers();
            while (readerServers.Read())
            {
                var intServerID = readerServers["ServerID"].ToString();
                publicModule.Resources.Add(new FileResources("/servers/" + intServerID + "/map/", YAMS.Core.RootFolder + "\\servers\\" + intServerID + "\\renders\\gmap\\output\\"));
                publicModule.Resources.Add(new FileResources("/servers/" + intServerID + "/renders/", YAMS.Core.RootFolder + "\\servers\\" + intServerID + "\\renders\\"));
            }
            publicServer.Add(publicModule);

            //Handle requests to API
            adminServer.Add(new Web.AdminAPI());
            publicServer.Add(new Web.PublicAPI());

            adminServer.Add(HttpListener.Create(IPAddress.Any, Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS"))));
            publicServer.Add(HttpListener.Create(IPAddress.Any, Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS"))));

            adminServer.ErrorPageRequested += new EventHandler<ErrorPageEventArgs>(myServer_ErrorPageRequested);
            publicServer.ErrorPageRequested += new EventHandler<ErrorPageEventArgs>(myServer_ErrorPageRequested);

            adminServerThread = new Thread(new ThreadStart(StartAdmin));
            publicServerThread = new Thread(new ThreadStart(StartPublic));
            adminServerThread.Start();
            publicServerThread.Start();
        }

        static void myServer_ErrorPageRequested(object sender, ErrorPageEventArgs e)
        {
            Database.AddLog(e.Exception.Message, "web", "error");
            e.Response.Reason = "Error - YAMS";
            e.Response.Connection.Type = ConnectionType.Close;
            byte[] buffer = Encoding.UTF8.GetBytes("<h1>500 Internal Server Error</h1><p>" + e.Exception.Message + "</p>");
            e.Response.Body.Write(buffer, 0, buffer.Length);
        }

        public static void StartAdmin()
        {
            int intTryCount = 0;
            try
            {
                adminServer.Start(5);
                //Start our session provider
                WebSession.Start(adminServer);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                //Previous service has not released the port, so hang on and try again.
                intTryCount++;
                Database.AddLog("Admin Web server port still in use, attempt " + intTryCount + ": " + e.Message, "web", "warn");
                Thread.Sleep(1000);
                if (intTryCount < 120)
                {
                    StartAdmin();
                }
                else
                {
                    EventLog myLog = new EventLog();
                    myLog.Source = "YAMS";
                    myLog.WriteEntry("Unable to start admin web server", EventLogEntryType.Error);
                }
            }
            catch (Exception e) {
                EventLog myLog = new EventLog();
                myLog.Source = "YAMS";
                myLog.WriteEntry("Exception: " + e.Data, EventLogEntryType.Error);
            }
            
        }

        public static void StartPublic()
        {
            int intTryCount = 0;
            try
            {
                publicServer.Start(5);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                //Previous service has not released the port, so hang on and try again.
                intTryCount++;
                Database.AddLog("Public Web server port still in use, attempt " + intTryCount + ": " + e.Message, "web", "warn");
                Thread.Sleep(1000);
                if (intTryCount < 120)
                {
                    StartPublic();
                }
                else
                {
                    EventLog myLog = new EventLog();
                    myLog.Source = "YAMS";
                    myLog.WriteEntry("Unable to start public web server", EventLogEntryType.Error);
                }
            }
            catch (Exception e)
            {
                EventLog myLog = new EventLog();
                myLog.Source = "YAMS";
                myLog.WriteEntry("Exception: " + e.Data, EventLogEntryType.Error);
            }

       
        }


        public static void Stop()
        {
            adminServerThread.Abort();
            publicServerThread.Abort();
        }

    }

    [Serializable]
    public class WebSession : Session
    {
        private static readonly SessionProvider<WebSession> _sessionProvider = new SessionProvider<WebSession>();

        static WebSession()
        {
            _sessionProvider.Cache = true;
        }

        /// <summary>
        /// Gets currently loaded session
        /// </summary>
        /// <remarks>
        /// Will not create sessions and manage new sessions, but returns a dummy one which is not handled by the provider class.
        /// Use the Create method to get a session that will be maintained by the provider class.
        /// </remarks>
        public static WebSession Current
        {
            get { return _sessionProvider.Current ?? new WebSession(); }
        }

        /// <summary>
        /// Gets or sets first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets current errors.
        /// </summary>
        public static List<string> Errors { get; set; }

        /// <summary>
        /// Creates a new session and also sets it as the current one.
        /// </summary>
        /// <returns>Created session.</returns>
        public static WebSession Create()
        {
            return _sessionProvider.Create();
        }

        internal static void Start(Server webServer)
        {
            _sessionProvider.Start(webServer);
        }
    }


}
