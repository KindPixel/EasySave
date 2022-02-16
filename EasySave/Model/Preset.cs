using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace EasySave.Model
{
    public class Preset
    {
        public string name { get; set; }
        public string sourcePath { get; set; }
        public string destPath { get; set; }

       public Preset (string name, string sourcePath, string destPath)
        {
            this.name = name;
            this.sourcePath = sourcePath;
            this.destPath = destPath;
        }

        public static List<string> getPresetsLists()
        {
            List<string> presetsID = new List<string> { };
            string pathJson = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            for (int id = 1; id <= 5; id++)
            {
                Preset preset = Preset.getPreset(id.ToString(), pathJson);
                if (preset != null)
                {
                    presetsID.Add($"[{id}] - Preset {preset.name}");
                }
                else
                {
                    presetsID.Add($"[{id}] - Available Slot");
                }
            }
            return presetsID;
        }

        public static Preset getPreset(string presetID, string pathJson)
        {
            if (!File.Exists($@"{pathJson}\EasySave\presets\preset{presetID}.json")) return null;

            StreamReader read = new StreamReader($@"{pathJson}\EasySave\presets\preset{presetID}.json");
            
            string jsonValue = read.ReadToEnd();
            Preset preset = JsonConvert.DeserializeObject<Preset>(jsonValue);
            read.Close();

            return preset;
        }


        public static void WritePreset(string jsonString,string id)
        {
            string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = $@"{pathDocument}\EasySave\\presets";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllText($@"{path}\preset{id}.json", jsonString);
            
        }
    }
}
