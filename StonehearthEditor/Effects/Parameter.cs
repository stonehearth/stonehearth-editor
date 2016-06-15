using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StonehearthEditor.Effects.ParameterKinds;

namespace StonehearthEditor.Effects
{
   public sealed class ParameterProperty : Property
   {
      private readonly string name;

      public ParameterProperty(
         string name,
         Dimension dimension = Dimension.Scalar,
         bool optional = true,
         bool timeVarying = false)
      {
         this.name = name;
         this.Dimension = dimension;
         this.Optional = optional;
         this.TimeVarying = timeVarying;
      }

      public Dimension Dimension { get; private set; }
      public bool Optional { get; private set; }
      public bool TimeVarying { get; private set; }

      public override string Name
      {
         get
         {
            return this.name;
         }
      }

      public override PropertyValue FromJson(JToken json)
      {
         if (json == null)
         {
            return new ParameterPropertyValue(true, null, null);
         }
         JObject jobj = (JObject)json;
         string kind = (string)jobj["kind"];
         ParameterKindOption option = ParameterKindRegistry.Get(kind, Dimension);
         ParameterKind parameter = option.FromJson(jobj["values"]);
         return new ParameterPropertyValue(false, option, parameter);
      }

      public override JToken ToJson(PropertyValue value)
      {
         ParameterPropertyValue val = (ParameterPropertyValue)value;
         JObject obj = new JObject();
         obj["kind"] = val.Option.Kind;
         obj["value"] = val.Parameter.ToJson();
         return obj;
      }
   }

   public sealed class ParameterPropertyValue : PropertyValue
   {
      public ParameterKindOption Option { get; set; }
      public ParameterKind Parameter { get; set; }

      private bool isMissing;

      public ParameterPropertyValue(bool isMissing, ParameterKindOption option, ParameterKind parameter)
      {
         this.isMissing = isMissing;
         this.Option = option;
         this.Parameter = parameter;
      }

      public override bool IsValid()
      {
         return Parameter == null || Parameter.IsValid;
      }

      public override bool IsMissing
      {
         get
         {
            return isMissing;
         }
      }

      public void SetIsMissing(bool isMissing)
      {
         this.isMissing = isMissing;
      }
   }
}
