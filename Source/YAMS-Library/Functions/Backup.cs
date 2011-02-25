using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using YAMS;

namespace YAMS
{
    public static class Backup
    {

        public static void BackupNow(MCServer s)
        {
            Database.AddLog("Backing up " + s.ServerTitle, "backup");

            //Check for a backup dir and create if not
            if (!Directory.Exists(s.ServerDirectory + @"\backups\")) Directory.CreateDirectory(s.ServerDirectory + @"\backups\");
                        
            //Force a save
            s.Save();
            s.DisableSaving();

            //Copy world to a temp Dir
            if (Directory.Exists(Core.RootFolder + @"\servers\" + s.ServerID.ToString() + @"\backups\temp\")) Directory.Delete(Core.RootFolder + @"\servers\" + s.ServerID.ToString() + @"\backups\temp\", true);
            if (!Directory.Exists(Core.RootFolder + @"\servers\" + s.ServerID.ToString() + @"\backups\temp\")) Directory.CreateDirectory(Core.RootFolder + @"\servers\" + s.ServerID.ToString() + @"\backups\temp\");
            Util.Copy(s.ServerDirectory + @"\world\", s.ServerDirectory + @"\backups\temp\");

            //Re-enable saving then force another save
            s.EnableSaving();
            s.Save();

            //Now zip up temp dir and move to backups
            FastZip z = new FastZip();
            z.CreateEmptyDirectories = true;
            z.CreateZip(s.ServerDirectory + @"\backups\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".zip", s.ServerDirectory + @"\backups\temp\", true, "");
        }

    }
}
