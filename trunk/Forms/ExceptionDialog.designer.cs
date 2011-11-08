namespace Paril.Windows.Dialogs
{
	partial class ExceptionDialog
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
			this.label1 = new System.Windows.Forms.Label();
			this.exceptionName = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.generalHelpLink = new System.Windows.Forms.TextBox();
			this.generalTargetMethod = new System.Windows.Forms.TextBox();
			this.generalSource = new System.Windows.Forms.TextBox();
			this.generalMessage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.stackTrace = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.listView1 = new System.Windows.Forms.ListView();
			this.KeyHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ValueHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.button2 = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(277, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "The application has encountered the following exception:";
			// 
			// exceptionName
			// 
			this.exceptionName.Location = new System.Drawing.Point(295, 6);
			this.exceptionName.Name = "exceptionName";
			this.exceptionName.ReadOnly = true;
			this.exceptionName.Size = new System.Drawing.Size(222, 20);
			this.exceptionName.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(442, 216);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "&OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Location = new System.Drawing.Point(15, 32);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(502, 178);
			this.tabControl1.TabIndex = 4;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.generalHelpLink);
			this.tabPage1.Controls.Add(this.generalTargetMethod);
			this.tabPage1.Controls.Add(this.generalSource);
			this.tabPage1.Controls.Add(this.generalMessage);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.label3);
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(494, 152);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// generalHelpLink
			// 
			this.generalHelpLink.Location = new System.Drawing.Point(109, 126);
			this.generalHelpLink.Name = "generalHelpLink";
			this.generalHelpLink.ReadOnly = true;
			this.generalHelpLink.Size = new System.Drawing.Size(372, 20);
			this.generalHelpLink.TabIndex = 7;
			// 
			// generalTargetMethod
			// 
			this.generalTargetMethod.Location = new System.Drawing.Point(109, 102);
			this.generalTargetMethod.Name = "generalTargetMethod";
			this.generalTargetMethod.ReadOnly = true;
			this.generalTargetMethod.Size = new System.Drawing.Size(372, 20);
			this.generalTargetMethod.TabIndex = 6;
			// 
			// generalSource
			// 
			this.generalSource.Location = new System.Drawing.Point(109, 78);
			this.generalSource.Name = "generalSource";
			this.generalSource.ReadOnly = true;
			this.generalSource.Size = new System.Drawing.Size(372, 20);
			this.generalSource.TabIndex = 5;
			// 
			// generalMessage
			// 
			this.generalMessage.Location = new System.Drawing.Point(109, 9);
			this.generalMessage.Multiline = true;
			this.generalMessage.Name = "generalMessage";
			this.generalMessage.ReadOnly = true;
			this.generalMessage.Size = new System.Drawing.Size(372, 58);
			this.generalMessage.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 15);
			this.label5.TabIndex = 3;
			this.label5.Text = "Help Link:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "Target Method:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 15);
			this.label3.TabIndex = 1;
			this.label3.Text = "Source:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 15);
			this.label2.TabIndex = 0;
			this.label2.Text = "Message:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.stackTrace);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(494, 152);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Stack Trace";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// stackTrace
			// 
			this.stackTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stackTrace.Location = new System.Drawing.Point(3, 3);
			this.stackTrace.Multiline = true;
			this.stackTrace.Name = "stackTrace";
			this.stackTrace.ReadOnly = true;
			this.stackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.stackTrace.Size = new System.Drawing.Size(488, 146);
			this.stackTrace.TabIndex = 0;
			this.stackTrace.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.treeView1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(494, 152);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Inner Exception";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(3, 3);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(488, 146);
			this.treeView1.TabIndex = 0;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.listView1);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(494, 152);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Other";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.KeyHeader,
            this.ValueHeader});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(3, 3);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(488, 146);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// KeyHeader
			// 
			this.KeyHeader.Text = "Key";
			this.KeyHeader.Width = 152;
			// 
			// ValueHeader
			// 
			this.ValueHeader.Text = "Value";
			this.ValueHeader.Width = 331;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(15, 216);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 5;
			this.button2.Text = "Save";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// ExceptionDialog
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(529, 248);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.exceptionName);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ExceptionDialog";
			this.Text = "Uh oh!";
			this.Load += new System.EventHandler(this.ExceptionDialog_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox exceptionName;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.TextBox generalHelpLink;
		private System.Windows.Forms.TextBox generalTargetMethod;
		private System.Windows.Forms.TextBox generalSource;
		private System.Windows.Forms.TextBox generalMessage;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox stackTrace;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader KeyHeader;
		private System.Windows.Forms.ColumnHeader ValueHeader;
	}
}