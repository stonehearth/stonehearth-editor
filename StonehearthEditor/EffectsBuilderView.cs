using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StonehearthEditor.subViews;
using Newtonsoft.Json.Linq;
using StonehearthEditor.Effects;
using StonehearthEditor.EffectsUI;
using Newtonsoft.Json;

namespace StonehearthEditor
{
   public partial class EffectsBuilderView : UserControl
   {
      private Control editorUI;
      private Property property;
      private PropertyValue propertyValue;

      public event EventHandler SaveRequested;
      public event EventHandler PreviewRequested;

      public EffectsBuilderView()
      {
         InitializeComponent();
      }

      public void ReloadEditor(JToken json, Property property)
      {
         if (editorUI != null)
         {
            pnlEditor.Controls.Remove(editorUI);
            editorUI.Dispose();
            editorUI = null;
         }

         this.property = property;
         this.propertyValue = property.FromJson(json);
         editorUI = EffectUICreator.CreateUI(property, propertyValue);
         pnlEditor.Controls.Add(editorUI);
      }

      private void btnSave_Click(object sender, EventArgs e)
      {
         if (!propertyValue.IsValid())
         {
            MessageBox.Show("Fix errors first");
            return;
         }
         EventHandler temp = SaveRequested;
         if (temp != null)
         {
            temp(this, EventArgs.Empty);
         }
      }

      private void btnPreview_Click(object sender, EventArgs e)
      {
         if (!propertyValue.IsValid())
         {
            MessageBox.Show("Fix errors first");
            return;
         }
         EventHandler temp = PreviewRequested;
         if (temp != null)
         {
            temp(this, EventArgs.Empty);
         }
      }

      public string GetJsonString()
      {
         var root = GetJson();
         JsonSerializerSettings settings = new JsonSerializerSettings();
         settings.Formatting = Formatting.Indented;
         return JsonConvert.SerializeObject(root, settings);
      }

      public JToken GetJson()
      {
         return this.property.ToJson(this.propertyValue);
      }
   }
}
