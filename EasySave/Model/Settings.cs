using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using Newtonsoft.Json;



namespace EasySave.Model
{
    class Settings
    {
        public List<string> proProcess { get; }

        private Settings()
        {
            this.proProcess = new List<string>();
            Model model = new Model();
        }

        public static Settings getSettingsObject(string pathDocument)
        {
            Settings settings = new Settings();

            if (!File.Exists($@"{pathDocument}\EasySave\settings.json")) settings.initiateSettingsFile(pathDocument);

            else
            {
                StreamReader file = new StreamReader($@"{pathDocument}\EasySave\settings.json");
                string json = file.ReadToEnd();

                settings = JsonConvert.DeserializeObject<Settings>(json);

                file.Close();
            }

            return settings;
        }

        public void initiateSettingsFile(string pathDocument)
        {
            string content = JsonConvert.SerializeObject(this);
            Model.checkorCreate($@"{pathDocument}\EasySave","settings.json");
            File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
        }

        public void checkProProcessRunning()
        {
            this.proProcess.ForEach((string processName) =>
            {
                Process[] process = Process.GetProcessesByName(processName);

                if (process.Length > 0)
                {
                    throw new System.InvalidOperationException("A profesionnal process is running");
                }
            });
        }

        public void addProProcess(string nameProcess, string pathDocument)
        {
            if(!this.proProcess.Contains(nameProcess))
            {
            this.proProcess.Add(nameProcess);
            string content = JsonConvert.SerializeObject(this);

            File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }

        public void removeProProcess(string nameProcess, string pathDocument)
        {
            if (this.proProcess.Contains(nameProcess))
            {
                this.proProcess.Remove(nameProcess);
                string content = JsonConvert.SerializeObject(this);

                File.WriteAllText($@"{pathDocument}\EasySave\settings.json", content);
            }
        }
    }
}
