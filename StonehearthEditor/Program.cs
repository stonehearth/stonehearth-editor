using System;
using System.Windows.Forms;

namespace StonehearthEditor
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         string path = (string)Properties.Settings.Default["ModsDirectory"];
         if (args.Length > 0)
         {
            path = args[0];
         }
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new StonehearthEditor(path));
      }
   }
}
