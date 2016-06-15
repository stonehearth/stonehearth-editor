using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonehearthEditor.Effects.ParameterKinds
{
   public sealed class Curve
   {
      public IList<TimePoint> Points { get; private set; }

      public Curve(IList<TimePoint> points)
      {
         this.Points = points;
      }

      public static Curve FromJson(JToken json)
      {
         JArray arr = (JArray)json;
         List<TimePoint> points = new List<TimePoint>();
         foreach (var element in arr)
         {
            JArray eleArr = (JArray)element;
            points.Add(new TimePoint
            {
               Time = (double)eleArr[0],
               Value = (double)eleArr[1],
            });
         }

         return new Curve(points);
      }

      public JToken ToJson()
      {
         JArray arr = new JArray();
         foreach (var point in this.Points)
         {
            JArray pointArr = new JArray();
            pointArr.Add(point.Time);
            pointArr.Add(point.Value);
            arr.Add(pointArr);
         }
         return arr;
      }

      /// <summary>
      /// Finds errors in the curve points. Maps index => error message; null
      /// index means it applies to the whole curve.
      /// </summary>
      /// <returns></returns>
      public Dictionary<int?, string> GetErrors()
      {
         Dictionary<int?, string> ret = new Dictionary<int?, string>();

         Action<int?, string> addError = (index, msg) =>
         {
            if (ret.ContainsKey(index))
            {
               ret[index] += " " + msg;
            }
            else
            {
               ret[index] = msg;
            }
         };

         if (Points.Count < 2)
         {
            addError(null, "Needs at least two points in curve.");
         }

         for (int i= 0; i < Points.Count; i++)
         {
            if (Points[i].Time == null)
            {
               addError(i, "Time is invalid");
            }
            if (Points[i].Value == null)
            {
               addError(i, "Value is invalid");
            }
         }

         if (Points.Count > 0)
         {
            if (Points[0].Time != 0.0)
            {
               addError(0, "First point must have time=0.0.");
            }

            /*if (Points[Points.Count - 1].Time != 1.0)
            {
               addError(Points.Count - 1, "Last point must have time=1.0.");
            }*/
         }

         for (int i = 0; i < Points.Count - 1; i++)
         {
            if (Points[i].Time >= Points[i + 1].Time)
            {
               addError(i + 1, "Time must be greater than last point's time.");
            }
         }

         return ret;
      }
   }
}
