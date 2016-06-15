using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects
{
   public sealed class OriginProperty : Property
   {
      private readonly string name;

      public OriginProperty(string name)
      {
         this.name = name;
      }

      public override PropertyValue FromJson(JToken json)
      {
         if (json == null)
         {
            return new OriginPropertyValue(true);
         }

         string surface = (string)json["surface"];
         JArray values = (JArray)json["values"];

         return new OriginPropertyValue(false, surface, (double)values[0], (double)values[1]);
      }

      public override JToken ToJson(PropertyValue value)
      {
         OriginPropertyValue val = (OriginPropertyValue)value;
         JObject obj = new JObject();
         obj.Add("surface", val.Surface);
         JArray values = new JArray();
         obj.Add("values", values);
         values.Add(val.Value1);
         values.Add(val.Value2);
         return obj;
      }

      public override string Name
      {
         get
         {
            return this.name;
         }
      }
   }

   public sealed class OriginPropertyValue : PropertyValue
   {
      private bool missing;

      public void SetIsMissing(bool value)
      {
         this.missing = value;
      }

      public string Surface { get; set; }

      public double? Value1 { get; set; }

      public double? Value2 { get; set; }

      public OriginPropertyValue(bool missing) : this(missing, "POINT", null, null) { }

      public OriginPropertyValue(bool missing, string surface, double? value1, double? value2)
      {
         this.missing = missing;
         this.Surface = surface;
         this.Value1 = value1;
         this.Value2 = value2;
      }

      public override bool IsMissing
      {
         get { return this.missing; }
      }

      public override bool IsValid()
      {
         return Value1 != null && Value2 != null;
      }
   }
}
