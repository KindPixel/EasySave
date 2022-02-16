using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace EasySave_Interface.Model
{
    public class Backup
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string sourcePath;
        public string destPath;
        public bool encrypt;
        public string type;

        public double filesNumber;
        public double filesSize;
        public double progression { get; set; }
        public double leftFiles;
        public double leftSize;

        public bool stop { get; set; }
        public bool stopped { get => !stop; }
        public bool running { get => this.blockBackup.WaitOne(0) && !stop; }
        public bool paused { get => !this.blockBackup.WaitOne(0) && !stop; }
        public string date;
        private ViewModel.ViewModel viewModel;
        private List<LogFile> logsFile;
        private Log log;
        private ManualResetEvent blockBackup;


        public Backup(string name, string sourcePath, string destPath, ViewModel.ViewModel viewModel ,bool encrypt, string type)
        {
            if (sourcePath == "" | destPath == "")
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourcePath);
            }

            this.name = name;
            this.sourcePath = sourcePath;
            this.destPath = destPath;
            this.encrypt = encrypt;

            this.filesNumber = 0;
            this.filesSize = 0;
            this.leftFiles = 0;
            this.leftSize = 0;

            this.date = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            this.viewModel = viewModel;
            this.type = type;
            this.logsFile = new List<LogFile>();
            this.stop = false;
            this.blockBackup = new ManualResetEvent(false);
            this.blockBackup.Set();
            this.ID = $"{name} - {this.date}";
        }

        public void LaunchProcess() {
            try
            {
                Model.checkorCreate($@"{this.destPath}\{this.name}", null);
                this.DirectoryCopy(this.sourcePath, this.destPath);

               switch (this.type)
                {
                    case "differential":
                        this.differential();
                        break;
                    case "incremential":
                        this.incremential();
                        break;
                    default:
                        this.complete();
                        break;
                }
            } 
            finally
            {
                this.log = new Log(this.name, this.sourcePath, this.destPath, this.logsFile.ToArray(), this.date);
                this.log.Writelog();
                this.progression = 100;
                this.stop = true;
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

        private void incremential()
        {
            List<string> dirbackups = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));
            string comparePath = null;
            bool final = false;

            if (dirbackups.Count != 0)
            {
                dirbackups.Sort();
                comparePath = dirbackups.First();
                int i = 0;

                if (dirbackups.Count == 1) final = true;

                CompareandWrite(this.sourcePath, comparePath, i, "incremential", final);

                dirbackups.Remove(comparePath);

                dirbackups.ForEach(delegate (string destdir)
                {
                    i++;
                    List<string> dirbackup = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));
                    dirbackup.Sort();
                    string incPath = dirbackup.Last();

                    if (i == dirbackups.Count) final = true;
                        
                    CompareandWrite(incPath, destdir, i, "incremential", final);
                    DeleteDirectory(incPath);

                });
            }
            else
            {
                complete();
            }

        }

        private void differential()
        {
            List<string> dirbackup = new List<string>(Directory.EnumerateDirectories($@"{this.destPath}\{this.name}"));


            dirbackup.Contains("complete");
            string comparePath = null;

            if (dirbackup.Count != 0)
            {
                dirbackup.Sort();
                comparePath = dirbackup.Last();
                int i = 0;
                CompareandWrite(this.sourcePath, comparePath, i, "differential", true);
            }
            else
            {
                complete();
            }

        }

        private void complete()
        {
            Directory.CreateDirectory($@"{this.destPath}\{this.name}\temp");
            string comparePath = $@"{this.destPath}\{this.name}\temp";


            int i = 0;
            CompareandWrite(this.sourcePath, comparePath, i, "complete", true);
            IfEmpty(comparePath);
        }

        private void CompareandWrite(string sourcePath, string comPath, int i, string typeBackup, bool final)
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
            string dirdest = $@"{this.destPath}\{this.name}\{this.date}{i} - {typeBackup}";
            if (!Directory.Exists(dirdest)) Directory.CreateDirectory(dirdest);

            files = this.sortFiles(files.ToList());

            if (final) this.initState(files.ToArray());

            foreach (FileInfo file in files)
            {
                string fileSourcePath = $@"{sourcePath}\{file.Name}";
                string fileDestPath = $@"{dirdest}\{file.Name}";

                if (File.Exists(fileDestPath)) File.Delete(fileDestPath);

                string tempPath = Path.Combine(dirdest, file.Name);

                //Play, Pause, Stop
                this.viewModel.settings.blockAllBackups.WaitOne();
                this.blockBackup.WaitOne();
                if (this.stop) return;
                
                //Large File
                if (file.Length > (this.viewModel.settings.largeSize * 8192)) this.viewModel.poolLargeFile.WaitOne();

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                if (this.encrypt)
                {
                    string path = "../../../CryptoSoft/CryptoSoft.exe";

                    Process process = new Process();
                    process.StartInfo.FileName = path;

                    process.StartInfo.ArgumentList.Add(fileSourcePath);
                    process.StartInfo.ArgumentList.Add(fileDestPath);

                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                }
                else 
                {
                    file.CopyTo(tempPath, false);
                }
                stopWatch.Stop();

                if (file.Length > this.viewModel.settings.largeSize) this.viewModel.poolLargeFile.Release();

                if (final)
                {
                    this.leftFiles--;
                    this.leftSize -= file.Length;
                    this.updateProgression();
                    this.viewModel.DisplayFileSaved(file.Name);

                    this.logsFile.Add(new LogFile(
                        this.name,
                        fileSourcePath,
                        fileDestPath,
                        file.Length,
                        stopWatch.ElapsedMilliseconds
                    ));
                }
            }
        }

        private void initState(FileInfo[] files)
        {

            foreach (FileInfo file in files)
            {
                this.filesNumber++;
                this.filesSize += file.Length;
            }

            this.leftFiles = this.filesNumber;
            this.leftSize = this.filesSize;

        }

        public void updateProgression()
        {
            double destSize = this.filesSize - this.leftSize;
            this.progression = destSize / this.filesSize * 100;
        }

        private List<FileInfo> sortFiles(List<FileInfo> files)
        {
            List<FileInfo> filesSorted = new List<FileInfo>();
            
            //Add first priority file
            foreach(FileInfo file in files.ToArray())
            {
                string fileExtension = Path.GetExtension(file.Name);
                bool priority = this.viewModel.settings.extensions.Contains(fileExtension);

                if (priority) 
                {
                    filesSorted.Add(file);
                    files.Remove(file);
                }
            }

            //Add non-priority file
            foreach (FileInfo file in files)
            {
                filesSorted.Add(file);
            }

            return filesSorted;
        }

        public void blockBackupSet()
        {
            this.blockBackup.Set();
        }

        public void blockBackupReset()
        {
            this.blockBackup.Reset();
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
            }
            else { }
        }
        
    }
}
