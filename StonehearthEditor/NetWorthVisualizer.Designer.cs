namespace StonehearthEditor
{
   partial class NetWorthVisualizer
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this.xAxisPanel = new System.Windows.Forms.Panel();
         this.xAxisLabel = new System.Windows.Forms.Label();
         this.imageTooltip = new System.Windows.Forms.ToolTip(this.components);
         this.panel1 = new System.Windows.Forms.Panel();
         this.canvas = new System.Windows.Forms.PictureBox();
         this.xAxisPanel.SuspendLayout();
         this.panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
         this.SuspendLayout();
         // 
         // xAxisPanel
         // 
         this.xAxisPanel.Controls.Add(this.xAxisLabel);
         this.xAxisPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.xAxisPanel.Location = new System.Drawing.Point(0, 528);
         this.xAxisPanel.Name = "xAxisPanel";
         this.xAxisPanel.Size = new System.Drawing.Size(1297, 34);
         this.xAxisPanel.TabIndex = 0;
         // 
         // xAxisLabel
         // 
         this.xAxisLabel.AutoSize = true;
         this.xAxisLabel.Location = new System.Drawing.Point(606, 9);
         this.xAxisLabel.Name = "xAxisLabel";
         this.xAxisLabel.Size = new System.Drawing.Size(28, 13);
         this.xAxisLabel.TabIndex = 0;
         this.xAxisLabel.Text = "Cost";
         // 
         // panel1
         // 
         this.panel1.AutoScroll = true;
         this.panel1.Controls.Add(this.canvas);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(0, 0);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(1297, 528);
         this.panel1.TabIndex = 0;
         // 
         // canvas
         // 
         this.canvas.Location = new System.Drawing.Point(0, 0);
         this.canvas.Name = "canvas";
         this.canvas.Size = new System.Drawing.Size(100, 50);
         this.canvas.TabIndex = 0;
         this.canvas.TabStop = false;
         this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
         this.canvas.DoubleClick += new System.EventHandler(this.canvas_DoubleClick);
         // 
         // NetWorthVisualizer
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1297, 562);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.xAxisPanel);
         this.Name = "NetWorthVisualizer";
         this.Text = "NetWorthVisualizer";
         this.xAxisPanel.ResumeLayout(false);
         this.xAxisPanel.PerformLayout();
         this.panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion
      private System.Windows.Forms.Panel xAxisPanel;
      private System.Windows.Forms.Label xAxisLabel;
      private System.Windows.Forms.ToolTip imageTooltip;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.PictureBox canvas;
   }
}