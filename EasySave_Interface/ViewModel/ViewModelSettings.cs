using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EasySave_Interface.ViewModel
{
    class ViewModelSettings
    {
        private Model.Settings settings;
        private Parametre parametre;

        public ViewModelSettings(Parametre parametre)
        {
            this.parametre =  parametre;
            this.settings = parametre.mainWindow.viewModel.settings;
        }

        public List<string> getProProcessList()
        {
            return this.settings.proAppsName;
        }

        public void AddSoft(string nameProcess)
        {
            this.settings.addProProcess(nameProcess);
        }

        public void RemoveSoft(string nameProcess)
        {
            this.settings.removeProProcess(nameProcess);
        }

        public List<string> GetExtensionList()
        {
            return this.settings.extensions;
        }

        public void AddExtension(string nameExtension)
        {
            this.settings.addExtension(nameExtension);
        }

        public void RemoveExtension(string nameExtension)
        {
            this.settings.removeExtension(nameExtension);
        }

        public int getLargeSizeFile()
        {
            return this.settings.largeSize;
        }

        public void UpdateLargeSizeFile(int size)
        {
            this.settings.UpdateLargeFileSize(size);
        }

        public void DisplayLogFolder()
        {
            string pathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Model.Model.checkorCreate($@"{pathDocument}\EasySave\logs", null);
            Process.Start("explorer.exe", $@"{pathDocument}\EasySave\logs");
        }
    }
}
