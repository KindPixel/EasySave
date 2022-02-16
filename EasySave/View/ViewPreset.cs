using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.View
{
    class ViewPreset
    {
        private Controller.Controller controller;
        public ViewPreset(Controller.Controller controller)
        {
            this.controller = controller;
        }

        public void printLaunchMenu()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Display Preset");
                Console.WriteLine("Which preset do you want to launch :");
            }
            else
            {
                Console.WriteLine("Affichage des presets");
                Console.WriteLine("Quel preset voulez-vous utiliser :");
            }

            this.PrintAllSlot();

            string response = Console.ReadLine();
            this.controller.LaunchPreset(response);
        }

        public void printCreationMenu()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Settings Creation Preset");
                Console.WriteLine("Which slot do you want use :");
            }
            else
            {
                Console.WriteLine("Menue de creation de preset");
                Console.WriteLine("Quel emplacement voulez-vous utiliser :");
            }
            this.PrintAllSlot();
            string id = Console.ReadLine();

            if (id == "q")
            {
                this.controller.DisplayMenu();
            }
            else
            {
                this.PrintCreationMenu2(id);
            }
        }

        private void PrintCreationMenu2(string id)
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Path of the source folder :");
            }
            else
            {
                Console.WriteLine("Chemin du dossier source :");
            }
            string sourcePath = Console.ReadLine();

            if (lang % 2 == 0)
            {
                Console.WriteLine("Path of the destination folder :");
            }
            else
            {
                Console.WriteLine("Chemin du dossier source :");
            }
            string destinationPath = Console.ReadLine();

            if (lang % 2 == 0)
            {
                Console.WriteLine("Name of the backup :");
            }
            else
            {
                Console.WriteLine("Nom de la sauvegarde :");
            }
            string name = Console.ReadLine();

            this.controller.CreatePreset(name, sourcePath, destinationPath, id);

            if (lang % 2 == 0)
            {
                Console.WriteLine("The preset was saved");
            }
            else
            {
                Console.WriteLine("Le preset est enregistrer");
            }
            this.controller.DisplayMenu();
        }

        public void AskWantCreatPreset(string id)
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("The chosen preset doesn't exist...");
                Console.WriteLine("Do you want create a new preset ? (y/N)");
            }
            else
            {
                Console.WriteLine("Le preset choisis n'existe pas...");
                Console.WriteLine("Voulez-vous crée un nouveau preset ? (y/N)");

            }
            ConsoleKey response = Console.ReadKey().Key;
            Console.WriteLine();
            if (response == ConsoleKey.Y)
            {
                this.PrintCreationMenu2(id);
            }
            else
            {
                this.controller.DisplayMenu();
            }
        }

        public void PrintPresetIDError()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Unknown Preset ID");
            }
            else
            {
                Console.WriteLine("Preset d'ID inconnu");
            }
        }

        private void PrintAllSlot()
        {
            List<string> presetsIDs = this.controller.getPresetsList();

            foreach (var presetID in presetsIDs)
            {
                Console.WriteLine(presetID);
            }
        }
    }
}
