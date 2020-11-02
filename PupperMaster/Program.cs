using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        static void Main(string[] args)
        {

            ConfigReader cr = null;

            if (args.Length > 0)
                cr = new ConfigReader(args[0]);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(cr));
        }
    }
}
