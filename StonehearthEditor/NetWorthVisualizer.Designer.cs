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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetWorthVisualizer));
         this.xAxisPanel = new System.Windows.Forms.Panel();
         this.xAxisLabel = new System.Windows.Forms.Label();
         this.imageTooltip = new System.Windows.Forms.ToolTip(this.components);
         this.panel1 = new System.Windows.Forms.Panel();
         this.canvas = new System.Windows.Forms.PictureBox();
         this.topBar = new System.Windows.Forms.Panel();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.zoomInButton = new System.Windows.Forms.ToolStripButton();
         this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
         this.xAxisPanel.SuspendLayout();
         this.panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
         this.topBar.SuspendLayout();
         this.toolStrip1.SuspendLayout();
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
         this.panel1.Location = new System.Drawing.Point(0, 50);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(1297, 478);
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
         this.canvas.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDoubleClick);
         this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
         // 
         // topBar
         // 
         this.topBar.Controls.Add(this.toolStrip1);
         this.topBar.Dock = System.Windows.Forms.DockStyle.Top;
         this.topBar.Location = new System.Drawing.Point(0, 0);
         this.topBar.Name = "topBar";
         this.topBar.Size = new System.Drawing.Size(1297, 50);
         this.topBar.TabIndex = 1;
         // 
         // toolStrip1
         // 
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInButton,
            this.zoomOutButton});
         this.toolStrip1.Location = new System.Drawing.Point(0, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(1297, 25);
         this.toolStrip1.TabIndex = 0;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // zoomInButton
         // 
         this.zoomInButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInButton.Image")));
         this.zoomInButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.zoomInButton.Name = "zoomInButton";
         this.zoomInButton.Size = new System.Drawing.Size(72, 22);
         this.zoomInButton.Text = "Zoom In";
         this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
         // 
         // zoomOutButton
         // 
         this.zoomOutButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutButton.Image")));
         this.zoomOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.zoomOutButton.Name = "zoomOutButton";
         this.zoomOutButton.Size = new System.Drawing.Size(82, 22);
         this.zoomOutButton.Text = "Zoom Out";
         this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
         // 
         // NetWorthVisualizer
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1297, 562);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.topBar);
         this.Controls.Add(this.xAxisPanel);
         this.Name = "NetWorthVisualizer";
         this.Text = "NetWorthVisualizer";
         this.xAxisPanel.ResumeLayout(false);
         this.xAxisPanel.PerformLayout();
         this.panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
         this.topBar.ResumeLayout(false);
         this.topBar.PerformLayout();
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion
      private System.Windows.Forms.Panel xAxisPanel;
      private System.Windows.Forms.Label xAxisLabel;
      private System.Windows.Forms.ToolTip imageTooltip;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.PictureBox canvas;
      private System.Windows.Forms.Panel topBar;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton zoomInButton;
      private System.Windows.Forms.ToolStripButton zoomOutButton;
   }
}