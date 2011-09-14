using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using YAMS;
using System.Reflection;
using System.IO;

namespace YAMS_Gui
{
    public static class Program
    {
        public static YAMS.MCServer myServer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (File.Exists(new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + @"\lib\ExceptionManager.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + @"\lib\ExceptionManager.dll");
                Type type = assembly.GetTypes()[12];
                //var obj = Activator.CreateInstance(type);
                type.GetMethods()[0].Invoke(null,
                  BindingFlags.Default | BindingFlags.InvokeMethod,
                  null,
                  null,
                  null);
                //UnhandledExceptionManager.AddHandler();
            }
            else
            {
                MessageBox.Show("not found");
            }

            YAMS.Core.StartUp();
            //Database.init();
            //Util.FirstRun();
            //WebServer.Init();
            //WebServer.StartPublic();
            //WebServer.StartAdmin();
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
