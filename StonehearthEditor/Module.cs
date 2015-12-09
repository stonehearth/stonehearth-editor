using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public class Module
   {
      private string mPath;
      private string mName;
      private JObject mManifestJson;
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
      public void InitializeFromManifest()
      {
         string stonehearthModManifest = Path + "/manifest.json";

         if (System.IO.File.Exists(stonehearthModManifest))
         {
            try {
               using (StreamReader sr = new StreamReader(stonehearthModManifest, Encoding.UTF8))
               {
                  string fileString = sr.ReadToEnd();
                  mManifestJson = JObject.Parse(fileString);

                  JToken aliases = mManifestJson["aliases"];
                  if (aliases != null)
                  {
                     foreach (JToken item in aliases.Children())
                     {
                        JProperty alias = item as JProperty;
                        string name = alias.Name.Trim();
                        string value = alias.Value.ToString().Trim();

                        ModuleFile moduleFile = new ModuleFile(this, name, value);
                        mAliases.Add(name, moduleFile);
                     }
                  }
               }
            } catch (Exception e)
            {
               MessageBox.Show("Failure while reading manifest file " + stonehearthModManifest + ". Error: " + e.Message);
            }
         }

         string englishLocalizationFilePath = Path + "/locales/en.json";
         if (System.IO.File.Exists(englishLocalizationFilePath))
         {
            try {
               using (StreamReader sr = new StreamReader(englishLocalizationFilePath, Encoding.UTF8))
               {
                  string fileString = sr.ReadToEnd();
                  mEnglishLocalizationJson = JObject.Parse(fileString);
               }
            } catch(Exception e)
            {
               MessageBox.Show("Exception while reading localization json " + englishLocalizationFilePath + ". Error: " + e.Message);
            }
         }
      }
      public void LoadFiles()
      {
         foreach (ModuleFile moduleFile in mAliases.Values) { 
            moduleFile.TryLoad();
         }
      }
      public ModuleFile GetAliasFile(string alias)
      {
         ModuleFile returned = null;
         mAliases.TryGetValue(alias, out returned);
         return returned;
      }
      private void Sort(JObject jObj)
      {
         List<JProperty> properties = new List<JProperty>(jObj.Properties());
         foreach (var prop in properties)
         {
            prop.Remove();
         }
         properties.Sort(
            delegate (JProperty a, JProperty b)
            {
               return a.Name.CompareTo(b.Name);
            }
         );
         foreach (var prop in properties)
         {
            jObj.Add(prop);
         }
      }

      public void AddToManifest(string alias, string path)
      {
         JToken aliases = mManifestJson["aliases"];
         if (aliases == null)
         {
            mManifestJson.Add("aliases", null);
            aliases = mManifestJson["aliases"];
         }
         JObject aliasesObject = aliases as JObject;
         aliasesObject.Add(alias, path);
         Sort(aliasesObject);
      }

      public void RemoveFromManifest(string alias)
      {
         JObject aliases = mManifestJson["aliases"] as JObject;
         aliases.Property(alias).Remove();
      }

      public void WriteManifestToFile()
      {
         string manifestPath = Path + "/manifest.json";
         using (StreamWriter wr = new StreamWriter(manifestPath, false, new UTF8Encoding(false)))
         {
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
            {
               jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
               jsonTextWriter.Indentation = 3;
               jsonTextWriter.IndentChar = ' ';

               JsonSerializer jsonSeralizer = new JsonSerializer();
               jsonSeralizer.Serialize(jsonTextWriter, mManifestJson);
            }
         }
      }

      public void WriteEnglishLocalizationToFile()
      {
         string englishLocalizationFilePath = Path + "/locales/en.json";
         using (StreamWriter wr = new StreamWriter(englishLocalizationFilePath, false, new UTF8Encoding(false)))
         {
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(wr))
            {
               jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
               jsonTextWriter.Indentation = 3;
               jsonTextWriter.IndentChar = ' ';

               JsonSerializer jsonSeralizer = new JsonSerializer();
               jsonSeralizer.Serialize(jsonTextWriter, mEnglishLocalizationJson);
            }
         }
      }
      public void PostLoadFixup()
      {
         foreach (ModuleFile moduleFile in mAliases.Values)
         {
            moduleFile.PostLoadFixup();
         }
      }
   }
}
