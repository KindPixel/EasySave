using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.View
{
    class ViewLog
    {
        private Controller.Controller controller;
        



        public ViewLog(Controller.Controller controller)
        {
            this.controller = controller;
        }

        public void PrintOpenMenu()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Folder open");
            }
            else
            {
                Console.WriteLine("Fichier ouvert");
            }
        }

        public void PrintOpenError()
        {
            int lang = this.controller.GetLang();
            if (lang % 2 == 0)
            {
                Console.WriteLine("Opening folder failed");
            }
            else
            {
                Console.WriteLine("L'ouverture des fichiers à echoué");
            }
            
        }
    }
}
