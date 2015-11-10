using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StonehearthEditor
{
   public enum JSONTYPE
   {
      NONE = 0,
      ENTITY = 1,
      BUFF = 2,
      AI_PACK = 3,
      EFFECT = 4,
      RECIPE = 5,
      COMMAND = 6,
      ANIMATION = 7,
      ENCOUNTER = 8,
      JOB = 9,
   };

   public class JsonFileData : FileData
   {
      private JSONTYPE mJsonType = JSONTYPE.NONE;
      private JObject mJson;
      private string mPath;
      private string mFileName;
      private List<ModuleFile> mLinkedAliases = new List<ModuleFile>();
      private List<string> mLinkedFiles = new List<string>();
      private List<JsonFileData> mOpenedJsonFiles = new List<JsonFileData>();

      public JsonFileData(string path)
      {
         mPath = path;
         mFileName = System.IO.Path.GetFileNameWithoutExtension(Path);
      }

      protected override void LoadInternal()
      {
         mOpenedJsonFiles.Add(this);
         string jsonString = FlatFileData;
         ParseLinkedAliases(jsonString);
         ParseLinkedFiles(jsonString);
         mJson = JObject.Parse(jsonString);
         JToken typeObject = mJson["type"];
         if (typeObject != null)
         {
            string typeString = typeObject.ToString().Trim().ToUpper();
            foreach (JSONTYPE jsonType in Enum.GetValues(typeof(JSONTYPE)))
            {
               if (typeString.Equals(jsonType.ToString()))
               {
                  mJsonType = jsonType;
               }
            }
         }

         string directory = System.IO.Path.GetDirectoryName(Path);

         if (mJsonType == JSONTYPE.ENTITY)
         {
            JToken entityFormsComponent = mJson.SelectToken("components.stonehearth:entity_forms");
            if (entityFormsComponent != null)
            {
               // Look for stonehearth:entity_forms
               JToken iconicForm = entityFormsComponent["iconic_form"];
               if (iconicForm != null)
               {
                  string iconicFilePath = JsonHelper.GetFileFromFileJson(iconicForm.ToString(), directory);
                  iconicFilePath = JsonHelper.NormalizeSystemPath(iconicFilePath);
                  JsonFileData iconic = new JsonFileData(iconicFilePath);
                  iconic.Load();
                  mOpenedJsonFiles.Add(iconic);
               }

               JToken ghostForm = entityFormsComponent["ghost_form"];
               if (ghostForm != null)
               {
                  string ghostFilePath = JsonHelper.GetFileFromFileJson(ghostForm.ToString(), directory);
                  ghostFilePath = JsonHelper.NormalizeSystemPath(ghostFilePath);
                  JsonFileData ghost = new JsonFileData(ghostFilePath);
                  ghost.Load();
                  mOpenedJsonFiles.Add(ghost);
               }
            }
         }
      }
      private void ParseLinkedFiles(string jsonString)
      {
         Regex matcher = new Regex("file\\([\\S]+\\)");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string linkedFile = JsonHelper.GetFileFromFileJson(match.Value, Path);
            linkedFile = JsonHelper.NormalizeSystemPath(linkedFile);
            mLinkedFiles.Add(linkedFile);
         }
      }

      private void ParseLinkedAliases(string jsonString)
      {
         Regex matcher = new Regex("\"([A-z|_|-]+\\:[\\S]*)\"");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string fullAlias = match.Groups[1].Value;
            int indexOfColon = fullAlias.IndexOf(':');
            string module = fullAlias.Substring(0, indexOfColon);
            string alias = fullAlias.Substring(indexOfColon + 1);
            Module mod = ModuleDataManager.GetInstance().GetMod(module);
            if (mod == null)
            {
               continue;
            }
            ModuleFile linkedAlias = mod.GetAliasFile(alias);
            if (linkedAlias == null)
            {
               continue;
            }
            mLinkedAliases.Add(linkedAlias);
         }
      }

      public override void UpdateTreeNode(TreeNode node)
      {
         node.SelectedImageIndex = (int) JsonType;
         node.ImageIndex = (int)JsonType;
      }

      public JSONTYPE JsonType
      {
         get { return mJsonType; }
      }
      public override List<ModuleFile> LinkedAliases
      {
         get { return mLinkedAliases; }
      }
      public override List<FileData> LinkedFiles
      {
         get { return null; }
      }
      public List<JsonFileData> OpenedJsonFiles
      {
         get { return mOpenedJsonFiles; }
      }

      public override string Path
      {
         get { return mPath; }
      }
      public string FileName
      {
         get { return mFileName; }
      }
   }
}
