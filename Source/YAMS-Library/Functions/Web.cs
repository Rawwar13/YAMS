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
            adminServer.Add(new AdminAPI());
            publicServer.Add(new PublicAPI());

            adminServer.Add(HttpListener.Create(IPAddress.Any, Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS"))));
            publicServer.Add(HttpListener.Create(IPAddress.Any, Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS"))));

            adminServer.ErrorPageRequested += new EventHandler<ErrorPageEventArgs>(myServer_ErrorPageRequested);
            publicServer.ErrorPageRequested += new EventHandler<ErrorPageEventArgs>(myServer_ErrorPageRequested);

            adminServerThread = new Thread(new ThreadStart(StartAdmin));
            publicServerThread = new Thread(new ThreadStart(StartPublic));
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
                Database.AddLog("Admin Web server port still in use, attempt " + intTryCount, "web", "warn");
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
            
            //try {
            //    Networking.OpenFirewallPort(Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS")), "YAMS Admin Web Server");
            //    Networking.OpenUPnP(Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS")), "YAMS Admin Web Server");
            //} catch (Exception e) {
            //    YAMS.Database.AddLog(e.Message);
            //}


        }

        public static void StartPublic()
        {
            int intTryCount = 0;
            try
            {
                publicServer.Start(10);
            }
            catch (System.Net.Sockets.SocketException e)
            {
                //Previous service has not released the port, so hang on and try again.
                intTryCount++;
                Database.AddLog("Public Web server port still in use, attempt " + intTryCount, "web", "warn");
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

            //try {
            //    Networking.OpenFirewallPort(Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS")), "YAMS Public Web Server");
            //    Networking.OpenUPnP(Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS")), "YAMS Public Web Server");
            //} catch (Exception e) {
            //    YAMS.Database.AddLog(e.Message);
            //}
        
        }


        public static void Stop()
        {
            //try
            //{
            //    Networking.CloseFirewallPort(Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS")));
            //    Networking.CloseUPnP(Convert.ToInt32(YAMS.Database.GetSetting("AdminListenPort", "YAMS")));
            //    Networking.CloseFirewallPort(Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS")));
            //    Networking.CloseUPnP(Convert.ToInt32(YAMS.Database.GetSetting("PublicListenPort", "YAMS")));
            //}
            //catch (Exception e)
            //{
            //    YAMS.Database.AddLog(e.Message);
            //}
            adminServerThread.Abort();
            publicServerThread.Abort();
        }

    }


    public class AdminAPI : IModule
    {
        public ProcessingResult Process(RequestContext context)
        {
            int intServerID = 0;
            MCServer s;
            
            if (context.Request.Uri.AbsoluteUri.Contains(@"/api/"))
            {
                //must be authenticated
                
                //what is the action?
                if (context.Request.Method == Method.Post && WebSession.Current.UserName == "admin")
                {
                    String strResponse = "";
                    IParameterCollection param = context.Request.Parameters;
                    switch (context.Request.Parameters["action"])
                    {
                        case "log":
                            //grabs lines from the log.
                            int intStartID = Convert.ToInt32(context.Request.Parameters["start"]);
                            int intNumRows = Convert.ToInt32(context.Request.Parameters["rows"]);
                            int intServer = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            string strLevel = context.Request.Parameters["level"];
                                                        
                            DataSet ds = Database.ReturnLogRows(intStartID, intNumRows, strLevel, intServer);

                            strResponse = JsonConvert.SerializeObject(ds, Formatting.Indented);
                            break;
                        case "list":
                            //List available servers
                            strResponse = "{ \"servers\" : [";
                            foreach(KeyValuePair<int, MCServer> kvp in Core.Servers)
                            {
                                strResponse += "{ \"id\" : " + kvp.Value.ServerID + ", " +
                                                 "\"title\" : \"" + kvp.Value.ServerTitle + "\", " +
                                                 "\"ver\" : \"" + kvp.Value.ServerVersion + "\" } ,";
                            };
                            strResponse = strResponse.Remove(strResponse.Length - 1);
                            strResponse += "]}";
                            break;
                        case "status":
                            //Get status of a server
                            s = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])];
                            strResponse = "{ \"serverid\" : " + s.ServerID + "," +
                                            "\"status\" : \"" + s.Running + "\"," +
                                            "\"ram\" : " + s.GetMemory() + "," +
                                            "\"vm\" : " + s.GetVMemory() + "," +
                                            "\"players\" : [";
                            if (s.Players.Count > 0)
                            {
                                foreach (KeyValuePair<string, Objects.Player> kvp in s.Players)
                                {
                                    strResponse += " { \"name\": \"" + kvp.Value.Username + "\", \"level\": \"" + kvp.Value.Level + "\" },";
                                };
                                strResponse = strResponse.Remove(strResponse.Length - 1);
                            }
                            strResponse += "]}";
                            break;
                        //case "gmap":
                        //    //Maps a server
                        //    s = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])];
                        //    AddOns.Overviewer gmap = new AddOns.Overviewer(s);
                        //    gmap.Start();
                        //    strResponse = "{ \"result\" : \"sent\" }";
                        //    break;
                        case "gmap":
                            //Maps a server
                            s = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])];
                            AddOns.c10t gmap = new AddOns.c10t(s);
                            gmap.Start();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "start":
                            //Starts a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Start();
                            strResponse = "{ \"result\" : \"sentstart\" }";
                            break;
                        case "stop":
                            //Stops a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Stop();
                            strResponse = "{ \"result\" : \"sentstop\" }";
                            break;
                        case "restart":
                            //Restarts a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Restart();
                            strResponse = "{ \"result\" : \"sentstart\" }";
                            break;
                        case "command":
                            //Sends literal command to a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Send(context.Request.Parameters["message"]);
                            strResponse = "{ \"result\" : \"sentcommand\" }";
                            break;
                        case "get-yams-settings":
                            DataSet dsSettings = Database.ReturnSettings();
                            JsonConvert.SerializeObject(dsSettings, Formatting.Indented);
                            break;
                        case "save-yams-settings":
                            //Settings update
                            foreach (Parameter p in param)
                            {
                                if (p.Name != "action") Database.SaveSetting(p.Name, p.Value);
                            }
                            break;
                        case "get-server-settings":
                            //retrieve all server settings as JSON
                            intServerID = Convert.ToInt32(param["serverid"]);
                            strResponse = "{ \"serverid\" : " + intServerID + "," +
                                              "\"title\" : \"" + Database.GetSetting(intServerID, "ServerTitle") + "\"," +
                                              "\"optimisations\" : \"" + Database.GetSetting(intServerID, "ServerEnableOptimisations") + "\"," +
                                              "\"memory\" : \"" + Database.GetSetting(intServerID, "ServerAssignedMemory") + "\"," +
                                              "\"autostart\" : \"" + Database.GetSetting(intServerID, "ServerAutoStart") + "\"," +
                                              "\"logonmode\" : \"" + Database.GetSetting(intServerID, "ServerLogonMode") + "\",";
                            //Minecraft Settings
                            strResponse += "\"hellworld\" : \"" + Database.GetSetting("hellworld", "MC", intServerID) + "\"," +
                                           "\"spawnmonsters\" : \"" + Database.GetSetting("spawn-monsters", "MC", intServerID) + "\"," +
                                           "\"onlinemode\" : \"" + Database.GetSetting("online-mode", "MC", intServerID) + "\"," +
                                           "\"spawnanimals\" : \"" + Database.GetSetting("spawn-animals", "MC", intServerID) + "\"," +
                                           "\"maxplayers\" : \"" + Database.GetSetting("max-players", "MC", intServerID) + "\"," +
                                           "\"serverip\" : \"" + Database.GetSetting("server-ip", "MC", intServerID) + "\"," +
                                           "\"pvp\" : \"" + Database.GetSetting("pvp", "MC", intServerID) + "\"," +
                                           "\"serverport\" : \"" + Database.GetSetting("server-port", "MC", intServerID) + "\"";

                            strResponse += "}";
                            break;
                        default:
                            return ProcessingResult.Abort;
                    }

                    context.Response.Reason = "Completed - YAMS";
                    context.Response.Connection.Type = ConnectionType.Close;
                    byte[] buffer = Encoding.UTF8.GetBytes(strResponse);
                    context.Response.Body.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    // not a post, so say bye bye!
                    return ProcessingResult.Abort;
                }
                
                return ProcessingResult.SendResponse;
            }
            else if (context.Request.Uri.AbsoluteUri.Contains(@"/admin"))
            {
                
                if (WebSession.Current.UserName != "admin")
                {
                    context.Response.Reason = "Completed - YAMS";
                    context.Response.Connection.Type = ConnectionType.Close;
                    byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(YAMS.Core.RootFolder + @"\web\admin\login.html"));
                    context.Response.Body.Write(buffer, 0, buffer.Length);
                    return ProcessingResult.SendResponse;
                }
                else
                {
                    context.Response.Reason = "Completed - YAMS";
                    context.Response.Connection.Type = ConnectionType.Close;
                    byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(YAMS.Core.RootFolder + @"\web\admin\index.html"));
                    context.Response.Body.Write(buffer, 0, buffer.Length);
                    return ProcessingResult.SendResponse;
                }
            }
            else if (context.Request.Uri.AbsoluteUri.Contains(@"/login"))
            {
                //This is a login request, check it's legit
                string userName = context.Request.Form["strUsername"];
                string password = context.Request.Form["strPassword"];

                if (userName == "admin" && password == Database.GetSetting("AdminPassword", "YAMS"))
                {
                    WebSession.Create();
                    WebSession.Current.UserName = "admin";
                    context.Response.Redirect(@"/admin");
                    return ProcessingResult.SendResponse;
                }
                else
                {
                    context.Response.Reason = "Completed - YAMS";
                    context.Response.Connection.Type = ConnectionType.Close;
                    byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(YAMS.Core.RootFolder + @"\web\admin\login.html"));
                    context.Response.Body.Write(buffer, 0, buffer.Length);
                    return ProcessingResult.SendResponse;
                }
            }
            else {
                return ProcessingResult.Abort;
            }

        }

    }

    public class PublicAPI : IModule
    {
        public ProcessingResult Process(RequestContext context)
        {

            //it's a public request, work out what they want
            // / = list servers
            // /[0-9]+/ = server home page including chat log
            // /[0-9]+/map = Google Map
            // /[0-9]+/renders = c10t renders

            Regex regRoot = new Regex(@"^/$");
            Regex regServerList = new Regex(@"^/servers/$");
            Regex regServerHome = new Regex(@"^/servers/([0-9]+)/$");
            Regex regServerGMap = new Regex(@"^/servers/([0-9]+)/map/");
            Regex regServerRenders = new Regex(@"^/servers/([0-9]+)/renders/");

            if (regServerGMap.Match(context.Request.Uri.AbsolutePath).Success || regServerRenders.Match(context.Request.Uri.AbsolutePath).Success)
            {
                return ProcessingResult.Continue;
            }
            else
            {
                string strTemplate = "No matching Template";
                Dictionary<string, string> dicTags = new Dictionary<string, string>();

                if (regRoot.Match(context.Request.Uri.AbsolutePath).Success)
                {
                    //Server Root
                    strTemplate = File.ReadAllText(Core.RootFolder + @"\web\templates\root.html");
                    dicTags.Add("PageTitle", "YAMS Hosted Server");
                    dicTags.Add("PageBody", "test");
                }
                else if (regServerList.Match(context.Request.Uri.AbsolutePath).Success)
                {
                    //List of Servers
                    strTemplate = File.ReadAllText(Core.RootFolder + @"\web\templates\server-list.html");
                    dicTags.Add("PageTitle", "Server List");
                    dicTags.Add("PageBody", "test");
                }
                else if (regServerHome.Match(context.Request.Uri.AbsolutePath).Success)
                {
                    //Individual Server home
                    strTemplate = File.ReadAllText(Core.RootFolder + @"\web\templates\server-home.html");
                    dicTags.Add("PageTitle", "Server Home");
                    dicTags.Add("PageBody", "test");
                }
                else
                {
                    //Unknown
                    context.Response.Status = HttpStatusCode.NotFound;
                    strTemplate = File.ReadAllText(Core.RootFolder + @"\web\templates\server-home.html");
                    dicTags.Add("PageTitle", "404");
                    dicTags.Add("PageBody", "<h1>404 - Not Found</h1>");
                }

                //Run through our replacer
                strTemplate = WebTemplate.ReplaceTags(strTemplate, dicTags);

                //And send to the browser
                context.Response.Reason = "Completed - YAMS";
                context.Response.Connection.Type = ConnectionType.Close;
                byte[] buffer = Encoding.UTF8.GetBytes(strTemplate);
                context.Response.Body.Write(buffer, 0, buffer.Length);
                return ProcessingResult.SendResponse;
            }

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
