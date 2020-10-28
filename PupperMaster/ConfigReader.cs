using System;

namespace PuppetMaster
{
    public class ConfigReader
    {
        private string file;

        private string[] pcss;

        public ConfigReader(string file)
        {
            this.file = file;

            this.pcss = System.IO.File.ReadAllLines(@"..\..\..\files\config\" + file);

        }

        public string[] Pcss { get => pcss; set => pcss = value; }

        public void printPcss() {
            System.Console.WriteLine("Process creation services available:");
            foreach (string line in pcss)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }

        }

    }
}