using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace CryptoSoft
{
    public class FonctionXor
    {
        private readonly static string key = "loremipFsumdolorsitamDeconsectetuadipisPcingelit";
        public long Fonction_Xor(string sourcefile, string resultfile)
        {

            byte[] buffer = new byte[2048];

            XORMelangeur Scrambler = new XORMelangeur(key);
            Stopwatch stopWatch = new Stopwatch();
            
            try
            {
                //Read the source file and create a new file destination
                FileStream iStream = new FileStream(sourcefile, FileMode.Open);
                FileStream oStream = new FileStream(resultfile, FileMode.CreateNew);

                int read;
                stopWatch.Start();
                while ((read = iStream.Read(buffer, 0, 2048)) > 0)
                {
                    oStream.Write(Scrambler.scramble(buffer), 0, read);
                }
                stopWatch.Stop();
                iStream.Close();
                oStream.Flush();
                oStream.Close();

                buffer = null;
                
                return stopWatch.ElapsedMilliseconds;
            }
            catch (Exception Ex)
            {
                Console.WriteLine("ERREUR : " + Ex);
                return -1;
            }
        }

        public class XORMelangeur
        {
            byte[] key;

            // Load the key and put in parametre
            public XORMelangeur(string keystring)
            {
                System.Text.ASCIIEncoding encodedData = new System.Text.ASCIIEncoding();
                // Et on la stocke dans un table de bytes
                key = encodedData.GetBytes(keystring);
            }

            // Xor the two binary file
            public byte[] scramble(byte[] b)
            {
                byte[] r = new byte[b.Length];
                for (int i = 0; i < b.Length; i++)
                {
                    r[i] = (byte)(b[i] ^ key[i % key.Length]);
                }
                return r;
            }
        }
    }
}
