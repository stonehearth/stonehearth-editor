namespace StonehearthEditor.Recipes
{
    partial class RecipesView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipesView));
            this.recipesGridView = new System.Windows.Forms.DataGridView();
            this.recipesCellContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeIngredientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewIngredientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.searchBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.filterCbx = new System.Windows.Forms.ToolStripComboBox();
            this.itemsTypeComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.baseModsButton = new System.Windows.Forms.ToolStripButton();
            this.helpButton = new System.Windows.Forms.ToolStripButton();
            this.unsavedFilesLabel = new System.Windows.Forms.ToolStripLabel();
            this.openJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRecipeJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.recipesGridView)).BeginInit();
            this.recipesCellContextMenu.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // recipesGridView
            // 
            this.recipesGridView.AllowUserToAddRows = false;
            this.recipesGridView.AllowUserToDeleteRows = false;
            this.recipesGridView.AllowUserToResizeColumns = false;
            this.recipesGridView.AllowUserToResizeRows = false;
            this.recipesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recipesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.recipesGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.recipesGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.recipesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.recipesGridView.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.Location = new System.Drawing.Point(0, 31);
            this.recipesGridView.Name = "recipesGridView";
            this.recipesGridView.RowHeadersVisible = false;
            this.recipesGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.recipesGridView.RowTemplate.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.RowTemplate.Height = 30;
            this.recipesGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.recipesGridView.Size = new System.Drawing.Size(758, 518);
            this.recipesGridView.TabIndex = 0;
            this.recipesGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.recipesGridView_CellBeginEdit);
            this.recipesGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.recipesGridView_CellEndEdit);
            this.recipesGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.recipesGridView_CellMouseClick);
            this.recipesGridView.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.recipesGridView_CellMouseUp);
            this.recipesGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.recipesGridView_CellValueChanged);
            this.recipesGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.recipesGridView_DataBindingComplete);
            this.recipesGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.recipesGridView_DataError);
            this.recipesGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.recipesGridView_EditingControlShowing);
            this.recipesGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.recipesGridView_KeyDown);
            // 
            // recipesCellContextMenu
            // 
            this.recipesCellContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeIngredientToolStripMenuItem,
            this.addNewIngredientToolStripMenuItem,
            this.openJSONToolStripMenuItem,
            this.openRecipeJSONToolStripMenuItem});
            this.recipesCellContextMenu.Name = "recipesCellContextMenu";
            this.recipesCellContextMenu.Size = new System.Drawing.Size(179, 114);
            // 
            // removeIngredientToolStripMenuItem
            // 
            this.removeIngredientToolStripMenuItem.Name = "removeIngredientToolStripMenuItem";
            this.removeIngredientToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.removeIngredientToolStripMenuItem.Text = "Remove ingredient";
            this.removeIngredientToolStripMenuItem.Click += new System.EventHandler(this.removeIngredientToolStripMenuItem_Click);
            // 
            // addNewIngredientToolStripMenuItem
            // 
            this.addNewIngredientToolStripMenuItem.Name = "addNewIngredientToolStripMenuItem";
            this.addNewIngredientToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.addNewIngredientToolStripMenuItem.Text = "Add new ingredient";
            this.addNewIngredientToolStripMenuItem.Click += new System.EventHandler(this.addNewIngredientToolStripMenuItem_Click);
            // 
            // searchPanel
            // 
            this.searchPanel.Controls.Add(this.toolStrip);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(761, 25);
            this.searchPanel.TabIndex = 1;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchBox,
            this.searchButton,
            this.filterCbx,
            this.itemsTypeComboBox,
            this.baseModsButton,
            this.helpButton,
            this.unsavedFilesLabel});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(761, 25);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.Text = "toolStrip";
            // 
            // searchBox
            // 
            this.searchBox.MaxLength = 1000;
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(100, 25);
            this.searchBox.ToolTipText = "Filter recipes";
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_Filter);
            // 
            // searchButton
            // 
            this.searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchButton.Image = global::StonehearthEditor.Properties.Resources.Find_56502;
            this.searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(23, 22);
            this.searchButton.Text = "searchButton";
            this.searchButton.ToolTipText = "Filter recipes";
            this.searchButton.Click += new System.EventHandler(this.searchBox_Filter);
            // 
            // filterCbx
            // 
            this.filterCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterCbx.Name = "filterCbx";
            this.filterCbx.Size = new System.Drawing.Size(121, 25);
            this.filterCbx.ToolTipText = "Select column to filter by";
            this.filterCbx.SelectedIndexChanged += new System.EventHandler(this.searchBox_Filter);
            // 
            // itemsTypeComboBox
            // 
            this.itemsTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemsTypeComboBox.Items.AddRange(new object[] {
            "Recipes",
            "Entities",
            "Iconics"});
            this.itemsTypeComboBox.Name = "itemsTypeComboBox";
            this.itemsTypeComboBox.Size = new System.Drawing.Size(121, 25);
            this.itemsTypeComboBox.ToolTipText = "Select a type of data to edit";
            this.itemsTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.itemsTypeComboBox_SelectedIndexChanged);
            // 
            // baseModsButton
            // 
            this.baseModsButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.baseModsButton.Checked = true;
            this.baseModsButton.CheckOnClick = true;
            this.baseModsButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.baseModsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.baseModsButton.Image = ((System.Drawing.Image)(resources.GetObject("baseModsButton.Image")));
            this.baseModsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.baseModsButton.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
            this.baseModsButton.Name = "baseModsButton";
            this.baseModsButton.Size = new System.Drawing.Size(113, 22);
            this.baseModsButton.Text = "Hide external mods";
            this.baseModsButton.ToolTipText = "Click to toggle showing of items from the \"stonehearth\" and \"rayyas_children\" mods";
            this.baseModsButton.CheckedChanged += new System.EventHandler(this.baseModsButton_CheckedChanged);
            // 
            // helpButton
            // 
            this.helpButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.helpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpButton.Image = ((System.Drawing.Image)(resources.GetObject("helpButton.Image")));
            this.helpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(36, 22);
            this.helpButton.Text = "Help";
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // unsavedFilesLabel
            // 
            this.unsavedFilesLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.unsavedFilesLabel.Name = "unsavedFilesLabel";
            this.unsavedFilesLabel.Size = new System.Drawing.Size(106, 22);
            this.unsavedFilesLabel.Text = "Unsaved Changes*";
            this.unsavedFilesLabel.Visible = false;
            // 
            // openJSONToolStripMenuItem
            // 
            this.openJSONToolStripMenuItem.Name = "openJSONToolStripMenuItem";
            this.openJSONToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.openJSONToolStripMenuItem.Text = "Open Entity JSON";
            this.openJSONToolStripMenuItem.Click += new System.EventHandler(this.openJSONToolStripMenuItem_Click);
            // 
            // openRecipeJSONToolStripMenuItem
            // 
            this.openRecipeJSONToolStripMenuItem.Name = "openRecipeJSONToolStripMenuItem";
            this.openRecipeJSONToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.openRecipeJSONToolStripMenuItem.Text = "Open Recipe JSON";
            this.openRecipeJSONToolStripMenuItem.Click += new System.EventHandler(this.openRecipeJSONToolStripMenuItem_Click);
            // 
            // RecipesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this.recipesGridView);
            this.Name = "RecipesView";
            this.Size = new System.Drawing.Size(761, 552);
            ((System.ComponentModel.ISupportInitialize)(this.recipesGridView)).EndInit();
            this.recipesCellContextMenu.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView recipesGridView;
        private System.Windows.Forms.ContextMenuStrip recipesCellContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewIngredientToolStripMenuItem;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripTextBox searchBox;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.ToolStripComboBox filterCbx;
        private System.Windows.Forms.ToolStripMenuItem removeIngredientToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton helpButton;
        private System.Windows.Forms.ToolStripLabel unsavedFilesLabel;
        private System.Windows.Forms.ToolStripComboBox itemsTypeComboBox;
        private System.Windows.Forms.ToolStripButton baseModsButton;
        private System.Windows.Forms.ToolStripMenuItem openJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRecipeJSONToolStripMenuItem;
    }
}
