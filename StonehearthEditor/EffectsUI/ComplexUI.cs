using StonehearthEditor.Effects;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
    public sealed class ComplexUI : TableLayoutPanel
   {
      private readonly ComplexProperty property;
      private readonly ComplexPropertyValue value;

      private readonly Label lblHeader;
      private readonly Button btnToggle;

      private readonly List<Control> addedControls = new List<Control>();

      private bool isRoot
      {
         get
         {
            return property.Name == null;
         }
      }

      public ComplexUI(ComplexProperty property, ComplexPropertyValue value)
      {
         this.property = property;
         this.value = value;

         this.DoubleBuffered = true;
         this.AutoSize = true;
         //this.ColumnCount = isRoot ? 2 : 3;
         this.RowCount = 1;
         if (!isRoot)
         {
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
         }
         this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
         this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));

         lblHeader = new Label();
         lblHeader.AutoSize = true;
         lblHeader.Text = property.Name;
         this.Controls.Add(lblHeader);
         this.SetColumnSpan(lblHeader, isRoot ? 1 : 2);

         if (this.property.Optional)
         {
            btnToggle = new Button();
            btnToggle.Width = 50;
            this.Controls.Add(btnToggle);
            this.SetRow(btnToggle, 0);
            this.SetColumn(btnToggle, isRoot ? 1 : 2);
            btnToggle.Click += BtnToggle_Click;
         }
         else
         {
            value.SetIsMissing(false);
         }

         SetMissing(value.IsMissing);
      }

      private void BtnToggle_Click(object sender, EventArgs e)
      {
         value.SetIsMissing(!value.IsMissing);
         SetMissing(value.IsMissing);
      }

      private void SetMissing(bool isMissing)
      {
         if (property.Optional)
         {
            btnToggle.Text = isMissing ? "Add" : "Del";
         }
         if (isMissing)
         {
            foreach (var control in addedControls)
            {
               this.Controls.Remove(control);
            }
            addedControls.Clear();
            this.RowCount = 1;
         }
         else
         {
            this.RowCount = 1 + this.value.Values.Count;
            for (int i = 0; i < this.property.Children.Count; i++)
            {
               var childProperty = this.property.Children[i];
               var childValue = this.value.Values[childProperty];
               Control control = EffectUICreator.CreateUI(childProperty, childValue);
               this.Controls.Add(control);
               addedControls.Add(control);
               this.SetColumn(control, isRoot ? 0 : 1);
               this.SetColumnSpan(control, 2);
               this.SetRow(control, i + 1);
            }
         }
      }
   }
}
