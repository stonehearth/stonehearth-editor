using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [ParameterKind("RANDOM_BETWEEN_CURVES", Dimension.Scalar, true)]
   public sealed class RandomBetweenCurvesScalarParameterKind : ParameterKind
   {
      public Curve Curve1 { get; private set; }
      public Curve Curve2 { get; private set; }

      public static RandomBetweenCurvesScalarParameterKind FromJson(JToken json)
      {
         JArray arr = (JArray)json;
         return new RandomBetweenCurvesScalarParameterKind(Curve.FromJson(arr[0]), Curve.FromJson(arr[1]));
      }

      public RandomBetweenCurvesScalarParameterKind(Curve curve1, Curve curve2)
      {
         this.Curve1 = curve1;
         this.Curve2 = curve2;
      }

      public RandomBetweenCurvesScalarParameterKind()
      {
         this.Curve1 = new Curve(new List<TimePoint>());
         this.Curve2 = new Curve(new List<TimePoint>());
      }

      public override JToken ToJson()
      {
         JArray arr = new JArray();
         arr.Add(Curve1.ToJson());
         arr.Add(Curve2.ToJson());
         return arr;
      }

      public override bool IsValid
      {
         get
         {
            return Curve1.GetErrors().Count == 0 && Curve2.GetErrors().Count == 0;
         }
      }
   }
}
