using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace StonehearthEditor
{
    public class EffectsChromeBrowser
    {
        private static EffectsChromeBrowser sInstance = null;
        private static readonly string myPath = Application.StartupPath;
        private static readonly string htmlResources = Path.Combine(myPath, "htmlResources");
        private static readonly string htmlPages = Path.Combine(htmlResources, "html");
        private ChromiumWebBrowser mChromeBrowser;

        public static EffectsChromeBrowser GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new EffectsChromeBrowser();
            }

            return sInstance;
        }

        public EffectsChromeBrowser()
        {
        }

        public void InitBrowser(Panel panel)
        {
            CefSettings cSettings = new CefSettings();
            Cef.Initialize(cSettings);
            mChromeBrowser = new ChromiumWebBrowser(GetPagePath("cubemitter.html"));
            mChromeBrowser.Dock = DockStyle.Fill;
            panel.Controls.Add(mChromeBrowser);
        }

        public void LoadGameTest()
        {
            string temp = "C:/Users/lcai/radiant/stonehearth/build/x86/source/stonehearth/RelWithDebInfo/Stonehearth.exe";
            string args = "--game.main_mod=stonehearth_tests --mods.stonehearth_tests.test=effect_test";
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = temp;
            startInfo.Arguments = args;
            startInfo.WorkingDirectory = "C:/Users/lcai/radiant/stonehearth/source/stonehearth_data";
            Process.Start(startInfo);
        }

        public void ShowDevTools()
        {
            mChromeBrowser.ShowDevTools();
        }

        private string GetPagePath(string pageName)
        {
            return Path.Combine(htmlPages, pageName);
        }

        private void SwitchPage(string pageName)
        {
            mChromeBrowser.Load(GetPagePath(pageName));
        }
    }
}

//string exeLocation, args;

//OpenFileDialog fileDialog = new OpenFileDialog();
//fileDialog.Title = "Open Stonehearth.exe";
//fileDialog.DefaultExt = ".exe";
//if (fileDialog.ShowDialog() == DialogResult.OK)
//{
//    exeLocation = fileDialog.FileName;
//}

//FolderBrowserDialog folderDialog = new FolderBrowserDialog();
//folderDialog.Description = "Set working directory (location so stonehearth_data)";
//if (folderDialog.ShowDialog() == DialogResult.OK)
//{
//    args = folderDialog.SelectedPath;
//}
// TODO: Use these file paths instead of hardcoded paths

// Launch game with effect test runninig
// TODO: Download mod test folder with effect test if it does not exist in mods folder
