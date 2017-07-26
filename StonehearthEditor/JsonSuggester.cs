using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AutocompleteMenuNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using ScintillaNET;
using static StonehearthEditor.JsonSchemaTools;

namespace StonehearthEditor
{
    internal class JsonSuggester : IEnumerable<AutocompleteItem>
    {
        private string filePath;
        private JsonSchema4 schema;
        private Scintilla textBox;
        private AutocompleteMenu menu;
        private Dictionary<string, Func<IEnumerable<string>>> customSourcesBySchemeId;

        public JsonSuggester(JsonSchema4 schema, Scintilla textBox, AutocompleteMenu menu, string filePath)
        {
            this.filePath = filePath;
            this.schema = schema;
            this.textBox = textBox;
            this.menu = menu;
            customSourcesBySchemeId = new Dictionary<string, Func<IEnumerable<string>>>();
        }

        public void AddCustomSource(string schemaId, Func<IEnumerable<string>> lister)
        {
            customSourcesBySchemeId[schemaId] = lister;
        }

        // The cursor context for suggesting values or property names.
        public struct Context
        {
            // If false, all other properties are garbage.
            public bool IsValid;

            // The JSON object or array surrounding the cursor.
            public JToken ObjectAroundCursor;

            // The JSON path to ObjectAroundCursor.
            public List<string> Path;

            // Whether the context is expecting a value (rather than a property name).
            public bool IsSuggestingValue;

            // The property name whose value is being suggested. Only meaningful if IsSuggestingValue == true.
            public string ActivePropertyName;

            // The top-level JSON object representing everything we managed to parse.
            public JObject KnownJson;
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var context = ParseOutContext(menu.Fragment.Start + 1);
            if (!context.IsValid)
            {
                yield break;
            }

            var targetSchemas = JsonSchemaTools.GetSchemasForPath(schema, context.Path, context.ObjectAroundCursor, context.ActivePropertyName);
            if (targetSchemas.Count == 0)
            {
                yield break;
            }

            List<KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>> sections =
                context.IsSuggestingValue ? SuggestValues(targetSchemas) :
                                            SuggestProperties(context.ObjectAroundCursor as JObject ?? new JObject(), targetSchemas);

            if (sections.Count == 1 || sections.All(p => p.Value.Count == 1))
            {
                // No headers/dividers required.
                foreach (var section in sections)
                {
                    foreach (var item in section.Value)
                    {
                        yield return item;
                    }
                }
            }
            else
            {
                // Add headers to sections.
                foreach (var section in sections)
                {
                    if (section.Value.Count == 1)
                    {
                        yield return section.Value.First();
                    }
                    else
                    {
                        yield return new HeaderItem(section.Key.Title);
                        foreach (var item in section.Value)
                        {
                            item.MenuText = "   " + item.MenuText;
                            yield return item;
                        }
                    }
                }
            }
        }

        private List<KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>> SuggestValues(ICollection<AnnotatedSchema> annotatedSchemas)
        {
            var sections = new List<KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>>();
            foreach (var annotatedSchema in annotatedSchemas)
            {
                var items = SuggestValues(annotatedSchema).ToList();
                if (items.Count > 0)
                {
                    sections.Add(new KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>(annotatedSchema, items));
                }
            }
            return sections;
        }

        private IEnumerable<AutocompleteItem> SuggestValues(AnnotatedSchema annotatedSchema)
        {
            // Help text.
            var title = annotatedSchema.Title ?? annotatedSchema.Name;
            var description = annotatedSchema.Description ?? JsonSchemaTools.DescribeSchema(annotatedSchema);

            var targetSchema = annotatedSchema.Schema;
            if (targetSchema.Id != null && customSourcesBySchemeId.ContainsKey(targetSchema.Id))
            {
                // Special handling for sources set by the environment (edge names, node names, files, etc.).
                foreach (var value in customSourcesBySchemeId[targetSchema.Id]())
                {
                    yield return new ValueSuggestItem(value, title != null ? title + ": " + value : value, description);
                }
            }
            else if (targetSchema.Id == "http://stonehearth.net/schemas/encounters/elements/file.json")
            {
                // Special handling for files (alias or file insertion prompts).
                yield return new AliasSuggestItem(title, description);
                yield return new FileSuggestItem(title, description, filePath);
            }
            else if (targetSchema.Enumeration.Count > 0)
            {
                // Suggest enumeration alternatives.
                foreach (var alternative in targetSchema.Enumeration)
                {
                    yield return new ValueSuggestItem(JsonSchemaTools.FormatEnumValue(alternative), title, description);
                }
            }
            else if (targetSchema.Type == JsonObjectType.Boolean)
            {
                // Suggest the two alternatives of bool.
                yield return new ValueSuggestItem("true", title, description);
                yield return new ValueSuggestItem("false", title, description);
            }
            else if (targetSchema.Default != null)
            {
                // If there's a default value, suggest that.
                yield return new ValueSuggestItem(JsonSchemaTools.FormatEnumValue(targetSchema.Default), title, description);
            }
            else if (targetSchema.Type == JsonObjectType.Object || targetSchema.ActualProperties.Count > 0)
            {
                // Construct as much of an object as required.
                var left = "{\n   ";
                var right = "\n}";
                var addedAnyToLeft = false;
                foreach (var property in targetSchema.ActualProperties)
                {
                    if (property.Value.IsRequired)
                    {
                        if (addedAnyToLeft) left += ",\n   ";
                        left += "\"" + property.Key + "\": ";

                        if (property.Value.Default != null)
                        {
                            left += JsonSchemaTools.FormatEnumValue(property.Value.Default);
                        }
                        else if (property.Value.Enumeration.Count == 1)
                        {
                            left += JsonSchemaTools.FormatEnumValue(property.Value.Enumeration.First());
                        }
                        else if (property.Value.Type == JsonObjectType.Number || property.Value.Type == JsonObjectType.Integer)
                        {
                            left += "0";
                        }
                        else if (property.Value.Type == JsonObjectType.Boolean)
                        {
                            left += "false";
                        }
                        else if (property.Value.Type == JsonObjectType.Null)
                        {
                            left += "null";
                        }
                        else if (property.Value.Type == JsonObjectType.Object)
                        {
                            left += "{}";
                        }
                        else if (property.Value.Type == JsonObjectType.Array)
                        {
                            left += "[]";
                        }
                        else if (property.Value.Type == JsonObjectType.String)
                        {
                            left += "\"\"";
                        }

                        addedAnyToLeft = true;
                    }
                }

                if (addedAnyToLeft)
                {
                    right = "," + right;
                }

                yield return new ValueSuggestItem(left, right, title, description);
            }
            else if (targetSchema.Type == JsonObjectType.Array)
            {
                // Trivial array form.
                yield return new ValueSuggestItem("[\n   ", "\n]", title, description);
            }
            else if (targetSchema.Type == JsonObjectType.String)
            {
                // Trivial string form.
                yield return new ValueSuggestItem("\"", "\"", title ?? "\"...\"", description);
            }
            else if (targetSchema.Type == JsonObjectType.Number || targetSchema.Type == JsonObjectType.Integer)
            {
                // Trivial number.
                yield return new ValueSuggestItem((targetSchema.Minimum ?? 0).ToString(), title, description);
            }
            else if (targetSchema.Type == JsonObjectType.Null)
            {
                // Null has only one option.
                yield return new ValueSuggestItem("null", title, description);
            }
        }

        private List<KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>> SuggestProperties(JObject contextObject, ICollection<AnnotatedSchema> annotatedSchemas)
        {
            var sections = new List<KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>>();
            foreach (var annotatedSchema in annotatedSchemas)
            {
                var items = SuggestProperties(contextObject, annotatedSchema).ToList();
                if (items.Count > 0)
                {
                    sections.Add(new KeyValuePair<AnnotatedSchema, List<AutocompleteItem>>(annotatedSchema, items));
                }
            }

            // Filter to match any partially entered name.
            var partial = Regex.Match(menu.Fragment.Text, @"\w+$");
            if (partial.Success)
            {
                foreach (var section in sections)
                {
                    section.Value.RemoveAll(item => !item.MenuText.StartsWith(partial.Value));
                }
            }

            // Sort properties with hardcoded values (e.g. type: foo) first within each section.
            foreach (var section in sections)
            {
                section.Value.Sort((a, b) =>
                {
                    if (a.MenuText.Contains(':') == b.MenuText.Contains(':'))
                    {
                        return a.MenuText.CompareTo(b.MenuText);
                    }
                    else
                    {
                        return a.MenuText.Contains(':') ? -1 : 1;
                    }
                });
            }

            return sections;
        }

        private IEnumerable<AutocompleteItem> SuggestProperties(JObject contextObject, AnnotatedSchema annotatedSchema)
        {
            var yieldedAny = false;

            var curSchema = annotatedSchema.Schema;
            do
            {
                foreach (var propertyDef in curSchema.ActualProperties)
                {
                    if (contextObject.Property(propertyDef.Key) == null)
                    {
                        yield return new PropertySuggestItem(propertyDef.Key, propertyDef.Value.ActualPropertySchema);
                        yieldedAny = true;
                    }
                }
            } while ((curSchema = curSchema.InheritedSchema) != null);

            if (!yieldedAny && annotatedSchema.Schema.PatternProperties.Count > 0)
            {
                yield return new PropertySuggestItem("", annotatedSchema.Schema.PatternProperties.First().Value);
            }
        }

        public Context ParseOutContext(int position)
        {
            var result = default(Context);
            result.IsValid = true;

            // Parse JSON until the the current position (or until earlier failure), constructing a path.
            result.Path = new List<string>();
            var currentJsonStack = new List<JToken>();
            string lastProperty = null;
            var lastPosition = 0;
            var reader = new JsonTextReader(new StringReader(textBox.Text.Substring(0, position)));
            while (true)
            {
                // TODO: To see existing properties beyond the cursor, we need to skip the
                //       invoking characters and continue parsing until the end of the current object.
                try
                {
                    if (!reader.Read())
                    {
                        break;
                    }
                }
                catch (JsonReaderException)
                {
                    // It's natural that sometimes the JSON will be broken.
                    // However, if it's before the current line, we can't trust our context.
                    var readerLineNumber = reader.LineNumber - 1;  // Convert to 0-based to match textBox.
                    if (readerLineNumber < textBox.CurrentLine)
                    {
                        result.IsValid = false;
                        return result;
                    }
                    else
                    {
                        // Make sure we aren't within a string.
                        var unparsed = textBox.Text.Substring(lastPosition, position - lastPosition);
                        if (unparsed.Count(c => c == '"') % 2 == 1)
                        {
                            // An odd number of quotes means we are probably inside a string (they could be escaped, but this is good enough).
                            result.IsValid = false;
                            return result;
                        }
                    }

                    break;
                }

                lastPosition = textBox.Lines[reader.LineNumber - 1].Position + reader.LinePosition;

                if (result.KnownJson == null)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        result.KnownJson = new JObject();
                        currentJsonStack.Add(result.KnownJson);
                    }
                    else
                    {
                        // We only allow objects at the top level.
                        result.IsValid = false;
                        return result;
                    }

                    continue;
                }

                Action<JToken> addNewEntryToCurrentObjectOrArray = (value) =>
                {
                    var target = currentJsonStack.Last();
                    if (target is JArray)
                    {
                        (target as JArray).Add(value);
                    }
                    else
                    {
                        target[lastProperty] = value;
                    }
                };

                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        var newObject = new JObject();
                        addNewEntryToCurrentObjectOrArray(newObject);
                        currentJsonStack.Add(newObject);
                        result.Path.Add(lastProperty ?? "0");
                        lastProperty = null;
                        break;
                    case JsonToken.StartArray:
                        var newArray = new JArray();
                        addNewEntryToCurrentObjectOrArray(newArray);
                        currentJsonStack.Add(newArray);
                        result.Path.Add(lastProperty ?? "0");
                        lastProperty = null;
                        break;
                    case JsonToken.PropertyName:
                        lastProperty = reader.Value as string;
                        if (lastProperty.Length == 0)
                        {
                            result.IsValid = false;
                            return result;
                        }

                        break;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        currentJsonStack.RemoveAt(currentJsonStack.Count - 1);
                        result.Path.RemoveAt(result.Path.Count - 1);
                        break;
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                        // Primitive value types. These don't affect the path.
                        var newValue = reader.Value == null ? null : JToken.FromObject(reader.Value);
                        addNewEntryToCurrentObjectOrArray(newValue);
                        lastProperty = null;
                        break;
                    case JsonToken.None:
                    case JsonToken.Raw:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                    case JsonToken.StartConstructor:
                    case JsonToken.EndConstructor:
                        // We don't expect to ever see these in valid SH JSON.
                        result.IsValid = false;
                        return result;
                    case JsonToken.Comment:
                    default:
                        break;
                }
            }

            result.ObjectAroundCursor = currentJsonStack.Count > 0 ? currentJsonStack.Last() : null;
            result.IsSuggestingValue = Regex.IsMatch(textBox.Text.Substring(0, position), @":\s*$") || currentJsonStack.Last() is JArray;
            if (result.IsSuggestingValue)
            {
                if (lastProperty != null)
                {
                    result.ActivePropertyName = lastProperty;
                }
                else if (currentJsonStack.Last() is JArray)
                {
                    result.ActivePropertyName = "0";  // We assume arrays are homogeneous, so all items are equivalent to [0].
                }
                else
                {
                    result.IsValid = false;
                }
            }

            return result;
        }

        public IEnumerator<AutocompleteItem> GetEnumerator()
        {
            return BuildList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // A suggest list entry for the property of an object.
        internal class PropertySuggestItem : AutocompleteItem
        {
            public PropertySuggestItem(string name, JsonSchema4 schema)
            {
                MenuText = string.IsNullOrEmpty(name) ? schema.Title : name;
                ToolTipTitle = schema.Title ?? name;
                ToolTipText = schema.Description ?? JsonSchemaTools.DescribeSchema(schema);
                Text = "\"" + name + "\": ";
                if (schema.Enumeration.Count == 1)
                {
                    var value = JsonSchemaTools.FormatEnumValue(schema.Enumeration.First());
                    MenuText += ": " + value;
                    Text += value;
                }
            }

            public override void OnSelected(SelectedEventArgs e)
            {
                // Construct the replacement text.
                var textboxWrapper = Parent.TargetControlWrapper as ScintillaWrapper;
                var fragmentText = Regex.Replace(Parent.Fragment.Text, @"\w+$", "");
                var result = fragmentText;
                if (!Regex.IsMatch(fragmentText, @"\n\s*$"))
                {
                    // Not already indented. Auto-indent.
                    var textbox = textboxWrapper.target;
                    var curLineText = textbox.Lines[textbox.CurrentLine].Text;
                    result += "\n" + Regex.Match(curLineText, @"^[ \t]*").Value;
                    if (Regex.IsMatch(fragmentText, @"[{\[]\s*$"))
                    {
                        result += "   ";
                    }
                }

                result += Text;

                // Replace the match.
                textboxWrapper.SelectionStart = Parent.Fragment.Start;
                textboxWrapper.SelectionLength = Parent.Fragment.End - Parent.Fragment.Start;
                textboxWrapper.SelectedText = result;

                // Place cursor at the end.
                textboxWrapper.SelectionStart = Parent.Fragment.Start + result.Length;
                textboxWrapper.SelectionLength = 0;

                // Try suggesting the value.
                Parent.ShowAutocomplete(false);
            }

            public override CompareResult Compare(string fragmentText)
            {
                return CompareResult.Visible;
            }
        }

        // A suggest list entry for property or array values.
        internal class ValueSuggestItem : AutocompleteItem
        {
            private string valueBeforeCursor;
            private string valueAfterCursor;

            public ValueSuggestItem(string value, string title, string description)
                : this(value, "", title, description)
            {
            }

            public ValueSuggestItem(string valueBeforeCursor, string valueAfterCursor, string title, string description)
            {
                this.valueBeforeCursor = valueBeforeCursor;
                this.valueAfterCursor = valueAfterCursor;

                Text = valueBeforeCursor + valueAfterCursor;
                MenuText = (title != null && Text.Contains("\n")) ? title : Text.Replace("\n", " ");
                ToolTipTitle = title ?? Text;
                ToolTipText = description;
            }

            public override void OnSelected(SelectedEventArgs e)
            {
                var textboxWrapper = Parent.TargetControlWrapper as ScintillaWrapper;

                // Auto-indent line breaks in the replacement text, if needed.
                if (Text.Contains("\n"))
                {
                    var textbox = textboxWrapper.target;
                    var curLineText = textbox.Lines[textbox.CurrentLine].Text;
                    var indent = Regex.Match(curLineText, @"^[ \t]*").Value;
                    valueBeforeCursor = valueBeforeCursor.Replace("\n", "\n" + indent);
                    valueAfterCursor = valueAfterCursor.Replace("\n", "\n" + indent);
                    Text = valueBeforeCursor + valueAfterCursor;
                }

                // Add a comma after the value, if there isn't one already.
                if (textboxWrapper.Text.ElementAtOrDefault(Parent.Fragment.End) != ',')
                {
                    Text += ",";
                    if (valueBeforeCursor.Length > 0 && valueAfterCursor.Length == 0)
                    {
                        // We are adding a value and the cursor is at the end. It is reasonable
                        // to assume that the user is more likely to want togo on to the next
                        // property now.
                        valueBeforeCursor += ",";
                    }
                    else
                    {
                        valueAfterCursor += ",";
                    }
                }

                // Replace the match.
                textboxWrapper.SelectionStart = Parent.Fragment.Start;
                textboxWrapper.SelectionLength = Parent.Fragment.End - Parent.Fragment.Start;
                textboxWrapper.SelectedText = Parent.Fragment.Text + Text;

                // Place cursor at the specified offset.
                textboxWrapper.SelectionStart = textboxWrapper.SelectionStart - valueAfterCursor.Length;
                textboxWrapper.SelectionLength = 0;

                // Try suggesting the next property or value.
                Parent.ShowAutocomplete(false);
            }

            public override CompareResult Compare(string fragmentText)
            {
                return CompareResult.Visible;
            }
        }

        // A suggest list entry that opens an alias selection dialog.
        internal class AliasSuggestItem : AutocompleteItem
        {
            public AliasSuggestItem(string title, string description)
            {
                Text = null;
                MenuText = "Select an alias...";
                ToolTipTitle = title ?? "File alias";
                ToolTipText = description;
            }

            public override void OnSelected(SelectedEventArgs e)
            {
                var textbox = (Parent.TargetControlWrapper as ScintillaWrapper).target;
                AliasSelectionDialog aliasDialog = new AliasSelectionDialog((aliases) =>
                {
                    var aliasList = aliases.ToList();
                    if (aliasList.Count > 0)
                    {
                        textbox.InsertText(textbox.SelectionStart, '"' + aliasList[0] + '"');
                        textbox.SelectionStart = textbox.SelectionEnd + aliasList[0].Length + 2;
                    }
                });
                aliasDialog.MultiSelect = false;
                aliasDialog.ShowDialog();
            }

            public override CompareResult Compare(string fragmentText)
            {
                return CompareResult.Visible;
            }
        }

        // A suggest list entry that opens a file selection dialog.
        internal class FileSuggestItem : AutocompleteItem
        {
            private string currentFilePath;

            public FileSuggestItem(string title, string description, string currentFilePath)
            {
                this.currentFilePath = currentFilePath;
                Text = null;
                MenuText = "Select a file...";
                ToolTipTitle = title ?? "File";
                ToolTipText = description;
            }

            public override void OnSelected(SelectedEventArgs e)
            {
                var textbox = (Parent.TargetControlWrapper as ScintillaWrapper).target;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Path.GetDirectoryName(currentFilePath);
                dialog.Multiselect = false;
                dialog.FileOk += (sender, args) =>
                {
                    string relativePath = new Uri(dialog.FileName).MakeRelativeUri(new Uri(currentFilePath)).ToString();
                    var replacement = "\"file(" + relativePath + ")\"";
                    textbox.InsertText(textbox.SelectionStart, replacement);
                    textbox.SelectionStart = textbox.SelectionEnd + replacement.Length;
                };
                dialog.ShowDialog();
            }

            public override CompareResult Compare(string fragmentText)
            {
                return CompareResult.Visible;
            }
        }

        // An unselectable suggest list entry used as a header for sections.
        internal class HeaderItem : AutocompleteItem
        {
            private const int kLength = 40;
            private const char kLineChar = '═';  // ASCII 205, double horizontal line.

            public HeaderItem()
                : this(null)
            {
            }

            public HeaderItem(string title)
            {
                ToolTipTitle = ToolTipText = Text = null;
                MenuText = new string(kLineChar, kLength);
                if (title != null)
                {
                    MenuText = new string(kLineChar, (kLength - 2 - title.Length) / 2) + ' ' + title + ' ' + MenuText;
                }
            }

            public override bool CanBeSelected()
            {
                return false;
            }

            public override CompareResult Compare(string fragmentText)
            {
                return CompareResult.Visible;
            }
        }
    }
}
