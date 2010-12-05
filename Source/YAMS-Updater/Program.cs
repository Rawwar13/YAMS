using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace YAMS_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** YAMS Updater ***");

            if (args[0] == "restart")
            {
                //We're just here to restart the service safely after some updates
                Console.WriteLine("Restarting service...");
                ServiceController scYAMS = new ServiceController("YAMS-Service");
                scYAMS.Stop();
                Console.WriteLine("Service stopped");
                scYAMS.Start();
                Console.WriteLine("Service started");

                Environment.Exit(0);
            }

            //Otherwise we're here to get the files needed to install the app
        }
    }
}
