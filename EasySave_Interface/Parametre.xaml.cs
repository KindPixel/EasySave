using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasySave_Interface
{
    /// <summary>
    /// Logique d'interaction pour Parametre.xaml
    /// </summary>
    public partial class Parametre : Window
    {
        private ViewModel.ViewModelSettings viewModelSettings;
        public MainWindow mainWindow;

        public Parametre(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            this.viewModelSettings = new ViewModel.ViewModelSettings(this);
            this.ProProcessInit();
            this.ExtensionInit();
            this.FileSize.Text = this.viewModelSettings.getLargeSizeFile().ToString();
        }

       

        private void ProProcessInit()
        {
            List<string> proProcessData = this.viewModelSettings.getProProcessList();
            this.proProcessList.Items.Clear();
            proProcessData.ForEach(proProcessName =>
            {
                this.proProcessList.Items.Add(proProcessName);
            });
        }

        private void addSoft_Click(object sender, RoutedEventArgs e)
        {
            if (this.softName.Text == "") return;

            string name = softName.Text;

            this.viewModelSettings.AddSoft(name);
            this.ProProcessInit();
        }

        private void delSoft_Click(object sender, RoutedEventArgs e)
        {
            if (this.proProcessList.SelectedItem == null) return;

            string name = this.proProcessList.SelectedItem.ToString();

            this.viewModelSettings.RemoveSoft(name);
            this.ProProcessInit();
        }

        private void ExtensionInit()
        {
            List<string> extensionList = this.viewModelSettings.GetExtensionList();
            this.ExtList.Items.Clear();
            extensionList.ForEach(extension =>
            {
                this.ExtList.Items.Add(extension);
            });
        }

        private void addExt_Click(object sender, RoutedEventArgs e)
        {
            this.viewModelSettings.AddExtension(this.ExtName.Text);
            this.ExtensionInit();
        }

        private void delExt_Click(object sender, RoutedEventArgs e)
        {
            this.viewModelSettings.RemoveExtension(this.ExtName.Text);
            this.ExtensionInit();
        }

        private void openLogs_Click(object sender, RoutedEventArgs e)
        {
            this.viewModelSettings.DisplayLogFolder();
        }

        private void Lang_Click(object sender, RoutedEventArgs e)
        {
            switch (this.lang.Content)
            {
                case "Switch to FR":
                    this.lang.Content = "Switch to US";
                    this.mainWindow.NameOfThePreset.Text = "Nom du preset";
                    this.mainWindow.savePreset.Content = "Enregistrer le preset";
                    this.mainWindow.SourcePath.Text = "Chemin source";
                    this.mainWindow.DestPath.Text = "Chemin de destination";
                    this.mainWindow.encryption.Content = "Activer le cryptage";
                    this.mainWindow.launch.Content = "Lancer la sauvegarde";                   
                    openLogs.Content = "Ouvrir les logs";
                    this.mainWindow.quit.Content = "Quitter";
                    this.mainWindow.ListOfPresets.Text = "Liste des Preset";
                    this.mainWindow.delPreset.Content = "Supprimer un preset";
                    ListSofttxt.Text = "List des logiciels pro";
                    addSoft.Content = "Ajouter";
                    delSoft.Content = "Supprimer";
                    this.mainWindow.differential.Content = "Différentiel";
                    this.mainWindow.incremential.Content = "Incrementiel";
                    this.mainWindow.complete.Content = "Complète";
                    this.mainWindow.SaveType.Text = "Type de la sauvegarde";
                    PriotaryExt.Text = "Extension prioritaire (Séparer avec un ; )";
                    simultaneousFileSize.Text = "Taille max des fichier simultané";
                    this.mainWindow.GoToParametre.Content = "Aller au parametre";
                    ExtNameBox.Text = "Nom de l'extension";
                    addExt.Content = "Ajouter";
                    delExt.Content = "Supprimer";
                    this.mainWindow.socketOpen.Content = "Socket Serveur";
                    this.mainWindow.socketClose.Content = "Deconnecter";
                    break;
                default:
                    this.lang.Content = "Switch to FR";
                    this.mainWindow.NameOfThePreset.Text = "Name of the preset";
                    this.mainWindow.savePreset.Content = "Save as a preset";
                    this.mainWindow.SourcePath.Text = "Source Path";
                    this.mainWindow.DestPath.Text = "Dest Path";
                    this.mainWindow.encryption.Content = "Enable encryption";
                    this.mainWindow.launch.Content = "Launch Backup";
                    openLogs.Content = "Open logs folder";
                    this.mainWindow.quit.Content = "Quit";
                    this.mainWindow.ListOfPresets.Text = "List Of Presets";
                    this.mainWindow.delPreset.Content = "Delete";
                    ListSofttxt.Text = "List of professional soft";
                    addSoft.Content = "Add";
                    delSoft.Content = "Delete";
                    this.mainWindow.differential.Content = "Differential";
                    this.mainWindow.incremential.Content = "Incremential";
                    this.mainWindow.complete.Content = "Complete";
                    this.mainWindow.SaveType.Text = "Type of the save mod";
                    PriotaryExt.Text = "List of priority extension";
                    simultaneousFileSize.Text = "Size of simultaeous file";
                    this.mainWindow.GoToParametre.Content = "Go to parametre";
                    ExtNameBox.Text = "Name of the extension";
                    addExt.Content = "Add";
                    delExt.Content = "Delete";
                    this.mainWindow.socketOpen.Content = "Socket Server";
                    this.mainWindow.socketClose.Content = "Diconnect";
                    break;
            }


        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FileSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            int size = 0;
            if(Int32.TryParse(this.FileSize.Text,out size))
            {
                this.viewModelSettings.UpdateLargeSizeFile(size);
            }
        }

    }
}
