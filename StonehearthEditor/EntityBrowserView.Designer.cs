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
         this.netWorthListView = new System.Windows.Forms.ListView();
         this.item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.modName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.filePreviewTabs = new System.Windows.Forms.TabControl();
         this.iconView = new System.Windows.Forms.PictureBox();
         this.killableEntitiesTab = new System.Windows.Forms.TabControl();
         this.netWorthItemsTab = new System.Windows.Forms.TabPage();
         this.weaponsTab = new System.Windows.Forms.TabPage();
         this.weaponsListView = new System.Windows.Forms.ListView();
         this.weaponAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.baseDamage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.mod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.defenseItemsTab = new System.Windows.Forms.TabPage();
         this.defenseItemsListView = new System.Windows.Forms.ListView();
         this.defenseItemAlias = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.damageReduction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.defenseItemModName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.tabPage1 = new System.Windows.Forms.TabPage();
         this.killableEntitiesListView = new System.Windows.Forms.ListView();
         this.killableEntity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.weaponILevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.defenseItemILevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).BeginInit();
         this.killableEntitiesTab.SuspendLayout();
         this.netWorthItemsTab.SuspendLayout();
         this.weaponsTab.SuspendLayout();
         this.defenseItemsTab.SuspendLayout();
         this.tabPage1.SuspendLayout();
         this.SuspendLayout();
         // 
         // netWorthListView
         // 
         this.netWorthListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
         this.netWorthListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.item,
            this.value,
            this.category,
            this.modName});
         this.netWorthListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.netWorthListView.FullRowSelect = true;
         this.netWorthListView.GridLines = true;
         this.netWorthListView.LabelEdit = true;
         this.netWorthListView.Location = new System.Drawing.Point(3, 3);
         this.netWorthListView.Name = "netWorthListView";
         this.netWorthListView.Size = new System.Drawing.Size(518, 586);
         this.netWorthListView.TabIndex = 1;
         this.netWorthListView.UseCompatibleStateImageBehavior = false;
         this.netWorthListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.netWorthListView_ColumnClick);
         this.netWorthListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.netWorthListView_ItemSelectionChanged);
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
         // filePreviewTabs
         // 
         this.filePreviewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
         this.filePreviewTabs.Location = new System.Drawing.Point(532, 0);
         this.filePreviewTabs.Name = "filePreviewTabs";
         this.filePreviewTabs.SelectedIndex = 0;
         this.filePreviewTabs.Size = new System.Drawing.Size(355, 618);
         this.filePreviewTabs.TabIndex = 2;
         // 
         // iconView
         // 
         this.iconView.Dock = System.Windows.Forms.DockStyle.Right;
         this.iconView.Location = new System.Drawing.Point(887, 0);
         this.iconView.Name = "iconView";
         this.iconView.Size = new System.Drawing.Size(218, 618);
         this.iconView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
         this.iconView.TabIndex = 3;
         this.iconView.TabStop = false;
         // 
         // killableEntitiesTab
         // 
         this.killableEntitiesTab.Controls.Add(this.netWorthItemsTab);
         this.killableEntitiesTab.Controls.Add(this.weaponsTab);
         this.killableEntitiesTab.Controls.Add(this.defenseItemsTab);
         this.killableEntitiesTab.Controls.Add(this.tabPage1);
         this.killableEntitiesTab.Dock = System.Windows.Forms.DockStyle.Left;
         this.killableEntitiesTab.Location = new System.Drawing.Point(0, 0);
         this.killableEntitiesTab.Name = "killableEntitiesTab";
         this.killableEntitiesTab.SelectedIndex = 0;
         this.killableEntitiesTab.Size = new System.Drawing.Size(532, 618);
         this.killableEntitiesTab.TabIndex = 4;
         // 
         // netWorthItemsTab
         // 
         this.netWorthItemsTab.Controls.Add(this.netWorthListView);
         this.netWorthItemsTab.Location = new System.Drawing.Point(4, 22);
         this.netWorthItemsTab.Name = "netWorthItemsTab";
         this.netWorthItemsTab.Padding = new System.Windows.Forms.Padding(3);
         this.netWorthItemsTab.Size = new System.Drawing.Size(524, 592);
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
         this.weaponsTab.Size = new System.Drawing.Size(524, 592);
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
            this.mod});
         this.weaponsListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.weaponsListView.LabelEdit = true;
         this.weaponsListView.Location = new System.Drawing.Point(3, 3);
         this.weaponsListView.Name = "weaponsListView";
         this.weaponsListView.Size = new System.Drawing.Size(518, 586);
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
         this.defenseItemsTab.Size = new System.Drawing.Size(524, 592);
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
         this.defenseItemsListView.Size = new System.Drawing.Size(518, 586);
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
         // defenseItemModName
         // 
         this.defenseItemModName.Text = "Mod Name";
         this.defenseItemModName.Width = -2;
         // 
         // tabPage1
         // 
         this.tabPage1.Controls.Add(this.killableEntitiesListView);
         this.tabPage1.Location = new System.Drawing.Point(4, 22);
         this.tabPage1.Name = "tabPage1";
         this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage1.Size = new System.Drawing.Size(524, 592);
         this.tabPage1.TabIndex = 3;
         this.tabPage1.Text = "Killable Entities";
         this.tabPage1.UseVisualStyleBackColor = true;
         // 
         // killableEntitiesListView
         // 
         this.killableEntitiesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.killableEntity});
         this.killableEntitiesListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.killableEntitiesListView.LabelEdit = true;
         this.killableEntitiesListView.Location = new System.Drawing.Point(3, 3);
         this.killableEntitiesListView.Name = "killableEntitiesListView";
         this.killableEntitiesListView.Size = new System.Drawing.Size(518, 586);
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
         // weaponILevel
         // 
         this.weaponILevel.Text = "iLevel";
         // 
         // defenseItemILevel
         // 
         this.defenseItemILevel.Text = "iLevel";
         // 
         // EntityBrowserView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.filePreviewTabs);
         this.Controls.Add(this.iconView);
         this.Controls.Add(this.killableEntitiesTab);
         this.Name = "EntityBrowserView";
         this.Size = new System.Drawing.Size(1105, 618);
         ((System.ComponentModel.ISupportInitialize)(this.iconView)).EndInit();
         this.killableEntitiesTab.ResumeLayout(false);
         this.netWorthItemsTab.ResumeLayout(false);
         this.weaponsTab.ResumeLayout(false);
         this.defenseItemsTab.ResumeLayout(false);
         this.tabPage1.ResumeLayout(false);
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
      private System.Windows.Forms.TabControl killableEntitiesTab;
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
      private System.Windows.Forms.TabPage tabPage1;
      private System.Windows.Forms.ListView killableEntitiesListView;
      private System.Windows.Forms.ColumnHeader killableEntity;
      private System.Windows.Forms.ColumnHeader weaponILevel;
      private System.Windows.Forms.ColumnHeader defenseItemILevel;
   }
}
