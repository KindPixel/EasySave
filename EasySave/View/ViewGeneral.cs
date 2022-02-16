using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.View
{
    class ViewGeneral
    {
        private Controller.Controller controller;
        private string[] options;

        public ViewGeneral(Controller.Controller controller)
        {
            this.controller = controller;
            

        }

        public void FirstPrintTitle()
        {
            Console.WriteLine(@"  ______                 _____                 
 |  ____|               / ____|                
 | |__   __ _ ___ _   _| (___   __ ___   _____ 
 |  __| / _` / __| | | |\___ \ / _` \ \ / / _ \
 | |___| (_| \__ \ |_| |____) | (_| |\ V /  __/
 |______\__,_|___/\__, |_____/ \__,_| \_/ \___|
                   __/ |                       
                  |___/                        ");
            Console.WriteLine();
        }

        public void PrintMenu()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                this.options = new string[] { "[F1] Display preset", "[F2] Create a preset", "[F3] Fast backup", "[F4] Open logs folder", "[F5] Quit", "[F6] Switch English to French" };
            }
            else
            {
                this.options = new string[] { "[F1] Voir les preset", "[F2] Crée un preset", "[F3] Sauvegarde rapide", "[F4] Ouvrir le fichier log", "[F5] Quitter", "[F6] Passer du français à l'anglais" };
            }

            if (lang % 2 == 0)
            {
                Console.WriteLine("Choose an option by typing the associated key :");
            }
            else
            {
                Console.WriteLine("Choisissez l'option en tappant la touche associée :");
            }
            foreach (var option in this.options)
            {
                Console.WriteLine(option);
            }

            ConsoleKey response = Console.ReadKey().Key;
            Console.WriteLine();

            switch (response)
            {
                case ConsoleKey.F1:
                    this.controller.DisplayPresetMenu();
                    break;
                case ConsoleKey.F2:
                    this.controller.DisplayPresetCreation();
                    break;
                case ConsoleKey.F3:
                    this.controller.DisplayFastBackup();
                    break;
                case ConsoleKey.F4:
                    this.controller.DisplayLogFolder();
                    break;
                case ConsoleKey.F5:
                    Environment.Exit(0);
                    break;
                case ConsoleKey.F6:
                    this.controller.UpdateLang();
                    break;
                default:
                    Console.WriteLine("Choix incorrect/incorrect choose");
                    this.PrintMenu();
                    break;
            }
        }
        public void PrintChangeLang()
        { 
                int lang = this.controller.GetLang();
                if (lang % 2 == 0)
                {
                    Console.WriteLine("La langue est désormais le français");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("The langage is now english");
                    Console.WriteLine();
                }
        }

    }
}
