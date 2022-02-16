using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace EasySave.Controller
{
    class Controller
    {
        private View.ViewGeneral viewGeneral;
        private View.ViewBackup viewBackup;
        private View.ViewPreset viewPreset;
        private View.ViewLog viewLog;
        private Model.Settings settings;
        private Model.Langage langage;


        public Controller()
        {
            this.langage = new Model.Langage(this);
            this.viewGeneral = new View.ViewGeneral(this);
            this.viewBackup = new View.ViewBackup(this);
            this.viewPreset = new View.ViewPreset(this);
            this.viewLog = new View.ViewLog(this);          
            this.settings = Model.Settings.getSettingsObject(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            this.viewGeneral.FirstPrintTitle();
            this.DisplayMenu();

            //this.settings.addProProcess("Notepad", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            //this.settings.removeProProcess("Notepad", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        }


        public void DisplayMenu()
        {
            this.viewGeneral.PrintMenu();
        }

        public void DisplayPresetMenu()
        {
            this.viewPreset.printLaunchMenu();
        }

        public void DisplayPresetCreation()
        {
            this.viewPreset.printCreationMenu();
        }

        public void DisplayFastBackup()
        {
            this.viewBackup.PrintFastBackup();
        }

        public void DisplayFileSaved(string filename)
        {
            this.viewBackup.PrintFileSaved(filename);
        }

        public void DisplayEmptySave()
        {
            this.viewBackup.PrintEmptyBackup();
        }

        public void LaunchBackup(string name, string sourcePath, string destinationPath)
        {
            try
            {
                settings.checkProProcessRunning();
            }
            catch
            {
                //Gerer les exceptions
            }

            Model.Backup backup = new Model.Backup(name, sourcePath, destinationPath, this);
            this.viewBackup.PrintLaunched();

            try
            {
                backup.LaunchProcess();
                int typeOfBackup = this.viewBackup.PrintChoiceTypeBackup();

                switch ( typeOfBackup )
                {
                    case 1:
                        backup.complete();
                        break;
                    case 2:
                        backup.differential();
                        break;
                    case 3:
                        backup.incremential();
                        break;
                }
                
            } 
            catch
            {
                this.viewBackup.PrintUnreachable();
            }
            finally
            {
                this.viewBackup.PrintBackupFinished();
                this.DisplayMenu();
            }
        }

        public void DisplayLogFolder()
        {
            this.viewLog.PrintOpenMenu();
            
            try
            {
                string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Model.Model.checkorCreate($@"{pathDocument}\EasySave\logs", null);
                Process.Start("explorer.exe", $@"{pathDocument}\EasySave\logs");
            }
            catch 
            {
                this.viewLog.PrintOpenError();
            }
            finally
            {
                this.DisplayMenu();
            }
        }

        public List<string> getPresetsList()
        {
            string path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EasySave\presets";

            Model.Model.checkorCreate(path, null);
            int Count = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;

            List<string> presetsID = new List<string> { };
            string pathJson = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            for (int id = 1; id <= Count; id++)
            {
                Model.Preset preset = Model.Preset.getPreset(id.ToString(), pathJson);
                if (preset != null)
                {
                    presetsID.Add($"[{id}] - Preset {preset.name}");

                }
                else
                {
                    //presetsID.Add($"[{id}] - Available Slot");
                }
            }
            presetsID.Add($"[{Count + 1}] - Available Slot");
            presetsID.Add($"[q] - return");

            return presetsID;
        }

        public void LaunchPreset(string presetID)
        {
            Regex rx = new Regex(@"^[0-9]+$");
            Boolean matches = rx.IsMatch(presetID);
            

            string pathJson = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Model.Preset preset = Model.Preset.getPreset(presetID, pathJson);
            if (preset != null)
            {
                this.LaunchBackup(preset.name, preset.sourcePath, preset.destPath);
            }
            else if (matches)
            {
                this.viewPreset.AskWantCreatPreset(presetID);
            }
            else if (presetID == "q")
            { }
            else
            {
                this.viewPreset.PrintPresetIDError();
            }

            this.DisplayMenu();
        }

        public void CreatePreset(string name, string sourcePath, string destinationPath, string id)
        {
            Model.Preset preset = new Model.Preset(name, sourcePath, destinationPath);
            string presetJson = JsonConvert.SerializeObject(preset);
            Model.Preset.WritePreset(presetJson, id);
        }

        public void UpdateLang()
        {
            this.viewGeneral.PrintChangeLang();
            this.langage.ChangeLangage();
        }

        public int GetLang()
        {
            return this.langage.lang;
        }
    }
}