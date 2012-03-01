namespace MCSkin3D.UpdateSystem
{
	partial class Updater
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Updater));
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.button2 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
			this.olvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			this.olvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(269, 39);
			this.label1.TabIndex = 1;
			this.label1.Text = "Check the updates you want to install, then click Install.\r\nMajor and minor updat" +
    "es should be installed, while\r\nskins and others are optional.";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Image = global::MCSkin3D.Properties.Resources.smaller3D;
			this.pictureBox1.Location = new System.Drawing.Point(437, -7);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(64, 64);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.Location = new System.Drawing.Point(3, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "Install";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.button2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(171, 372);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(162, 29);
			this.tableLayoutPanel1.TabIndex = 4;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(84, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 4;
			this.button2.Text = "Cancel";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(-5, 365);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(518, 48);
			this.panel1.TabIndex = 5;
			// 
			// panel2
			// 
			this.panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.label2);
			this.panel2.Location = new System.Drawing.Point(152, 169);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(200, 69);
			this.panel2.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(33, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(132, 26);
			this.label2.TabIndex = 0;
			this.label2.Text = "Downloading update data,\r\nplease wait...";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// objectListView1
			// 
			this.objectListView1.AllColumns.Add(this.olvColumn1);
			this.objectListView1.AllColumns.Add(this.olvColumn2);
			this.objectListView1.AllColumns.Add(this.olvColumn5);
			this.objectListView1.AllColumns.Add(this.olvColumn3);
			this.objectListView1.AllColumns.Add(this.olvColumn4);
			this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn1,
            this.olvColumn2,
            this.olvColumn3,
            this.olvColumn4});
			this.objectListView1.Cursor = System.Windows.Forms.Cursors.Default;
			this.objectListView1.Location = new System.Drawing.Point(4, 48);
			this.objectListView1.Name = "objectListView1";
			this.objectListView1.Size = new System.Drawing.Size(497, 311);
			this.objectListView1.SortGroupItemsByPrimaryColumn = false;
			this.objectListView1.TabIndex = 7;
			this.objectListView1.UseCompatibleStateImageBehavior = false;
			this.objectListView1.UseHotItem = true;
			this.objectListView1.UseTranslucentHotItem = true;
			this.objectListView1.UseTranslucentSelection = true;
			this.objectListView1.View = System.Windows.Forms.View.Details;
			// 
			// olvColumn1
			// 
			this.olvColumn1.AspectName = "Name";
			this.olvColumn1.CheckBoxes = true;
			this.olvColumn1.IsEditable = false;
			this.olvColumn1.Text = "Name";
			// 
			// olvColumn2
			// 
			this.olvColumn2.AspectName = "Information";
			this.olvColumn2.FillsFreeSpace = true;
			this.olvColumn2.IsEditable = false;
			this.olvColumn2.Text = "Information";
			// 
			// olvColumn5
			// 
			this.olvColumn5.AspectName = "Progress";
			this.olvColumn5.DisplayIndex = 1;
			this.olvColumn5.FillsFreeSpace = true;
			this.olvColumn5.IsEditable = false;
			this.olvColumn5.IsVisible = false;
			this.olvColumn5.Text = "Progress";
			// 
			// olvColumn3
			// 
			this.olvColumn3.AspectName = "Size";
			this.olvColumn3.IsEditable = false;
			this.olvColumn3.Text = "Size";
			// 
			// olvColumn4
			// 
			this.olvColumn4.AspectName = "RealDate";
			this.olvColumn4.AspectToStringFormat = "{0:d}";
			this.olvColumn4.IsEditable = false;
			this.olvColumn4.Text = "Date";
			// 
			// Updater
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(505, 407);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.objectListView1);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Updater";
			this.ShowIcon = false;
			this.Text = "MCSkin3D Updater";
			this.Load += new System.EventHandler(this.Updater_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label2;
		private BrightIdeasSoftware.ObjectListView objectListView1;
		private BrightIdeasSoftware.OLVColumn olvColumn1;
		private BrightIdeasSoftware.OLVColumn olvColumn2;
		private BrightIdeasSoftware.OLVColumn olvColumn3;
		private BrightIdeasSoftware.OLVColumn olvColumn4;
		private BrightIdeasSoftware.OLVColumn olvColumn5;

	}
}