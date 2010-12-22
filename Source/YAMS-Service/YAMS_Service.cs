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
        }

        protected override void OnStart(string[] args)
        {
            YAMS.Core.StartUp();
        }

        protected override void OnStop()
        {
            YAMS.Core.ShutDown();
        }
    }
}
