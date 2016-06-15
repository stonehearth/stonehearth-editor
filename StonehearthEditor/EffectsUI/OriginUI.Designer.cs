namespace StonehearthEditor.EffectsUI
{
   partial class OriginUI
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
         this.lblSurface = new System.Windows.Forms.Label();
         this.lblValue1 = new System.Windows.Forms.Label();
         this.lblValue2 = new System.Windows.Forms.Label();
         this.cmbSurface = new System.Windows.Forms.ComboBox();
         this.txtValue1 = new System.Windows.Forms.TextBox();
         this.txtValue2 = new System.Windows.Forms.TextBox();
         this.lblOrigin = new System.Windows.Forms.Label();
         this.btnToggle = new System.Windows.Forms.Button();
         this.wrnValue2 = new StonehearthEditor.EffectsUI.WarningIcon();
         this.wrnValue1 = new StonehearthEditor.EffectsUI.WarningIcon();
         this.SuspendLayout();
         // 
         // lblSurface
         // 
         this.lblSurface.AutoSize = true;
         this.lblSurface.Location = new System.Drawing.Point(70, 73);
         this.lblSurface.Name = "lblSurface";
         this.lblSurface.Size = new System.Drawing.Size(113, 32);
         this.lblSurface.TabIndex = 0;
         this.lblSurface.Text = "Surface";
         // 
         // lblValue1
         // 
         this.lblValue1.AutoSize = true;
         this.lblValue1.Location = new System.Drawing.Point(69, 121);
         this.lblValue1.Name = "lblValue1";
         this.lblValue1.Size = new System.Drawing.Size(112, 32);
         this.lblValue1.TabIndex = 1;
         this.lblValue1.Text = "Value 1";
         // 
         // lblValue2
         // 
         this.lblValue2.AutoSize = true;
         this.lblValue2.Location = new System.Drawing.Point(70, 162);
         this.lblValue2.Name = "lblValue2";
         this.lblValue2.Size = new System.Drawing.Size(112, 32);
         this.lblValue2.TabIndex = 2;
         this.lblValue2.Text = "Value 2";
         // 
         // cmbSurface
         // 
         this.cmbSurface.FormattingEnabled = true;
         this.cmbSurface.Items.AddRange(new object[] {
            "POINT",
            "RECTANGLE"});
         this.cmbSurface.Location = new System.Drawing.Point(204, 70);
         this.cmbSurface.Name = "cmbSurface";
         this.cmbSurface.Size = new System.Drawing.Size(342, 39);
         this.cmbSurface.TabIndex = 3;
         this.cmbSurface.SelectedValueChanged += new System.EventHandler(this.SurfaceChanged);
         // 
         // txtValue1
         // 
         this.txtValue1.Location = new System.Drawing.Point(204, 115);
         this.txtValue1.Name = "txtValue1";
         this.txtValue1.Size = new System.Drawing.Size(345, 38);
         this.txtValue1.TabIndex = 4;
         this.txtValue1.TextChanged += new System.EventHandler(this.Value1Changed);
         // 
         // txtValue2
         // 
         this.txtValue2.Location = new System.Drawing.Point(204, 159);
         this.txtValue2.Name = "txtValue2";
         this.txtValue2.Size = new System.Drawing.Size(345, 38);
         this.txtValue2.TabIndex = 5;
         this.txtValue2.TextChanged += new System.EventHandler(this.Value2Changed);
         // 
         // lblOrigin
         // 
         this.lblOrigin.AutoSize = true;
         this.lblOrigin.Location = new System.Drawing.Point(3, 10);
         this.lblOrigin.Name = "lblOrigin";
         this.lblOrigin.Size = new System.Drawing.Size(92, 32);
         this.lblOrigin.TabIndex = 6;
         this.lblOrigin.Text = "Origin";
         // 
         // btnToggle
         // 
         this.btnToggle.Location = new System.Drawing.Point(101, 10);
         this.btnToggle.Name = "btnToggle";
         this.btnToggle.Size = new System.Drawing.Size(123, 50);
         this.btnToggle.TabIndex = 7;
         this.btnToggle.UseVisualStyleBackColor = true;
         this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
         // 
         // wrnValue2
         // 
         this.wrnValue2.Error = null;
         this.wrnValue2.Location = new System.Drawing.Point(555, 162);
         this.wrnValue2.Name = "wrnValue2";
         this.wrnValue2.Size = new System.Drawing.Size(49, 46);
         this.wrnValue2.TabIndex = 9;
         // 
         // wrnValue1
         // 
         this.wrnValue1.Error = null;
         this.wrnValue1.Location = new System.Drawing.Point(555, 115);
         this.wrnValue1.Name = "wrnValue1";
         this.wrnValue1.Size = new System.Drawing.Size(49, 46);
         this.wrnValue1.TabIndex = 8;
         // 
         // OriginUI
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.wrnValue2);
         this.Controls.Add(this.wrnValue1);
         this.Controls.Add(this.btnToggle);
         this.Controls.Add(this.lblOrigin);
         this.Controls.Add(this.txtValue2);
         this.Controls.Add(this.txtValue1);
         this.Controls.Add(this.cmbSurface);
         this.Controls.Add(this.lblValue2);
         this.Controls.Add(this.lblValue1);
         this.Controls.Add(this.lblSurface);
         this.Name = "OriginUI";
         this.Size = new System.Drawing.Size(615, 212);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label lblSurface;
      private System.Windows.Forms.Label lblValue1;
      private System.Windows.Forms.Label lblValue2;
      private System.Windows.Forms.ComboBox cmbSurface;
      private System.Windows.Forms.TextBox txtValue1;
      private System.Windows.Forms.TextBox txtValue2;
      private System.Windows.Forms.Label lblOrigin;
      private System.Windows.Forms.Button btnToggle;
      private WarningIcon wrnValue1;
      private WarningIcon wrnValue2;
   }
}
