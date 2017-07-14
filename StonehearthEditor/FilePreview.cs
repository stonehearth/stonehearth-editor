using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using Color = System.Drawing.Color;

namespace StonehearthEditor
{
    public partial class FilePreview : UserControl
    {
        private FileData mFileData;
        private string mI18nLocKey = null;
        private IReloadable mOwner;
        private int maxLineNumberCharLength;

        /// <summary>
        /// Index of the indicator for i18n()
        /// </summary>
        private const int kI18nIndicator = 8;

        /// <summary>
        /// Index of the indicator for file() and file-ish things
        /// </summary>
        private const int kFileIndicator = 9;

        /// <summary>
        /// Indicator for i18n()
        /// </summary>
        private Indicator mI18nIndicator => this.textBox.Indicators[kI18nIndicator];

        /// <summary>
        /// Indicator for file() and file-ish things
        /// </summary>
        private Indicator mFileIndicator => this.textBox.Indicators[kFileIndicator];

        public FilePreview(IReloadable owner, FileData fileData)
        {
            mFileData = fileData;
            InitializeComponent();
            textBox.Text = mFileData.FlatFileData;
            mOwner = owner;

            this.configureScintilla();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            mFileData.TrySetFlatFileData(textBox.Text);
        }

        private void textBox_MouseMove(object sender, MouseEventArgs e)
        {
            // Translate mouse xy to character position
            var position = this.textBox.CharPositionFromPoint(e.X, e.Y);
            var locKey = this.getLocKey(position);

            if (locKey == this.mI18nLocKey)
                return;

            this.mI18nLocKey = locKey;

            if (locKey == null)
            {
                this.textBox.CallTipCancel();
                return;
            }

            try
            {
                // Translate and display it as a tip
                var translated = ModuleDataManager.GetInstance().LocalizeString(locKey);
                translated = JsonHelper.WordWrap(translated, 100).Trim();
                this.textBox.CallTipShow(position, translated);
            }
            catch (Exception)
            {
                this.textBox.CallTipShow(position, $"(Uncaught exception while trying to find i18n for {locKey})");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (!mFileData.TrySetFlatFileData(textBox.Text))
            {
                MessageBox.Show("Unable to save " + mFileData.FileName + ". Invalid Json");
                return;
            }

            mFileData.TrySaveFile();

            this.textBox.SetSavePoint();
            TabPage parentControl = Parent as TabPage;
            if (parentControl != null)
            {
                int caretPosition = textBox.SelectionStart;
                textBox.Text = mFileData.FlatFileData;
                textBox.SelectionStart = caretPosition;
                textBox.ScrollCaret();
                parentControl.Text = mFileData.FileName;
            }
        }

        private void textBox_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            var text = e.Text;

            // Replace tabs with 3 whitespaces
            text = text.Replace("\t", "   ");
            var x = this.textBox.Lines.Select(l => l.Indentation).ToList();

            // TODO: Handle newlines and indention here somehow? Or can Indentation do that?

            // Return the modified text.
            e.Text = text;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
            }
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(mFileData.Path);
        }

        private void openFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(mFileData.Path));
        }

        private void saveFile_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void localizeFile_Click(object sender, EventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            string generateLocPythonFile = Application.StartupPath + "/scripts/generate_loc_keys.py";
            start.FileName = generateLocPythonFile;
            string filePath = mFileData.Path;
            string modsRoot = ModuleDataManager.GetInstance().ModsDirectoryPath;
            start.Arguments = string.Format("-r {0} {1}", modsRoot, filePath);
            ////MessageBox.Show("executing command: " + generateLocPythonFile + " -r " + modsRoot + " " + filePath);

            Process myProcess = Process.Start(start);
            if (myProcess == null)
            {
                MessageBox.Show("Could not launch generate_loc_keys.py");
                return;
            }

            myProcess.WaitForExit();
            if (mOwner != null)
            {
                mOwner.Reload();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessTabKey(bool forward)
        {
            return false;
        }

        private void insertAliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AliasSelectionDialog aliasDialog = new AliasSelectionDialog(new AliasSelectCallback(textBox));
            aliasDialog.ShowDialog();
        }

        private class AliasSelectCallback : AliasSelectionDialog.IDialogCallback
        {
            private Scintilla mTextBox;

            public AliasSelectCallback(Scintilla textbox)
            {
                mTextBox = textbox;
            }

            public bool OnAccept(IEnumerable<string> aliases)
            {
                // Enquotes every single alias and joins it using "\n,"
                string aliasInsert = string.Join(
                    "," + Environment.NewLine,
                    aliases.Select(alias => string.Concat('"', alias, '"')));

                mTextBox.InsertText(mTextBox.SelectionStart, aliasInsert);
                mTextBox.SelectionEnd = mTextBox.SelectionEnd + aliasInsert.Length;
                return true;
            }

            public void OnCancelled()
            {
            }
        }

        private class EditLocStringCallback : InputDialog.IDialogCallback
        {
            private string mLocKey;

            public EditLocStringCallback(string key)
            {
                mLocKey = key;
            }

            public bool OnAccept(string inputMessage)
            {
                string key = mLocKey;
                string[] split = key.Split(':');
                string modName = "stonehearth";

                if (split.Length > 1)
                {
                    modName = split[0];
                    key = split[1];
                }

                Module mod = ModuleDataManager.GetInstance().GetMod(modName);
                if (mod == null)
                {
                    MessageBox.Show("Could not change loc key. There is no mod named " + modName);
                    return true;
                }

                JValue token = mod.EnglishLocalizationJson.SelectToken(key) as JValue;
                if (token == null)
                {
                    // No such localization key. Try to add it.
                    string[] tokenSplit = key.Split('.');
                    JObject parent = mod.EnglishLocalizationJson;
                    for (int i = 0; i < tokenSplit.Length - 1; ++i)
                    {
                        if (parent == null)
                        {
                            break;
                        }

                        if (parent[tokenSplit[i]] == null)
                        {
                            parent[tokenSplit[i]] = new JObject();
                        }

                        parent = parent[tokenSplit[i]] as JObject;
                    }

                    if (parent == null)
                    {
                        MessageBox.Show("Could not insert localization token " + mLocKey);
                        return true;
                    }

                    parent.Add(tokenSplit[tokenSplit.Length - 1], inputMessage);
                }
                else
                {
                    token.Value = inputMessage;
                }

                mod.WriteEnglishLocalizationToFile();
                return true;
            }

            public void OnCancelled()
            {
            }
        }

        private void editLocStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mI18nLocKey == null)
            {
                return;
            }

            EditLocStringCallback callback = new EditLocStringCallback(mI18nLocKey);
            string translated = ModuleDataManager.GetInstance().LocalizeString(mI18nLocKey);
            InputDialog dialog = new InputDialog("Edit Loc String", "Edit Loc Text For: " + mI18nLocKey, translated, "Edit");
            dialog.SetCallback(callback);
            dialog.ShowDialog();
        }

        private void filePreviewContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (mI18nLocKey != null)
            {
                editLocStringToolStripMenuItem.Visible = true;
            }
            else
            {
                editLocStringToolStripMenuItem.Visible = false;
            }
        }

        /// <summary>
        /// Sets up some more things about <see cref="textBox"/>.
        /// </summary>
        private void configureScintilla()
        {
            // Make sure we start proper
            textBox.SetSavePoint();
            textBox.EmptyUndoBuffer();

            // Set the default style
            textBox.StyleResetDefault();
            textBox.Styles[Style.Default].Font = "Consolas";
            textBox.Styles[Style.Default].Size = 10;
            textBox.Styles[Style.Default].ForeColor = Color.Black;
            textBox.Margins[0].Width = 16;
            textBox.StyleClearAll();

            // Based on the extension, we need to choose the right lexer/style
            switch (System.IO.Path.GetExtension(mFileData.Path))
            {
                case ".lua":
                case ".luac":
                    this.configureLuaHighlighting();
                    break;
                case ".js":
                case ".json":
                    this.configureJsonHighlighting();
                    break;
                case ".css":
                case ".less":
                    textBox.Lexer = ScintillaNET.Lexer.Css;
                    break;
                case ".html":
                case ".htm":
                    textBox.Lexer = ScintillaNET.Lexer.Html;
                    break;
                case ".md":
                    textBox.Lexer = ScintillaNET.Lexer.Markdown;
                    break;
                case ".xml":
                    textBox.Lexer = ScintillaNET.Lexer.Xml;
                    break;
            }

            this.textBox.Styles[Style.CallTip].SizeF = 8.25f;
            this.textBox.Styles[Style.CallTip].ForeColor = Color.Black;
            this.textBox.Styles[Style.CallTip].BackColor = Color.White;
            this.textBox.Styles[Style.CallTip].Font = "Verdana";
            this.textBox.Styles[Style.CallTip].Hotspot = true;

            this.textBox.TextChanged += (sender, e) => this.restyleDocument();
            this.restyleDocument();
        }

        /// <summary>
        /// Configures Scintilla for Json highlighting
        /// </summary>
        private void configureJsonHighlighting()
        {
            textBox.Lexer = ScintillaNET.Lexer.Cpp;

            var scintilla = textBox;
            scintilla.Styles[ScintillaNET.Style.Cpp.Number].Bold = true;
            scintilla.Styles[ScintillaNET.Style.Cpp.Number].ForeColor = Color.Navy;
            scintilla.Styles[ScintillaNET.Style.Cpp.Number].Weight = 700;
            scintilla.Styles[ScintillaNET.Style.Cpp.String].ForeColor = Color.Purple;
            scintilla.Styles[ScintillaNET.Style.Cpp.Identifier].Bold = true;
            scintilla.Styles[ScintillaNET.Style.Cpp.Identifier].ForeColor = Color.ForestGreen;
            scintilla.Styles[ScintillaNET.Style.Cpp.Identifier].Weight = 700;

            // Prepare indicator
            this.mI18nIndicator.Style = IndicatorStyle.FullBox;
            this.mI18nIndicator.ForeColor = Color.CornflowerBlue;
            this.mI18nIndicator.HoverForeColor = Color.CadetBlue;
            this.mI18nIndicator.Alpha = 50;

            this.mFileIndicator.Style = IndicatorStyle.FullBox;
            this.mFileIndicator.ForeColor = Color.Green;
            this.mFileIndicator.HoverForeColor = Color.DarkGreen;
            this.mFileIndicator.Alpha = 50;
        }

        /// <summary>
        /// Helper used in <see cref="restyleDocument"/>
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="targetTransform"></param>
        private void indicate(string pattern, Action targetTransform = null)
        {
            textBox.TargetWholeDocument();

            while (textBox.SearchInTarget(pattern) != -1)
            {
                // Invoke transformation, if any was submitted
                targetTransform?.Invoke();

                // Only indicate if there's something to indicate
                if (textBox.TargetStart != textBox.TargetEnd)
                    textBox.IndicatorFillRange(textBox.TargetStart, textBox.TargetEnd - textBox.TargetStart);
                textBox.TargetStart = textBox.TargetEnd;
                textBox.TargetEnd = textBox.TextLength;
            }
        }

        /// <summary>
        /// Called whenever a change was made;
        /// used to re-style the whole document.
        /// Sets indicators.
        /// </summary>
        private void restyleDocument()
        {
            var scintilla = this.textBox;

            // Reset all indicators
            scintilla.IndicatorClearRange(0, scintilla.TextLength);

            // Search for fitting texts
            scintilla.SearchFlags = SearchFlags.Regex | SearchFlags.Posix;

            scintilla.IndicatorCurrent = kI18nIndicator;
            this.indicate(@"i18n\(.+?\)");
            scintilla.IndicatorCurrent = kFileIndicator;
            this.indicate(@"""[^"" ]*?:[^"" ]*?""", this.transformFileNames);
            this.indicate(@"file\(.+?\)", this.transformFileNames);
            this.indicate(@"""[^""]*?/[^""]*?""", this.transformFileNames);

            // Update line number margin width
            // From https://github.com/jacobslusser/ScintillaNET/wiki/Displaying-Line-Numbers
            var maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength != this.maxLineNumberCharLength)
            {
                const int padding = 2;
                scintilla.Margins[0].Width = scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
                this.maxLineNumberCharLength = maxLineNumberCharLength;
            }
        }

        /// <summary>
        /// Helper function for <see cref="restyleDocument"/>/<see cref="indicate(string, Action)"/>
        /// </summary>
        private void transformFileNames()
        {
            var text = this.textBox.TargetText;

            // Trim quotes if necessary
            if (text.StartsWith(@"""") && text.EndsWith(@""""))
            {
                this.textBox.TargetStart++;
                this.textBox.TargetEnd--;
                text = text.Substring(1, text.Length - 2);
            }

            // If it's not a valid file, we won't highlight it
            if (!this.isValidFileName(text))
                this.textBox.TargetStart = this.textBox.TargetEnd;
        }

        /// <summary>
        /// Configures Scintilla for lua highlighting
        /// </summary>
        private void configureLuaHighlighting()
        {
            var scintilla = textBox;
            scintilla.Lexer = ScintillaNET.Lexer.Lua;

            scintilla.Styles[ScintillaNET.Style.Lua.Comment].BackColor = Color.LightCyan;
            scintilla.Styles[ScintillaNET.Style.Lua.Comment].FillLine = true;
            scintilla.Styles[ScintillaNET.Style.Lua.Comment].ForeColor = Color.Green;
            scintilla.Styles[ScintillaNET.Style.Lua.CommentLine].ForeColor = Color.Green;
            scintilla.Styles[ScintillaNET.Style.Lua.Number].ForeColor = Color.DarkCyan;
            scintilla.Styles[ScintillaNET.Style.Lua.Word].ForeColor = Color.Navy;
            scintilla.Styles[ScintillaNET.Style.Lua.String].ForeColor = Color.BlueViolet;
            scintilla.Styles[ScintillaNET.Style.Lua.Character].ForeColor = Color.BlueViolet;
            scintilla.Styles[ScintillaNET.Style.Lua.StringEol].BackColor = Color.BlueViolet;
            scintilla.Styles[ScintillaNET.Style.Lua.StringEol].ForeColor = Color.White;
            scintilla.Styles[ScintillaNET.Style.Lua.Word2].BackColor = Color.Maroon;

            scintilla.SetKeywords(0, "and break do else elseif end for function if in local nil not or repeat return then until while");
        }

        /// <summary>
        /// Returns the index of the indicator at <paramref name="position"/>
        /// </summary>
        /// <param name="position">Position in the <see cref="textBox"/> that should be queried.</param>
        /// <returns>The index of the indicator if found, -1 otherwise.</returns>
        private int getIndicatorAt(int position)
        {
            // We're not going to style stuff with two indicators.
            switch (this.textBox.IndicatorAllOnFor(position))
            {
                case 1 << kI18nIndicator:
                    return kI18nIndicator;
                case 1 << kFileIndicator:
                    return kFileIndicator;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Returns the text that's indicated with <paramref name="indicator"/>
        /// at position <paramref name="position"/>
        /// </summary>
        /// <param name="indicator">Indicator used to style the text</param>
        /// <param name="position">Position to query</param>
        /// <returns>The complete text that was styled with this indicator</returns>
        private string getIndicatorText(Indicator indicator, int position)
        {
            var startPos = indicator.Start(position);
            var endPos = indicator.End(position);
            return this.textBox.GetTextRange(startPos, endPos - startPos);
        }

        /// <summary>
        /// Returns the localisation key at position <paramref name="position"/>.
        /// </summary>
        /// <param name="position">Position that should be queried.</param>
        /// <returns>The localisation key, without i18n()</returns>
        private string getLocKey(int position)
        {
            if (this.getIndicatorAt(position) != kI18nIndicator)
                return null;

            var indicator = this.mI18nIndicator;
            var startPos = indicator.Start(position);
            var endPos = indicator.End(position);
            var locKey = this.textBox.GetTextRange(startPos, endPos - startPos);
            var i18nLength = "i18n(".Length;
            return locKey.Substring(i18nLength, locKey.Length - i18nLength - 1);
        }

        /// <summary>
        /// Whenever the user clicked on an indicator.
        /// Used instead of IndicatorClick because the opening dialog is eating the
        /// Release event, so the control thinks there's text to be selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_IndicatorRelease(object sender, IndicatorReleaseEventArgs e)
        {
            if (!ModifierKeys.HasFlag(Keys.Control))
                return;

            int indicator = this.getIndicatorAt(e.Position);

            if (indicator == kI18nIndicator)
            {
                var locKey = this.getLocKey(e.Position);

                if (locKey != null)
                {
                    this.mI18nLocKey = locKey;
                    EditLocStringCallback callback = new EditLocStringCallback(this.mI18nLocKey);
                    string translated = ModuleDataManager.GetInstance().LocalizeString(this.mI18nLocKey);
                    InputDialog dialog = new InputDialog("Edit Loc String", $"Edit Loc Text For: {mI18nLocKey}", translated, "Edit");
                    dialog.SetCallback(callback);
                    dialog.ShowDialog();
                }
            }
            else if (indicator == kFileIndicator)
            {
                var text = this.getIndicatorText(this.mFileIndicator, e.Position);
                ModuleFile module;
                if (!this.tryGetModuleFile(text, out module) || module == null)
                    return;

                var selectable = mOwner as IFileDataSelectable;

                // TODO: instead of using an interface, try to have some function in
                // the main view that allows switching to the manifest view + displaying a modulefile/filedata
                if (selectable != null)
                    selectable.SetSelectedFileData(module.FileData);
            }
        }

        /// <summary>
        /// Tries to convert a string to an (openable) file
        /// </summary>
        /// <param name="text"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        private bool tryGetModuleFile(string text, out ModuleFile module)
        {
            module = null;

            // Simple checks
            if (text.StartsWith("i18n(") || text.IndexOfAny(new[] { ':', '/' }) == -1)
                return false;

            var parent = System.IO.Path.GetDirectoryName(this.mFileData.Path);
            return ModuleDataManager.GetInstance().TryGetModuleFile(text, parent, out module);
        }

        /// <summary>
        /// Returns whether a certain string maps to a file (that can be opened)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool isValidFileName(string text)
        {
            ModuleFile dummy;
            return tryGetModuleFile(text, out dummy) && dummy != null;
        }

        /// <summary>
        /// Called whenever the text is modified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_SavePointLeft(object sender, EventArgs e)
        {
            TabPage parentControl = Parent as TabPage;

            if (parentControl != null)
            {
                parentControl.Text = mFileData.FileName + "*";
            }
        }

        /// <summary>
        /// Called whenever <see cref="Scintilla.SetSavePoint"/> is called,
        /// i.e. in <see cref="Save"/>
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        private void textBox_SavePointReached(object sender, EventArgs e)
        {
            TabPage parentControl = Parent as TabPage;
            if (parentControl != null)
            {
                parentControl.Text = mFileData.FileName;
            }
        }
    }
}
