using Newtonsoft.Json.Linq;
using System;

namespace StonehearthEditor
{
   public enum JSONTYPE
   {
      NONE = 0,
      ENTITY = 1,
      BUFF = 2,
      DEBUFF = 3,
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

      public JsonFileData(ModuleFile owner)
      {
         mOwner = owner;
      }

      public void Load(string json)
      {
         mJson = JObject.Parse(json);
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

         if (mJsonType == JSONTYPE.ENTITY || mJsonType == JSONTYPE.NONE)
         {
            
            JToken components = mJson["components"];
            if (components != null)
            {
               // Look for stonehearth:entity_forms
            }
         }
      }

      public JSONTYPE JsonType
      {
         get { return mJsonType; }
      }
   }
}
