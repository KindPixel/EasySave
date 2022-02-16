using System;
using System.IO;
using Newtonsoft.Json;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

namespace EasySave_Interface.Model
{

    public class StateFile
    {
        private ViewModel.ViewModel viewModel;
        private Semaphore poolStateFile;
        private System.Timers.Timer timer;

        public StateFile(Semaphore poolStateFile, ViewModel.ViewModel viewModel)
        {
            this.poolStateFile = poolStateFile;
            this.viewModel = viewModel;
            this.timer = new System.Timers.Timer(750);

            timer.Elapsed += ((Object source, ElapsedEventArgs e) => this.WriteStateFile());
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        public void WriteStateFile()
        {
            this.poolStateFile.WaitOne();
            string pathToCreate = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string jsonPath = pathToCreate + "\\EasySave\\StateFile";

            if (!Directory.Exists(jsonPath)) Directory.CreateDirectory(jsonPath);

            string json = JsonConvert.SerializeObject(this.viewModel.backups);

            try
            {
                File.WriteAllText($"{jsonPath}\\statefile.json", json);
            }
            finally 
            {
                this.poolStateFile.Release();
            }
        }


    }
}
