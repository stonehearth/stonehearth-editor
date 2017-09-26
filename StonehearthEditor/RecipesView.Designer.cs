namespace StonehearthEditor
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
            this.recipesGridView = new System.Windows.Forms.DataGridView();
            this.recipesCellContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewIngredientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.searchBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.filterByColumn = new System.Windows.Forms.ToolStripComboBox();
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
            this.recipesGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.recipesGridView.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.Location = new System.Drawing.Point(0, 31);
            this.recipesGridView.Name = "recipesGridView";
            this.recipesGridView.RowHeadersVisible = false;
            this.recipesGridView.RowTemplate.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.RowTemplate.Height = 30;
            this.recipesGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.recipesGridView.Size = new System.Drawing.Size(758, 518);
            this.recipesGridView.TabIndex = 0;
            this.recipesGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.recipesGridView_CellMouseClick);
            this.recipesGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.recipesGridView_CellValueChanged);
            this.recipesGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.recipesGridView_DataError);
            this.recipesGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.recipesGridView_EditingControlShowing);
            this.recipesGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.recipesGridView_KeyDown);
            // 
            // recipesCellContextMenu
            // 
            this.recipesCellContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewIngredientToolStripMenuItem});
            this.recipesCellContextMenu.Name = "recipesCellContextMenu";
            this.recipesCellContextMenu.Size = new System.Drawing.Size(179, 26);
            // 
            // addNewIngredientToolStripMenuItem
            // 
            this.addNewIngredientToolStripMenuItem.Name = "addNewIngredientToolStripMenuItem";
            this.addNewIngredientToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.addNewIngredientToolStripMenuItem.Text = "Add new ingredient";
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
            this.filterByColumn});
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
            // filterByColumn
            // 
            this.filterByColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterByColumn.Name = "filterByColumn";
            this.filterByColumn.Size = new System.Drawing.Size(121, 25);
            this.filterByColumn.ToolTipText = "Select column to filter by";
            this.filterByColumn.SelectedIndexChanged += new System.EventHandler(this.searchBox_Filter);
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
        private System.Windows.Forms.ToolStripComboBox filterByColumn;
    }
}
