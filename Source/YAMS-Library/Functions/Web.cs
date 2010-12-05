using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using HttpServer;
using HttpServer.Authentication;
using HttpServer.Modules;
using HttpServer.Resources;
using HttpServer.Tools;
using HttpListener = HttpServer.HttpListener;

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

            //Handle page requests
            myServer.Add(new Page());

            myServer.Add(HttpListener.Create(IPAddress.Any, 8085));
            //myServer.RequestReceived += new EventHandler<RequestEventArgs>(RequestReceived);

            serverThread = new Thread(new ThreadStart(Start));
        }

        public static void Start()
        {
            myServer.Start(5);

            //Start our session provider
            WebSession.Start(myServer);
        }

        public static void Stop()
        {
            serverThread.Abort();
        }

        public static void RequestReceived(object sender, RequestEventArgs e)
        {
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

    public class API : IModule
    {
        public ProcessingResult Process(RequestContext context)
        {
            return ProcessingResult.Abort;
        }
    }

    public class Page : IModule
    {
        public ProcessingResult Process(RequestContext context)
        {
            return ProcessingResult.Abort;
        }
    }
}
