using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool debug = false;
            if (args.Length > 0)
                if (bool.TryParse(args[0], out debug));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            new JavaInstallerManager().JavaManagerInit(debug);
        }
    }
}
