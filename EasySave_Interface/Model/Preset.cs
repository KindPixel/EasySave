using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace EasySave_Interface.Model
{
    public class Preset
    {
        public string sourcePath { get; set; }
        public string destPath { get; set; }

       public Preset (string sourcePath, string destPath)
        {
            this.sourcePath = sourcePath;
            this.destPath = destPath;
        }

        public static List<string> getPresetsLists()
        {
            string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EasySave\presets";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            DirectoryInfo dirSource = new DirectoryInfo($@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EasySave\presets");
            FileInfo[] files = dirSource.GetFiles("*.*", SearchOption.AllDirectories);

            List<string> presetsID = new List<string> { };

            foreach (FileInfo file in files)
            {
                string presetID = file.Name;

                presetsID.Add(presetID.Replace(file.Extension, ""));
            }

            return presetsID;
        }

        public static Preset getPreset(string presetID, string pathDoc)
        {
            string path = $@"{pathDoc}\EasySave\presets\{presetID}.json";

            if (!File.Exists(path)) return null;

            StreamReader read = new StreamReader(path);
            
            string jsonValue = read.ReadToEnd();
            Preset preset = JsonConvert.DeserializeObject<Preset>(jsonValue);
            read.Close();

            return preset;
        }


        public static void WritePreset(string name, string sourcePath, string destinationPath)
        {
            string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = $@"{pathDocument}\EasySave\presets";

            Preset preset = new Preset(sourcePath, destinationPath);
            string presetJson = JsonConvert.SerializeObject(preset);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllText($@"{path}\{name}.json", presetJson);
            
        }

        public static void DeletePreset(string presetID)
        {
            string pathJson = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EasySave\presets";
            File.Delete($@"{pathJson}\{presetID}.json");
        }
    }
}
