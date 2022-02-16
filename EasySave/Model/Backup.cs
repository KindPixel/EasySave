using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.Json;


namespace EasySave.Model
{
    class Backup
    {
        private string name;
        private string sourcePath;
        private string destPath;
        private DateTime dateHour;
        private Controller.Controller controller;
        private List<LogFile> logsFile;
        private Log log;
        private StateFile stateFile;


        public Backup(string name, string sourcePath, string destPath, Controller.Controller controller)
        {
            DateTime date = DateTime.Now;
            

            this.name = name;
            this.sourcePath = sourcePath;
            this.destPath = destPath;
            this.dateHour = date;
            this.controller = controller;
            this.logsFile = new List<LogFile>();
            this.stateFile = new StateFile(sourcePath, destPath, name);
        }

        public void LaunchProcess() {
            try
            {
                Model.checkorCreate($@"{this.destPath}\{this.name}", null);
                this.DirectoryCopy(this.sourcePath, this.destPath);
            } 
            finally
            {
                this.log = new Log(this.name, this.sourcePath, this.destPath, this.logsFile.ToArray(), this.dateHour);
                this.log.Writelog();
                this.stateFile.endTimer();
            }
        }

        private void DirectoryCopy(string sourcePath, string destPath)
        {
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(sourcePath);
            System.IO.DirectoryInfo dirDest = new System.IO.DirectoryInfo(destPath);

            if (!dirSource.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourcePath);
            }
        }

        public void incremential()
        {
            List<string> dirbackup = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));
            string comparePath = null;

            if (dirbackup.Count != 0)
            {
                dirbackup.Sort();
                comparePath = dirbackup.First();
                int i = 0;
                CompareandWrite(this.sourcePath, comparePath, i, "incremential");

                dirbackup.Remove(comparePath);
                i++;
                dirbackup.ForEach(delegate (string destdir)
                {
                    List<string> dirbackup = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));
                    dirbackup.Sort();
                    string incPath = dirbackup.Last();
                    CompareandWrite(incPath, destdir, i, "incremential");
                    i++;
                    DeleteDirectory(incPath);

                });
                IfEmpty(dirbackup.Last());
            }
            else
            {
                complete();
            }
                
        }

        public void differential()
        {
            List<string> dirbackup = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));
            dirbackup.Contains("complete");
            string comparePath = null;
            
            if (dirbackup.Count != 0)
            {
                dirbackup.Sort();
                comparePath = dirbackup.Last();
                int i = 0;
                CompareandWrite(this.sourcePath, comparePath, i, "differential");
            }
            else
            {
                complete();
            }
            
        }

        public void complete()
        {
            Directory.CreateDirectory($@"{this.destPath}\{this.name}\temp");
            string comparePath = $@"{this.destPath}\{this.name}\temp";
            int i=0;
            CompareandWrite(this.sourcePath, comparePath, i, "complete");
            IfEmpty(comparePath);
        }

        private void CompareandWrite(string sourcePath, string comPath, int i, string typeBackup)
        {
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(sourcePath);
            System.IO.DirectoryInfo dirCom = new System.IO.DirectoryInfo(comPath);

            IEnumerable<System.IO.FileInfo> listSource = dirSource.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.FileInfo> listDest = dirCom.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare();

            var files = (from file in listSource select file).Except(listDest, myFileCompare);

            // Get the subdirectories for the specified directory.
            DirectoryInfo[] dirsSource = dirSource.GetDirectories();
            
            // If the destination directory doesn't exist, create it.       
            string dirdest = $@"{this.destPath}\{this.name}\{this.dateHour.ToString("yyyy-MM-dd_HH.mm.ss")}{i} - {typeBackup}";
            if (!Directory.Exists(dirdest)) Directory.CreateDirectory(dirdest);
;           
            foreach (FileInfo file in files)
            {
                string fileSourcePath = $@"{sourcePath}\{file.Name}";
                string fileDestPath = $@"{dirdest}\{file.Name}";

                if (File.Exists(fileDestPath)) File.Delete(fileDestPath);

                //this.controller.DisplayFileSaved(file.Name);

                string tempPath = Path.Combine(dirdest, file.Name);

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                file.CopyTo(tempPath, false);


                stopWatch.Stop();

                this.logsFile.Add(new LogFile(
                    this.name,
                    fileSourcePath,
                    fileDestPath,
                    file.Length,
                    stopWatch.ElapsedMilliseconds
                ));
            }

            foreach (DirectoryInfo subdir in dirsSource)
            {
                string tempPath = Path.Combine(dirdest, subdir.Name);
                this.DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public void IfEmpty(string path)
        {
            if (Directory.GetFileSystemEntries(path).Length == 0)
            {
                Directory.Delete(path);
                //this.controller.DisplayEmptySave();
            }
            else { }
        }
    }
}
