using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StonehearthEditor.Effects
{
   public sealed class StringProperty : Property
   {
      private readonly string name;

      public StringProperty(string name)
      {
         this.name = name;
      }

      public override PropertyValue FromJson(JToken json)
      {
         if (json == null)
         {
            return new StringPropertyValue(null);
         }

         return new StringPropertyValue((string)json);
      }

      public override JToken ToJson(PropertyValue value)
      {
         return (JToken)((StringPropertyValue)value).Value;
      }

      public override string Name
      {
         get
         {
            return this.name;
         }
      }
   }

   public sealed class StringPropertyValue : PropertyValue
   {
      public string Value { get; set; }

      public StringPropertyValue(string value)
      {
         this.Value = value;
      }

      public override bool IsMissing
      {
         get { return this.Value == null; }
      }

      public override bool IsValid()
      {
         return true;
      }
   }
}
