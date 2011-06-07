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

namespace YAMS.Web
{
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
                    Match matServerHome = regServerHome.Match(context.Request.Uri.AbsolutePath);
                    int intServerID = Convert.ToInt32(matServerHome.Groups[1].Value);
                    MCServer s = Core.Servers[intServerID];

                    string strOverviewer = "";
                    string strTectonicus = "";
                    string strImages = "";
                    string strBackups = "";

                    if (File.Exists(s.ServerDirectory + @"\renders\overviewer\output\index.html")) {
                        strOverviewer = "<h3>Overviewer Map</h3><div><a href=\"renders/overviewer/output/index.html\">Click here to open map</a>";
                    }
                    if (File.Exists(s.ServerDirectory + @"\renders\tectonicus\map.html"))
                    {
                        strTectonicus = "<h3>Tectonicus Map</h3><div><a href=\"renders/tectonicus/map.html\">Click here to open map</a>";
                    }

                    strImages = "<h3>Images</h3><ul>";
                    DirectoryInfo di = new DirectoryInfo(s.ServerDirectory + @"\renders\");
                    FileInfo[] fileEntries = di.GetFiles();
                    foreach (FileInfo fi in fileEntries)
                    {
                        strImages += "<li><a href=\"renders/" + fi.Name + "\">" + fi.Name + "</a></li>";
                    }
                    strImages += "</ul>";

                    strBackups = "<h3>Backups</h3><ul>";
                    DirectoryInfo di2 = new DirectoryInfo(s.ServerDirectory + @"\backups\");
                    FileInfo[] fileEntries2 = di2.GetFiles();
                    foreach (FileInfo fi in fileEntries2)
                    {
                        strBackups += "<li><a href=\"backups/" + fi.Name + "\">" + fi.Name + "</a></li>";
                    }
                    strBackups += "</ul>";

                    strTemplate = File.ReadAllText(Core.RootFolder + @"\web\templates\server-home.html");
                    dicTags.Add("PageTitle", s.ServerTitle);
                    dicTags.Add("RenderOverviewer", strOverviewer);
                    dicTags.Add("RenderTectonicus", strTectonicus);
                    dicTags.Add("RenderImages", strImages);
                    dicTags.Add("BackupList", strBackups);
                    dicTags.Add("PageBody", "Body");
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
}
