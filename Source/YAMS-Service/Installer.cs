using System.ComponentModel;
using System.ServiceProcess;


namespace YAMS_Service
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            ServiceProcessInstaller serviceProcessInstaller =
                               new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information

            serviceInstaller.DisplayName = "YAMS";
            serviceInstaller.Description = "Allows running multiple Minecraft Servers as a Windows Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            serviceInstaller.ServiceName = "YAMS_Service";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
