using StonehearthEditor.Recipes;

namespace StonehearthEditor
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.manifestTab = new System.Windows.Forms.TabPage();
            this.encounterTab = new System.Windows.Forms.TabPage();
            this.entityBrowserTab = new System.Windows.Forms.TabPage();
            this.effectsEditorTab = new System.Windows.Forms.TabPage();
            this.recipesTab = new System.Windows.Forms.TabPage();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainFormMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeModDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.netWorthVisualizerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manifestView = new StonehearthEditor.ManifestView();
            this.encounterDesignerView = new StonehearthEditor.EncounterDesignerView();
            this.entityBrowserView = new StonehearthEditor.EntityBrowserView();
            this.effectsEditorView = new StonehearthEditor.EffectsEditorView();
            this.recipesView = new StonehearthEditor.Recipes.RecipesView();
            this.tabControl.SuspendLayout();
            this.manifestTab.SuspendLayout();
            this.encounterTab.SuspendLayout();
            this.entityBrowserTab.SuspendLayout();
            this.effectsEditorTab.SuspendLayout();
            this.recipesTab.SuspendLayout();
            this.mainFormMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.manifestTab);
            this.tabControl.Controls.Add(this.encounterTab);
            this.tabControl.Controls.Add(this.entityBrowserTab);
            this.tabControl.Controls.Add(this.effectsEditorTab);
            this.tabControl.Controls.Add(this.recipesTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 28);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1123, 577);
            this.tabControl.TabIndex = 3;
            this.tabControl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // manifestTab
            // 
            this.manifestTab.Controls.Add(this.manifestView);
            this.manifestTab.Location = new System.Drawing.Point(4, 25);
            this.manifestTab.Name = "manifestTab";
            this.manifestTab.Padding = new System.Windows.Forms.Padding(3);
            this.manifestTab.Size = new System.Drawing.Size(1115, 548);
            this.manifestTab.TabIndex = 0;
            this.manifestTab.Text = "Manifest";
            this.manifestTab.UseVisualStyleBackColor = true;
            // 
            // encounterTab
            // 
            this.encounterTab.Controls.Add(this.encounterDesignerView);
            this.encounterTab.Location = new System.Drawing.Point(4, 25);
            this.encounterTab.Name = "encounterTab";
            this.encounterTab.Padding = new System.Windows.Forms.Padding(3);
            this.encounterTab.Size = new System.Drawing.Size(1115, 548);
            this.encounterTab.TabIndex = 1;
            this.encounterTab.Text = "Encounter Designer";
            this.encounterTab.UseVisualStyleBackColor = true;
            // 
            // entityBrowserTab
            // 
            this.entityBrowserTab.Controls.Add(this.entityBrowserView);
            this.entityBrowserTab.Location = new System.Drawing.Point(4, 25);
            this.entityBrowserTab.Name = "entityBrowserTab";
            this.entityBrowserTab.Padding = new System.Windows.Forms.Padding(3);
            this.entityBrowserTab.Size = new System.Drawing.Size(1115, 548);
            this.entityBrowserTab.TabIndex = 2;
            this.entityBrowserTab.Text = "Entity Stats Browser";
            this.entityBrowserTab.UseVisualStyleBackColor = true;
            // 
            // effectsEditorTab
            // 
            this.effectsEditorTab.Controls.Add(this.effectsEditorView);
            this.effectsEditorTab.Location = new System.Drawing.Point(4, 25);
            this.effectsEditorTab.Name = "effectsEditorTab";
            this.effectsEditorTab.Padding = new System.Windows.Forms.Padding(3);
            this.effectsEditorTab.Size = new System.Drawing.Size(1115, 548);
            this.effectsEditorTab.TabIndex = 3;
            this.effectsEditorTab.Text = "Effects Editor";
            this.effectsEditorTab.UseVisualStyleBackColor = true;
            // 
            // recipesTab
            // 
            this.recipesTab.Controls.Add(this.recipesView);
            this.recipesTab.Location = new System.Drawing.Point(4, 25);
            this.recipesTab.Name = "recipesTab";
            this.recipesTab.Padding = new System.Windows.Forms.Padding(3);
            this.recipesTab.Size = new System.Drawing.Size(1115, 548);
            this.recipesTab.TabIndex = 4;
            this.recipesTab.Text = "Recipe Editor";
            this.recipesTab.UseVisualStyleBackColor = true;
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // mainFormMenu
            // 
            this.mainFormMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainFormMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.mainFormMenu.Location = new System.Drawing.Point(0, 0);
            this.mainFormMenu.Name = "mainFormMenu";
            this.mainFormMenu.Size = new System.Drawing.Size(1123, 28);
            this.mainFormMenu.TabIndex = 2;
            this.mainFormMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newModToolStripMenuItem,
            this.saveAllToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.changeModDirectoryToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newModToolStripMenuItem
            // 
            this.newModToolStripMenuItem.Name = "newModToolStripMenuItem";
            this.newModToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+N";
            this.newModToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newModToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.newModToolStripMenuItem.Text = "New Mod";
            this.newModToolStripMenuItem.Click += new System.EventHandler(this.newModToolStripMenuItem_Click);
            // 
            // saveAllToolStripMenuItem
            // 
            this.saveAllToolStripMenuItem.Name = "saveAllToolStripMenuItem";
            this.saveAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAllToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.saveAllToolStripMenuItem.Text = "Save All";
            this.saveAllToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // changeModDirectoryToolStripMenuItem
            // 
            this.changeModDirectoryToolStripMenuItem.Name = "changeModDirectoryToolStripMenuItem";
            this.changeModDirectoryToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.changeModDirectoryToolStripMenuItem.Text = "Change Mod Directory";
            this.changeModDirectoryToolStripMenuItem.Click += new System.EventHandler(this.changeModDirectoryToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.netWorthVisualizerToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // netWorthVisualizerToolStripMenuItem
            // 
            this.netWorthVisualizerToolStripMenuItem.Name = "netWorthVisualizerToolStripMenuItem";
            this.netWorthVisualizerToolStripMenuItem.Size = new System.Drawing.Size(219, 26);
            this.netWorthVisualizerToolStripMenuItem.Text = "Net Worth Visualizer";
            this.netWorthVisualizerToolStripMenuItem.Click += new System.EventHandler(this.netWorthVisualizerToolStripMenuItem_Click);
            // 
            // manifestView
            // 
            this.manifestView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manifestView.Location = new System.Drawing.Point(3, 3);
            this.manifestView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.manifestView.Name = "manifestView";
            this.manifestView.Size = new System.Drawing.Size(1109, 542);
            this.manifestView.TabIndex = 0;
            // 
            // encounterDesignerView
            // 
            this.encounterDesignerView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encounterDesignerView.Location = new System.Drawing.Point(3, 3);
            this.encounterDesignerView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.encounterDesignerView.Name = "encounterDesignerView";
            this.encounterDesignerView.Size = new System.Drawing.Size(1109, 542);
            this.encounterDesignerView.TabIndex = 0;
            // 
            // entityBrowserView
            // 
            this.entityBrowserView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityBrowserView.Location = new System.Drawing.Point(3, 3);
            this.entityBrowserView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.entityBrowserView.Name = "entityBrowserView";
            this.entityBrowserView.Size = new System.Drawing.Size(1109, 542);
            this.entityBrowserView.TabIndex = 0;
            // 
            // effectsEditorView
            // 
            this.effectsEditorView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.effectsEditorView.Location = new System.Drawing.Point(3, 3);
            this.effectsEditorView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.effectsEditorView.Name = "effectsEditorView";
            this.effectsEditorView.Size = new System.Drawing.Size(1109, 542);
            this.effectsEditorView.TabIndex = 0;
            // 
            // recipesView
            // 
            this.recipesView.AutoSize = true;
            this.recipesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recipesView.Location = new System.Drawing.Point(3, 3);
            this.recipesView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.recipesView.Name = "recipesView";
            this.recipesView.Size = new System.Drawing.Size(1109, 542);
            this.recipesView.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1123, 605);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.mainFormMenu);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::StonehearthEditor.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = global::StonehearthEditor.Properties.Settings.Default.MainFormLocation;
            this.MainMenuStrip = this.mainFormMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stonehearth Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tabControl.ResumeLayout(false);
            this.manifestTab.ResumeLayout(false);
            this.encounterTab.ResumeLayout(false);
            this.entityBrowserTab.ResumeLayout(false);
            this.effectsEditorTab.ResumeLayout(false);
            this.recipesTab.ResumeLayout(false);
            this.recipesTab.PerformLayout();
            this.mainFormMenu.ResumeLayout(false);
            this.mainFormMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage encounterTab;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.MenuStrip mainFormMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeModDirectoryToolStripMenuItem;
        private System.Windows.Forms.TabPage manifestTab;
        private ManifestView manifestView;
        private EncounterDesignerView encounterDesignerView;
        private EntityBrowserView entityBrowserView;
        private System.Windows.Forms.TabPage entityBrowserTab;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem netWorthVisualizerToolStripMenuItem;
        private System.Windows.Forms.TabPage effectsEditorTab;
        private System.Windows.Forms.ToolStripMenuItem newModToolStripMenuItem;
        private EffectsEditorView effectsEditorView;
        private System.Windows.Forms.TabPage recipesTab;
        private RecipesView recipesView;
    }
}