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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.swatchDisplayer1 = new MCSkin3D.SwatchDisplayer();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(0, 0);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(270, 21);
			this.comboBox1.TabIndex = 0;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.swatchDisplayer1);
			this.panel1.Controls.Add(this.vScrollBar1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 21);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(270, 169);
			this.panel1.TabIndex = 1;
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
			this.vScrollBar1.Location = new System.Drawing.Point(251, 0);
			this.vScrollBar1.Maximum = 0;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 167);
			this.vScrollBar1.TabIndex = 0;
			// 
			// swatchDisplayer1
			// 
			this.swatchDisplayer1.Colors = null;
			this.swatchDisplayer1.Location = new System.Drawing.Point(127, 39);
			this.swatchDisplayer1.Name = "swatchDisplayer1";
			this.swatchDisplayer1.Scale = 0;
			this.swatchDisplayer1.ScrollBar = null;
			this.swatchDisplayer1.Size = new System.Drawing.Size(75, 23);
			this.swatchDisplayer1.TabIndex = 1;
			this.swatchDisplayer1.Text = "swatchDisplayer1";
			// 
			// SwatchContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.comboBox1);
			this.Name = "SwatchContainer";
			this.Size = new System.Drawing.Size(270, 190);
			this.Load += new System.EventHandler(this.SwatchContainer_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private SwatchDisplayer swatchDisplayer1;
	}
}
