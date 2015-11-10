using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
   };

   public class JsonFileData
   {
      private JSONTYPE mJsonType = JSONTYPE.NONE;
      private JObject mJson;
      private ModuleFile mOwner;
      private List<ModuleFile> mLinkedAliases = new List<ModuleFile>();
      private List<string> mLinkedFiles = new List<string>();

      public JsonFileData(ModuleFile owner)
      {
         mOwner = owner;
      }

      public void Load(string jsonString)
      {
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

         if (mJsonType == JSONTYPE.ENTITY)
         {

            JToken entityFormsComponent = mJson.SelectToken("components.stonehearth:entity_forms");
            if (entityFormsComponent != null)
            {
               // Look for stonehearth:entity_forms
               
            }
         }
      }
      private void ParseLinkedFiles(string jsonString)
      {
         Regex matcher = new Regex("file\\([\\S]+\\)");
         foreach (Match match in matcher.Matches(jsonString))
         {
            string linkedFile = JsonHelper.GetFileFromFileJson(match.Value, mOwner.ResolvedPath);
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

      public JSONTYPE JsonType
      {
         get { return mJsonType; }
      }
      public List<ModuleFile> LinkedAliases
      {
         get { return mLinkedAliases; }
      }
      public List<string> LinkedFiles
      {
         get { return mLinkedFiles; }
      }
   }
}
