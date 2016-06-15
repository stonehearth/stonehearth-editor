namespace StonehearthEditor.EffectsUI.ParameterKinds
{
   partial class RandomBetweenScalarParameterKindUI
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
         this.lblMin = new System.Windows.Forms.Label();
         this.lblMax = new System.Windows.Forms.Label();
         this.txtMin = new System.Windows.Forms.TextBox();
         this.txtMax = new System.Windows.Forms.TextBox();
         this.wrnMin = new StonehearthEditor.EffectsUI.WarningIcon();
         this.wrnMax = new StonehearthEditor.EffectsUI.WarningIcon();
         this.SuspendLayout();
         // 
         // lblMin
         // 
         this.lblMin.AutoSize = true;
         this.lblMin.Location = new System.Drawing.Point(20, 10);
         this.lblMin.Name = "lblMin";
         this.lblMin.Size = new System.Drawing.Size(24, 13);
         this.lblMin.TabIndex = 0;
         this.lblMin.Text = "Min";
         // 
         // lblMax
         // 
         this.lblMax.AutoSize = true;
         this.lblMax.Location = new System.Drawing.Point(20, 36);
         this.lblMax.Name = "lblMax";
         this.lblMax.Size = new System.Drawing.Size(27, 13);
         this.lblMax.TabIndex = 1;
         this.lblMax.Text = "Max";
         // 
         // txtMin
         // 
         this.txtMin.Location = new System.Drawing.Point(50, 7);
         this.txtMin.Name = "txtMin";
         this.txtMin.Size = new System.Drawing.Size(113, 20);
         this.txtMin.TabIndex = 2;
         this.txtMin.TextChanged += new System.EventHandler(this.minChanged);
         // 
         // txtMax
         // 
         this.txtMax.Location = new System.Drawing.Point(50, 33);
         this.txtMax.Name = "txtMax";
         this.txtMax.Size = new System.Drawing.Size(113, 20);
         this.txtMax.TabIndex = 3;
         this.txtMax.TextChanged += new System.EventHandler(this.maxChanged);
         // 
         // wrnMin
         // 
         this.wrnMin.Error = null;
         this.wrnMin.Location = new System.Drawing.Point(169, 7);
         this.wrnMin.Name = "wrnMin";
         this.wrnMin.Size = new System.Drawing.Size(30, 24);
         this.wrnMin.TabIndex = 4;
         this.wrnMin.Text = "warningIcon1";
         // 
         // wrnMax
         // 
         this.wrnMax.Error = null;
         this.wrnMax.Location = new System.Drawing.Point(169, 33);
         this.wrnMax.Name = "wrnMax";
         this.wrnMax.Size = new System.Drawing.Size(30, 24);
         this.wrnMax.TabIndex = 5;
         this.wrnMax.Text = "warningIcon2";
         // 
         // RandomBetweenScalarParameterKindUI
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.wrnMax);
         this.Controls.Add(this.wrnMin);
         this.Controls.Add(this.txtMax);
         this.Controls.Add(this.txtMin);
         this.Controls.Add(this.lblMax);
         this.Controls.Add(this.lblMin);
         this.Name = "RandomBetweenScalarParameterKindUI";
         this.Size = new System.Drawing.Size(210, 65);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label lblMin;
      private System.Windows.Forms.Label lblMax;
      private System.Windows.Forms.TextBox txtMin;
      private System.Windows.Forms.TextBox txtMax;
      private WarningIcon wrnMin;
      private WarningIcon wrnMax;
   }
}
