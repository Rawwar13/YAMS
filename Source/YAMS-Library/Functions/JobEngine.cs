using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlServerCe;
using YAMS;

namespace YAMS
{
    public class JobEngine
    {

        public Timer timJob;

        public void Init()
        {
            //Tick every minute
            timJob = new Timer(new TimerCallback(Tick), null, 0, 1 * 60 * 1000);
        }

        public void Tick(object t)
        {
            DateTime datNow = DateTime.Now;
            int intMinutes = datNow.Minute;
            int intHour = datNow.Hour;
            SqlCeDataReader rdJobs = Database.GetJobs(intHour, intMinutes);

            MCServer s;
            
            while (rdJobs.Read())
            {
                switch (rdJobs["JobAction"].ToString()) {
                    case "overviewer":
                        s = Core.Servers[Convert.ToInt32(rdJobs["JobServer"])];
                        AddOns.Overviewer gmap = new AddOns.Overviewer(s, rdJobs["JobParams"].ToString());
                        gmap.Start();
                        break;
                    case "biome-extractor":
                        s = Core.Servers[Convert.ToInt32(rdJobs["JobServer"])];
                        AddOns.BiomeExtractor extractor = new AddOns.BiomeExtractor(s, rdJobs["JobParams"].ToString());
                        extractor.Start();
                        break;
                    default:
                        Database.AddLog("Invalid entry in Job database", "job", "warn");
                        break;
                }
            }
        }
    }
}
