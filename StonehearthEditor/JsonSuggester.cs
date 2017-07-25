using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutocompleteMenuNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using ScintillaNET;
using System.Windows.Forms;

namespace StonehearthEditor
{
    internal class JsonSuggester : IEnumerable<AutocompleteItem>
    {
        private string filePath;
        private JsonSchema4 schema;
        private Scintilla textBox;
        private Dictionary<string, Func<IEnumerable<string>>> customSourcesBySchemeId;

        public JsonSuggester(JsonSchema4 schema, Scintilla textBox, string filePath)
        {
            this.filePath = filePath;
            this.schema = schema;
            this.textBox = textBox;
            customSourcesBySchemeId = new Dictionary<string, Func<IEnumerable<string>>>();
        }

        public void AddCustomSource(string schemaId, Func<IEnumerable<string>> lister)
        {
            customSourcesBySchemeId[schemaId] = lister;
        }

        public struct Context
        {
            public bool IsValid;
            public List<string> Path;
            public bool SuggestingValue;
            public List<string> ExistingProperties;
        }

        private IEnumerable<AutocompleteItem> BuildList()
        {
            var context = ParseOutContext(textBox.CurrentPosition);
            if (!context.IsValid)
            {
                yield break;
            }

            var targetSchemas = JsonSchemaTools.GetSchemasForPath(schema, context.Path);
            if (targetSchemas.Count == 0)
            {
                yield break;
            }

            if (context.SuggestingValue)
            {
                foreach (var targetSchema in targetSchemas)
                {
                    // Add a divider between schema alternatives.
                    if (targetSchema != targetSchemas.First())
                    {
                        yield return new DividerItem();
                    }

                    // Help text.
                    var title = targetSchema.Title ?? (targetSchema as JsonProperty)?.Name;
                    var description = targetSchema.Description ?? JsonSchemaTools.DescribeSchema(targetSchema);

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
                    else if (targetSchema.Type == JsonObjectType.Object)
                    {
                        // Construct as much of an object as required.
                        var left = "{\n   ";
                        var right = "\n}";
                        var addedAnyToLeft = false;
                        var toAddOnLeft = 1;
                        foreach (var property in targetSchema.ActualProperties)
                        {
                            if (property.Value.IsRequired)
                            {
                                var propertyText = "\"" + property.Key + "\": ";
                                if (property.Value.Enumeration.Count == 1)
                                {
                                    propertyText += JsonSchemaTools.FormatEnumValue(property.Value.Enumeration.First());
                                    toAddOnLeft++;  // Keep adding required properties before the cursor.
                                }

                                if (toAddOnLeft > 0)
                                {
                                    addedAnyToLeft = true;
                                    toAddOnLeft--;
                                    left += propertyText;
                                    if (toAddOnLeft > 0)
                                    {
                                        left += ",\n   ";
                                    }
                                }
                                else
                                {
                                    right = "\n   " + propertyText + "," + right;
                                }
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
                }
            }
            else
            {
                foreach (var targetSchema in targetSchemas)
                {
                    var curSchema = targetSchema;
                    do
                    {
                        foreach (var propertyDef in curSchema.ActualProperties)
                        {
                            if (!context.ExistingProperties.Contains(propertyDef.Key))
                            {
                                yield return new PropertySuggestItem(propertyDef.Key, propertyDef.Value.ActualPropertySchema);
                            }
                        }
                    } while ((curSchema = curSchema.InheritedSchema) != null);

                    if (targetSchema != targetSchemas.Last())
                    {
                        yield return new DividerItem();
                    }
                }
            }
        }

        public Context ParseOutContext(int position)
        {
            var result = default(Context);
            result.IsValid = true;

            // Parse JSON until the the current position (or until earlier failure), constructing a path.
            result.Path = new List<string>();
            var existingPropertiesStack = new List<List<string>>();
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

                switch (reader.TokenType)
                {
                    case Newtonsoft.Json.JsonToken.StartObject:
                        existingPropertiesStack.Add(new List<string>());
                        if (lastProperty != null)
                        {
                            result.Path.Add(lastProperty);
                            lastProperty = null;
                        }

                        break;
                    case Newtonsoft.Json.JsonToken.StartArray:
                        if (lastProperty != null)
                        {
                            result.Path.Add(lastProperty);
                        }

                        lastProperty = "0";  // We assume arrays are heterogeneous, so all items are equivalent to [0].
                        break;
                    case Newtonsoft.Json.JsonToken.PropertyName:
                        lastProperty = reader.Value as string;
                        existingPropertiesStack.Last().Add(lastProperty);
                        break;
                    case Newtonsoft.Json.JsonToken.EndObject:
                        existingPropertiesStack.RemoveAt(existingPropertiesStack.Count - 1);
                        lastProperty = result.Path.Last();
                        result.Path.RemoveAt(result.Path.Count - 1);
                        break;
                    case Newtonsoft.Json.JsonToken.EndArray:
                        lastProperty = result.Path.Last();
                        result.Path.RemoveAt(result.Path.Count - 1);

                        break;
                    case Newtonsoft.Json.JsonToken.Integer:
                    case Newtonsoft.Json.JsonToken.Float:
                    case Newtonsoft.Json.JsonToken.String:
                    case Newtonsoft.Json.JsonToken.Boolean:
                    case Newtonsoft.Json.JsonToken.Null:
                    case Newtonsoft.Json.JsonToken.Undefined:
                        // Primitive value types. These don't affect the path.
                        break;
                    case Newtonsoft.Json.JsonToken.None:
                    case Newtonsoft.Json.JsonToken.Raw:
                    case Newtonsoft.Json.JsonToken.Date:
                    case Newtonsoft.Json.JsonToken.Bytes:
                    case Newtonsoft.Json.JsonToken.StartConstructor:
                    case Newtonsoft.Json.JsonToken.EndConstructor:
                        // We don't expect to ever see these.
                        break;
                    case Newtonsoft.Json.JsonToken.Comment:
                    default:
                        break;
                }
            }

            result.SuggestingValue = Regex.IsMatch(textBox.Text.Substring(0, position), @":\s*$") || lastProperty == "0";
            if (result.SuggestingValue)
            {
                if (lastProperty != null)
                {
                    result.Path.Add(lastProperty);
                }
            }
            else
            {
                if (existingPropertiesStack.Count > 0)
                {
                    result.ExistingProperties = existingPropertiesStack.Last();
                }
                else
                {
                    result.ExistingProperties = new List<string>();
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
                MenuText = name;
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
                var result = Parent.Fragment.Text;
                if (!Regex.IsMatch(Parent.Fragment.Text, @"\n\s*$"))
                {
                    // Not already indented. Auto-indent.
                    var textbox = textboxWrapper.target;
                    var curLineText = textbox.Lines[textbox.CurrentLine].Text;
                    result += "\n" + Regex.Match(curLineText, @"^[ \t]*").Value;
                    if (Regex.IsMatch(Parent.Fragment.Text, @"[{\[]\s*$"))
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

        // A suggest list entry with no text to divide between sections.
        internal class DividerItem : AutocompleteItem
        {
            public DividerItem()
            {
                MenuText = "---------------------------------------";
                ToolTipTitle = ToolTipText = Text = null;
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
