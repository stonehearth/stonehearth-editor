using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StonehearthEditor.Effects;
using StonehearthEditor.EffectsUI;

namespace StonehearthEditor.Effects
{
    public class EffectsEventHandler
    {
        private Control editorUI;
        private Property property;
        private PropertyValue propertyValue;

        public event EventHandler SaveRequested;

        public event EventHandler PreviewRequested;

        public void ReloadEditor(JToken json, Property property)
        {
            this.property = property;
            this.propertyValue = property.FromJson(json);
            editorUI = EffectUICreator.CreateUI(property, propertyValue);
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
