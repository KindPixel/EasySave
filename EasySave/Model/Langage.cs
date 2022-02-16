using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.Model
{
    class Langage
    {
        public int lang = 0;
        private Controller.Controller controller;
        public Langage(Controller.Controller controller)
        {
            this.controller = controller;
        }
        public void ChangeLangage()
        {
            lang = lang+1;
            this.controller.DisplayMenu();
        }

        public int UseLang()
        {
            return lang;
        }
    }
}
