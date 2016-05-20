using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StonehearthEditor.Effects.ParameterKinds;

namespace StonehearthEditor.EffectsUI.ParameterKinds
{
   public static class ParameterKindUICreator
   {
      private static readonly IReadOnlyDictionary<Type, Type> propertyTypeToUIType = new Dictionary<Type, Type>
      {
         { typeof(ConstantRgbaParameterKind), null },
         { typeof(ConstantScalarParameterKind), null },
         { typeof(CurveScalarParameterKind), null },
         { typeof(RandomBetweenCurvesScalarParameterKind), null },
         { typeof(RandomBetweenScalarParameterKind), typeof(RandomBetweenScalarParameterKindUI) },
      };

      public static Control CreateUI(ParameterKindOption option, ParameterKind value)
      {
         if (value == null)
         {
            return new Control();
         }

         var uiType = propertyTypeToUIType[value.GetType()];
         if (uiType == null)
         {
            Label x = new Label();
            x.Text = "TODO";
            return x;
         }
         return (Control)Activator.CreateInstance(uiType, option, value);
      }
   }
}
