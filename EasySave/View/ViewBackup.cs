using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.View
{
    class ViewBackup
    {
        private Controller.Controller controller;
        public ViewBackup(Controller.Controller controller)
        {
            this.controller = controller;
        }

        public void PrintLaunched()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Backup: Starting");
            }
            else
            {
                Console.WriteLine("Sauvegarde: Commencement");
            }
        }

        public void PrintUnreachable()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Folder(s)/File(s) unreachable");
            }
            else
            {
                Console.WriteLine("Fichiers/Dossier inatteignable");
            }
        }

        public void PrintFileSaved(string fileName)
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine($"Name of the file that will updated : {fileName}");
            }
            else
            {
                Console.WriteLine($"Nom du ficher enregsitrer : {fileName}");
            }
        }

        public void PrintFastBackup()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Path of the source folder :");
            }
            else
            {
                Console.WriteLine("chemin du dossier source :");
            }
            string sourcePath = Console.ReadLine();

            string name = sourcePath.Substring(sourcePath.LastIndexOf(@"\") + 1);

            if (lang % 2 == 0)
            {
                Console.WriteLine("Path of the destination folder :");
            }
            else
            {
                Console.WriteLine("Chemin du dossier source :");
            }
            string destinationPath = Console.ReadLine();

            this.controller.LaunchBackup(name, sourcePath, destinationPath);
        }

        public void PrintBackupFinished()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Success !");
            }
            else
            {
                Console.WriteLine("succès !");
            }
        }

        public void PrintEmptyBackup()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Everything is already saved");
            }
            else
            {
                Console.WriteLine("Tout es déjà sauvegarder");
            }
        }

        public int PrintChoiceTypeBackup()
        {
            string[] choiceType;
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Choose an option by typing the associated key :");
            }
            else
            {
                Console.WriteLine("Choisissez l'option en tappant la touche associée :");
            }
            if (lang % 2 == 0)
            {
                choiceType = new string[] { "[F1] Complete save", "[F2] Differential save", "[F3] Incremential save" };
            }
            else
            {
                choiceType = new string[] { "[F1] Savegarde complete", "[F2] Sauvegarde différentiel", "[F3] Sauvegarde incrémentiel" };
            }

            foreach (var type in choiceType)
            {
                Console.WriteLine(type);
            }

            ConsoleKey response = Console.ReadKey().Key;
            Console.WriteLine();
            int choice;

            switch (response)
            {
                case ConsoleKey.F1:
                    choice = 1;
                    break;
                case ConsoleKey.F2:
                    choice = 2;
                    break;
                case ConsoleKey.F3:
                    choice = 3;
                    break;
                default:
                    if (lang % 2 == 0)
                    {
                        Console.WriteLine("Incorrect choose");
                    }
                    else
                    {
                        Console.WriteLine("Choix incorrect");
                    }
                    choice = 0;
                    this.PrintChoiceTypeBackup();
                    break;
            }
        return choice;
        }
    }
}
