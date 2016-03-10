using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
   public enum FileType
   {
      UNKNOWN = 0,
      LUA = 1,
      JSON = 2,
   };
   public class ModuleFile
   {
      private Module mModule;
      private string mAlias;
      private string mOriginalFilePath;
      private string mRootFile;
      private string mShortName;
      private FileData mFileData = null;
      private FileType mType = FileType.UNKNOWN;
      private bool mIsFineVersion = false;
      // Needed because at the timer we load aliases, the referenced alias might not be loaded
      protected Dictionary<string, FileData> mReferencesCache = new Dictionary<string, FileData>();
      public ModuleFile(Module module, string alias, string filePath)
      {
         mModule = module;
         mAlias = alias;
         mOriginalFilePath = filePath;
         mRootFile = JsonHelper.GetFileFromFileJson(filePath, module.Path);
         int lastColon = Name.LastIndexOf(':');
         mShortName = lastColon > -1 ? Name.Substring(lastColon + 1) : Name;
         if (mShortName.Equals("fine"))
         {
            string oneBefore = Name.Substring(0, lastColon);
            int secondToLastColon = oneBefore.LastIndexOf(':');
            oneBefore = secondToLastColon > -1 ? oneBefore.Substring(secondToLastColon + 1) : oneBefore;
            mShortName = oneBefore;
            mIsFineVersion = true;
         }
         DetermineFileType();
      }

      public bool IsFineVersion
      {
         get { return mIsFineVersion; }
      }

      private void DetermineFileType()
      {
         if (mRootFile.EndsWith(".lua"))
         {
            mType = FileType.LUA;
         }
         else if (mRootFile.EndsWith(".json"))
         {
            mType = FileType.JSON;
         }
      }

      public void TryLoad()
      {
         IModuleFileData fileData = null;
         if (mType == FileType.JSON) // only load Json
         {
            fileData = new JsonFileData(ResolvedPath);

         } else if (mType == FileType.LUA)
         {
            fileData = new LuaFileData(ResolvedPath);
         }

         if (fileData != null)
         {
            fileData.SetModuleFile(this);
            mFileData = fileData as FileData;
            mFileData.Load();
            foreach (KeyValuePair<string, FileData> data in mReferencesCache)
            {
               mFileData.ReferencedByFileData[data.Key] = data.Value;
            }
         }
      }

      public FileData GetFileData(string[] path)
      {
         if (path.Length == 3)
         {
            return this.FileData;
         }
         return FindFileData(FileData, path, 3);
      }

      private FileData FindFileData(FileData start, string[] path, int startIndex)
      {
         if (startIndex >= path.Length || start == null)
         {
            return start;
         }
         string subfileName = path[startIndex];
         FileData found = null;
         foreach (FileData openedFile in start.OpenedFiles)
         {
            if (openedFile.FileName.Equals(subfileName))
            {
               found = openedFile;
               break;
            }
         }
         if (found == null)
         {
            foreach (FileData openedFile in start.RelatedFiles)
            {
               if (openedFile.FileName.Equals(subfileName))
               {
                  found = openedFile;
                  break;
               }
            }
         }

         return FindFileData(found, path, startIndex + 1);
      }

      public TreeNode GetTreeNode(string filter)
      {
         TreeNode treeNode = new TreeNode(Name);
         if (mFileData != null)
         {
            bool matchesFilter = mFileData.UpdateTreeNode(treeNode, filter);
            if (matchesFilter)
            {
               return treeNode;
            }
         }
         return null;
      }

      public string Name
      {
         get { return mAlias; }
      }
      public Module Module
      {
         get { return mModule; }
      }

      public FileType FileType
      {
         get { return mType; }
      }
      public FileData FileData
      {
         get { return mFileData; }
      }
      public string FlatFileData
      {
         get { return mFileData.FlatFileData; }
      }

      public string OriginalPath
      {
         get { return mOriginalFilePath; }
      }

      public string ResolvedPath
      {
         get { return mRootFile; }
      }

      public bool Clone(CloneObjectParameters parameters, HashSet<string> alreadyCloned, bool execute)
      {
         string newAlias = parameters.TransformAlias(mAlias);
         if (mModule.GetAliasFile(newAlias) != null)
         {
            MessageBox.Show("The alias " + newAlias + " already exists in manifest.json");
            return false;
         }
         string newPath = parameters.TransformParameter(ResolvedPath);
         if (!FileData.Clone(newPath, parameters, alreadyCloned, execute))
         {
            return false;
         }
         alreadyCloned.Add(mModule.Name + ':' + newAlias);
         if (execute)
         {
            string fileLocation = "file(" + newPath.Replace(mModule.Path + "/", "") + ")";
            ModuleFile file = new ModuleFile(Module, newAlias, fileLocation);
            file.TryLoad();
            if (file.FileData != null)
            {
               mModule.AddToManifest(newAlias, fileLocation);
               mModule.WriteManifestToFile();
               return true;
            }
         }
         else
         {
            return true;
         }
         return false;
      }

      public JsonFileData GetJsonFileDataByTerm(string filterTerm)
      {
         JsonFileData jsonFileData = FileData as JsonFileData;
         if (jsonFileData == null) return null;
         JObject json = jsonFileData.Json;
         if (json != null)
         {
            JToken token = json.SelectToken(filterTerm);
            return token == null ? null : jsonFileData;
         }
         return null;
      }

      public string ShortName
      {
         get { return mShortName; }
      }
      public string FullAlias
      {
         get { return mModule.Name + ':' + mAlias; }
      }

      public void AddReference(string name, FileData fileData)
      {
         if (FileData != null)
         {
            FileData.ReferencedByFileData[name] = fileData;
         }
         else
         {
            mReferencesCache[name] = fileData;
         }
      }

      public void PostLoadFixup()
      {
         FixupLootTables();
         RecommendNetWorth();
      }

      private static int kWorkUnitsWorth = 2;
      private void RecommendNetWorth()
      {
         JsonFileData jsonFileData = FileData as JsonFileData;
         if (jsonFileData == null)
         {
            return;
         }
         foreach (FileData reference in jsonFileData.ReferencedByFileData.Values)
         {
            JsonFileData refJson = reference as JsonFileData;
            if (refJson != null && refJson.JsonType == JSONTYPE.RECIPE)
            {
               JArray produces = refJson.Json["produces"] as JArray;
               int productCount = 0;
               foreach (JToken product in produces)
               {
                  JToken item = product["item"];
                  if (item != null && item.ToString().Equals(FullAlias))
                  {
                     productCount++;
                  }
                  JToken fine = product["fine"];
                  if (fine != null && fine.ToString().Equals(FullAlias))
                  {
                     productCount++;
                  }
               }

               if (productCount <= 0)
               {
                  continue;
               }

               int totalCost = 0;
               // If we are created by a recipe, look at the ingredients for the recipe to calculate net worth of all ingredients ...
               JArray ingredients = refJson.Json["ingredients"] as JArray;
               if (ingredients != null)
               {
                  foreach(JToken ingredient in ingredients)
                  {
                     int costPer = 0;
                     JToken material = ingredient["material"];
                     if (material != null)
                     {
                        // try get cost of material
                        string materialString = material.ToString();
                        costPer = ModuleDataManager.GetInstance().GetAverageMaterialCost(materialString);
                     }
                     JToken uri = ingredient["uri"];
                     if (uri != null)
                     {
                        // try get cost of material
                        string uriString = uri.ToString();
                        ModuleFile file = ModuleDataManager.GetInstance().GetModuleFile(uriString);
                        if (file != null)
                        {
                           costPer = (file.FileData as JsonFileData).NetWorth;
                        }
                     }
                     int count = int.Parse(ingredient["count"].ToString());
                     totalCost = totalCost + costPer * count;
                  }
               }
               jsonFileData.RecommendedMinNetWorth = totalCost / productCount;

               JToken workUnits = refJson.Json["work_units"];
               if (workUnits != null)
               {
                  int units = int.Parse(workUnits.ToString());
                  totalCost = totalCost + units * kWorkUnitsWorth;
               }
               jsonFileData.RecommendedMaxNetWorth = totalCost / productCount;
            }
         }
      }

      private void FixupLootTables()
      {
         // Fixup loot tables
         if (Name == "mining:base_loot_table")
         {
            JsonFileData jsonFileData = FileData as JsonFileData;

            if (JsonHelper.FixupLootTable(jsonFileData.Json, "mineable_blocks.*"))
            {
               jsonFileData.TrySetFlatFileData(jsonFileData.GetJsonFileString());
               jsonFileData.TrySaveFile();
            }
         }
         else
         {
            JsonFileData jsonFileData = FileData as JsonFileData;
            if (jsonFileData != null && jsonFileData.Json != null)
            {
               JToken harvestLootTable = jsonFileData.Json.SelectToken("entity_data.stonehearth:harvest_beast_loot_table");
               if (harvestLootTable != null)
               {
                  if (harvestLootTable["entries"] == null)
                  {
                     if (JsonHelper.FixupLootTable(jsonFileData.Json, "entity_data.stonehearth:harvest_beast_loot_table"))
                     {
                        jsonFileData.TrySetFlatFileData(jsonFileData.GetJsonFileString());
                        jsonFileData.TrySaveFile();
                     }
                  }
               }
               JToken destroyedLootTable = jsonFileData.Json.SelectToken("entity_data.stonehearth:destroyed_loot_table");
               if (destroyedLootTable != null)
               {
                  if (destroyedLootTable["entries"] == null)
                  {
                     if (JsonHelper.FixupLootTable(jsonFileData.Json, "entity_data.stonehearth:destroyed_loot_table"))
                     {
                        jsonFileData.TrySetFlatFileData(jsonFileData.GetJsonFileString());
                        jsonFileData.TrySaveFile();
                     }
                  }
               }
            }
         }
      }
   }
}