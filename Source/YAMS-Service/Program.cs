using System.ServiceProcess;
using System.IO;
using System.Reflection;
using System;

//using ExceptionManager;

namespace YAMS_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (File.Exists(new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + @"\lib\ExceptionManager.dll"))
            {
                //Load in the exception handler and start it up
                Assembly assembly = Assembly.LoadFrom(new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + @"\lib\ExceptionManager.dll");
                Type type = assembly.GetTypes()[12];
                //var obj = Activator.CreateInstance(type);
                type.GetMethods()[0].Invoke(null,
                  BindingFlags.Default | BindingFlags.InvokeMethod,
                  null,
                  null,
                  null);
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new YAMS_Service() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
