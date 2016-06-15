using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace StonehearthEditor.Effects
{
   public sealed class IntProperty : Property
   {
      private readonly string name;

      public IntProperty(string name)
      {
         this.name = name;
      }

      public override PropertyValue FromJson(JToken json)
      {
         if (json == null)
         {
            return new IntPropertyValue(null);
         }

         return new IntPropertyValue((int)json);
      }

      public override JToken ToJson(PropertyValue value)
      {
         return (JToken)((IntPropertyValue)value).Value;
      }

      public override string Name
      {
         get
         {
            return this.name;
         }
      }
   }

   public sealed class IntPropertyValue : PropertyValue
   {
      public int? Value { get; set; }

      public IntPropertyValue(int? value)
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
