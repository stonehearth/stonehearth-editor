namespace StonehearthEditor
{
   partial class EffectsBuilderView
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
         this.btnSave = new System.Windows.Forms.Button();
         this.pnlEditor = new System.Windows.Forms.Panel();
         this.btnPreview = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // btnSave
         // 
         this.btnSave.Location = new System.Drawing.Point(8, 7);
         this.btnSave.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.btnSave.Name = "btnSave";
         this.btnSave.Size = new System.Drawing.Size(197, 69);
         this.btnSave.TabIndex = 1;
         this.btnSave.Text = "Save";
         this.btnSave.UseVisualStyleBackColor = true;
         this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
         // 
         // pnlEditor
         // 
         this.pnlEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.pnlEditor.AutoScroll = true;
         this.pnlEditor.Location = new System.Drawing.Point(8, 91);
         this.pnlEditor.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.pnlEditor.Name = "pnlEditor";
         this.pnlEditor.Size = new System.Drawing.Size(1133, 1276);
         this.pnlEditor.TabIndex = 2;
         // 
         // btnPreview
         // 
         this.btnPreview.Location = new System.Drawing.Point(221, 8);
         this.btnPreview.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.btnPreview.Name = "btnPreview";
         this.btnPreview.Size = new System.Drawing.Size(197, 69);
         this.btnPreview.TabIndex = 3;
         this.btnPreview.Text = "Preview";
         this.btnPreview.UseVisualStyleBackColor = true;
         this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
         // 
         // EffectsBuilderView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.btnPreview);
         this.Controls.Add(this.pnlEditor);
         this.Controls.Add(this.btnSave);
         this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
         this.Name = "EffectsBuilderView";
         this.Size = new System.Drawing.Size(1149, 1374);
         this.ResumeLayout(false);

      }

      #endregion
      private System.Windows.Forms.Button btnSave;
      private System.Windows.Forms.Panel pnlEditor;
      private System.Windows.Forms.Button btnPreview;
   }
}
