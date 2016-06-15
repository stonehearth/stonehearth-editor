using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   [ParameterKind("RANDOM_BETWEEN", Dimension.Scalar, false)]
   public sealed class RandomBetweenScalarParameterKind : ParameterKind
   {
      public double? MinValue { get; set; }
      public double? MaxValue { get; set; }

      public static RandomBetweenScalarParameterKind FromJson(JToken json)
      {
         JArray arr = (JArray)json;
         return new RandomBetweenScalarParameterKind((double)arr[0], (double)arr[1]);
      }

      public RandomBetweenScalarParameterKind(double? minValue, double? maxValue)
      {
         this.MinValue = minValue;
         this.MaxValue = maxValue;
      }

      public RandomBetweenScalarParameterKind() { }

      public override JToken ToJson()
      {
         JArray arr = new JArray();
         arr.Add(MinValue);
         arr.Add(MaxValue);
         return arr;
      }

      public override bool IsValid
      {
         get
         {
            return MinValue != null && MaxValue != null;
         }
      }
   }
}
