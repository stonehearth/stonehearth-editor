using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [ParameterKind("CONSTANT", Dimension.Rgba, false)]
   public sealed class ConstantRgbaParameterKind : ParameterKind
   {
      public double? R { get; set; }
      public double? G { get; set; }
      public double? B { get; set; }
      public double? A { get; set; }

      public static ConstantRgbaParameterKind FromJson(JToken json)
      {
         JArray arr = (JArray)json;
         return new ConstantRgbaParameterKind((double)arr[0], (double)arr[1], (double)arr[2], (double)arr[3]);
      }

      public ConstantRgbaParameterKind(double? r, double? g, double? b, double? a)
      {
         this.R = r;
         this.G = g;
         this.B = b;
         this.A = a;
      }

      public ConstantRgbaParameterKind() { }

      public override JToken ToJson()
      {
         JArray arr = new JArray();
         arr.Add(this.R);
         arr.Add(this.G);
         arr.Add(this.B);
         arr.Add(this.A);
         return arr;
      }

      public override bool IsValid
      {
         get
         {
            return R != null && G != null && B != null && A != null;
         }
      }
   }
}
