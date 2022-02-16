using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EasySave.Model
{
    class Model
    {
        public static void checkorCreate(string pathdir, string namefile)
        {
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            if (namefile != null)
            {
                if (!File.Exists(namefile))
                {
                    File.Create(namefile);
                }
            }
        }
    }
}
