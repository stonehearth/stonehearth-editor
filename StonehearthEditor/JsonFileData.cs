using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.IO;

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
      private XDocument mFile;
      private ModuleFile mOwner;
      private XDocument mIconic;
      private XDocument mGhost;

      public JsonFileData(ModuleFile owner)
      {
         mOwner = owner;
      }

      public void Load(StreamReader sr)
      {
         XmlDictionaryReaderQuotas quota = new XmlDictionaryReaderQuotas();
         quota.MaxNameTableCharCount = 500000;
         XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(sr.BaseStream, quota);
         mFile = XDocument.Load(reader);
         XElement type = mFile.Root.Element("type");
         if (type != null && type.Value != null)
         {
            string typeString = type.Value.Trim().ToUpper();
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
            XElement components = mFile.Root.Element("components");
            if (components != null)
            {
               // Look for stonehearth:entity_forms
               foreach (XElement component in components.Elements())
               {
                  string name = component.Name.ToString();
                  string value = component.Value;
                  if (component.Attribute("item") != null)
                  {
                     name = component.Attribute("item").Value;
                  }

                  if (name.Equals("stonehearth:entity_forms"))
                  {
                     mJsonType = JSONTYPE.ENTITY;
                     // Grab the iconic and the ghost forms
                     XElement iconic = component.Element("iconic_form");
                     if (iconic != null)
                     {
                        string iconicFilePath = iconic.Value;

                     }
                  }
               }
            }
         }
      }

      public JSONTYPE JsonType
      {
         get { return mJsonType; }
      }
   }
}
