using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
   public static class Util
   {
      public static string DoubleToStringRep(double? value)
      {
         if (value == null)
         {
            return string.Empty;
         }

         return value.Value.ToString("F4", CultureInfo.InvariantCulture);
      }

      public static double? DoubleFromStringRep(string value)
      {
         double result;
         if (!double.TryParse(value, out result))
         {
            return null;
         }

         return result;
      }
   }
}
