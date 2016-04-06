using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace StonehearthEditor
{
    public static class JsonHelper
    {
        public static string GetJsonStringValue(XElement root, string elementName)
        {
            string returned = string.Empty;
            XElement selectedElement = root.Element(elementName);
            if (selectedElement != null && selectedElement.Value != null)
            {
                returned = selectedElement.Value.Trim();
            }

            return returned;
        }

        public static List<string> GetJsonStringArray(XElement root, string elementName)
        {
            List<string> returned = new List<string>();
            XElement selectedElement = root.Element(elementName);
            if (selectedElement != null)
            {
                if (selectedElement.Attribute("type").Value.Equals("array"))
                {
                    foreach (XElement child in selectedElement.Elements())
                    {
                        returned.Add(child.Value);
                    }
                }
                else
                {
                    returned.Add(selectedElement.Value);
                }
            }

            return returned;
        }

        public static List<string> GetJsonStringArray(JToken root, string elementName)
        {
            List<string> returned = new List<string>();
            JToken selectedElement = root[elementName];
            if (selectedElement != null)
            {
                IList<JToken> results = selectedElement.ToList();
                if (results.Count > 0)
                {
                    foreach (JToken child in results)
                    {
                        returned.Add(child.ToString());
                    }
                }
                else
                {
                    returned.Add(selectedElement.ToString());
                }
            }

            return returned;
        }

        public static Dictionary<string, string> GetJsonStringDictionary(XElement root, string elementName)
        {
            Dictionary<string, string> returned = new Dictionary<string, string>();
            XElement selectedElement = root.Element(elementName);
            if (selectedElement != null)
            {
                foreach (XElement element in selectedElement.Elements())
                {
                    string name = element.Name.ToString();
                    string value = element.Value;
                    if (element.Attribute("item") != null)
                    {
                        name = element.Attribute("item").Value;
                    }

                    name = name.Trim();
                    value = value.Trim();
                    returned.Add(name, value);
                }
            }

            return returned;
        }

        public static Dictionary<string, string> GetJsonStringDictionary(JToken root, string elementName)
        {
            Dictionary<string, string> returned = new Dictionary<string, string>();
            JToken selectedElement = root[elementName];
            if (selectedElement != null)
            {
                foreach (JToken item in selectedElement)
                {
                    JProperty property = (JProperty)item;
                    string name = property.Name;
                    if (property.Value.HasValues)
                    {
                        returned.Add(property.Name, property.Value.First.ToString());
                    }
                    else
                    {
                        returned.Add(property.Name, property.Value.ToString());
                    }
                }
            }

            return returned;
        }

        // This function from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");
            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                // path can't be made relative.
                return toPath;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            return relativePath;
        }

        public static string GetFileFromFileJson(string fileJson, string parentPath)
        {
            parentPath = JsonHelper.NormalizeSystemPath(parentPath);
            string fullPath = string.Empty;
            bool startedWithFile = false;
            if (fileJson.StartsWith("file("))
            {
                string temp = fileJson.Substring(5);
                fullPath = temp.Substring(0, temp.Length - 1);
                startedWithFile = true;
            }
            else
            {
                fullPath = fileJson;
            }

            if (fullPath.IndexOf("../") >= 0)
            {
                // If there's a relative directory
                int index = fullPath.IndexOf("../");
                char[] delimeter = "/".ToCharArray();
                string[] splitParentPath = parentPath.Split(delimeter);
                int parentPathCount = splitParentPath.Length + 1;
                while (index >= 0)
                {
                    parentPathCount--;
                    fullPath = fullPath.Substring(index + 3);
                    index = fullPath.IndexOf("../");
                }

                splitParentPath = parentPath.Split(delimeter, parentPathCount);
                splitParentPath[splitParentPath.Length - 1] = "";
                parentPath = string.Join("/", splitParentPath);
                parentPath = parentPath.Substring(0, parentPath.Length - 1);
            }
            else if (fileJson.IndexOf('.') < 0)
            {
                string folderName = fullPath.Substring(fullPath.LastIndexOf('/') + 1);
                fullPath = fullPath + "/" + folderName + ".json";
            }

            if (fullPath.StartsWith("/"))
            {
                if (startedWithFile)
                {
                    string mod = parentPath.Replace(MainForm.kModsDirectoryPath + '/', "");
                    int firstSlash = mod.IndexOf('/');
                    if (firstSlash >= 0)
                    {
                        mod = mod.Substring(0, firstSlash);
                    }

                    fullPath = ModuleDataManager.GetInstance().ModsDirectoryPath + '/' + mod + fullPath;
                }
                else
                {
                    fullPath = ModuleDataManager.GetInstance().ModsDirectoryPath + fullPath;
                }
            }
            else
            {
                fullPath = parentPath + "/" + fullPath;
            }

            return fullPath;
        }

        public static string NormalizeSystemPath(string path)
        {
            string normalized = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (normalized.Length > 1 && normalized[1] == ':')
            {
                normalized = char.ToUpperInvariant(normalized[0]) + normalized.Substring(1);
            }

            return normalized;
        }

        // Word Wrap code taken from http://www.softcircuits.com/Blog/post/2010/01/10/Implementing-Word-Wrap-in-C.aspx
        private const string kNewline = "\r\n";

        public static string WordWrap(string the_string, int width)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (width < 1)
                return the_string;

            // Parse each line of text
            for (pos = 0; pos < the_string.Length; pos = next)
            {
                // Find end of line
                int eol = the_string.IndexOf(kNewline, pos);

                if (eol == -1)
                    next = eol = the_string.Length;
                else
                    next = eol + kNewline.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > width)
                            len = BreakLine(the_string, pos, width);

                        sb.Append(the_string, pos, len);
                        sb.Append(kNewline);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && char.IsWhiteSpace(the_string[pos]))
                            pos++;
                    }
                    while (eol > pos);
                }
                else sb.Append(kNewline); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;
            while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
                i--;
            if (i < 0)
                return max; // No whitespace found; break at maximum length
                            // Find start of whitespace
            while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
                i--;

            // Return length of text before whitespace
            return i + 1;
        }

        public static string GetFormattedJsonString(JObject json)
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                using (CustomJsonWriter jsonTextWriter = new CustomJsonWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                    jsonTextWriter.Indentation = 3;
                    jsonTextWriter.IndentChar = ' ';

                    JsonSerializer jsonSeralizer = new JsonSerializer();
                    jsonSeralizer.NullValueHandling = NullValueHandling.Ignore;
                    jsonSeralizer.Serialize(jsonTextWriter, json);
                }

                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not convert json to string because of exception " + e.Message);
            }

            return null;
        }

        private class CustomJsonWriter : JsonTextWriter
        {
            private bool mWritingMinMax = false;

            public CustomJsonWriter(TextWriter textWriter)
                : base(textWriter)
            {
            }

            protected override void WriteIndent()
            {
                if (mWritingMinMax)
                {
                    WriteIndentSpace();
                }
                else
                {
                    base.WriteIndent();
                }
            }

            public override void WriteEndObject()
            {
                base.WriteEndObject();
                mWritingMinMax = false;
            }

            public override void WritePropertyName(string name)
            {
                if (name == "x" || name == "y" || name == "z")
                {
                    mWritingMinMax = true;
                }
                else
                {
                    mWritingMinMax = false;
                }

                base.WritePropertyName(name);
            }

            public override void WriteValue(object value)
            {
                base.WriteValue(value);
            }
        }

        public static bool FixupLootTable(JObject json, string selector)
        {
            bool modified = false;
            foreach (JToken lootDrops in json.SelectTokens(selector))
            {
                // Try to convert
                if (lootDrops != null && lootDrops["entries"] == null)
                {
                    // this loot drops is using the old system
                    JProperty parent = lootDrops.Parent as JProperty;
                    JObject newLootDrops = new JObject();
                    JObject entries = new JObject();
                    JObject always = new JObject();
                    JObject items = new JObject();
                    if (lootDrops["num_rolls"] != null)
                    {
                        always.Add("num_rolls", lootDrops["num_rolls"]);
                    }

                    foreach (JToken itemToken in lootDrops["items"].Children())
                    {
                        JObject item = itemToken as JObject;
                        string uri = item["uri"] != null ? item["uri"].ToString() : "";
                        if (string.IsNullOrWhiteSpace(item["uri"].ToString()))
                        {
                            items.Add("none", item);
                        }
                        else
                        {
                            int lastColon = uri.LastIndexOf(':');
                            string shortUri = lastColon > -1 ? uri.Substring(lastColon + 1) : uri;
                            string uriTest = shortUri;
                            int index = 1;
                            while (items[uriTest] != null)
                            {
                                index++;
                                uriTest = shortUri + index;
                            }

                            items.Add(uriTest, item);
                        }
                    }

                    always.Add("items", items);
                    entries.Add("default", always);
                    newLootDrops.Add("entries", entries);
                    parent.Value = newLootDrops;
                    modified = true;
                }
            }

            return modified;
        }
    }
}
