using System;
using System.IO;
using Newtonsoft.Json;
using System.Timers;

namespace EasySave.Model
{

    public class StateFile
    {
        public string backupName;
        public DateTime timeStamp;
        public bool backupState;
        public int nbFiles;
        public long sizeFiles;
        public double progression;
        public int leftFiles;
        public long leftSize;
        public string sourcePath;
        public string destPath;
        private System.Timers.Timer timer;
        private string timestamp;

        public StateFile(string sourcePath, string destPath, string backupName)
        {
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
            this.sourcePath = sourcePath;
            this.destPath = destPath;
            this.backupName = backupName;
            this.nbFiles = 0;
            this.timeStamp = DateTime.Now;
            this.GetSourceState();
            this.timer = new System.Timers.Timer(10);
            timer.Elapsed += GetTargetState;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void endTimer()
        {
            this.timer.Stop();
            this.timer.Dispose();
        }

        public void GetSourceState()
        {
            //get source directory info (size, files)
            DirectoryInfo sourcedi = new DirectoryInfo(sourcePath);
            var sourcedirectories = sourcedi.GetFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo d in sourcedirectories)
            {
                this.sizeFiles += d.Length;
                this.nbFiles++;

            }
        }

        public void GetTargetState(Object source, ElapsedEventArgs e)
        {
            //get target directory info (size, files)
            DirectoryInfo targetdi = new DirectoryInfo(this.destPath);
            var targetdirectories = targetdi.GetFiles("*", SearchOption.AllDirectories);

            //tmp variables for target path
            long targetSize = 0;
            int targetFiles = 0;

            foreach (FileInfo d in targetdirectories)
            {
                targetSize += d.Length;
                targetFiles++;
            }

            this.leftSize = this.sizeFiles - targetSize;
            this.leftFiles = this.nbFiles - targetFiles;

            double tmp1 = targetSize / 1.0;
            double tmp2 = this.sizeFiles / 1.0;

            this.progression = tmp1 / tmp2 * 100;

            this.WriteStateFile();
        }


        private void WriteStateFile()
        {
            string pathToCreate = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string jsonPath = pathToCreate + "\\EasySave\\StateFile";

            if (!Directory.Exists(jsonPath)) Directory.CreateDirectory(jsonPath);

            string json = JsonConvert.SerializeObject(this);

            try
            {
            File.WriteAllText($"{jsonPath}\\statefile.json", json);
            }
            catch { }
        }


    }
}
