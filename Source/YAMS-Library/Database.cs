using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Text;
using System.IO;

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
            string dbfile = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\dbYAMS.sdf";
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

        public static void AddLog(string strMessage, string strSource = "app", string strLevel = "info", bool bolSendToAdmin = false)
        {
            if (strMessage == null) strMessage = "Null message received";

            string sqlIns = "INSERT INTO Log (LogSource, LogMessage, LogLevel) VALUES (@source, @msg, @level)";
            try
            {
                SqlCeCommand cmdIns = new SqlCeCommand(sqlIns, connLocal);
                cmdIns.Parameters.Add("@source", strSource);
                cmdIns.Parameters.Add("@msg", strMessage);
                cmdIns.Parameters.Add("@level", strLevel);
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
        public static void BuildServerProperties()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Minecraft server properties: built by YAMS\n");
            //sb.Append("#Sun Nov 28 19:26:26 GMT 2010\n");

            SqlCeCommand comProperties = new SqlCeCommand("SELECT * FROM MCSettings", connLocal);
            SqlCeDataReader readerProperties = null;
            readerProperties = comProperties.ExecuteReader();
            while (readerProperties.Read())
            {
                sb.Append(readerProperties["SettingName"].ToString() + "=" + readerProperties["SettingValue"].ToString() + "\n");
            }

            //Save it as our update file in case the current is in use
            File.WriteAllText(Directory.GetCurrentDirectory() + @"\config\server.properties.UPDATE", sb.ToString());

        }

        //Get and set settings
        public static bool SaveSetting(string strSettingName, string strSettingValue, string strType = "YAMS")
        {
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.Connection = connLocal;

            String strTableName = "";

            switch (strType)
            {
                case "YAMS":
                    strTableName = "YAMS";
                    break;
                case "MC":
                    strTableName = "MC";
                    break;
            }

            if (GetSetting(strSettingName, strType) == null)
            {
                //Doesn't exist in DB already, so insert
                cmd.CommandText = "INSERT INTO " + strTableName + " (SettingName, SettingValue) VALUES (@name, @value);";
            }
            else
            {
                //Exists, so need to update
                cmd.CommandText = "UPDATE " + strTableName + " SET SettingValue=@value WHERE SettingName=@name;";
            }
            cmd.Parameters.Add("@name", strSettingName);
            cmd.Parameters.Add("@value", strSettingValue);
            cmd.ExecuteNonQuery();
            return true;
        }

        public static string GetSetting(string strSettingName, string strType)
        {
            String strTableName = "";

            switch (strType)
            {
                case "YAMS":
                    strTableName = "YAMS";
                    break;
                case "MC":
                    strTableName = "MC";
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

        ~Database()
        {
            connLocal.Close();
        }

    }
}
