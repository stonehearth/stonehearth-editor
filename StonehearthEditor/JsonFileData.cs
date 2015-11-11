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
      private List<ModuleFile> mLinkedAliases = new List<ModuleFile>();
      private List<string> mLinkedFilePaths = new List<string>();
      private List<FileData> mOpenedJsonFiles = new List<FileData>();
      private List<FileData> mRelatedJsonFiles = new List<FileData>();

      public JsonFileData(string path)
      {
         mPath = path;
      }

      protected override void LoadInternal()
      {
         mOpenedJsonFiles.Add(this);
         string jsonString = FlatFileData;
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
         ParseLinkedAliases(jsonString);
         ParseLinkedFiles(jsonString);
         ParseJsonSpecificData();
      }
      private void ParseJsonSpecificData()
      {
         string directory = System.IO.Path.GetDirectoryName(Path);
         switch (mJsonType)
         {
            case JSONTYPE.ENTITY:
               JToken entityFormsComponent = mJson.SelectToken("components.stonehearth:entity_forms");
               if (entityFormsComponent != null)
               {
                  // Look for stonehearth:entity_forms
                  JToken ghostForm = entityFormsComponent["ghost_form"];
                  if (ghostForm != null)
                  {
                     string ghostFilePath = JsonHelper.GetFileFromFileJson(ghostForm.ToString(), directory);
                     ghostFilePath = JsonHelper.NormalizeSystemPath(ghostFilePath);
                     JsonFileData ghost = new JsonFileData(ghostFilePath);
                     ghost.Load();
                     mOpenedJsonFiles.Add(ghost);
                  }
                  JToken iconicForm = entityFormsComponent["iconic_form"];
                  if (iconicForm != null)
                  {
                     string iconicFilePath = JsonHelper.GetFileFromFileJson(iconicForm.ToString(), directory);
                     iconicFilePath = JsonHelper.NormalizeSystemPath(iconicFilePath);
                     JsonFileData iconic = new JsonFileData(iconicFilePath);
                     iconic.Load();
                     mOpenedJsonFiles.Add(iconic);
                  }
               }
               break;
            case JSONTYPE.JOB:
               // Parse crafter stuff
               JToken crafter = mJson["crafter"];
               if (crafter != null)
               {
                  // This is a crafter, load its recipes
                  string recipeListLocation = crafter["recipe_list"].ToString();
                  recipeListLocation = JsonHelper.GetFileFromFileJson(recipeListLocation, directory);
                  JsonFileData recipes = new JsonFileData(recipeListLocation);
                  recipes.Load();
                  foreach (string recipePath in recipes.LinkedFilePaths)
                  {
                     JsonFileData recipe = new JsonFileData(recipePath);
                     recipe.Load();
                     recipes.mRelatedJsonFiles.Add(recipe);
                  }
                  mOpenedJsonFiles.Add(recipes);
               }
               break;
            case JSONTYPE.RECIPE:
               JToken portrait = mJson["portrait"];
               if (portrait != null)
               {
                  string portraitImageLocation = portrait.ToString();
                  portraitImageLocation = JsonHelper.GetFileFromFileJson(portraitImageLocation, directory);
                  mLinkedFilePaths.Add(portraitImageLocation);
               }

               break;
         }
      }

      private void ParseLinkedFiles(string jsonString)
      {
         string directory = System.IO.Path.GetDirectoryName(Path);
         Regex matcher = new Regex("file\\([\\S]+\\)");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string linkedFile = JsonHelper.GetFileFromFileJson(match.Value, directory);
            linkedFile = JsonHelper.NormalizeSystemPath(linkedFile);
            mLinkedFilePaths.Add(linkedFile);
         }
      }

      private void ParseLinkedAliases(string jsonString)
      {
         Regex matcher = new Regex("\"([A-z|_|-]+\\:[\\S]*)\"");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string fullAlias = match.Groups[1].Value;
            ModuleFile linkedAlias = ModuleDataManager.GetInstance().GetModuleFile(fullAlias);
            if (linkedAlias == null)
            {
               continue;
            }
            mLinkedAliases.Add(linkedAlias);
         }
      }

      public override void UpdateTreeNode(TreeNode node)
      {
         node.SelectedImageIndex = (int)JsonType;
         node.ImageIndex = (int)JsonType;
         if (JsonType == JSONTYPE.JOB)
         {
            if (mOpenedJsonFiles.Count > 1)
            {
               FileData recipeJsonData = mOpenedJsonFiles[1];
               TreeNode recipes = new TreeNode(recipeJsonData.FileName);
               foreach (string recipePath in recipeJsonData.LinkedFilePaths)
               {
                  string recipeName = System.IO.Path.GetFileNameWithoutExtension(recipePath);
                  TreeNode recipeNode = new TreeNode(recipeName);
                  recipeNode.ImageIndex = (int)JSONTYPE.RECIPE;
                  recipeNode.SelectedImageIndex = (int)JSONTYPE.RECIPE;
                  recipes.Nodes.Add(recipeNode);
               }
               node.Nodes.Add(recipes);
            }
         }
      }

      public JSONTYPE JsonType
      {
         get { return mJsonType; }
      }
      public override List<ModuleFile> LinkedAliases
      {
         get { return mLinkedAliases; }
      }
      public override List<string> LinkedFilePaths
      {
         get { return mLinkedFilePaths; }
      }

      public override string Path
      {
         get { return mPath; }
      }
      public override List<FileData> OpenedFiles
      {
         get { return mOpenedJsonFiles; }
      }
      public override List<FileData> RelatedFiles
      {
         get { return mRelatedJsonFiles; }
      }
   }
}
