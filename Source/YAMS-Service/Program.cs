using System.ServiceProcess;
using System.IO;
using System.Reflection;
using System;
using ExceptionManager;

namespace YAMS_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            UnhandledExceptionManager.AddHandler();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new YAMS_Service() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
