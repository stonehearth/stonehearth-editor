using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

   public interface IModuleFileData
   {
      void SetModuleFile(ModuleFile moduleFile);
      ModuleFile GetModuleFile();
   }

   public class JsonFileData : FileData, IModuleFileData
   {
      private ModuleFile mOwner;
      private JSONTYPE mJsonType = JSONTYPE.NONE;
      private JObject mJson;
      private string mPath;
      private string mDirectory;
      private List<ModuleFile> mLinkedAliases = new List<ModuleFile>();
      private List<string> mLinkedFilePaths = new List<string>();
      private List<FileData> mOpenedJsonFiles = new List<FileData>();
      private List<FileData> mRelatedJsonFiles = new List<FileData>();

      public JsonFileData(string path)
      {
         mPath = path;
         mDirectory = JsonHelper.NormalizeSystemPath(System.IO.Path.GetDirectoryName(Path));
      }

      protected override void LoadInternal()
      {
         try {
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
         } catch(Exception e)
         {
            MessageBox.Show("Failed to load json file " + mPath + ". Error: " + e.Message);
         }
      }
      private void ParseJsonSpecificData()
      {
         string directory = Directory;
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
                     recipe.mRelatedJsonFiles.Add(recipes);
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
      public string GetJsonFileString()
      {
         try
         {
            StringWriter stringWriter = new StringWriter();
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
            {
               jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
               jsonTextWriter.Indentation = 3;
               jsonTextWriter.IndentChar = ' ';

               JsonSerializer jsonSeralizer = new JsonSerializer();
               jsonSeralizer.Serialize(jsonTextWriter, mJson);
            }
            return stringWriter.ToString();
         }
         catch (Exception e)
         {
            Console.WriteLine("Could not convert " + mPath + " to string because of exception " + e.Message);
         }
         return "INVALID JSON";
      }

      private void ParseLinkedFiles(string jsonString)
      {
         string directory = Directory;
         Regex matcher = new Regex("file\\([\\S]+\\)");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string linkedFile = JsonHelper.GetFileFromFileJson(match.Value, directory);
            linkedFile = JsonHelper.NormalizeSystemPath(linkedFile);

            if (!System.IO.File.Exists(linkedFile) && ! System.IO.Directory.Exists(linkedFile))
            {
               MessageBox.Show("File " + Path + " links to non-existent file " + linkedFile);
               continue;
            }
               
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

      // Returns true if should show parent node
      public override bool UpdateTreeNode(TreeNode node, string filter)
      {
         node.SelectedImageIndex = (int)JsonType;
         node.ImageIndex = (int)JsonType;
         bool hasChildMatchingFilter = false;
         if (JsonType == JSONTYPE.JOB)
         {
            if (mOpenedJsonFiles.Count > 1)
            {
               FileData recipeJsonData = mOpenedJsonFiles[1];
               TreeNode recipes = new TreeNode(recipeJsonData.FileName);
               foreach (string recipePath in recipeJsonData.LinkedFilePaths)
               {
                  string recipeName = System.IO.Path.GetFileNameWithoutExtension(recipePath);
                  if (string.IsNullOrEmpty(filter) || recipeName.Contains(filter))
                  {
                     TreeNode recipeNode = new TreeNode(recipeName);
                     recipeNode.ImageIndex = (int)JSONTYPE.RECIPE;
                     recipeNode.SelectedImageIndex = (int)JSONTYPE.RECIPE;
                     recipes.Nodes.Add(recipeNode);
                     hasChildMatchingFilter = true;
                  }
               }
               if (!string.IsNullOrEmpty(filter) && recipes.Nodes.Count <= 0)
               {
                  return false;
               }
               node.Nodes.Add(recipes);
            }
         }

         ModuleFile owner = GetModuleFile();
         if (!hasChildMatchingFilter && !string.IsNullOrEmpty(filter) && owner != null && !owner.Name.Contains(filter))
         {
            return false;
         }
         
         return true;
      }

      public override bool Clone(string newPath, string oldName, string newFileName, HashSet<string> alreadyCloned, bool execute)
      {
         string oldNameToUse = oldName;
         string newNameToUse = newFileName;
         if (JsonType == JSONTYPE.RECIPE)
         {
            oldNameToUse = oldName.Replace("_recipe", "");
            newNameToUse = newFileName.Replace("_recipe", "");
            if (execute)
            {
               JsonFileData recipesList = mRelatedJsonFiles[mRelatedJsonFiles.Count - 1] as JsonFileData;
               JObject json = recipesList.mJson;
               JToken foundParent = null;
               foreach (JToken token in json["craftable_recipes"].Children())
               {
                  if (foundParent != null)
                  {
                     break;
                  }
                  foreach (JToken recipe in token.First["recipes"].Children())
                  {
                     if (recipe.Last.ToString().Contains(FileName))
                     {
                        foundParent = token.First["recipes"];
                        break;
                     }
                  }
               }
               if (foundParent != null)
               {
                  string recipeFileName = System.IO.Path.GetFileName(newPath);
                  (foundParent as JObject).Add(newNameToUse, JObject.Parse("{\"recipe\": \"file(" + recipeFileName + ")\"}"));
                  recipesList.TrySetFlatFileData(recipesList.GetJsonFileString());
                  recipesList.TrySaveFile();
               }
            }
         }
         return base.Clone(newPath, oldNameToUse, newNameToUse, alreadyCloned, execute);
      }

      public override bool ShouldCloneDependency(string dependencyName, string oldName)
      {
         if (JsonType == JSONTYPE.RECIPE)
         {
            JToken produces = mJson["produces"];
            if (produces != null)
            {
               foreach (JToken child in produces.Children())
               {
                  if (child["item"] != null && child["item"].ToString().Equals(dependencyName))
                  {
                     return true;
                  }
               }
            }
         }
         return base.ShouldCloneDependency(dependencyName, oldName);
      }

      public override string GetNameForCloning()
      {
         string fileName = FileName;
         switch(JsonType)
         {
            case JSONTYPE.RECIPE:
               fileName = fileName.Replace("_recipe", "");
               break;
            case JSONTYPE.JOB:
               fileName = fileName.Replace("_description", "");
               break;
         }
         return fileName;
      }

      public bool AddIconicVersion()
      {
         if (JsonType != JSONTYPE.ENTITY)
         {
            return false;
         }
         foreach (FileData openedJsonFile in OpenedFiles)
         {
            if (openedJsonFile.Path.EndsWith("_iconic.json"))
            {
               return false; // already have an iconic
            }
         }
         string originalFileName = FileName;
         string iconicFilePath = Directory + "/" + originalFileName + "_iconic.json";
         MessageBox.Show("Adding file " + iconicFilePath);
         try
         {
            string iconicJson = System.Text.Encoding.UTF8.GetString(StonehearthEditor.Properties.Resources.defaultIconic);
            if (iconicJson != null)
            {
               //JsonFileData iconicFileData = 
               iconicJson = iconicJson.Replace("default", originalFileName);
               using (StreamWriter wr = new StreamWriter(iconicFilePath, false, new UTF8Encoding(false)))
               {
                  wr.Write(iconicJson);
               }
               JsonFileData iconic = new JsonFileData(iconicFilePath);
               iconic.Load();

               //mOpenedJsonFiles.Add(iconic);
            }
         }
         catch (Exception ee)
         {
            MessageBox.Show("Unable to add iconic file because " + ee.Message);
            return false;
         }
         return true;
      }

      public void SetModuleFile(ModuleFile moduleFile)
      {
         mOwner = moduleFile;
      }

      public ModuleFile GetModuleFile()
      {
         return mOwner;
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
      public string Directory
      {
         get { return mDirectory; }
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
