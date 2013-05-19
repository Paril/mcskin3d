namespace MCSkin3D.ExceptionHandler
{
	partial class ExceptionForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionForm));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.button2 = new System.Windows.Forms.Button();
			this.languageProvider1 = new MCSkin3D.Languages.LanguageProvider();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(48, 48);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Location = new System.Drawing.Point(55, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(340, 166);
			this.panel1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label1.Location = new System.Drawing.Point(11, 11);
			this.label1.Name = "label1";
			this.languageProvider1.SetPropertyNames(this.label1, "Text");
			this.label1.Size = new System.Drawing.Size(165, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "M_EXPT_APPERROR";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(15, 43);
			this.label3.Name = "label3";
			this.languageProvider1.SetPropertyNames(this.label3, "Text");
			this.label3.Size = new System.Drawing.Size(322, 119);
			this.label3.TabIndex = 1;
			this.label3.Text = "M_EXPT_ERRORTEXT";
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Controls.Add(this.linkLabel1);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.pictureBox2);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 227);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(407, 35);
			this.panel2.TabIndex = 2;
			this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint_1);
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(221, 11);
			this.linkLabel1.Name = "linkLabel1";
			this.languageProvider1.SetPropertyNames(this.linkLabel1, "Text");
			this.linkLabel1.Size = new System.Drawing.Size(86, 13);
			this.linkLabel1.TabIndex = 2;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "M_EXPT_HERE";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(34, 11);
			this.label2.Name = "label2";
			this.languageProvider1.SetPropertyNames(this.label2, "Text");
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "M_EXPT_VIEW";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.Location = new System.Drawing.Point(8, 5);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(24, 24);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox2.TabIndex = 0;
			this.pictureBox2.TabStop = false;
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.SystemColors.Control;
			this.panel3.Controls.Add(this.checkBox2);
			this.panel3.Controls.Add(this.button2);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(0, 180);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(407, 47);
			this.panel3.TabIndex = 3;
			this.panel3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(14, 16);
			this.checkBox2.Name = "checkBox2";
			this.languageProvider1.SetPropertyNames(this.checkBox2, "Text");
			this.checkBox2.Size = new System.Drawing.Size(109, 17);
			this.checkBox2.TabIndex = 3;
			this.checkBox2.Text = "M_EXPT_SHOW";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.FlatAppearance.BorderSize = 0;
			this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button2.Location = new System.Drawing.Point(320, 11);
			this.button2.Name = "button2";
			this.languageProvider1.SetPropertyNames(this.button2, "Text");
			this.button2.Size = new System.Drawing.Size(75, 24);
			this.button2.TabIndex = 2;
			this.button2.Text = "M_EXPT_CLOSE";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// languageProvider1
			// 
			this.languageProvider1.BaseControl = this;
			// 
			// ExceptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(407, 262);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExceptionForm";
			this.languageProvider1.SetPropertyNames(this, "Text");
			this.ShowIcon = false;
			this.Text = "M_EXPT_EXCEPTION";
			this.Load += new System.EventHandler(this.ExceptionForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBox2;
		public MCSkin3D.Languages.LanguageProvider languageProvider1;

	}
}