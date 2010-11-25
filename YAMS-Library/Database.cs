using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

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

        ~Database()
        {
            connLocal.Close();
        }

    }
}
