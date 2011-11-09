namespace Paril.Components.Shortcuts
{
	partial class ShortcutEditor
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.labelControl1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Window;
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainer1.Panel2.Controls.Add(this.button2);
			this.splitContainer1.Size = new System.Drawing.Size(434, 297);
			this.splitContainer1.SplitterDistance = 264;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.listBox1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.labelControl1);
			this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
			this.splitContainer2.Panel2.Controls.Add(this.textBox1);
			this.splitContainer2.Panel2.Controls.Add(this.label1);
			this.splitContainer2.Size = new System.Drawing.Size(434, 264);
			this.splitContainer2.SplitterDistance = 162;
			this.splitContainer2.TabIndex = 0;
			// 
			// listBox1
			// 
			this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(0, 0);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(162, 264);
			this.listBox1.TabIndex = 0;
			this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// labelControl1
			// 
			this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelControl1.BackColor = System.Drawing.SystemColors.Window;
			this.labelControl1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.labelControl1.Location = new System.Drawing.Point(31, 165);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(225, 55);
			this.labelControl1.TabIndex = 2;
			this.labelControl1.Text = "Watt";
			this.labelControl1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelControl1.Visible = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pictureBox1.ErrorImage = null;
			this.pictureBox1.Image = global::MCSkin3D.Properties.Resources._109_AllAnnotations_Error_16x16_72;
			this.pictureBox1.Location = new System.Drawing.Point(14, 185);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.textBox1.Location = new System.Drawing.Point(14, 140);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(242, 20);
			this.textBox1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label1.Location = new System.Drawing.Point(14, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(242, 72);
			this.label1.TabIndex = 0;
			this.label1.Text = "M_SHORTCUTS";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// button2
			// 
			this.button2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.button2.Location = new System.Drawing.Point(180, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// ShortcutEditor
			// 
			this.AcceptButton = this.button2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 297);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "ShortcutEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Shortcut Keys";
			this.Load += new System.EventHandler(this.ShortcutEditor_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label labelControl1;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}