using System;
using System.IO;

namespace CryptoSoft
{
    class Program
    {
        static void Main(string[] args)
        {
            FonctionXor crypt = new FonctionXor();

            string sourcePath = args[0];
            string destinationPath = args[1];

            File.Delete(destinationPath);
            Console.WriteLine("Encrypting file | " + args[0]);

            long timestamp = crypt.Fonction_Xor(sourcePath, destinationPath);
        }
    }
}