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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecipesView));
            this.recipesGridView = new System.Windows.Forms.DataGridView();
            this.recipesCellContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewIngredientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.recipesGridView)).BeginInit();
            this.recipesCellContextMenu.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // recipesGridView
            // 
            this.recipesGridView.AllowUserToAddRows = false;
            this.recipesGridView.AllowUserToDeleteRows = false;
            this.recipesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recipesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.searchPanel.Controls.Add(this.searchBox);
            this.searchPanel.Controls.Add(this.searchButton);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(761, 25);
            this.searchPanel.TabIndex = 1;
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(3, 3);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(177, 20);
            this.searchBox.TabIndex = 5;
            this.searchBox.WordWrap = false;
            // 
            // searchButton
            // 
            this.searchButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.searchButton.Image = ((System.Drawing.Image)(resources.GetObject("searchButton.Image")));
            this.searchButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.searchButton.Location = new System.Drawing.Point(183, 3);
            this.searchButton.Margin = new System.Windows.Forms.Padding(0);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(23, 22);
            this.searchButton.TabIndex = 6;
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView recipesGridView;
        private System.Windows.Forms.ContextMenuStrip recipesCellContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewIngredientToolStripMenuItem;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Button searchButton;
    }
}
