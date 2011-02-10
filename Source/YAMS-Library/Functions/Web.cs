using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HttpServer;
using HttpServer.Authentication;
using HttpServer.Headers;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Tools;
using Newtonsoft.Json;
using HttpListener = HttpServer.HttpListener;
using YAMS;

namespace YAMS
{
    public static class WebServer
    {
        private static Server myServer;

        private static Thread serverThread;

        private static string OpenTag = @"\{\?Y\:";
        private static string CloseTag = @"\?\}";
        private static Regex TagFinder = new Regex("(" + OpenTag + ")([^}]+)(" + CloseTag + ")");

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
            
            myServer = new Server();

            //Handle the requests for static files
            var module = new FileModule();
            module.Resources.Add(new FileResources("/assets/", YAMS.Core.RootFolder + "\\web\\assets\\"));
            myServer.Add(module);

            //Handle requests to API
            myServer.Add(new API());

            myServer.Add(HttpListener.Create(IPAddress.Any, Convert.ToInt32(YAMS.Database.GetSetting("ListenPort", "YAMS"))));
            //myServer.RequestReceived += new EventHandler<RequestEventArgs>(RequestReceived);

            serverThread = new Thread(new ThreadStart(Start));
        }

        public static void Start()
        {
            myServer.Start(5);

            //Start our session provider
            //WebSession.Start(myServer);
        }

        public static void Stop()
        {
            serverThread.Abort();
        }

        public static string ReplaceTags(string strInput, Dictionary<string,string> dicTags)
        {
            var strOutput = strInput;

            MatchCollection results = TagFinder.Matches(strInput);
            foreach (Match match in results)
            {
                Regex replacer = new Regex(OpenTag + match.Value + CloseTag);
                if (dicTags.ContainsKey(match.Value)) strOutput = replacer.Replace(strOutput, dicTags[match.Value]);
            }

            return strOutput;
        }

    }


    public class API : IModule
    {
        public ProcessingResult Process(RequestContext context)
        {
            int intServerID = 0;
            
            if (context.Request.Uri.AbsoluteUri.Contains(@"/api/"))
            {
                //what is the action?
                if (context.Request.Method == Method.Post)
                {
                    String strResponse = "";
                    switch (context.Request.Parameters["action"])
                    {
                        case "log":
                            //grabs lines from the log.
                            int intStartID = Convert.ToInt32(context.Request.Parameters["start"]);
                            int intNumRows = Convert.ToInt32(context.Request.Parameters["rows"]);
                            int intServer = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            string strLevel = context.Request.Parameters["level"];

                            DataSet ds = YAMS.Database.ReturnLogRows(intStartID, intNumRows, strLevel, intServer);

                            strResponse = JsonConvert.SerializeObject(ds, Formatting.Indented);
                            break;
                        case "list":
                            //List available servers
                            strResponse = "{ \"servers\" : [";
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                strResponse += "{ \"id\" : " + s.ServerID + ", " +
                                                 "\"title\" : \"" + s.ServerTitle + "\", " +
                                                 "\"ver\" : \"" + s.ServerVersion + "\" } ,";
                            });
                            strResponse = strResponse.Remove(strResponse.Length - 1);
                            strResponse += "]}";
                            break;
                        case "status":
                            //Get status of a server
                            intServerID = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                if (s.ServerID == intServerID)
                                {
                                    strResponse = "{ \"serverid\" : " + intServerID + "," +
                                                    "\"status\" : \"" + s.Running + "\"," +
                                                    "\"ram\" : " + s.GetMemory() + "," +
                                                    "\"vm\" : " + s.GetVMemory() + " }";
                                };
                            });
                            break;
                        case "players":
                            //Get status of a server
                            intServerID = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                if (s.ServerID == intServerID)
                                {
                                    strResponse = "{ \"serverid\" : " + intServerID + ",";
                                    strResponse += "\"players\" : [";
                                    if (s.Players.Count > 0)
                                    {
                                        s.Players.ForEach(delegate(string p)
                                        {
                                            strResponse += "\"" + p + "\",";
                                        });
                                        strResponse = strResponse.Remove(strResponse.Length - 1);
                                    }
                                    strResponse += "]}";
                                };
                            });
                            break;
                        case "start":
                            //Starts a server
                            intServerID = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                if (s.ServerID == intServerID) s.Start();
                            });
                            strResponse = "{ \"result\" : \"sentstart\" }";
                            break;
                        case "stop":
                            //Stops a server
                            intServerID = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                if (s.ServerID == intServerID) s.Stop();
                            });
                            strResponse = "{ \"result\" : \"sentstop\" }";
                            break;
                        case "command":
                            //Sends literal command to a server
                            intServerID = Convert.ToInt32(context.Request.Parameters["serverid"]);
                            Core.Servers.ForEach(delegate(MCServer s)
                            {
                                if (s.ServerID == intServerID) s.Send(context.Request.Parameters["message"]);
                            });
                            strResponse = "{ \"result\" : \"sentcommand\" }";
                            break;
                        default:
                            return ProcessingResult.Abort;
                    }

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
                context.Response.Connection.Type = ConnectionType.Close;
                byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(YAMS.Core.RootFolder + @"\web\admin\index.html"));
                context.Response.Body.Write(buffer, 0, buffer.Length);
                return ProcessingResult.SendResponse;
            }
            else
            {
                //it's a public request, work out what they want
                // / = list servers
                // /[0-9]+/ = server home page including chat log
                // /[0-9]+/map = Google Map
                // /[0-9]+/renders = c10t renders
                var strTemplate = File.ReadAllText(Core.RootFolder + @"\web\template.html");
                Dictionary<string, string> dicTags = new Dictionary<string,string>();
                
                if (context.Request.Uri.AbsoluteUri.Equals(@"/"))
                {
                    //List servers available
                    dicTags.Add("PageTitle", "Server List");
                }

                //Run through our replacer
                strTemplate = YAMS.WebServer.ReplaceTags(strTemplate, dicTags);

                //And send to the browser
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
