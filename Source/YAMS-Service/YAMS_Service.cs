using System.Diagnostics;
using System.ServiceProcess;


namespace YAMS_Service
{
    public partial class YAMS_Service : ServiceBase
    {
        public static Process minecraftServer;
        
        public YAMS_Service()
        {
            InitializeComponent();
            this.ServiceName = "YAMS";
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("YAMS Startup", EventLogEntryType.Information);
            YAMS.Core.StartUp();
        }

        protected override void OnStop()
        {
            YAMS.Core.ShutDown();
            EventLog.WriteEntry("YAMS Shutdown", EventLogEntryType.Information);
        }
    }
}
