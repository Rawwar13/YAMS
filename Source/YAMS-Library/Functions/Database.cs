using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace YAMS
{
    public class Database
    {
        private static SqlCeConnection connLocal;

        public static void init()
        {
            //Open our DB connection for use all over the place
            connLocal = GetConnection();
            connLocal.Open();
        }

        private static SqlCeConnection GetConnection()
        {
            string dbfile = YAMS.Core.RootFolder + "\\dbYAMS.sdf";
            SqlCeConnection connection = new SqlCeConnection("datasource=" + dbfile);
            return connection;
        }

        public static DataSet ReturnLogRows(int intStartID = 0, int intNumRows = 0, string strLevels = "all")
        {
            DataSet ds = new DataSet();
            SqlCeCommand command = connLocal.CreateCommand();
            command.CommandText = "SELECT * FROM Log ORDER BY LogDateTime DESC";
            SqlCeDataAdapter adapter = new SqlCeDataAdapter(command);
            adapter.Fill(ds);
            return ds;
        }

        public static void AddLog(string strMessage, string strSource = "app", string strLevel = "info", bool bolSendToAdmin = false, int intServerID = 0)
        {
            if (strMessage == null) strMessage = "Null message received";

            string sqlIns = "INSERT INTO Log (LogSource, LogMessage, LogLevel, ServerID) VALUES (@source, @msg, @level, @serverid)";
            try
            {
                SqlCeCommand cmdIns = new SqlCeCommand(sqlIns, connLocal);
                cmdIns.Parameters.Add("@source", strSource);
                cmdIns.Parameters.Add("@msg", strMessage);
                cmdIns.Parameters.Add("@level", strLevel);
                cmdIns.Parameters.Add("@serverid", intServerID);
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString(), ex);
            }
        }

        // Returns the stored etag for the specified URL or blank string if no etag saved
        public static string GetEtag(string strURL)
        {
            try
            {
                SqlCeCommand cmd = new SqlCeCommand("SELECT VersionETag FROM FileVersions WHERE VersionURL = @url", connLocal);
                cmd.Parameters.Add("@url", strURL);
                string eTag = (string)cmd.ExecuteScalar();
                return eTag;
            }
            catch (Exception ex)
            {
                AddLog("YAMS.Database.GetEtag Exception: " + ex.Message, "database", "error");
                return "";
            }
        }

        //Sets the Etag for a URL, replacing or adding the URL as needed
        public static bool SaveEtag(string strUrl, string strEtag)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = connLocal;
            if (GetEtag(strUrl) == null)
            {
                //Doesn't exist in DB already, so insert
                cmd.CommandText = "INSERT INTO FileVersions (VersionURL, VersionETag) VALUES (@url, @etag);";
            }
            else
            {
                //Exists, so need to update
                cmd.CommandText = "UPDATE FileVersions SET VersionETag=@etag WHERE VersionURL=@url;";
            }
            cmd.Parameters.Add("@url", strUrl);
            cmd.Parameters.Add("@etag", strEtag);
            cmd.ExecuteNonQuery();
            return true;
        }

        //Builds the server.properties file from current settings
        public static void BuildServerProperties(int intServerID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Minecraft server properties: built by YAMS\n");
            //sb.Append("#Sun Nov 28 19:26:26 GMT 2010\n");

            SqlCeCommand comProperties = new SqlCeCommand("SELECT * FROM MCSettings WHERE ServerID = @serverid", connLocal);
            comProperties.Parameters.Add("@serverid", intServerID);
            SqlCeDataReader readerProperties = null;
            readerProperties = comProperties.ExecuteReader();
            while (readerProperties.Read())
            {
                sb.Append(readerProperties["SettingName"].ToString() + "=" + readerProperties["SettingValue"].ToString() + "\n");
            }

            //Save it as our update file in case the current is in use
            File.WriteAllText(YAMS.Core.RootFolder + @"\servers\" + intServerID.ToString() + @"\config\server.properties.UPDATE", sb.ToString());
        }

        //Get and set settings
        public static bool SaveSetting(string strSettingName, string strSettingValue)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = connLocal;

            if (GetSetting(strSettingName, "YAMS") == null)
            {
                //Doesn't exist in DB already, so insert
                cmd.CommandText = "INSERT INTO YAMSSettings (SettingName, SettingValue) VALUES (@name, @value);";
            }
            else
            {
                //Exists, so need to update
                cmd.CommandText = "UPDATE YAMSSettings SET SettingValue=@value WHERE SettingName=@name;";
            }
            cmd.Parameters.Add("@name", strSettingName);
            cmd.Parameters.Add("@value", strSettingValue);
            cmd.ExecuteNonQuery();
            return true;
        }
        public static bool SaveSetting(int intServerID, string strSettingName, string strSettingValue)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = connLocal;

            if (GetSetting(strSettingName, "MC", intServerID) == null)
            {
                //Doesn't exist in DB already, so insert
                cmd.CommandText = "INSERT INTO MCSettings (SettingName, SettingValue, ServerID) VALUES (@name, @value, @id);";
            }
            else
            {
                //Exists, so need to update
                cmd.CommandText = "UPDATE MCSettings SET SettingValue=@value WHERE SettingName=@name AND ServerID=@id;";
            }
            cmd.Parameters.Add("@name", strSettingName);
            cmd.Parameters.Add("@value", strSettingValue);
            cmd.Parameters.Add("@id", intServerID);
            cmd.ExecuteNonQuery();
            return true;
        }

        public static string GetSetting(string strSettingName, string strType, int intServerID = 0)
        {
            String strTableName = "";

            switch (strType)
            {
                case "YAMS":
                    strTableName = "YAMSSettings";
                    break;
                case "MC":
                    strTableName = "MCSettings";
                    break;
            }

            try
            {
                SqlCeCommand cmd = new SqlCeCommand("SELECT SettingValue FROM " + strTableName + " WHERE SettingName = @name", connLocal);
                cmd.Parameters.Add("@name", strSettingName);
                string strSettingValue = (string)cmd.ExecuteScalar();
                return strSettingValue;
            }
            catch (Exception ex)
            {
                AddLog("YAMS.Database.GetSetting Exception: " + ex.Message, "database", "error");
                return "";
            }
        }

        public static object GetSetting(int intServerID, string strSettingName)
        {

            try
            {
                SqlCeCommand cmd = new SqlCeCommand("SELECT " + strSettingName + " FROM MCServers WHERE ServerID = @id", connLocal);
                cmd.Parameters.Add("@id", intServerID);
                var strSettingValue = cmd.ExecuteScalar();
                return strSettingValue;
            }
            catch (Exception ex)
            {
                AddLog("YAMS.Database.GetSetting Exception: " + ex.Message, "database", "error");
                return "";
            }
        }


        public static int NewServer(List<KeyValuePair<string, string>> listServer, string strServerTitle)
        {
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = connLocal;

            //Create the server and get an ID
            cmd.CommandText = "INSERT INTO MCServers (ServerTitle, ServerWrapperMode) VALUES (@title, 0)";
            cmd.Parameters.Add("@title", strServerTitle);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT @@IDENTITY";
            int intNewID = Convert.ToInt32(cmd.ExecuteScalar());

            //Insert the settings into the DB for this server
            foreach (var element in listServer)
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO MCSettings (ServerID, SettingName, SettingValue) VALUES (@id, @name, @value);";
                cmd.Parameters.Add("@id", intNewID);
                cmd.Parameters.Add("@name", element.Key);
                cmd.Parameters.Add("@value", element.Value);
                cmd.ExecuteNonQuery();
            }

            //Set up Files + Folders
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString())) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString());
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\");
            if (!Directory.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\world\")) Directory.CreateDirectory(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\world\");
            if (!File.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\intbanned-ips.txt")) File.Create(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\banned-ips.txt");
            if (!File.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\banned-players.txt")) File.Create(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\banned-players.txt");
            if (!File.Exists(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\ops.txt")) File.Create(YAMS.Core.RootFolder + @"\servers\" + intNewID.ToString() + @"\config\ops.txt");

            //Create default config files
            BuildServerProperties(intNewID);

            return intNewID;
        }

        public static void UpdateDB()
        {
            switch (Convert.ToInt32(GetSetting("DBSchema", "YAMS")))
            {
                case 1:
                    //Update from Schema 1
                    goto case 2;
                case 2:
                    //Update from Schema 2
                    //goto case 3; //etc
                default:
                    break;
            }

        }

        public static SqlCeDataReader GetServers()
        {
            SqlCeCommand comServers = new SqlCeCommand("SELECT * FROM MCServers", connLocal);
            SqlCeDataReader readerServers = null;
            readerServers = comServers.ExecuteReader();
            return readerServers;
        }

        ~Database()
        {
            connLocal.Close();
        }
    
    }
}
