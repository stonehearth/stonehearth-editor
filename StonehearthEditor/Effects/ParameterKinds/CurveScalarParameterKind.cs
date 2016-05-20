using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [ParameterKind("CURVE", Dimension.Scalar, true)]
   public sealed class CurveScalarParameterKind : ParameterKind
   {
      public Curve Curve { get; private set; }

      public static CurveScalarParameterKind FromJson(JToken json)
      {
         return new CurveScalarParameterKind(Curve.FromJson(json));
      }

      public CurveScalarParameterKind(Curve curve)
      {
         this.Curve = curve;
      }

      public CurveScalarParameterKind()
      {
         this.Curve = new Curve(new List<TimePoint>());
      }

      public override JToken ToJson()
      {
         return Curve.ToJson();
      }

      public override bool IsValid
      {
         get
         {
            return Curve.GetErrors().Count == 0;
         }
      }
   }
}
