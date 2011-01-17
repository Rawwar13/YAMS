using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Data;
using HttpServer;
using HttpServer.Authentication;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Tools;
using HttpServer.Headers;
using HttpListener = HttpServer.HttpListener;
using Newtonsoft.Json;

namespace YAMS
{
    public static class WebServer
    {
        private static Server myServer;

        private static Thread serverThread;

        //Control
        public static void Init()
        {
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
                            StringBuilder sb = new StringBuilder();

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
            else
            {
                context.Response.Connection.Type = ConnectionType.Close;
                byte[] buffer = Encoding.UTF8.GetBytes(File.ReadAllText(YAMS.Core.RootFolder + "\\web\\index.html"));
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
