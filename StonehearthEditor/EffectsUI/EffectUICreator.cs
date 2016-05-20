using StonehearthEditor.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
   public static class EffectUICreator
   {
      private static readonly IReadOnlyDictionary<Type, Type> propertyTypeToUIType = new Dictionary<Type, Type>
      {
         { typeof(StringProperty), typeof(StringUI) },
         { typeof(IntProperty), typeof(IntUI) },
         { typeof(ComplexProperty), typeof(ComplexUI) },
         { typeof(ParameterProperty), typeof(ParameterUI) },
         { typeof(OriginProperty), typeof(OriginUI) },
      };

      public static Control CreateUI(Property property, PropertyValue value)
      {
         var uiType = propertyTypeToUIType[property.GetType()];
         if (uiType == null)
         {
            Label x = new Label();
            x.Text = "TODO";
            return x;
         }
         return (Control)Activator.CreateInstance(uiType, property, value);
      }
   }
}
