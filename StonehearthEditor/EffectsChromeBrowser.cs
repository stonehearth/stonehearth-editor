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
        private string mEffectKind;
        private string mJson;
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
            cSettings.RemoteDebuggingPort = 8088;
            Cef.Initialize(cSettings);

            // Open main page
            mChromeBrowser = new ChromiumWebBrowser(GetPagePath("main.html"));
            mEffectsJsObject = new EffectsJsObject();
            mChromeBrowser.RegisterJsObject("EffectsJsObject", mEffectsJsObject);
            mChromeBrowser.LoadError += MChromeBrowser_LoadError;
            panel.Controls.Add(mChromeBrowser);
            mChromeBrowser.Dock = DockStyle.Fill;
        }

        private void MChromeBrowser_LoadError(object sender, LoadErrorEventArgs e)
        {
            MessageBox.Show("file type not supported yet");
        }

        public void LoadFromJson(string effectKind, string json, Action<string> saveAction)
        {
            mEffectKind = effectKind;
            mJson = json;
            this.Refresh();
            mEffectsJsObject.saveAction = saveAction;
        }

        public void RunScript(string script)
        {
            mChromeBrowser.EvaluateScriptAsync(script).Wait();
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
        public class RenderProcessMessageHandler : IRenderProcessMessageHandler
        {
            // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
            // If the page has no javascript, no context will be created.
            void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
                frame.ExecuteJavaScriptAsync(
                        string.Format(
                            @"
                       document.addEventListener('DOMContentLoaded', function() {{
                            CsApi.effectKind = ""{0}"";
                            CsApi.json = {1};
                        }});",
                            effectKind,
                            json));
            }

            public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
            {
            }

            private string effectKind;
            private string json;

            public RenderProcessMessageHandler(string effectKind, string json)
            {
                this.effectKind = effectKind;
                this.json = json;
            }
        }

        public void Refresh()
        {
            // HAX HAX HAX !!!
            if (Directory.Exists(@"..\..\..\pages"))
            {
                Directory.Delete("pages", true);
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = @"C:\WINDOWS\system32\xcopy.exe";
                proc.StartInfo.Arguments = @"..\..\..\pages pages /E /I";
                proc.Start();
                proc.WaitForExit();
            }
            mChromeBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler(mEffectKind, mJson);
            mChromeBrowser.Reload(true);
        }
    }
}
