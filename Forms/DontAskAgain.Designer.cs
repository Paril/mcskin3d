namespace MCSkin3D.Forms
{
	partial class DontAskAgain
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.languageProvider1 = new MCSkin3D.Language.LanguageProvider();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(127, 122);
			this.button1.Name = "button1";
			this.languageProvider1.SetPropertyNames(this.button1, "Text");
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "C_NO";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.button2.Location = new System.Drawing.Point(208, 122);
			this.button2.Name = "button2";
			this.languageProvider1.SetPropertyNames(this.button2, "Text");
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "C_YES";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(208, 93);
			this.checkBox1.Name = "checkBox1";
			this.languageProvider1.SetPropertyNames(this.checkBox1, "Text");
			this.checkBox1.Size = new System.Drawing.Size(92, 17);
			this.checkBox1.TabIndex = 2;
			this.checkBox1.Text = "D_DONTASK";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.languageProvider1.SetPropertyNames(this.label1, "Text");
			this.label1.Size = new System.Drawing.Size(387, 81);
			this.label1.TabIndex = 3;
			this.label1.Text = "M_IRREVERSIBLE";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// DontAskAgain
			// 
			this.AcceptButton = this.button2;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(411, 151);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DontAskAgain";
			this.Load += new System.EventHandler(this.DontAskAgain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label1;
		private Language.LanguageProvider languageProvider1;
	}
}