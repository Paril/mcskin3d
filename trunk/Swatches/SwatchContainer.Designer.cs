namespace MCSkin3D
{
	partial class SwatchContainer
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.panel2 = new System.Windows.Forms.Panel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.nativeToolStrip1 = new Paril.Controls.NativeToolStrip();
			this.newSwatchToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteSwatchToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
			this.convertSwatchTtripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1 = new Paril.Controls.NativeToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.editModeToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.addSwatchToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.removeSwatchToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.languageProvider1 = new MCSkin3D.Language.LanguageProvider();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.nativeToolStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.vScrollBar1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 50);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(270, 140);
			this.panel1.TabIndex = 1;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1.Location = new System.Drawing.Point(251, 0);
			this.vScrollBar1.Maximum = 10;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 138);
			this.vScrollBar1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.comboBox1);
			this.panel2.Controls.Add(this.nativeToolStrip1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(270, 25);
			this.panel2.TabIndex = 4;
			// 
			// comboBox1
			// 
			this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(0, 0);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(177, 21);
			this.comboBox1.TabIndex = 4;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// nativeToolStrip1
			// 
			this.nativeToolStrip1.AllowMerge = false;
			this.nativeToolStrip1.AutoSize = false;
			this.nativeToolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
			this.nativeToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.nativeToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSwatchToolStripButton,
            this.deleteSwatchToolStripButton,
            this.toolStripButton3,
            this.convertSwatchTtripButton});
			this.nativeToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.nativeToolStrip1.Location = new System.Drawing.Point(177, 0);
			this.nativeToolStrip1.Name = "nativeToolStrip1";
			this.nativeToolStrip1.Size = new System.Drawing.Size(93, 25);
			this.nativeToolStrip1.TabIndex = 3;
			this.nativeToolStrip1.Text = "nativeToolStrip1";
			// 
			// newSwatchToolStripButton
			// 
			this.newSwatchToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.newSwatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.newSwatchToolStripButton.Image = global::MCSkin3D.Properties.Resources.newswatch;
			this.newSwatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.newSwatchToolStripButton.Name = "newSwatchToolStripButton";
			this.languageProvider1.SetPropertyNames(this.newSwatchToolStripButton, "Text");
			this.newSwatchToolStripButton.Size = new System.Drawing.Size(23, 20);
			this.newSwatchToolStripButton.Text = "M_NEWPALETTE";
			// 
			// deleteSwatchToolStripButton
			// 
			this.deleteSwatchToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.deleteSwatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.deleteSwatchToolStripButton.Image = global::MCSkin3D.Properties.Resources.deleteswatch;
			this.deleteSwatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.deleteSwatchToolStripButton.Name = "deleteSwatchToolStripButton";
			this.languageProvider1.SetPropertyNames(this.deleteSwatchToolStripButton, "Text");
			this.deleteSwatchToolStripButton.Size = new System.Drawing.Size(23, 20);
			this.deleteSwatchToolStripButton.Text = "M_DELETEPALETTE";
			// 
			// toolStripButton3
			// 
			this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton3.Image = global::MCSkin3D.Properties.Resources.renameswatch;
			this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new System.Drawing.Size(23, 20);
			this.toolStripButton3.Text = "toolStripButton3";
			// 
			// convertSwatchTtripButton
			// 
			this.convertSwatchTtripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.convertSwatchTtripButton.Image = global::MCSkin3D.Properties.Resources.convertswatch;
			this.convertSwatchTtripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.convertSwatchTtripButton.Name = "convertSwatchTtripButton";
			this.languageProvider1.SetPropertyNames(this.convertSwatchTtripButton, "Text");
			this.convertSwatchTtripButton.Size = new System.Drawing.Size(23, 20);
			this.convertSwatchTtripButton.Text = "M_CONVERTSWATCH";
			this.convertSwatchTtripButton.Click += new System.EventHandler(this.convertSwatchTtripButton_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.editModeToolStripButton,
            this.addSwatchToolStripButton,
            this.removeSwatchToolStripButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 25);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(270, 25);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton1
			// 
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = global::MCSkin3D.Properties.Resources.ZoomOutHS;
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Black;
			this.toolStripButton1.Name = "toolStripButton1";
			this.languageProvider1.SetPropertyNames(this.toolStripButton1, "Text");
			this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton1.Text = "T_TREE_ZOOMOUT";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			// 
			// toolStripButton2
			// 
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = global::MCSkin3D.Properties.Resources.ZoomInHS;
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Black;
			this.toolStripButton2.Name = "toolStripButton2";
			this.languageProvider1.SetPropertyNames(this.toolStripButton2, "Text");
			this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton2.Text = "T_TREE_ZOOMIN";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// editModeToolStripButton
			// 
			this.editModeToolStripButton.CheckOnClick = true;
			this.editModeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.editModeToolStripButton.Image = global::MCSkin3D.Properties.Resources.pipette;
			this.editModeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.editModeToolStripButton.Name = "editModeToolStripButton";
			this.languageProvider1.SetPropertyNames(this.editModeToolStripButton, "Text");
			this.editModeToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.editModeToolStripButton.Text = "T_SWATCHEDIT";
			// 
			// addSwatchToolStripButton
			// 
			this.addSwatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.addSwatchToolStripButton.Image = global::MCSkin3D.Properties.Resources._112_Plus_Green_16x16_72;
			this.addSwatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.addSwatchToolStripButton.Name = "addSwatchToolStripButton";
			this.languageProvider1.SetPropertyNames(this.addSwatchToolStripButton, "Text");
			this.addSwatchToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.addSwatchToolStripButton.Text = "T_ADDSWATCH";
			// 
			// removeSwatchToolStripButton
			// 
			this.removeSwatchToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.removeSwatchToolStripButton.Image = global::MCSkin3D.Properties.Resources._112_Minus_Orange_16x16_72;
			this.removeSwatchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.removeSwatchToolStripButton.Name = "removeSwatchToolStripButton";
			this.languageProvider1.SetPropertyNames(this.removeSwatchToolStripButton, "Text");
			this.removeSwatchToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.removeSwatchToolStripButton.Text = "T_DELETESWATCH";
			// 
			// SwatchContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.panel2);
			this.Name = "SwatchContainer";
			this.Size = new System.Drawing.Size(270, 190);
			this.Load += new System.EventHandler(this.SwatchContainer_Load);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.nativeToolStrip1.ResumeLayout(false);
			this.nativeToolStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private Paril.Controls.NativeToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		public Language.LanguageProvider languageProvider1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton editModeToolStripButton;
		private System.Windows.Forms.ToolStripButton removeSwatchToolStripButton;
		private System.Windows.Forms.ToolStripButton addSwatchToolStripButton;
		private Paril.Controls.NativeToolStrip nativeToolStrip1;
		private System.Windows.Forms.ToolStripButton newSwatchToolStripButton;
		private System.Windows.Forms.ToolStripButton deleteSwatchToolStripButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.ToolStripButton convertSwatchTtripButton;
		private System.Windows.Forms.ToolStripButton toolStripButton3;
	}
}
