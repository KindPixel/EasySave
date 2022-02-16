using EasySave_Interface.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace EasySave_Interface.ViewModel
{
    public class ViewModel
    {
        public MainWindow mainWindow;
        public Model.Settings settings;
        public List<Model.Backup> backups;
        private Model.StateFile stateFile;
        public Semaphore poolStateFile;
        public Semaphore poolLargeFile;
        private Model.SocketServer socketServer;


        public ViewModel(MainWindow mainWindow)
        {
            this.backups = new List<Model.Backup>();

            this.mainWindow = mainWindow;
            this.settings = new Model.Settings(this);
            this.socketServer = new Model.SocketServer(this);

            this.poolLargeFile = new Semaphore(1, 1);
            this.poolStateFile = new Semaphore(1, 1);
            this.stateFile = new Model.StateFile(poolStateFile, this);
        }

        public void CountAndClose()
        {
            Model.CountInstance countInstance = new Model.CountInstance();
            countInstance.CountCloseInstance();
        }

        public void PrepareBackup(string name, string sourcePath, string destinationPath, string type, bool encrypt)
        {
            Model.Backup backup = new Model.Backup( name, sourcePath, destinationPath, this, encrypt, type);

            Thread thread = new Thread(() => this.LaunchBackup(backup));
            thread.Name = name;

            thread.Start();
            this.backups.Add(backup);
        }


        private void LaunchBackup(Model.Backup backup)
        {
            try
            {
                this.mainWindow.printLaunch($"Backup - {backup.name}");
                backup.LaunchProcess();
                this.mainWindow.printSuccess($"Backup - {backup.name}");

            }
            catch (Exception e)
            {
                if(e is System.InvalidOperationException)
                {
                    this.mainWindow.PrintError("A professional software is runnig");
                }
               else if(e is System.IO.DirectoryNotFoundException)
                {
                    this.mainWindow.PrintError($"Folder(s) / File(s) unreachable");
                }
                else
                {
                    this.mainWindow.PrintError("Unknown error");
                }
            }
            finally
            {
                //this.backups.Remove(backup);
            }
        }

        public void DisplayFileSaved(string fileName)
        {
            this.mainWindow.printFileSaved(fileName);
        }

        public List<string> getPresetList()
        {
            return Model.Preset.getPresetsLists();
        }

        public void setPreset(string presetID)
        {
            string pathDoc = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}";

            Model.Preset preset = Model.Preset.getPreset(presetID, pathDoc);

            if (preset != null)
            {
                this.mainWindow.initField(preset.sourcePath, preset.destPath, presetID);
            } 
            else
            {
                this.mainWindow.PrintError("Unknown Preset ID");
            }
        }

        public void DeletePreset(string presetID)
        {
            Model.Preset.DeletePreset(presetID);
        }

        public void CreatePreset(string name, string sourcePath, string destinationPath)
        {
            Model.Preset.WritePreset(name, sourcePath, destinationPath);
        }

        public void addSoft(string nameProcess)
        {
            this.settings.addProProcess(nameProcess);
        }

        public void removeSoft(string nameProcess)
        {
            this.settings.removeProProcess(nameProcess);
        }

        public void DisplayLogFolder()
        {
            string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Model.Model.checkorCreate($@"{pathDocument}\EasySave\logs", null);
            Process.Start("explorer.exe", $@"{pathDocument}\EasySave\logs");
        }

        public void PauseBackup()
        {
            this.backups.ForEach(backup =>
            {
                backup.blockBackupReset();
            });
        }

        public void PauseBackup(Backup backup)
        {
            backup.blockBackupReset();
        }

        public void ResumeBackup(Backup backup)
        {
            backup.blockBackupSet();
        }

        public void StopBackup(Backup backup)
        {
            backup.stop = true;
        }

        public void Warning(string content)
        {
            this.mainWindow.PrintWarning(content);
        }

        public void LaunchSS()
        {
            this.socketServer.ToConnect();
        }

        public void DisplaySocket(string texte)
        {
            this.mainWindow.PrintSocket(texte);
        }

        public void LaunchSSDis()
        {
            this.socketServer.Close(); ;
        }

        internal StateFile getStateFile()
        {
            return this.stateFile;
        }
    }
}
