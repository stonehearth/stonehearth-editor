using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
   public sealed class WarningIcon : Control
   {
      private string error;
      private ToolTip tooltip;

      public string Error
      {
         get { return error; }

         set
         {
            if (value == this.error)
            {
               return;
            }
            this.error = value;
            if (string.IsNullOrEmpty(value))
            {
               tooltip.SetToolTip(this, null);
            }
            else
            {
               tooltip.SetToolTip(this, value);
            }
            this.Invalidate();
         }
      }

      public WarningIcon()
      {
         tooltip = new ToolTip();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         if (!string.IsNullOrEmpty(error))
         {
            e.Graphics.DrawIcon(SystemIcons.Error, new Rectangle(0, 0, this.Width, this.Height));
         }
         base.OnPaint(e);
      }
   }
}
