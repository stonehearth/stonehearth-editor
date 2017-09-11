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
            ((System.ComponentModel.ISupportInitialize)(this.recipesGridView)).BeginInit();
            this.recipesCellContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // recipesGridView
            // 
            this.recipesGridView.AllowUserToAddRows = false;
            this.recipesGridView.AllowUserToDeleteRows = false;
            this.recipesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recipesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.recipesGridView.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.Location = new System.Drawing.Point(0, 0);
            this.recipesGridView.Name = "recipesGridView";
            this.recipesGridView.RowHeadersVisible = false;
            this.recipesGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.recipesGridView.RowTemplate.ContextMenuStrip = this.recipesCellContextMenu;
            this.recipesGridView.RowTemplate.Height = 30;
            this.recipesGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.recipesGridView.Size = new System.Drawing.Size(758, 546);
            this.recipesGridView.TabIndex = 0;
            this.recipesGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.recipeGridView_CellMouseClick);
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
            // RecipesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.recipesGridView);
            this.Name = "RecipesView";
            this.Size = new System.Drawing.Size(761, 549);
            ((System.ComponentModel.ISupportInitialize)(this.recipesGridView)).EndInit();
            this.recipesCellContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView recipesGridView;
        private System.Windows.Forms.ContextMenuStrip recipesCellContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addNewIngredientToolStripMenuItem;
    }
}
