using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;



namespace EasySave_Interface.Model
{
    public class Settings
    {
        public List<string> proAppsName;
        public List<string> extensions;
        public int largeSize;
        private List<ProApp> proApps;
        public List<string> proAppRunning;
        public ManualResetEvent blockAllBackups;
        private ViewModel.ViewModel viewModel;
        private string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        

        private class FileSettings
        {
            public List<string> proAppsName;
            public List<string> extensions;
            public int largeSize;

            public FileSettings()
            {
                this.proAppsName = new List<string>();
                this.extensions = new List<string>();
                this.largeSize = 0;
            }
        }

        public Settings(ViewModel.ViewModel viewModel)
        {
            FileSettings fileSettings = this.GetFileSettingsObject();
            this.viewModel = viewModel;
            this.proApps = new List<ProApp>();
            this.extensions = fileSettings.extensions;
            
            this.proAppRunning = new List<string>();
            this.blockAllBackups = new ManualResetEvent(false);
            this.blockAllBackups.Set();
            this.proAppsName = fileSettings.proAppsName;
            this.initiateWatchers();

            this.largeSize = fileSettings.largeSize;


        }

        private FileSettings GetFileSettingsObject()
        {
            if (!Directory.Exists($@"{pathDocument}\EasySave")) Directory.CreateDirectory($@"{pathDocument}\EasySave");
            if (!File.Exists($@"{pathDocument}\EasySave\settings.json")) this.initiateSettingsFile();

            StreamReader file = new StreamReader($@"{pathDocument}\EasySave\settings.json");
            string json = file.ReadToEnd();

            file.Close();
            return JsonConvert.DeserializeObject<FileSettings>(json);
        }

        private void initiateSettingsFile()
        {
            string content = JsonConvert.SerializeObject(new FileSettings());

            File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
        }

        public void UpdateLargeFileSize(int size)
        {
            this.largeSize = size;

            string content = JsonConvert.SerializeObject(this);
            File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
        }

        private void initiateWatchers()
        {
            this.proAppsName.ForEach(name =>
            {
                this.proApps.Add(new ProApp(name, this));
            });
        }


        public void addProProcess(string nameProcess)
        {
            if(!this.proAppsName.Contains(nameProcess))
            {
                this.proAppsName.Add(nameProcess);
                string content = JsonConvert.SerializeObject(this);
                this.proApps.Add(new ProApp(nameProcess, this));

                File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }

        public void removeProProcess(string nameProcess)
        {
            if (this.proAppsName.Contains(nameProcess))
            {
                foreach(ProApp app in proApps.ToArray())
                {
                    if (app.Name == nameProcess)
                    {
                        app.stopWatcher = true;
                        this.proApps.Remove(app);
                    }

                }

                this.proAppsName.Remove(nameProcess);
                string content = JsonConvert.SerializeObject(this);

                File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }

        public void addExtension(string nameExtension)
        {
            if (!this.extensions.Contains(nameExtension))
            {
                this.extensions.Add(nameExtension);
                string content = JsonConvert.SerializeObject(this);
                File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }

        public void removeExtension(string nameExtension)
        {
            if (this.extensions.Contains(nameExtension))
            {
                this.extensions.Remove(nameExtension);
                string content = JsonConvert.SerializeObject(this);
                File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }

        public void proAppRun(string appName)
        {
            if (this.proAppRunning.Exists(x => x == appName)) return;
            this.proAppRunning.Add(appName);
            this.blockAllBackups.Reset();
            this.viewModel.Warning($"{appName} is running");
        }

        public void proAppShutdown(string appName)
        {
            if (!this.proAppRunning.Exists(x => x == appName)) return;
            if (this.proAppRunning.Count > 0) this.blockAllBackups.Set();
            this.proAppRunning.Remove(appName);
            this.viewModel.Warning($"{appName} has stopped");
        }

    }
}
