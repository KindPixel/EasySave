using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

namespace EasySave_Interface.Model
{
    public class CountInstance
    {
        public int NbInstance;


        public void CountCloseInstance()
        {
            Process[] localByName = Process.GetProcessesByName("EasySave_Interface");
            foreach (Process element in localByName)
            {
                NbInstance++;
            }

            if (NbInstance >= 2)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
