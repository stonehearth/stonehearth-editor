using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StonehearthEditor.Effects;

namespace StonehearthEditor.EffectsUI
{
   public partial class OriginUI : UserControl
   {
      private readonly OriginProperty property;
      private readonly OriginPropertyValue value;
      private readonly int expandedHeight;

      public OriginUI()
      {
         InitializeComponent();
         this.expandedHeight = this.Height;
      }

      public OriginUI(OriginProperty property, OriginPropertyValue value) : this()
      {
         this.property = property;
         this.value = value;

         SetMissing(value.IsMissing);

         txtValue1.Text = Util.DoubleToStringRep(value.Value1);
         txtValue2.Text = Util.DoubleToStringRep(value.Value2);
         cmbSurface.SelectedItem = value.Surface;
      }

      private void btnToggle_Click(object sender, EventArgs e)
      {
         value.SetIsMissing(!value.IsMissing);
         SetMissing(value.IsMissing);
      }

      private void SetMissing(bool isMissing)
      {
         btnToggle.Text = isMissing ? "Add" : "Del";
         if (isMissing)
         {
            this.Height = this.btnToggle.Bottom;
         }
         else
         {
            this.Height = this.expandedHeight;
         }
      }

      private string GetError(double? value)
      {
         if (value == null)
         {
            return "Invalid value";
         }

         return null;
      }

      private void SurfaceChanged(object sender, EventArgs e)
      {
         value.Surface = (string)cmbSurface.SelectedItem;
      }

      private void Value1Changed(object sender, EventArgs e)
      {
         value.Value1 = Util.DoubleFromStringRep(txtValue1.Text);
         wrnValue1.Error = GetError(value.Value1);
      }

      private void Value2Changed(object sender, EventArgs e)
      {
         value.Value2 = Util.DoubleFromStringRep(txtValue2.Text);
         wrnValue2.Error = GetError(value.Value2);
      }
   }
}
