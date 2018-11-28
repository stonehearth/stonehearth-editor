using System;
using System.Windows.Forms;

namespace StonehearthEditor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            string path = (string)Properties.Settings.Default["ModsDirectory"];
            string steamUploadsPath = (string)Properties.Settings.Default["SteamUploadsDirectory"];
            if (args.Length > 0)
            {
                path = args[0];
            }
            if (args.Length > 1)
            {
                steamUploadsPath = args[1];
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(path, steamUploadsPath));
        }
    }
}
