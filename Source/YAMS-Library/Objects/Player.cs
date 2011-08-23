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
                //We're letting anyone in these days, so add to the DB
                Database.AddUser(this.Username, this.Server.ServerID);
                this.Level = "guest";
            }

            //check the op list
            if (Util.SearchFile(s.ServerDirectory + "ops.txt", strName)) this.Level = "op";

            //Emulate MOTD
            if (Database.GetSetting("motd", "MC", this.Server.ServerID) != "") this.SendMessage(Database.GetSetting("motd", "MC", this.Server.ServerID));
        }

        public void SendMessage(string strMessage)
        {
            this.Server.Whisper(this.Username, strMessage);
        }

    }
}
