using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public class ModuleDataManager
   {
      private static ModuleDataManager sInstance = null;
      public static ModuleDataManager GetInstance()
      {
         return sInstance;

      }
      private string mModsDirectoryPath;
      private Dictionary<String, Module> mModules = new Dictionary<String, Module>();
      public ModuleDataManager(string modsDirectoryPath)
      {
         mModsDirectoryPath = modsDirectoryPath;
         sInstance = this;
      }

      public string ModsDirectoryPath
      {
         get { return mModsDirectoryPath; }
      }

      public void Load()
      {
         // Parse Manifests
         string[] modFolders = Directory.GetDirectories(mModsDirectoryPath);
         foreach (string modPath in modFolders)
         {
            string formatted = JsonHelper.NormalizeSystemPath(modPath);
            Module module = new Module(formatted);
            module.Load();
            mModules.Add(module.Name, module);
         }
      }

      public void FillAliasTree(TreeView treeView)
      {
         foreach (Module module in mModules.Values)
         {
            ICollection<ModuleFile> aliases = module.GetAliases();
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (ModuleFile alias in aliases)
            {
               if (alias.FileType == FileType.JSON && alias.FileData != null)
               {
                  JSONTYPE type = alias.FileData.JsonType;
                  nodes.Add(new TreeNode(alias.Name, (int)type, (int)type));
               }
            }

            TreeNode treeNode = new TreeNode(module.Name, nodes.ToArray());
            treeNode.ImageIndex = 100;
            treeNode.SelectedImageIndex = 100;
            treeView.Nodes.Add(treeNode);
         }
      }

      public ModuleFile GetSelectedModuleFile(TreeNode selected)
      {
         if (selected != null && selected.Parent != null)
         {
            Module module = mModules[selected.Parent.Text];
            ModuleFile file = module.GetAliasFile(selected.Text);
            return file;
         }
         return null;
      }

      public ICollection<Module> GetAllModules()
      {
         return mModules.Values;
      }

      public Module GetMod(string modName)
      {
         if (!mModules.ContainsKey(modName))
         {
            return null;
         }
         return mModules[modName];
      }

      public string LocalizeString(string key)
      {
         string[] split = key.Split(':');
         string modName = "stonehearth";
         if (split.Length > 1)
         {
            modName = split[0];
            key = split[1];
         }
         Module mod = GetMod(modName);
         if (mod != null)
         {
            JToken token = mod.EnglishLocalizationJson.SelectToken(key);
            if (token != null)
            {
               return token.ToString();
            }
         }
         return key;
      }
   }
}
