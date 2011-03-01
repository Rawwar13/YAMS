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
                            foreach (KeyValuePair<int, MCServer> kvp in Core.Servers)
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
                        case "gmap":
                            //Maps a server
                            s = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])];
                            AddOns.Overviewer gmap = new AddOns.Overviewer(s);
                            gmap.Start();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "c10t":
                            //Maps a server
                            s = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])];
                            AddOns.c10t c10t = new AddOns.c10t(s);
                            c10t.Start();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "start":
                            //Starts a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Start();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "stop":
                            //Stops a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Stop();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "restart":
                            //Restarts a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Restart();
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "delayed-restart":
                            //Restarts a server after a specified time and warns players
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].DelayedRestart(Convert.ToInt32(param["delay"]));
                            strResponse = "{ \"result\" : \"sent\" }";
                            break;
                        case "command":
                            //Sends literal command to a server
                            Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].Send(context.Request.Parameters["message"]);
                            strResponse = "{ \"result\" : \"sent\" }";
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
                                           "\"serverport\" : \"" + Database.GetSetting("server-port", "MC", intServerID) + "\"" +
                                           "\"whitelist\" : \"" + Database.GetSetting("white-list", "MC", intServerID) + "\"";

                            strResponse += "}";
                            break;
                        case "get-config-file":
                            List<string> listConfig = Core.Servers[Convert.ToInt32(context.Request.Parameters["serverid"])].ReadConfig(param["file"]);
                            strResponse = JsonConvert.SerializeObject(listConfig, Formatting.Indented);
                            break;
                        case "upload-world":
                            var test = context.Request.Files["new-world"];
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
            else
            {
                return ProcessingResult.Abort;
            }

        }

    }
}
