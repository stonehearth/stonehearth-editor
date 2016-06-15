using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StonehearthEditor.Effects.ParameterKinds;

namespace StonehearthEditor.EffectsUI.ParameterKinds
{
   public partial class RandomBetweenScalarParameterKindUI : UserControl
   {
      private readonly ParameterKindOption option;
      private readonly RandomBetweenScalarParameterKind value;

      public RandomBetweenScalarParameterKindUI()
      {
         InitializeComponent();
      }

      public RandomBetweenScalarParameterKindUI(ParameterKindOption option, RandomBetweenScalarParameterKind value)
         : this()
      {
         this.option = option;
         this.value = value;

         txtMin.Text = Util.DoubleToStringRep(value.MinValue);
         txtMax.Text = Util.DoubleToStringRep(value.MaxValue);
      }

      private string GetError(double? value)
      {
         if (value == null)
         {
            return "Invalid value";
         }

         return null;
      }

      private void minChanged(object sender, EventArgs e)
      {
         value.MinValue = Util.DoubleFromStringRep(txtMin.Text);
         wrnMin.Error = GetError(value.MinValue);
      }

      private void maxChanged(object sender, EventArgs e)
      {
         value.MaxValue = Util.DoubleFromStringRep(txtMax.Text);
         wrnMax.Error = GetError(value.MaxValue);

         if (string.IsNullOrEmpty(wrnMax.Error) && value.MinValue >= value.MaxValue)
         {
            wrnMax.Error = "Must be greater than min";
         }
      }
   }
}
