using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.Validation;
using ScintillaNET;
using Color = System.Drawing.Color;

namespace StonehearthEditor
{
    public partial class FilePreview : UserControl
    {
        // Index of the indicator for i18n()
        private const int kI18nIndicator = 8;

        // Index of the indicator for file() and file-ish things
        private const int kFileIndicator = 9;

        // The margin used to display errors.
        private const int kErrorMarginNumber = 1;

        // The marker used to display errors.
        private const int kErrorMarkerNumber = 3;

        // The lastTipAnchor value when showing error tips.
        private const int kAnchorError = -2;

        // The lastTipAnchor value when not showing any tips.
        private const int kAnchorNone = -1;

        private FileData mFileData;
        private string mI18nLocKey = null;
        private IReloadable mOwner;
        private int maxLineNumberCharLength;
        private int lastTipAnchor = kAnchorNone;

        // JSON validation stuff.
        private JsonSchema4 jsonValidationSchema;
        private Dictionary<int, string> validationErrors = new Dictionary<int, string>();
        private Timer validationDelayTimer;

        /// <summary>
        /// Indicator for i18n()
        /// </summary>
        private Indicator mI18nIndicator => this.textBox.Indicators[kI18nIndicator];

        /// <summary>
        /// Indicator for file() and file-ish things
        /// </summary>
        private Indicator mFileIndicator => this.textBox.Indicators[kFileIndicator];

        public delegate void ModifiedChangedHandler(bool isModified);

        public event ModifiedChangedHandler OnModifiedChanged;

        public FilePreview(IReloadable owner, FileData fileData)
        {
            mFileData = fileData;
            InitializeComponent();
            textBox.Text = mFileData.FlatFileData;
            mOwner = owner;

            this.configureScintilla();
        }

        public string GetText()
        {
            return textBox.Text;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            if (mFileData.FlatFileData != textBox.Text)
            {
                mFileData.TrySetFlatFileData(textBox.Text);
                OnModifiedChanged?.Invoke(true);
            }
        }

        private void textBox_MouseMove(object sender, MouseEventArgs e)
        {
            // Translate mouse xy to character position
            var position = this.textBox.CharPositionFromPoint(e.X, e.Y);
            var line = this.textBox.LineFromPosition(position);

            // Figure out if we are hovering over an error indicator.
            var isInErrorMargin = e.X > textBox.Margins[0].Width && e.X <= textBox.Margins[0].Width + textBox.Margins[kErrorMarginNumber].Width;
            if (isInErrorMargin && validationErrors.ContainsKey(line))
            {
                this.textBox.CallTipShow(position, validationErrors[line]);
                lastTipAnchor = kAnchorError;
                return;
            }

            var currentAnchor = this.getIndicatorStartPosition(position);

            if (lastTipAnchor == currentAnchor)
            {
                return;  // Nothing to change.
            }
            else
            {
                this.textBox.CallTipCancel();
                lastTipAnchor = currentAnchor;
            }

            var hoveredIndicator = this.getIndicatorAt(position);
            if (hoveredIndicator == kFileIndicator)
            {
                this.textBox.CallTipShow(position, "Ctrl-click to open file.");
            }
            else if (hoveredIndicator == kI18nIndicator)
            {
                var locKey = this.getLocKey(position);
                if (string.IsNullOrEmpty(locKey))
                {
                    return;
                }

                this.mI18nLocKey = locKey;
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
            OnModifiedChanged?.Invoke(false);
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

            // Replace tabs with 3 spaces.
            text = text.Replace("\t", "   ");

            // Auto indent.
            if (text == "\r\n")
            {
                var curLine = textBox.LineFromPosition(e.Position);
                var curLineText = textBox.Lines[curLine].Text;

                // Copy last line's indent.
                var indent = Regex.Match(curLineText, @"^[ \t]*");
                text += indent.Value;

                // For JSON, add one level of indent if the line ends with a bracket.
                if (textBox.Lexer == ScintillaNET.Lexer.Json && Regex.IsMatch(curLineText, @"[\[\{]\s*$"))
                {
                    text += "   ";
                }
            }

            // Return the modified text.
            e.Text = text;
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

        internal void SetValidationSchema(JsonSchema4 schema)
        {
            jsonValidationSchema = schema;
            ValidateSchema();
        }

        private void ValidateSchema()
        {
            if (textBox.Lexer != ScintillaNET.Lexer.Json)
            {
                // No validation possible.
                textBox.Styles[Style.LineNumber].BackColor = Color.LightGray;
                return;
            }

            // Find errors.
            validationErrors.Clear();
            try
            {
                if (jsonValidationSchema == null)
                {
                    // No schema. Just make sure the JSON is valid.
                    JToken.Parse(GetText());
                }
                else
                {
                    // Validate based on the schema.
                    foreach (var error in jsonValidationSchema.Validate(GetText()))
                    {
                        FillValidationErrorsFromJsonSchemaError(error);
                    }
                }
            }
            catch (JsonReaderException exception)
            {
                validationErrors[exception.LineNumber - 1] = exception.Message;
            }

            // Display errors.
            textBox.MarkerDeleteAll(kErrorMarkerNumber);
            textBox.Styles[Style.LineNumber].BackColor = validationErrors.Count > 0 ? Color.IndianRed : (jsonValidationSchema == null ? Color.LightGray : Color.LightGreen);
            if (validationErrors.Count > 0)
            {
                foreach (var error in validationErrors)
                {
                    textBox.Lines[error.Key].MarkerAdd(kErrorMarkerNumber);
                }
            }
        }

        private void FillValidationErrorsFromJsonSchemaError(ValidationError error)
        {
            if (!error.HasLineInfo)
            {
                return;
            }

            switch (error.Kind)
            {
                case ValidationErrorKind.PatternMismatch:
                    AddValidationError(error.LineNumber, string.Format("The value of '{0}' must match the regex pattern: {1}", error.Property, error.Schema.Pattern));
                    break;
                case ValidationErrorKind.NotInEnumeration:
                    string choices = "";
                    foreach (var choice in error.Schema.Enumeration)
                    {
                        if (choices.Length > 0)
                        {
                            choices += ", ";
                        }

                        choices += '"';
                        choices += choice == null ? "null" : choice.ToString();
                        choices += '"';
                    }

                    AddValidationError(error.LineNumber, string.Format("The value of '{0}' must be one of: {1}", error.Property, choices));
                    break;
                case ValidationErrorKind.NoAdditionalPropertiesAllowed:
                    string validProperties = string.Join(", ", error.Schema.ActualProperties.Keys);
                    AddValidationError(error.LineNumber, string.Format(
                        "Property '{0}' is not expected in this object. Valid properties: {1}", error.Property, validProperties));
                    break;
                case ValidationErrorKind.PropertyRequired:
                    AddValidationError(error.LineNumber, string.Format("Missing required property '{0}'.", error.Property));
                    break;
                case ValidationErrorKind.NotAnyOf:
                    string validFormats = " Valid formats:";
                    foreach (var alternativeSchema in error.Schema.AnyOf)
                    {
                        if (alternativeSchema.Title == null)
                        {
                            // We can't name the formats, so skip the extra info.
                            validFormats = "";
                            break;
                        }

                        validFormats += "\n  ";
                        validFormats += alternativeSchema.Title;
                    }

                    AddValidationError(error.LineNumber, string.Format(
                        "None of the {0} valid formats for {1} match.{2}",
                        error.Schema.AnyOf.Count,
                        string.IsNullOrEmpty(error.Property) ? "the element" : ("'" + error.Property + "'"),
                        validFormats));

                    // Show sub-errors for the closest matching alternative.
                    var multiError = error as ChildSchemaValidationError;
                    if (multiError != null)
                    {
                        int minNumSubErrors = int.MaxValue;
                        ICollection<ValidationError> bestSubErrorsList = null;
                        foreach (var subErrors in multiError.Errors)
                        {
                            if (subErrors.Value.Count <= minNumSubErrors)
                            {
                                bestSubErrorsList = subErrors.Value;
                                minNumSubErrors = bestSubErrorsList.Count;
                            }
                        }

                        if (bestSubErrorsList != null)
                        {
                            foreach (var subError in bestSubErrorsList)
                            {
                                FillValidationErrorsFromJsonSchemaError(subError);
                            }
                        }
                    }

                    return;  // Don't process sub-errors. We've handled them already.
                default:
                    var errorString = Regex.Replace(error.Kind.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).ToLower().Trim();
                    if (error.Property != null)
                    {
                        errorString = error.Property + ": " + errorString;
                    }

                    AddValidationError(error.LineNumber, errorString);
                    break;
            }

            // Show all sub-errors.
            if (error is ChildSchemaValidationError)
            {
                foreach (var subErrors in (error as ChildSchemaValidationError).Errors)
                {
                    foreach (var subError in subErrors.Value)
                    {
                        FillValidationErrorsFromJsonSchemaError(subError);
                    }
                }
            }
        }

        private void AddValidationError(int line, string message)
        {
            var zeroBasedLine = line - 1;
            if (validationErrors.ContainsKey(zeroBasedLine))
            {
                validationErrors[zeroBasedLine] += '\n' + message;
            }
            else
            {
                validationErrors[zeroBasedLine] = message;
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
            textBox.Margins[0].Cursor = MarginCursor.Arrow;
            textBox.StyleClearAll();

            // Configure error margin & marker style.
            this.textBox.Margins[kErrorMarginNumber].Width = 0;
            this.textBox.Margins[kErrorMarginNumber].Type = MarginType.Symbol;
            this.textBox.Margins[kErrorMarginNumber].Mask = Marker.MaskAll;
            this.textBox.Margins[kErrorMarginNumber].Cursor = MarginCursor.Arrow;

            this.textBox.Markers[kErrorMarkerNumber].Symbol = MarkerSymbol.ShortArrow;
            this.textBox.Markers[kErrorMarkerNumber].SetForeColor(Color.Red);
            this.textBox.Markers[kErrorMarkerNumber].SetBackColor(Color.IndianRed);

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

            // Configure tooltip.
            this.textBox.Styles[Style.CallTip].SizeF = 8.25f;
            this.textBox.Styles[Style.CallTip].ForeColor = Color.Black;
            this.textBox.Styles[Style.CallTip].BackColor = Color.White;
            this.textBox.Styles[Style.CallTip].Font = "Verdana";
            this.textBox.Styles[Style.CallTip].Hotspot = true;

            // Restyle now and on demand.
            this.textBox.TextChanged += (sender, e) => this.restyleDocument();
            this.restyleDocument();
        }

        /// <summary>
        /// Configures Scintilla for Json highlighting
        /// </summary>
        private void configureJsonHighlighting()
        {
            textBox.Lexer = ScintillaNET.Lexer.Json;

            var scintilla = textBox;

            scintilla.Styles[Style.Json.Operator].ForeColor = Color.Black;
            scintilla.Styles[Style.Json.Operator].Bold = true;
            scintilla.Styles[Style.Json.Operator].Weight = 700;

            scintilla.Styles[Style.Json.Number].ForeColor = Color.Navy;
            scintilla.Styles[Style.Json.Number].Bold = true;
            scintilla.Styles[Style.Json.Number].Weight = 700;

            scintilla.Styles[Style.Json.Keyword].ForeColor = Color.Navy;
            scintilla.Styles[Style.Json.Keyword].Bold = true;
            scintilla.Styles[Style.Json.Keyword].Weight = 700;

            // Unfortunately as of ScintillaNET 3.6.3, "true", "false", and "null" are parsed as errors.
            scintilla.Styles[Style.Json.Error].ForeColor = Color.DarkRed;
            scintilla.Styles[Style.Json.Error].Bold = true;
            scintilla.Styles[Style.Json.Error].Weight = 700;

            scintilla.Styles[Style.Json.String].ForeColor = Color.Purple;

            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.ForestGreen;
            scintilla.Styles[Style.Json.PropertyName].Bold = true;
            scintilla.Styles[Style.Json.PropertyName].Weight = 700;

            scintilla.Styles[Style.Json.StringEol].ForeColor = Color.Red;

            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.Green;
            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.Green;

            // Prepare indicator
            this.mI18nIndicator.Style = IndicatorStyle.FullBox;
            this.mI18nIndicator.ForeColor = Color.CornflowerBlue;
            this.mI18nIndicator.HoverForeColor = Color.CadetBlue;
            this.mI18nIndicator.Alpha = 50;

            this.mFileIndicator.Style = IndicatorStyle.FullBox;
            this.mFileIndicator.ForeColor = Color.Green;
            this.mFileIndicator.HoverForeColor = Color.DarkGreen;
            this.mFileIndicator.Alpha = 50;

            // Prepare validator.
            validationDelayTimer = new Timer();  // To be started by textBox_TextChanged().
            validationDelayTimer.Interval = 100;
            validationDelayTimer.Enabled = true;
            validationDelayTimer.Tick += new EventHandler((s, e) => {
                validationDelayTimer.Stop();
                ValidateSchema();
            });
            this.textBox.Margins[kErrorMarginNumber].Width = 16;
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

        private int getIndicatorStartPosition(int position)
        {
            var indicatorId = this.getIndicatorAt(position);
            if (indicatorId == kFileIndicator)
            {
                return this.mFileIndicator.Start(position);
            }
            else if (indicatorId == kI18nIndicator)
            {
                return this.mI18nIndicator.Start(position);
            }
            else
            {
                return kAnchorNone;
            }
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
                    InputDialog dialog = new InputDialog("Edit i18n Text", $"Edit English text for:\n{mI18nLocKey}", translated, "Save");
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
                {
                    selectable.SetSelectedFileData(module.FileData);
                }
                else
                {
                    // No one is listening. Just open the file in the default OS viewer.
                    System.Diagnostics.Process.Start(module.ResolvedPath);
                }
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

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (validationDelayTimer != null)
            {
                validationDelayTimer.Stop();
                validationDelayTimer.Start();
            }
        }

        private void textBox_MouseLeave(object sender, EventArgs e)
        {
            this.textBox.CallTipCancel();
            lastTipAnchor = kAnchorNone;
        }
    }
}
