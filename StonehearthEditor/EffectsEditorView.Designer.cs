namespace StonehearthEditor
{
   partial class EffectsEditorView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EffectsEditorView));
         this.effectsSplitContainer = new System.Windows.Forms.SplitContainer();
         this.effectsEditorListView = new System.Windows.Forms.ListView();
         this.effectsSplitContainer2 = new System.Windows.Forms.SplitContainer();
         this.filePreviewTabs = new System.Windows.Forms.TabControl();
         this.editOptionsListView = new System.Windows.Forms.ListView();
         this.effectsToolStrip = new System.Windows.Forms.ToolStrip();
         this.newFileButton = new System.Windows.Forms.ToolStripButton();
         this.effectsOpenFileButton = new System.Windows.Forms.ToolStripButton();
         this.openEffectsFileDialog = new System.Windows.Forms.OpenFileDialog();
         ((System.ComponentModel.ISupportInitialize)(this.effectsSplitContainer)).BeginInit();
         this.effectsSplitContainer.Panel1.SuspendLayout();
         this.effectsSplitContainer.Panel2.SuspendLayout();
         this.effectsSplitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.effectsSplitContainer2)).BeginInit();
         this.effectsSplitContainer2.Panel1.SuspendLayout();
         this.effectsSplitContainer2.Panel2.SuspendLayout();
         this.effectsSplitContainer2.SuspendLayout();
         this.effectsToolStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // effectsSplitContainer
         // 
         this.effectsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.effectsSplitContainer.Location = new System.Drawing.Point(0, 25);
         this.effectsSplitContainer.Name = "effectsSplitContainer";
         // 
         // effectsSplitContainer.Panel1
         // 
         this.effectsSplitContainer.Panel1.Controls.Add(this.effectsEditorListView);
         // 
         // effectsSplitContainer.Panel2
         // 
         this.effectsSplitContainer.Panel2.Controls.Add(this.effectsSplitContainer2);
         this.effectsSplitContainer.Size = new System.Drawing.Size(756, 548);
         this.effectsSplitContainer.SplitterDistance = 218;
         this.effectsSplitContainer.TabIndex = 1;
         // 
         // effectsEditorListView
         // 
         this.effectsEditorListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.effectsEditorListView.Location = new System.Drawing.Point(0, 0);
         this.effectsEditorListView.Name = "effectsEditorListView";
         this.effectsEditorListView.Size = new System.Drawing.Size(218, 548);
         this.effectsEditorListView.TabIndex = 0;
         this.effectsEditorListView.UseCompatibleStateImageBehavior = false;
         // 
         // effectsSplitContainer2
         // 
         this.effectsSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.effectsSplitContainer2.Location = new System.Drawing.Point(0, 0);
         this.effectsSplitContainer2.Name = "effectsSplitContainer2";
         // 
         // effectsSplitContainer2.Panel1
         // 
         this.effectsSplitContainer2.Panel1.Controls.Add(this.filePreviewTabs);
         // 
         // effectsSplitContainer2.Panel2
         // 
         this.effectsSplitContainer2.Panel2.Controls.Add(this.editOptionsListView);
         this.effectsSplitContainer2.Size = new System.Drawing.Size(534, 548);
         this.effectsSplitContainer2.SplitterDistance = 325;
         this.effectsSplitContainer2.TabIndex = 0;
         // 
         // filePreviewTabs
         // 
         this.filePreviewTabs.Dock = System.Windows.Forms.DockStyle.Fill;
         this.filePreviewTabs.Location = new System.Drawing.Point(0, 0);
         this.filePreviewTabs.Name = "filePreviewTabs";
         this.filePreviewTabs.SelectedIndex = 0;
         this.filePreviewTabs.Size = new System.Drawing.Size(325, 548);
         this.filePreviewTabs.TabIndex = 3;
         // 
         // editOptionsListView
         // 
         this.editOptionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.editOptionsListView.Location = new System.Drawing.Point(0, 0);
         this.editOptionsListView.Name = "editOptionsListView";
         this.editOptionsListView.Size = new System.Drawing.Size(205, 548);
         this.editOptionsListView.TabIndex = 0;
         this.editOptionsListView.UseCompatibleStateImageBehavior = false;
         // 
         // effectsToolStrip
         // 
         this.effectsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFileButton,
            this.effectsOpenFileButton});
         this.effectsToolStrip.Location = new System.Drawing.Point(0, 0);
         this.effectsToolStrip.Name = "effectsToolStrip";
         this.effectsToolStrip.Size = new System.Drawing.Size(756, 25);
         this.effectsToolStrip.TabIndex = 2;
         this.effectsToolStrip.Text = "Effects Tool Strip";
         // 
         // newFileButton
         // 
         this.newFileButton.BackColor = System.Drawing.SystemColors.ButtonShadow;
         this.newFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.newFileButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
         this.newFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.newFileButton.Name = "newFileButton";
         this.newFileButton.Size = new System.Drawing.Size(105, 22);
         this.newFileButton.Text = "+ New Effects File";
         this.newFileButton.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
         // 
         // effectsOpenFileButton
         // 
         this.effectsOpenFileButton.BackColor = System.Drawing.SystemColors.ButtonShadow;
         this.effectsOpenFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.effectsOpenFileButton.Image = ((System.Drawing.Image)(resources.GetObject("effectsOpenFileButton.Image")));
         this.effectsOpenFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.effectsOpenFileButton.Margin = new System.Windows.Forms.Padding(3, 1, 0, 2);
         this.effectsOpenFileButton.Name = "effectsOpenFileButton";
         this.effectsOpenFileButton.Size = new System.Drawing.Size(61, 22);
         this.effectsOpenFileButton.Text = "Open File";
         this.effectsOpenFileButton.Click += new System.EventHandler(this.effectsOpenFileButton_Click);
         // 
         // openEffectsFileDialog
         // 
         this.openEffectsFileDialog.FileName = "openEffectsFileDialog";
         this.openEffectsFileDialog.Filter = "JSON Files|*.json|Lua Files|*.lua";
         this.openEffectsFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openEffectsFileDialog_FileOk);
         // 
         // EffectsEditorView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.effectsSplitContainer);
         this.Controls.Add(this.effectsToolStrip);
         this.Name = "EffectsEditorView";
         this.Size = new System.Drawing.Size(756, 573);
         this.effectsSplitContainer.Panel1.ResumeLayout(false);
         this.effectsSplitContainer.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.effectsSplitContainer)).EndInit();
         this.effectsSplitContainer.ResumeLayout(false);
         this.effectsSplitContainer2.Panel1.ResumeLayout(false);
         this.effectsSplitContainer2.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.effectsSplitContainer2)).EndInit();
         this.effectsSplitContainer2.ResumeLayout(false);
         this.effectsToolStrip.ResumeLayout(false);
         this.effectsToolStrip.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.SplitContainer effectsSplitContainer;
      private System.Windows.Forms.ListView effectsEditorListView;
      private System.Windows.Forms.SplitContainer effectsSplitContainer2;
      private System.Windows.Forms.ListView editOptionsListView;
      private System.Windows.Forms.TabControl filePreviewTabs;
      private System.Windows.Forms.ToolStrip effectsToolStrip;
      private System.Windows.Forms.ToolStripButton newFileButton;
      private System.Windows.Forms.ToolStripButton effectsOpenFileButton;
      private System.Windows.Forms.OpenFileDialog openEffectsFileDialog;
   }
}
