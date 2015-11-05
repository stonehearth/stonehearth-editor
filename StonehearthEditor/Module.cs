using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
   public class Module
   {
      private string mPath;
      private string mName;
      private XDocument mManifestDocument;
      private Dictionary<String, ModuleFile> mAliases = new Dictionary<String, ModuleFile>();
      private JObject mEnglishLocalizationJson;
      public Module(string modPath)
      {
         mPath = modPath;
         mName = modPath.Substring(modPath.LastIndexOf('/') + 1);
      }
      public ICollection<String> GetAliasNames()
      {
         return mAliases.Keys;
      }
      public ICollection<ModuleFile> GetAliases()
      {
         return mAliases.Values;
      }
      public String Name
      {
         get { return mName; }
      }
      public String Path
      {
         get { return mPath; }
      }

      public JObject EnglishLocalizationJson
      {
         get { return mEnglishLocalizationJson; }
      }
      public void Load()
      {
         string stonehearthModManifest = Path + "/manifest.json";

         XmlDictionaryReaderQuotas quota = new XmlDictionaryReaderQuotas();
         quota.MaxNameTableCharCount = 500000;
         if (System.IO.File.Exists(stonehearthModManifest))
         {
            using (StreamReader sr = new StreamReader(stonehearthModManifest, Encoding.UTF8))
            {
               XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(sr.BaseStream, quota);
               mManifestDocument = XDocument.Load(reader);
               XElement aliases = mManifestDocument.Root.Element("aliases");
               if (aliases != null)
               {
                  foreach (XElement alias in aliases.Elements())
                  {
                     string name = alias.Name.ToString();
                     string value = alias.Value;
                     if (alias.Attribute("item") != null)
                     {
                        name = alias.Attribute("item").Value;
                     }
                     name = name.Trim();
                     value = value.Trim();

                     ModuleFile moduleFile = new ModuleFile(this, name, value);
                     moduleFile.TryLoad();
                     mAliases.Add(name, moduleFile);
                  }
               }
            }
         }

         string englishLocalizationFilePath = Path + "/locales/en.json";
         if (System.IO.File.Exists(englishLocalizationFilePath))
         {
            using (StreamReader sr = new StreamReader(englishLocalizationFilePath, Encoding.UTF8))
            {
               string fileString = sr.ReadToEnd();
               mEnglishLocalizationJson = JObject.Parse(fileString);
            }
         }
      }

      public ModuleFile GetAliasFile(string alias)
      {
         ModuleFile returned = null;
         mAliases.TryGetValue(alias, out returned);
         return returned;
      }
   }
}
