using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace StonehearthEditor
{
    public partial class EntityBrowserView : UserControl, IReloadable
    {
        private int sortColumn = -1;
        private Dictionary<string, JsonFileData> netWorthJsonFiles;
        private Dictionary<string, JsonFileData> weaponsJsonFiles;
        private Dictionary<string, JsonFileData> defenseJsonFiles;
        private Dictionary<string, JsonFileData> killableEntitiesJsonFiles;
        private Dictionary<string, string> modNames;
        private Dictionary<string, string> netWorthImagePaths;
        private Dictionary<string, string> weaponsImagePaths;
        private Dictionary<string, string> defenseImagePaths;

        private string[] attributesOfInterest = new string[] { "max_health",
                                                             "speed",
                                                             "menace",
                                                             "courage",
                                                             "muscle",
                                                             "exp_reward" };

        public EntityBrowserView()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            new ModuleDataManager(MainForm.kModsDirectoryPath);
            ModuleDataManager.GetInstance().Load();

            netWorthListView.View = View.Details;
            netWorthListView.GridLines = true;
            netWorthListView.FullRowSelect = true;
            netWorthListView.ShowItemToolTips = true;

            weaponsListView.View = View.Details;
            weaponsListView.GridLines = true;
            weaponsListView.FullRowSelect = true;

            defenseItemsListView.View = View.Details;
            defenseItemsListView.GridLines = true;
            defenseItemsListView.FullRowSelect = true;

            killableEntitiesListView.View = View.Details;
            killableEntitiesListView.GridLines = true;
            killableEntitiesListView.FullRowSelect = true;

            InitializeNetWorthItemsView();
            InitializeWeaponsView();
            InitializeDefenseItemsListView();
            InitializeKillableEntitiesListView();
        }

        public void InitializeTab(int index)
        {
            switch (index)
            {
                case 0:
                    netWorthListView.Items.Clear();
                    InitializeNetWorthItemsView();
                    break;
                case 1:
                    weaponsListView.Items.Clear();
                    InitializeWeaponsView();
                    break;
                case 2:
                    defenseItemsListView.Items.Clear();
                    InitializeDefenseItemsListView();
                    break;
                case 3:
                    killableEntitiesListView.Items.Clear();
                    InitializeKillableEntitiesListView();
                    break;
                default:
                    throw new IndexOutOfRangeException("Selected tab index does not match InitializeTab indices!");
            }
        }

        public void Reload()
        {
            int index = this.entityBrowserTabControl.SelectedIndex;
            try
            {
                InitializeTab(index);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine("Could not reload tab with index "
                   + index.ToString()
                   + ". Exception: "
                   + e.Message);
            }
        }

        private void InitializeNetWorthItemsView()
        {
            netWorthImagePaths = new Dictionary<string, string>();
            object[] data = ModuleDataManager.GetInstance().FilterJsonByTerm(netWorthListView, "entity_data.stonehearth:net_worth");
            netWorthJsonFiles = (Dictionary<string, JsonFileData>)data[0];
            modNames = (Dictionary<string, string>)data[1];
            ImageList imageList = new ImageList();

            // populate listView
            int index = 0;
            foreach (KeyValuePair<string, JsonFileData> entry in netWorthJsonFiles)
            {
                JsonFileData jsonFileData = entry.Value;
                JObject json = jsonFileData.Json;
                ListViewItem item = new ListViewItem(entry.Key); // Item alias
                JToken token = json.SelectToken("entity_data.stonehearth:net_worth.value_in_gold");
                string goldValue = token == null ? "" : token.ToString();
                item.SubItems.Add(goldValue); // Net Worth
                string material = "";
                string category = "";

                foreach (JsonFileData file in jsonFileData.OpenedFiles)
                {
                    JObject fileJson = file.Json;
                    JToken categoryToken = fileJson.SelectToken("components.item.category");
                    JToken materialToken = fileJson.SelectToken("components.stonehearth:material.tags");
                    if (categoryToken != null)
                    {
                        if (category != "" && category != categoryToken.ToString())
                        {
                            item.BackColor = Color.Red;
                            item.ToolTipText = "WARNING: Category specified in more than one place and are not identical";
                            break;
                        }
                        category = categoryToken.ToString();
                    }
                    if (materialToken != null)
                    {
                        if (material != "" && material != materialToken.ToString())
                        {
                            item.BackColor = Color.Red;
                            item.ToolTipText = "WARNING: Material specified in more than one place and are not identical";
                            break;
                        }
                        material = materialToken.ToString();
                    }
                }

                item.SubItems.Add(category); // Category
                string modName = modNames[entry.Key];
                item.SubItems.Add(modName); // Mod Name
                item.SubItems.Add(material);

                this.addImages(jsonFileData, netWorthImagePaths, imageList, item, entry, ref index);

                netWorthListView.Items.Add(item);
            }
            netWorthListView.SmallImageList = imageList;
        }

        private void InitializeWeaponsView()
        {
            weaponsImagePaths = new Dictionary<string, string>();
            object[] data = ModuleDataManager.GetInstance().FilterJsonByTerm(netWorthListView, "entity_data.stonehearth:combat:weapon_data");
            weaponsJsonFiles = (Dictionary<string, JsonFileData>)data[0];
            Dictionary<string, string> modNames = (Dictionary<string, string>)data[1];
            ImageList imageList = new ImageList();

            // populate listView
            int index = 0;
            foreach (KeyValuePair<string, JsonFileData> entry in weaponsJsonFiles)
            {
                JsonFileData jsonFileData = entry.Value;
                JObject json = jsonFileData.Json;
                ListViewItem item = new ListViewItem(entry.Key);
                JToken token = json.SelectToken("entity_data.stonehearth:combat:weapon_data.base_damage");
                string baseDamage = token == null ? "" : token.ToString();
                item.SubItems.Add(baseDamage);
                JToken iLevelToken = json.SelectToken("components.stonehearth:equipment_piece.ilevel");
                string iLevel = iLevelToken == null ? "none" : iLevelToken.ToString(); //TODO: if none found, insert ilevel into json with value 0
                item.SubItems.Add(iLevel);
                JToken meleeAttackAnim = json.SelectToken("entity_data.stonehearth:combat:melee_attacks.[0].name");
                if (meleeAttackAnim == null || meleeAttackAnim.ToString().Contains("1h"))
                {
                    item.SubItems.Add("1h");
                }
                else
                {
                    item.SubItems.Add("2h");
                }
                JToken rolesToken = json.SelectToken("components.stonehearth:equipment_piece.roles");
                string roles = rolesToken == null ? "none" : rolesToken.ToString();
                item.SubItems.Add(roles);

                string modName = modNames[entry.Key];
                item.SubItems.Add(modName);

                addImages(jsonFileData, weaponsImagePaths, imageList, item, entry, ref index);

                weaponsListView.Items.Add(item);
            }
            weaponsListView.SmallImageList = imageList;
        }

        private void InitializeDefenseItemsListView()
        {
            defenseImagePaths = new Dictionary<string, string>();
            object[] data = ModuleDataManager.GetInstance().FilterJsonByTerm(defenseItemsListView, "entity_data.stonehearth:combat:armor_data");
            defenseJsonFiles = (Dictionary<string, JsonFileData>)data[0];
            Dictionary<string, string> modNames = (Dictionary<string, string>)data[1];
            ImageList imageList = new ImageList();

            // populate listView
            int index = 0;
            foreach (KeyValuePair<string, JsonFileData> entry in defenseJsonFiles)
            {
                JsonFileData jsonFileData = entry.Value;
                JObject json = jsonFileData.Json;
                ListViewItem item = new ListViewItem(entry.Key);
                JToken token = json.SelectToken("entity_data.stonehearth:combat:armor_data.base_damage_reduction");
                string damageReduction = token == null ? "" : token.ToString();
                item.SubItems.Add(damageReduction);
                JToken iLevelToken = json.SelectToken("components.stonehearth:equipment_piece.ilevel");
                string iLevel = iLevelToken == null ? "none" : iLevelToken.ToString();
                item.SubItems.Add(iLevel);
                string modName = modNames[entry.Key];
                item.SubItems.Add(modName);

                this.addImages(jsonFileData, defenseImagePaths, imageList, item, entry, ref index);

                defenseItemsListView.Items.Add(item);
            }
            defenseItemsListView.SmallImageList = imageList;
        }

        // Not really killable entities per se. More like stuff with attributes.
        private void InitializeKillableEntitiesListView()
        {
            InitializeKillableEntitiesColumns();

            object[] data = ModuleDataManager.GetInstance().FilterJsonByTerm(killableEntitiesListView, "components.stonehearth:attributes");
            killableEntitiesJsonFiles = (Dictionary<string, JsonFileData>)data[0];

            foreach (KeyValuePair<string, JsonFileData> entry in killableEntitiesJsonFiles)
            {
                JsonFileData jsonFileData = entry.Value;
                JObject json = jsonFileData.Json;
                ListViewItem item = new ListViewItem(entry.Key);
                List<ListViewItem.ListViewSubItem> subItems = new List<ListViewItem.ListViewSubItem>();
                populateWithDefaultValue<ListViewItem.ListViewSubItem>(
                    subItems,
                    new ListViewItem.ListViewSubItem(),
                    killableEntitiesListView.Columns.Count);
                JObject jAttributes = (JObject)json.SelectToken("components.stonehearth:attributes");

                foreach (JProperty attribute in jAttributes.Properties())
                {
                    string attributeName = attribute.Name;
                    if (killableEntitiesListView.Columns.IndexOfKey(attributeName) == -1)
                    {
                        continue;
                    }
                    JToken jValue = attribute.Value.SelectToken("value");
                    if (jValue != null)
                    {
                        int i = killableEntitiesListView.Columns.IndexOfKey(attributeName);
                        subItems.RemoveAt(i - 1);
                        subItems.Insert(i - 1, new ListViewItem.ListViewSubItem(item, jValue.ToString()));
                    }
                }
                item.SubItems.AddRange(subItems.ToArray());
                killableEntitiesListView.Items.Add(item);
            }
        }

        private void InitializeKillableEntitiesColumns()
        {
            foreach (string attribute in attributesOfInterest)
            {
                killableEntitiesListView.Columns.Add(attribute, attribute, -2);
            }
        }

        private static void populateWithDefaultValue<T>(List<T> array, T value, int num)
        {
            for (int i = 0; i < num; i++)
            {
                array.Add(value);
            }
        }

        private void addImages(
            JsonFileData jsonFileData,
            Dictionary<string, string> imgPaths,
            ImageList imgList,
            ListViewItem listItem,
            KeyValuePair<string, JsonFileData> jsonEntry,
            ref int index)
        {
            foreach (FileData openedFile in jsonFileData.OpenedFiles)
            {
                if (imgPaths.ContainsKey(jsonEntry.Key))
                {
                    continue;
                }
                foreach (KeyValuePair<string, FileData> linkedFile in openedFile.LinkedFileData)
                {
                    if (linkedFile.Value is ImageFileData)
                    {
                        if (System.IO.File.Exists(linkedFile.Value.Path))
                        {
                            imgPaths[jsonEntry.Key] = linkedFile.Value.Path;
                            imgList.Images.Add(ThumbnailCache.GetThumbnail(linkedFile.Value.Path));
                            imgList.ImageSize = new Size(32, 32);
                            listItem.ImageIndex = index;
                            index++;
                            break;
                        }
                    }
                }
            }
        }

        private void netWorthListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            onColumnClick(netWorthListView, e);
        }

        private void weaponsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            onColumnClick(weaponsListView, e);
        }

        private void defenseItemsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            onColumnClick(defenseItemsListView, e);
        }
        private void killableEntitiesListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            onColumnClick(killableEntitiesListView, e);
        }

        private void netWorthListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                this.updateOnItemSelection(netWorthJsonFiles, netWorthImagePaths, e.Item.Text);
            }
        }

        private void weaponsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                this.updateOnItemSelection(weaponsJsonFiles, weaponsImagePaths, e.Item.Text);
            }
        }

        private void killableEntitiesListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                this.updateOnItemSelection(killableEntitiesJsonFiles, null, e.Item.Text);
            }
        }

        private void defenseItemsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                this.updateOnItemSelection(defenseJsonFiles, defenseImagePaths, e.Item.Text);
            }
        }

        private void updateOnItemSelection(
            Dictionary<string,
            JsonFileData> categoryJsonFiles,
            Dictionary<string, string> imgPaths,
            string alias)
        {
            filePreviewTabs.TabPages.Clear();
            iconView.ImageLocation = "";
            JsonFileData json;
            categoryJsonFiles.TryGetValue(alias, out json);

            if (json != null)
            {
                foreach (FileData openedFile in json.OpenedFiles)
                {
                    TabPage newTabPage = new TabPage();
                    newTabPage.Text = openedFile.FileName;
                    if (openedFile.IsModified)
                    {
                        newTabPage.Text = newTabPage.Text + "*";
                    }
                    if (openedFile.HasErrors)
                    {
                        newTabPage.ImageIndex = 0;
                        newTabPage.ToolTipText = openedFile.Errors;
                    }
                    FilePreview filePreview = new FilePreview(this, openedFile);
                    filePreview.Dock = DockStyle.Fill;
                    newTabPage.Controls.Add(filePreview);
                    filePreviewTabs.TabPages.Add(newTabPage);
                    string path;
                    if (imgPaths != null && imgPaths.TryGetValue(alias, out path))
                    {
                        iconView.ImageLocation = path;
                    }
                }
            }
        }

        private void onColumnClick(ListView lv, ColumnClickEventArgs e)
        {
            lv.ListViewItemSorter = new ListViewItemComparer(e.Column);
            lv.Sort();


            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                lv.Sorting = SortOrder.Ascending;
            }
            else
            {
                if (lv.Sorting == SortOrder.Ascending)
                    lv.Sorting = SortOrder.Descending;
                else
                    lv.Sorting = SortOrder.Ascending;
            }

            lv.Sort();
            lv.ListViewItemSorter = new ListViewItemComparer(e.Column, lv.Sorting);
        }

        public static Dictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }

        private void netWorthListView_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void EntityBrowserView_Load(object sender, EventArgs e)
        {

        }

        private void filterListViewButton_Click(object sender, EventArgs e)
        {
            ListView listView = null;
            foreach (Control control in entityBrowserTabControl.SelectedTab.Controls)
            {
                if (control.Name.Contains("ListView"))
                {
                    listView = control as ListView;
                    break;
                }
            }
            if (listView != null)
            {
                InputDialog dialog = new InputDialog("Filter By", "(Separate search terms with a comma) Filter items containing text:", "", "Filter");
                InputDialog.IDialogCallback callback = new FilterItemsCallback(this, listView);
                dialog.SetCallback(callback);
                dialog.Show();
            }
        }

        private void reloadToolStripItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private class FilterItemsCallback : InputDialog.IDialogCallback
        {
            private ListView mView;
            private EntityBrowserView mOwner;
            public FilterItemsCallback(EntityBrowserView owner, ListView listView)
            {
                mView = listView;
                mOwner = owner;
            }
            public void onCancelled()
            {
                // Do nothing. user cancelled
            }

            public bool OnAccept(string inputMessage)
            {
                if (mView == null)
                {
                    return false;
                }
                mView.BeginUpdate();
                mOwner.Reload();

                // TODO: Only check materials column
                List<ListViewItem> items = new List<ListViewItem>(mView.Items.Cast<ListViewItem>());
                string[] filterTerms = inputMessage.Replace(" ", "").Split(',');
                foreach (ListViewItem item in items)
                {
                    if (!ContainsAllStrings(item.Text, filterTerms))
                    {
                        bool containsFilterTerm = false;
                        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                        {
                            if (ContainsAllStrings(subItem.Text, filterTerms))
                            {
                                containsFilterTerm = true;
                            }
                        }
                        if (!containsFilterTerm)
                        {
                            mView.Items.Remove(item);
                        }
                    }
                }
                mView.EndUpdate();
                return true;
            }

            private bool ContainsAllStrings(string input, string[] strings)
            {
                bool result = true;
                foreach (string s in strings)
                {
                    result = result && input.Contains(s);
                }
                return result;
            }
        }
    }
}
