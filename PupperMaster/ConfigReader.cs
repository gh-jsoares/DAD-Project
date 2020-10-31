using System;

namespace PuppetMaster
{
    public class ConfigReader
    {
        private string file;

        private string[] initialSetup;

        public ConfigReader(string file)
        {
            this.file = file;

            this.initialSetup = System.IO.File.ReadAllLines(@"..\..\..\files\scripts\" + file);

        }

        public string[] InitialSetup { get => initialSetup; set => initialSetup = value; }
    }
}