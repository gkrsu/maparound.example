namespace WinFormsExample
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnViewAll = new System.Windows.Forms.ToolStripButton();
            this.btnTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnFlyTransform = new System.Windows.Forms.ToolStripMenuItem();
            this.btnThematic = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.mapControl = new MapAround.UI.WinForms.MapControl();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapControl)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnZoomIn,
            this.btnZoomOut,
            this.btnViewAll,
            this.btnTools});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(509, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "zzzz";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(40, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.Image")));
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(54, 22);
            this.btnZoomIn.Text = "Zoom +";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.Image")));
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(51, 22);
            this.btnZoomOut.Text = "Zoom -";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnViewAll
            // 
            this.btnViewAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnViewAll.Image = ((System.Drawing.Image)(resources.GetObject("btnViewAll.Image")));
            this.btnViewAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewAll.Name = "btnViewAll";
            this.btnViewAll.Size = new System.Drawing.Size(51, 22);
            this.btnViewAll.Text = "View all";
            this.btnViewAll.Click += new System.EventHandler(this.btnViewAll_Click);
            // 
            // btnTools
            // 
            this.btnTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFlyTransform,
            this.btnThematic,
            this.btnSelect});
            this.btnTools.Image = ((System.Drawing.Image)(resources.GetObject("btnTools.Image")));
            this.btnTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTools.Name = "btnTools";
            this.btnTools.Size = new System.Drawing.Size(49, 22);
            this.btnTools.Text = "Tools";
            // 
            // btnFlyTransform
            // 
            this.btnFlyTransform.CheckOnClick = true;
            this.btnFlyTransform.Name = "btnFlyTransform";
            this.btnFlyTransform.Size = new System.Drawing.Size(152, 22);
            this.btnFlyTransform.Text = "FlyTransform";
            this.btnFlyTransform.Click += new System.EventHandler(this.btnFlyTransform_Click);
            // 
            // btnThematic
            // 
            this.btnThematic.CheckOnClick = true;
            this.btnThematic.Name = "btnThematic";
            this.btnThematic.Size = new System.Drawing.Size(152, 22);
            this.btnThematic.Text = "Thematic";
            this.btnThematic.Click += new System.EventHandler(this.btnThematic_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.CheckOnClick = true;
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(152, 22);
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // mapControl
            // 
            this.mapControl.AlignmentWhileZooming = true;
            this.mapControl.Animation = true;
            this.mapControl.AnimationTime = 400;
            this.mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl.DragMode = MapAround.UI.WinForms.MapControl.DraggingMode.Pan;
            this.mapControl.DragThreshold = 1;
            this.mapControl.Editor = null;
            this.mapControl.IsDragging = false;
            this.mapControl.Location = new System.Drawing.Point(0, 25);
            this.mapControl.Map = null;
            this.mapControl.MouseWheelZooming = true;
            this.mapControl.Name = "mapControl";
            this.mapControl.SelectionMargin = 3;
            this.mapControl.SelectionRectangleColor = System.Drawing.SystemColors.Highlight;
            this.mapControl.Size = new System.Drawing.Size(509, 287);
            this.mapControl.TabIndex = 1;
            this.mapControl.Text = "mapControl1";
            this.mapControl.ZoomPercent = 60;
            this.mapControl.SelectionRectangleDefined += new System.EventHandler<MapAround.UI.WinForms.ViewBoxEventArgs>(this.mapControl_SelectionRectangleDefined);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 312);
            this.Controls.Add(this.mapControl);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "WinFormsExample";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mapControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnViewAll;
        private MapAround.UI.WinForms.MapControl mapControl;
        private System.Windows.Forms.ToolStripDropDownButton btnTools;
        private System.Windows.Forms.ToolStripMenuItem btnFlyTransform;
        private System.Windows.Forms.ToolStripMenuItem btnThematic;
        private System.Windows.Forms.ToolStripMenuItem btnSelect;
    }
}

