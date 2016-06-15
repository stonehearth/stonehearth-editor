using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [ParameterKind("CONSTANT", Dimension.Scalar, false)]
   public sealed class ConstantScalarParameterKind : ParameterKind
   {
      public double? Value { get; set; }

      public static ConstantScalarParameterKind FromJson(JToken json)
      {
         JArray arr = (JArray)json;
         return new ConstantScalarParameterKind((double)arr[0]);
      }

      public ConstantScalarParameterKind(double? value)
      {
         this.Value = value;
      }

      public ConstantScalarParameterKind() { }

      public override JToken ToJson()
      {
         JArray arr = new JArray();
         arr.Add(this.Value);
         return arr;
      }

      public override bool IsValid
      {
         get
         {
            return Value != null;
         }
      }
   }
}
