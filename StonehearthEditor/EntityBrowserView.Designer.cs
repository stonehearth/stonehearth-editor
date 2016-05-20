namespace StonehearthEditor
{
    partial class EntityBrowserView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityBrowserView));
            this.netWorthListView = new System.Windows.Forms.ListView();
            this.item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.materialTags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filePreviewTabs = new System.Windows.Forms.TabControl();
            this.iconView = new System.Windows.Forms.PictureBox();
            this.entityBrowserTabControl = new System.Windows.Forms.TabControl();
            this.netWorthItemsTab = new System.Windows.Forms.TabPage();
            this.weaponsTab = new System.Windows.Forms.TabPage();
            this.weaponsListView = new System.Windows.Forms.ListView();
            this.weaponAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.baseDamage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.weaponILevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.handedness = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.roles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.defenseItemsTab = new System.Windows.Forms.TabPage();
            this.defenseItemsListView = new System.Windows.Forms.ListView();
            this.defenseItemAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.damageReduction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.defenseItemILevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.defenseItemModName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.killableEntitiesTab = new System.Windows.Forms.TabPage();
            this.killableEntitiesListView = new System.Windows.Forms.ListView();
            this.killableEntity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.entityBrowserSplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.entityBrowserToolStrip = new System.Windows.Forms.ToolStrip();
            this.filterListViewButton = new System.Windows.Forms.ToolStripButton();
            this.reloadToolStripItem = new System.Windows.Forms.ToolStripButton();
            this.entityBrowserSplitContainer2 = new System.Windows.Forms.SplitContainer();
            this.entityBrowserSplitContainer3 = new System.Windows.Forms.SplitContainer();
            this.fileDetailsListBox = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconView)).BeginInit();
            this.entityBrowserTabControl.SuspendLayout();
            this.netWorthItemsTab.SuspendLayout();
            this.weaponsTab.SuspendLayout();
            this.defenseItemsTab.SuspendLayout();
            this.killableEntitiesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer1)).BeginInit();
            this.entityBrowserSplitContainer1.Panel1.SuspendLayout();
            this.entityBrowserSplitContainer1.Panel2.SuspendLayout();
            this.entityBrowserSplitContainer1.SuspendLayout();
            this.entityBrowserToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer2)).BeginInit();
            this.entityBrowserSplitContainer2.Panel1.SuspendLayout();
            this.entityBrowserSplitContainer2.Panel2.SuspendLayout();
            this.entityBrowserSplitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer3)).BeginInit();
            this.entityBrowserSplitContainer3.Panel1.SuspendLayout();
            this.entityBrowserSplitContainer3.Panel2.SuspendLayout();
            this.entityBrowserSplitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // netWorthListView
            // 
            this.netWorthListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.netWorthListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.item,
            this.value,
            this.category,
            this.modName,
            this.materialTags});
            this.netWorthListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.netWorthListView.FullRowSelect = true;
            this.netWorthListView.GridLines = true;
            this.netWorthListView.LabelEdit = true;
            this.netWorthListView.Location = new System.Drawing.Point(3, 3);
            this.netWorthListView.Name = "netWorthListView";
            this.netWorthListView.Size = new System.Drawing.Size(496, 561);
            this.netWorthListView.TabIndex = 1;
            this.netWorthListView.UseCompatibleStateImageBehavior = false;
            this.netWorthListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.netWorthListView_ColumnClick);
            this.netWorthListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.netWorthListView_ItemSelectionChanged);
            this.netWorthListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.netWorthListView_KeyDown);
            // 
            // item
            // 
            this.item.Text = "Item Alias";
            this.item.Width = 200;
            // 
            // value
            // 
            this.value.Text = "Gold Value";
            this.value.Width = -2;
            // 
            // category
            // 
            this.category.Text = "Category";
            this.category.Width = -2;
            // 
            // modName
            // 
            this.modName.Text = "Mod Name";
            this.modName.Width = -2;
            // 
            // materialTags
            // 
            this.materialTags.Text = "materials";
            this.materialTags.Width = -2;
            // 
            // filePreviewTabs
            // 
            this.filePreviewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filePreviewTabs.Location = new System.Drawing.Point(0, 0);
            this.filePreviewTabs.Name = "filePreviewTabs";
            this.filePreviewTabs.SelectedIndex = 0;
            this.filePreviewTabs.Size = new System.Drawing.Size(380, 618);
            this.filePreviewTabs.TabIndex = 2;
            // 
            // iconView
            // 
            this.iconView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconView.Location = new System.Drawing.Point(0, 0);
            this.iconView.Name = "iconView";
            this.iconView.Size = new System.Drawing.Size(207, 314);
            this.iconView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.iconView.TabIndex = 3;
            this.iconView.TabStop = false;
            // 
            // entityBrowserTabControl
            // 
            this.entityBrowserTabControl.Controls.Add(this.netWorthItemsTab);
            this.entityBrowserTabControl.Controls.Add(this.weaponsTab);
            this.entityBrowserTabControl.Controls.Add(this.defenseItemsTab);
            this.entityBrowserTabControl.Controls.Add(this.killableEntitiesTab);
            this.entityBrowserTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityBrowserTabControl.Location = new System.Drawing.Point(0, 25);
            this.entityBrowserTabControl.Name = "entityBrowserTabControl";
            this.entityBrowserTabControl.SelectedIndex = 0;
            this.entityBrowserTabControl.Size = new System.Drawing.Size(510, 593);
            this.entityBrowserTabControl.TabIndex = 4;
            // 
            // netWorthItemsTab
            // 
            this.netWorthItemsTab.Controls.Add(this.netWorthListView);
            this.netWorthItemsTab.Location = new System.Drawing.Point(4, 22);
            this.netWorthItemsTab.Name = "netWorthItemsTab";
            this.netWorthItemsTab.Padding = new System.Windows.Forms.Padding(3);
            this.netWorthItemsTab.Size = new System.Drawing.Size(502, 567);
            this.netWorthItemsTab.TabIndex = 0;
            this.netWorthItemsTab.Text = "Net Worth Items";
            this.netWorthItemsTab.UseVisualStyleBackColor = true;
            // 
            // weaponsTab
            // 
            this.weaponsTab.Controls.Add(this.weaponsListView);
            this.weaponsTab.Location = new System.Drawing.Point(4, 22);
            this.weaponsTab.Name = "weaponsTab";
            this.weaponsTab.Padding = new System.Windows.Forms.Padding(3);
            this.weaponsTab.Size = new System.Drawing.Size(502, 567);
            this.weaponsTab.TabIndex = 1;
            this.weaponsTab.Text = "Weapons";
            this.weaponsTab.UseVisualStyleBackColor = true;
            // 
            // weaponsListView
            // 
            this.weaponsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.weaponAlias,
            this.baseDamage,
            this.weaponILevel,
            this.handedness,
            this.roles,
            this.mod});
            this.weaponsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weaponsListView.LabelEdit = true;
            this.weaponsListView.Location = new System.Drawing.Point(3, 3);
            this.weaponsListView.Name = "weaponsListView";
            this.weaponsListView.Size = new System.Drawing.Size(496, 561);
            this.weaponsListView.TabIndex = 0;
            this.weaponsListView.UseCompatibleStateImageBehavior = false;
            this.weaponsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.weaponsListView_ColumnClick);
            this.weaponsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.weaponsListView_ItemSelectionChanged);
            // 
            // weaponAlias
            // 
            this.weaponAlias.Text = "Weapon Alias";
            this.weaponAlias.Width = -2;
            // 
            // baseDamage
            // 
            this.baseDamage.Text = "Base Damage";
            this.baseDamage.Width = -2;
            // 
            // weaponILevel
            // 
            this.weaponILevel.Text = "iLevel";
            // 
            // handedness
            // 
            this.handedness.Text = "Handedness";
            this.handedness.Width = -2;
            // 
            // roles
            // 
            this.roles.Text = "Roles";
            this.roles.Width = -2;
            // 
            // mod
            // 
            this.mod.Text = "Mod Name";
            this.mod.Width = -2;
            // 
            // defenseItemsTab
            // 
            this.defenseItemsTab.Controls.Add(this.defenseItemsListView);
            this.defenseItemsTab.Location = new System.Drawing.Point(4, 22);
            this.defenseItemsTab.Name = "defenseItemsTab";
            this.defenseItemsTab.Padding = new System.Windows.Forms.Padding(3);
            this.defenseItemsTab.Size = new System.Drawing.Size(502, 567);
            this.defenseItemsTab.TabIndex = 2;
            this.defenseItemsTab.Text = "Defensive Items";
            this.defenseItemsTab.UseVisualStyleBackColor = true;
            // 
            // defenseItemsListView
            // 
            this.defenseItemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.defenseItemAlias,
            this.damageReduction,
            this.defenseItemILevel,
            this.defenseItemModName});
            this.defenseItemsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defenseItemsListView.LabelEdit = true;
            this.defenseItemsListView.Location = new System.Drawing.Point(3, 3);
            this.defenseItemsListView.Name = "defenseItemsListView";
            this.defenseItemsListView.Size = new System.Drawing.Size(496, 561);
            this.defenseItemsListView.TabIndex = 1;
            this.defenseItemsListView.UseCompatibleStateImageBehavior = false;
            this.defenseItemsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.defenseItemsListView_ColumnClick);
            this.defenseItemsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.defenseItemsListView_ItemSelectionChanged);
            // 
            // defenseItemAlias
            // 
            this.defenseItemAlias.Text = "Item Alias";
            this.defenseItemAlias.Width = -2;
            // 
            // damageReduction
            // 
            this.damageReduction.Text = "Damage Reduction";
            this.damageReduction.Width = -2;
            // 
            // defenseItemILevel
            // 
            this.defenseItemILevel.Text = "iLevel";
            // 
            // defenseItemModName
            // 
            this.defenseItemModName.Text = "Mod Name";
            this.defenseItemModName.Width = -2;
            // 
            // killableEntitiesTab
            // 
            this.killableEntitiesTab.Controls.Add(this.killableEntitiesListView);
            this.killableEntitiesTab.Location = new System.Drawing.Point(4, 22);
            this.killableEntitiesTab.Name = "killableEntitiesTab";
            this.killableEntitiesTab.Padding = new System.Windows.Forms.Padding(3);
            this.killableEntitiesTab.Size = new System.Drawing.Size(502, 567);
            this.killableEntitiesTab.TabIndex = 3;
            this.killableEntitiesTab.Text = "Killable Entities";
            this.killableEntitiesTab.UseVisualStyleBackColor = true;
            // 
            // killableEntitiesListView
            // 
            this.killableEntitiesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.killableEntity});
            this.killableEntitiesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.killableEntitiesListView.LabelEdit = true;
            this.killableEntitiesListView.Location = new System.Drawing.Point(3, 3);
            this.killableEntitiesListView.Name = "killableEntitiesListView";
            this.killableEntitiesListView.Size = new System.Drawing.Size(496, 561);
            this.killableEntitiesListView.TabIndex = 2;
            this.killableEntitiesListView.UseCompatibleStateImageBehavior = false;
            this.killableEntitiesListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.killableEntitiesListView_ColumnClick);
            this.killableEntitiesListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.killableEntitiesListView_ItemSelectionChanged);
            // 
            // killableEntity
            // 
            this.killableEntity.Text = "Entity";
            this.killableEntity.Width = -2;
            // 
            // entityBrowserSplitContainer1
            // 
            this.entityBrowserSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityBrowserSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.entityBrowserSplitContainer1.Name = "entityBrowserSplitContainer1";
            // 
            // entityBrowserSplitContainer1.Panel1
            // 
            this.entityBrowserSplitContainer1.Panel1.Controls.Add(this.entityBrowserTabControl);
            this.entityBrowserSplitContainer1.Panel1.Controls.Add(this.entityBrowserToolStrip);
            // 
            // entityBrowserSplitContainer1.Panel2
            // 
            this.entityBrowserSplitContainer1.Panel2.Controls.Add(this.filePreviewTabs);
            this.entityBrowserSplitContainer1.Size = new System.Drawing.Size(894, 618);
            this.entityBrowserSplitContainer1.SplitterDistance = 510;
            this.entityBrowserSplitContainer1.TabIndex = 5;
            // 
            // entityBrowserToolStrip
            // 
            this.entityBrowserToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterListViewButton,
            this.reloadToolStripItem});
            this.entityBrowserToolStrip.Location = new System.Drawing.Point(0, 0);
            this.entityBrowserToolStrip.MaximumSize = new System.Drawing.Size(0, 50);
            this.entityBrowserToolStrip.Name = "entityBrowserToolStrip";
            this.entityBrowserToolStrip.Size = new System.Drawing.Size(510, 25);
            this.entityBrowserToolStrip.TabIndex = 5;
            this.entityBrowserToolStrip.Text = "toolStrip1";
            // 
            // filterListViewButton
            // 
            this.filterListViewButton.BackColor = System.Drawing.SystemColors.ControlDark;
            this.filterListViewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.filterListViewButton.Image = ((System.Drawing.Image)(resources.GetObject("filterListViewButton.Image")));
            this.filterListViewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filterListViewButton.Margin = new System.Windows.Forms.Padding(0, 1, 4, 2);
            this.filterListViewButton.Name = "filterListViewButton";
            this.filterListViewButton.Size = new System.Drawing.Size(85, 22);
            this.filterListViewButton.Text = "Filter Items By";
            this.filterListViewButton.Click += new System.EventHandler(this.filterListViewButton_Click);
            // 
            // reloadToolStripItem
            // 
            this.reloadToolStripItem.BackColor = System.Drawing.SystemColors.ControlDark;
            this.reloadToolStripItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.reloadToolStripItem.Image = ((System.Drawing.Image)(resources.GetObject("reloadToolStripItem.Image")));
            this.reloadToolStripItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reloadToolStripItem.Name = "reloadToolStripItem";
            this.reloadToolStripItem.Size = new System.Drawing.Size(47, 22);
            this.reloadToolStripItem.Text = "Reload";
            this.reloadToolStripItem.Click += new System.EventHandler(this.reloadToolStripItem_Click);
            // 
            // entityBrowserSplitContainer2
            // 
            this.entityBrowserSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityBrowserSplitContainer2.Location = new System.Drawing.Point(0, 0);
            this.entityBrowserSplitContainer2.Name = "entityBrowserSplitContainer2";
            // 
            // entityBrowserSplitContainer2.Panel1
            // 
            this.entityBrowserSplitContainer2.Panel1.Controls.Add(this.entityBrowserSplitContainer1);
            // 
            // entityBrowserSplitContainer2.Panel2
            // 
            this.entityBrowserSplitContainer2.Panel2.Controls.Add(this.entityBrowserSplitContainer3);
            this.entityBrowserSplitContainer2.Size = new System.Drawing.Size(1105, 618);
            this.entityBrowserSplitContainer2.SplitterDistance = 894;
            this.entityBrowserSplitContainer2.TabIndex = 6;
            // 
            // entityBrowserSplitContainer3
            // 
            this.entityBrowserSplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityBrowserSplitContainer3.Location = new System.Drawing.Point(0, 0);
            this.entityBrowserSplitContainer3.Name = "entityBrowserSplitContainer3";
            this.entityBrowserSplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // entityBrowserSplitContainer3.Panel1
            // 
            this.entityBrowserSplitContainer3.Panel1.Controls.Add(this.iconView);
            // 
            // entityBrowserSplitContainer3.Panel2
            // 
            this.entityBrowserSplitContainer3.Panel2.Controls.Add(this.fileDetailsListBox);
            this.entityBrowserSplitContainer3.Size = new System.Drawing.Size(207, 618);
            this.entityBrowserSplitContainer3.SplitterDistance = 314;
            this.entityBrowserSplitContainer3.TabIndex = 4;
            // 
            // fileDetailsListBox
            // 
            this.fileDetailsListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fileDetailsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileDetailsListBox.FormattingEnabled = true;
            this.fileDetailsListBox.Location = new System.Drawing.Point(0, 0);
            this.fileDetailsListBox.Name = "fileDetailsListBox";
            this.fileDetailsListBox.Size = new System.Drawing.Size(207, 300);
            this.fileDetailsListBox.TabIndex = 0;
            // 
            // EntityBrowserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entityBrowserSplitContainer2);
            this.Name = "EntityBrowserView";
            this.Size = new System.Drawing.Size(1105, 618);
            this.Load += new System.EventHandler(this.EntityBrowserView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconView)).EndInit();
            this.entityBrowserTabControl.ResumeLayout(false);
            this.netWorthItemsTab.ResumeLayout(false);
            this.weaponsTab.ResumeLayout(false);
            this.defenseItemsTab.ResumeLayout(false);
            this.killableEntitiesTab.ResumeLayout(false);
            this.entityBrowserSplitContainer1.Panel1.ResumeLayout(false);
            this.entityBrowserSplitContainer1.Panel1.PerformLayout();
            this.entityBrowserSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer1)).EndInit();
            this.entityBrowserSplitContainer1.ResumeLayout(false);
            this.entityBrowserToolStrip.ResumeLayout(false);
            this.entityBrowserToolStrip.PerformLayout();
            this.entityBrowserSplitContainer2.Panel1.ResumeLayout(false);
            this.entityBrowserSplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer2)).EndInit();
            this.entityBrowserSplitContainer2.ResumeLayout(false);
            this.entityBrowserSplitContainer3.Panel1.ResumeLayout(false);
            this.entityBrowserSplitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.entityBrowserSplitContainer3)).EndInit();
            this.entityBrowserSplitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView netWorthListView;
        private System.Windows.Forms.ColumnHeader item;
        private System.Windows.Forms.ColumnHeader value;
        private System.Windows.Forms.TabControl filePreviewTabs;
        private System.Windows.Forms.PictureBox iconView;
        private System.Windows.Forms.ColumnHeader category;
        private System.Windows.Forms.ColumnHeader modName;
        private System.Windows.Forms.TabControl entityBrowserTabControl;
        private System.Windows.Forms.TabPage netWorthItemsTab;
        private System.Windows.Forms.TabPage weaponsTab;
        private System.Windows.Forms.ListView weaponsListView;
        private System.Windows.Forms.ColumnHeader weaponAlias;
        private System.Windows.Forms.ColumnHeader baseDamage;
        private System.Windows.Forms.ColumnHeader mod;
        private System.Windows.Forms.TabPage defenseItemsTab;
        private System.Windows.Forms.ListView defenseItemsListView;
        private System.Windows.Forms.ColumnHeader defenseItemAlias;
        private System.Windows.Forms.ColumnHeader damageReduction;
        private System.Windows.Forms.ColumnHeader defenseItemModName;
        private System.Windows.Forms.TabPage killableEntitiesTab;
        private System.Windows.Forms.ListView killableEntitiesListView;
        private System.Windows.Forms.ColumnHeader killableEntity;
        private System.Windows.Forms.ColumnHeader weaponILevel;
        private System.Windows.Forms.ColumnHeader defenseItemILevel;
        private System.Windows.Forms.SplitContainer entityBrowserSplitContainer1;
        private System.Windows.Forms.SplitContainer entityBrowserSplitContainer2;
        private System.Windows.Forms.ColumnHeader materialTags;
        private System.Windows.Forms.ColumnHeader handedness;
        private System.Windows.Forms.ColumnHeader roles;
        private System.Windows.Forms.ToolStrip entityBrowserToolStrip;
        private System.Windows.Forms.ToolStripButton filterListViewButton;
        private System.Windows.Forms.ToolStripButton reloadToolStripItem;
        private System.Windows.Forms.SplitContainer entityBrowserSplitContainer3;
        private System.Windows.Forms.ListBox fileDetailsListBox;
    }
}
