using System;
using System.Collections.Generic;
using System.Text;
using YAMS;

namespace YAMS.Objects
{
    public class Player
    {
        //The minecraft login name of the player
        public string Username;

        //Their level
        public string Level = "guest";

        //Which server are they on?
        private MCServer Server;

        public Player(string strName, MCServer s)
        {
            this.Username = strName;
            this.Server = s;

            this.Level = Database.GetPlayerLevel(strName, s.ServerID);
            if (this.Level == null)
            {
                //Player doesn't exist on this server yet
                if (this.Server.LogonMode == "whitelist")
                {
                    //If you're not on the list, you ain't coming in
                    this.Server.Send("kick " + this.Username);
                    return;
                }
                else
                {
                    //We're letting anyone in these days, so add to the DB
                    Database.AddUser(this.Username, this.Server.ServerID);
                    this.Level = "guest";
                };
            }

            //Emulate MOTD
            if (Database.GetSetting("motd", "MC", this.Server.ServerID) != "") this.Server.Whisper(this.Username, Database.GetSetting("motd", "MC", this.Server.ServerID));
        }

    }
}
