namespace ChaosIslandHacking
{
    partial class Form3
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
            this.picMap = new System.Windows.Forms.PictureBox();
            this.picPalette = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aIMarkersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passabilityMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reservedLocsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.terrainObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewAIMarkers = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewPassability = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewRes = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewTerrain = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewTerrainObj = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewUnits = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.scrollPalette = new System.Windows.Forms.VScrollBar();
            this.scrollMapX = new System.Windows.Forms.HScrollBar();
            this.scrollMapY = new System.Windows.Forms.VScrollBar();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picMap
            // 
            this.picMap.Location = new System.Drawing.Point(170, 27);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(707, 538);
            this.picMap.TabIndex = 0;
            this.picMap.TabStop = false;
            this.picMap.Paint += new System.Windows.Forms.PaintEventHandler(this.picMap_Paint);
            this.picMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseMove);
            this.picMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseUp);
            // 
            // picPalette
            // 
            this.picPalette.Location = new System.Drawing.Point(12, 66);
            this.picPalette.Name = "picPalette";
            this.picPalette.Size = new System.Drawing.Size(128, 518);
            this.picPalette.TabIndex = 1;
            this.picPalette.TabStop = false;
            this.picPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.picPalette_Paint);
            this.picPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picPalette_MouseUp);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.layerToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(901, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // layerToolStripMenuItem
            // 
            this.layerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aIMarkersToolStripMenuItem,
            this.passabilityMapToolStripMenuItem,
            this.reservedLocsToolStripMenuItem,
            this.terrainToolStripMenuItem,
            this.terrainObjectsToolStripMenuItem,
            this.unitsToolStripMenuItem});
            this.layerToolStripMenuItem.Name = "layerToolStripMenuItem";
            this.layerToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.layerToolStripMenuItem.Text = "&Layer";
            this.layerToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.layerToolStripMenuItem_DropDownItemClicked);
            // 
            // aIMarkersToolStripMenuItem
            // 
            this.aIMarkersToolStripMenuItem.Name = "aIMarkersToolStripMenuItem";
            this.aIMarkersToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aIMarkersToolStripMenuItem.Text = "&AI Markers";
            // 
            // passabilityMapToolStripMenuItem
            // 
            this.passabilityMapToolStripMenuItem.Name = "passabilityMapToolStripMenuItem";
            this.passabilityMapToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.passabilityMapToolStripMenuItem.Text = "&Passability Map";
            // 
            // reservedLocsToolStripMenuItem
            // 
            this.reservedLocsToolStripMenuItem.Name = "reservedLocsToolStripMenuItem";
            this.reservedLocsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reservedLocsToolStripMenuItem.Text = "&ReservedLocs";
            // 
            // terrainToolStripMenuItem
            // 
            this.terrainToolStripMenuItem.Name = "terrainToolStripMenuItem";
            this.terrainToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.terrainToolStripMenuItem.Text = "&Terrain";
            // 
            // terrainObjectsToolStripMenuItem
            // 
            this.terrainObjectsToolStripMenuItem.Name = "terrainObjectsToolStripMenuItem";
            this.terrainObjectsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.terrainObjectsToolStripMenuItem.Text = "Terrain &Objects";
            // 
            // unitsToolStripMenuItem
            // 
            this.unitsToolStripMenuItem.Checked = true;
            this.unitsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unitsToolStripMenuItem.Name = "unitsToolStripMenuItem";
            this.unitsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.unitsToolStripMenuItem.Text = "&Units";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miViewAIMarkers,
            this.miViewPassability,
            this.miViewRes,
            this.miViewTerrain,
            this.miViewTerrainObj,
            this.miViewUnits});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // miViewAIMarkers
            // 
            this.miViewAIMarkers.Checked = true;
            this.miViewAIMarkers.CheckOnClick = true;
            this.miViewAIMarkers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miViewAIMarkers.Name = "miViewAIMarkers";
            this.miViewAIMarkers.Size = new System.Drawing.Size(152, 22);
            this.miViewAIMarkers.Text = "&AI Markers";
            this.miViewAIMarkers.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // miViewPassability
            // 
            this.miViewPassability.Checked = true;
            this.miViewPassability.CheckOnClick = true;
            this.miViewPassability.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miViewPassability.Name = "miViewPassability";
            this.miViewPassability.Size = new System.Drawing.Size(152, 22);
            this.miViewPassability.Text = "&Passability Map";
            this.miViewPassability.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // miViewRes
            // 
            this.miViewRes.Checked = true;
            this.miViewRes.CheckOnClick = true;
            this.miViewRes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miViewRes.Name = "miViewRes";
            this.miViewRes.Size = new System.Drawing.Size(152, 22);
            this.miViewRes.Text = "&ReservedLocs";
            this.miViewRes.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // miViewTerrain
            // 
            this.miViewTerrain.Checked = true;
            this.miViewTerrain.CheckOnClick = true;
            this.miViewTerrain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miViewTerrain.Name = "miViewTerrain";
            this.miViewTerrain.Size = new System.Drawing.Size(152, 22);
            this.miViewTerrain.Text = "&Terrain";
            this.miViewTerrain.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // miViewTerrainObj
            // 
            this.miViewTerrainObj.CheckOnClick = true;
            this.miViewTerrainObj.Name = "miViewTerrainObj";
            this.miViewTerrainObj.Size = new System.Drawing.Size(152, 22);
            this.miViewTerrainObj.Text = "Terrain &Objects";
            this.miViewTerrainObj.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // miViewUnits
            // 
            this.miViewUnits.CheckOnClick = true;
            this.miViewUnits.Name = "miViewUnits";
            this.miViewUnits.Size = new System.Drawing.Size(152, 22);
            this.miViewUnits.Text = "&Units";
            this.miViewUnits.Click += new System.EventHandler(this.viewMenuSubitem_Click);
            // 
            // scrollPalette
            // 
            this.scrollPalette.LargeChange = 1;
            this.scrollPalette.Location = new System.Drawing.Point(143, 66);
            this.scrollPalette.Maximum = 0;
            this.scrollPalette.Name = "scrollPalette";
            this.scrollPalette.Size = new System.Drawing.Size(16, 518);
            this.scrollPalette.TabIndex = 3;
            this.scrollPalette.ValueChanged += new System.EventHandler(this.scrollPalette_ValueChanged);
            // 
            // scrollMapX
            // 
            this.scrollMapX.LargeChange = 20;
            this.scrollMapX.Location = new System.Drawing.Point(170, 568);
            this.scrollMapX.Name = "scrollMapX";
            this.scrollMapX.Size = new System.Drawing.Size(707, 16);
            this.scrollMapX.SmallChange = 5;
            this.scrollMapX.TabIndex = 4;
            this.scrollMapX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollMapX_Scroll);
            // 
            // scrollMapY
            // 
            this.scrollMapY.LargeChange = 20;
            this.scrollMapY.Location = new System.Drawing.Point(880, 27);
            this.scrollMapY.Name = "scrollMapY";
            this.scrollMapY.Size = new System.Drawing.Size(16, 538);
            this.scrollMapY.SmallChange = 5;
            this.scrollMapY.TabIndex = 5;
            this.scrollMapY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollMapY_Scroll);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 588);
            this.Controls.Add(this.scrollMapY);
            this.Controls.Add(this.scrollMapX);
            this.Controls.Add(this.scrollPalette);
            this.Controls.Add(this.picPalette);
            this.Controls.Add(this.picMap);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form3";
            this.Text = "Chaos Island Hacking -  Map Editor";
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPalette)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picMap;
        private System.Windows.Forms.PictureBox picPalette;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem terrainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aIMarkersToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem terrainObjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reservedLocsToolStripMenuItem;
        private System.Windows.Forms.VScrollBar scrollPalette;
        private System.Windows.Forms.HScrollBar scrollMapX;
        private System.Windows.Forms.VScrollBar scrollMapY;
        private System.Windows.Forms.ToolStripMenuItem passabilityMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miViewAIMarkers;
        private System.Windows.Forms.ToolStripMenuItem miViewPassability;
        private System.Windows.Forms.ToolStripMenuItem miViewRes;
        private System.Windows.Forms.ToolStripMenuItem miViewTerrain;
        private System.Windows.Forms.ToolStripMenuItem miViewTerrainObj;
        private System.Windows.Forms.ToolStripMenuItem miViewUnits;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
    }
}