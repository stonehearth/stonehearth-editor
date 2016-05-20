using StonehearthEditor.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
   public abstract class TextboxUI : TableLayoutPanel
   {
      private Label lblLabel;
      private TextBox txtValue;

      protected void Initialize(Property property)
      {
         this.AutoSize = true;
         this.RowCount = 1;
         this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
         this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));

         lblLabel = new Label();
         lblLabel.AutoSize = true;
         lblLabel.Text = property.Name;
         this.Controls.Add(lblLabel);

         txtValue = new TextBox();
         txtValue.Left = lblLabel.Left + 3;
         txtValue.Width = this.TextWidth;
         txtValue.Text = GetStringValue();
         txtValue.TextChanged += TxtValue_TextChanged;
         this.Controls.Add(txtValue);
         this.SetRow(txtValue, 0);
         this.SetColumn(txtValue, 1);
      }

      private void TxtValue_TextChanged(object sender, EventArgs e)
      {
         SetValueFromString(txtValue.Text);
      }

      protected abstract string GetStringValue();

      protected abstract bool SetValueFromString(string stringValue);

      protected abstract int TextWidth { get; }
   }

   public sealed class StringUI : TextboxUI
   {
      private readonly StringProperty property;
      private readonly StringPropertyValue value;

      public StringUI(StringProperty property, StringPropertyValue value)
      {
         this.property = property;
         this.value = value;
         this.Initialize(property);
      }

      protected override string GetStringValue()
      {
         return this.value.Value;
      }

      protected override bool SetValueFromString(string stringValue)
      {
         this.value.Value = stringValue;
         return true;
      }

      protected override int TextWidth
      {
         get
         {
            return 150;
         }
      }
   }

   public sealed class IntUI : TextboxUI
   {
      private readonly IntProperty property;
      private readonly IntPropertyValue value;

      public IntUI(IntProperty property, IntPropertyValue value)
      {
         this.property = property;
         this.value = value;
         this.Initialize(property);
      }

      protected override string GetStringValue()
      {
         return this.value.Value.ToString();
      }

      protected override bool SetValueFromString(string stringValue)
      {
         this.value.Value = int.Parse(stringValue);
         return true;
      }

      protected override int TextWidth
      {
         get
         {
            return 50;
         }
      }
   }
}
