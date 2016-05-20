using StonehearthEditor.Effects;
using StonehearthEditor.Effects.ParameterKinds;
using StonehearthEditor.EffectsUI.ParameterKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonehearthEditor.EffectsUI
{
   public sealed class ParameterUI : TableLayoutPanel
   {
      private readonly ParameterProperty property;
      private readonly ParameterPropertyValue value;

      private readonly Label lblHeader;
      private readonly Button btnToggle;
      private ComboBox cmbKind;
      private Control kindEditor;

      private readonly List<ParameterKindOption> options;

      public ParameterUI(ParameterProperty property, ParameterPropertyValue value)
      {
         this.property = property;
         this.value = value;

         this.DoubleBuffered = true;
         this.AutoSize = true;
         this.RowCount = 1;
         this.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30));
         this.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
         this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));

         options = ParameterKindRegistry.GetOptions(property.Dimension, property.TimeVarying);

         lblHeader = new Label();
         lblHeader.AutoSize = true;
         lblHeader.Text = property.Name;
         this.Controls.Add(lblHeader);
         this.SetColumnSpan(lblHeader, 2);

         if (this.property.Optional)
         {
            btnToggle = new Button();
            btnToggle.Width = 50;
            this.Controls.Add(btnToggle);
            this.SetRow(btnToggle, 0);
            this.SetColumn(btnToggle, 2);
            btnToggle.Click += BtnToggle_Click;
         }
         else
         {
            value.SetIsMissing(false);
         }

         SetMissing(value.IsMissing);
      }

      private void CmbKind_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (value.Option == options[cmbKind.SelectedIndex])
         {
            return;
         }

         value.Option = options[cmbKind.SelectedIndex];
         value.Parameter = value.Option.Create();

         ResetKindEditor();
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
            if (cmbKind != null)
            {
               this.Controls.Remove(cmbKind);
               cmbKind = null;
            }
            if (kindEditor != null)
            {
               this.Controls.Remove(kindEditor);
               kindEditor = null;
            }
            this.RowCount = 1;
         }
         else
         {
            this.RowCount = 3;

            cmbKind = new ComboBox();
            cmbKind.DropDownStyle = ComboBoxStyle.DropDownList;
            this.SetColumnSpan(cmbKind, 2);
            this.SetColumn(cmbKind, 1);
            this.SetRow(cmbKind, 1);
            foreach (var option in this.options)
            {
               cmbKind.Items.Add(option.Kind);
            }
            cmbKind.SelectedIndexChanged += CmbKind_SelectedIndexChanged;
            cmbKind.SelectedIndex = options.IndexOf(this.value.Option);
            this.Controls.Add(cmbKind);
            ResetKindEditor();
         }
      }

      private void ResetKindEditor()
      {
         if (kindEditor != null)
         {
            this.Controls.Remove(kindEditor);
         }

         kindEditor = ParameterKindUICreator.CreateUI(value.Option, value.Parameter);
         this.SetRow(kindEditor, 2);
         this.SetColumn(kindEditor, 1);
         this.SetColumnSpan(kindEditor, 2);
         this.Controls.Add(kindEditor);
      }
   }
}
