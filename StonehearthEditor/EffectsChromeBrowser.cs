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
using Newtonsoft.Json.Linq;
using StonehearthEditor.Effects;

namespace StonehearthEditor
{
    public class EffectsChromeBrowser
    {
        private static readonly string myPath = Application.StartupPath;
        private static readonly string myPages = Path.Combine(myPath, "pages");
        private static EffectsChromeBrowser sInstance = null;
        private bool isFrameLoaded = false;
        private ChromiumWebBrowser mChromeBrowser;
        private EffectsJsObject mEffectsJsObject;

        public static EffectsChromeBrowser GetInstance()
        {
            if (sInstance == null)
            {
                sInstance = new EffectsChromeBrowser();
            }

            return sInstance;
        }

        private EffectsChromeBrowser()
       {
        }

        public void InitBrowser(Panel panel)
        {
            CefSettings cSettings = new CefSettings();
            Cef.Initialize(cSettings);

            // Open main page
            mChromeBrowser = new ChromiumWebBrowser(GetPagePath("main.html"));
            panel.Controls.Add(mChromeBrowser);
            mChromeBrowser.Dock = DockStyle.Fill;

            ExposeObjects();
        }

        private void ExposeObjects()
        {
            // Test javascript
            mChromeBrowser.RegisterJsObject("eventHandler", new Effects.EffectsEventHandler());

            mEffectsJsObject = new EffectsJsObject();
            mChromeBrowser.RegisterJsObject("effectsJsObject", mEffectsJsObject);
        }

        public void LoadFromJson(JObject jObject)
        {
            // TODO: check if file is a cubemitter vs animated light
            Property cubemitter = EffectKinds.Cubemitter;
            PropertyValue value = cubemitter.FromJson(jObject);
            UpdatePropertyValue(cubemitter, value);
        }

        public void UpdatePropertyValue(Property property, PropertyValue propertyValue)
        {
            mEffectsJsObject.property = property;
            mEffectsJsObject.propertyValue = propertyValue;
        }

        public void RunScript(string script)
        {
            if (!isFrameLoaded)
            {
                mChromeBrowser.FrameLoadEnd += (sender, args) =>
                {
                    // Wait for the MainFrame to finish loading
                    if (args.Frame.IsMain)
                    {
                        isFrameLoaded = true;
                        args.Frame.ExecuteJavaScriptAsync(script);
                    }
                };
            }
            else
            {
                mChromeBrowser.ExecuteScriptAsync(script);
            }
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
            return Path.Combine(myPages, pageName);
        }

        private void SwitchPage(string pageName)
        {
            mChromeBrowser.Load(GetPagePath(pageName));
        }
    }
}
