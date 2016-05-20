using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor.Effects
{
   public sealed class ComplexProperty : Property
   {
      private readonly string name;
      private readonly List<Property> children;
      public IReadOnlyList<Property> Children { get; private set; }

      public bool Optional { get; private set; }

      public ComplexProperty(string name, bool optional, params Property[] children)
      {
         this.name = name;
         this.Optional = optional;
         this.children = children.ToList();
         this.Children = children;
      }

      public override string Name
      {
         get
         {
            return this.name;
         }
      }

      public override PropertyValue FromJson(JToken json)
      {
         Dictionary<Property, PropertyValue> values = new Dictionary<Property, PropertyValue>();

         if (json == null)
         {
            foreach (var property in this.children)
            {
               values[property] = property.FromJson(null);
            }

            return new ComplexPropertyValue(true, values, new List<JProperty>());
         }

         JObject jsonObject = (JObject)json;
         var seenKeys = new HashSet<string>();

         foreach (var property in this.children)
         {
            var propertyJson = json[property.Name];
            values[property] = property.FromJson(propertyJson);
            seenKeys.Add(property.Name);
         }

         List<JProperty> extra = jsonObject
            .Properties()
            .Where(p => !seenKeys.Contains(p.Name))
            .ToList();

         return new ComplexPropertyValue(false, values, extra);
      }

      public override JToken ToJson(PropertyValue value)
      {
         JObject parent = new JObject();

         ComplexPropertyValue val = (ComplexPropertyValue)value;
         foreach (var property in this.children)
         {
            var propertyValue = val.Values[property];
            if (!propertyValue.IsMissing)
            {
               parent.Add(property.Name, property.ToJson(propertyValue));
            }
         }

         foreach (var extra in val.Extra)
         {
            parent.Add(extra.Name, extra.Value);
         }

         return parent;
      }
   }

   public sealed class ComplexPropertyValue : PropertyValue
   {
      private readonly Dictionary<Property, PropertyValue> values;

      public List<JProperty> Extra { get; private set; }

      private bool isMissing;

      public IReadOnlyDictionary<Property, PropertyValue> Values { get; private set; }

      public ComplexPropertyValue(bool isMissing, Dictionary<Property, PropertyValue> values, List<JProperty> extra)
      {
         this.isMissing = isMissing;
         this.values = new Dictionary<Property, PropertyValue>(values);
         this.Values = values;
         this.Extra = extra;
      }

      public override bool IsMissing
      {
         get
         {
            return isMissing;
         }
      }

      public void SetIsMissing(bool value)
      {
         this.isMissing = value;
      }

      public override bool IsValid()
      {
         foreach (var prop in this.values)
         {
            if (prop.Value.IsMissing)
            {
               continue;
            }

            if (!prop.Value.IsValid())
            {
               return false;
            }
         }

         return true;
      }
   }
}
