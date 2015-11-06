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
         string returned = String.Empty;
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

      public static string GetFileFromFileJson(string fileJson, string parentPath)
      {
         string fullPath = String.Empty;
         if (fileJson.StartsWith("file("))
         {
            string temp = fileJson.Substring(5);
            fullPath = temp.Substring(0, temp.Length - 1);
         }
         else
         {
            fullPath = fileJson;
         }

         if (fileJson.IndexOf('.') < 0)
         {
            string folderName = fullPath.Substring(fullPath.LastIndexOf('/') + 1);
            fullPath = fullPath + "/" + folderName + ".json";
         }
         if (fullPath.StartsWith("/"))
         {
            fullPath = ModuleDataManager.GetInstance().ModsDirectoryPath + fullPath;
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
         return normalized;
      }

      // Word Wrap code taken from http://www.softcircuits.com/Blog/post/2010/01/10/Implementing-Word-Wrap-in-C.aspx
      public const string _newline = "\r\n";

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
            int eol = the_string.IndexOf(_newline, pos);

            if (eol == -1)
               next = eol = the_string.Length;
            else
               next = eol + _newline.Length;

            // Copy this line of text, breaking into smaller lines as needed
            if (eol > pos)
            {
               do
               {
                  int len = eol - pos;

                  if (len > width)
                     len = BreakLine(the_string, pos, width);

                  sb.Append(the_string, pos, len);
                  sb.Append(_newline);

                  // Trim whitespace following break
                  pos += len;

                  while (pos < eol && Char.IsWhiteSpace(the_string[pos]))
                     pos++;

               } while (eol > pos);
            }
            else sb.Append(_newline); // Empty line
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
         while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
            i--;
         if (i < 0)
            return max; // No whitespace found; break at maximum length
                        // Find start of whitespace
         while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
            i--;
         // Return length of text before whitespace
         return i + 1;
      }
   }
}
