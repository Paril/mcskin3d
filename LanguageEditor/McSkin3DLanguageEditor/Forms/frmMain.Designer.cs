namespace MCSkin3DLanguageEditor
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.lvItems = new MCSkin3DLanguageEditor.Jonas.fixListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvItemsOthers = new MCSkin3DLanguageEditor.Jonas.fixListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pNew = new MCSkin3DLanguageEditor.Jonas.fixPanel();
            this.linkNew = new MCSkin3DLanguageEditor.Jonas.fixLinkLabel();
            this.pSave = new MCSkin3DLanguageEditor.Jonas.fixPanel();
            this.pOpen = new MCSkin3DLanguageEditor.Jonas.fixPanel();
            this.linkSave = new MCSkin3DLanguageEditor.Jonas.fixLinkLabel();
            this.linkOpenFile = new MCSkin3DLanguageEditor.Jonas.fixLinkLabel();
            this.pTop.SuspendLayout();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pTop
            // 
            this.pTop.BackColor = System.Drawing.SystemColors.Control;
            this.pTop.Controls.Add(this.pNew);
            this.pTop.Controls.Add(this.linkNew);
            this.pTop.Controls.Add(this.pSave);
            this.pTop.Controls.Add(this.pOpen);
            this.pTop.Controls.Add(this.linkSave);
            this.pTop.Controls.Add(this.linkOpenFile);
            this.pTop.Controls.Add(this.label1);
            this.pTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pTop.Location = new System.Drawing.Point(0, 0);
            this.pTop.Name = "pTop";
            this.pTop.Size = new System.Drawing.Size(560, 62);
            this.pTop.TabIndex = 1;
            this.pTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pTop_Paint);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(451, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Welcome to the official language file editor for MCSkin3D.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 62);
            this.spcMain.Name = "spcMain";
            this.spcMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.lvItems);
            // 
            // spcMain.Panel2
            // 
            this.spcMain.Panel2.Controls.Add(this.lvItemsOthers);
            this.spcMain.Size = new System.Drawing.Size(560, 480);
            this.spcMain.SplitterDistance = 163;
            this.spcMain.TabIndex = 15;
            // 
            // lvItems
            // 
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItems.FullRowSelect = true;
            this.lvItems.Location = new System.Drawing.Point(0, 0);
            this.lvItems.MultiSelect = false;
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(560, 163);
            this.lvItems.TabIndex = 13;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 158;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 267;
            // 
            // lvItemsOthers
            // 
            this.lvItemsOthers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvItemsOthers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItemsOthers.FullRowSelect = true;
            this.lvItemsOthers.Location = new System.Drawing.Point(0, 0);
            this.lvItemsOthers.MultiSelect = false;
            this.lvItemsOthers.Name = "lvItemsOthers";
            this.lvItemsOthers.Size = new System.Drawing.Size(560, 313);
            this.lvItemsOthers.TabIndex = 15;
            this.lvItemsOthers.UseCompatibleStateImageBehavior = false;
            this.lvItemsOthers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 158;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Value";
            this.columnHeader4.Width = 267;
            // 
            // pNew
            // 
            this.pNew.BackgroundImage = global::MCSkin3DLanguageEditor.Properties.Resources.new_doc;
            this.pNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pNew.Location = new System.Drawing.Point(11, 34);
            this.pNew.Name = "pNew";
            this.pNew.Size = new System.Drawing.Size(16, 16);
            this.pNew.TabIndex = 7;
            this.pNew.Click += new System.EventHandler(this.pNew_Click);
            // 
            // linkNew
            // 
            this.linkNew.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(14)))), ((int)(((byte)(151)))));
            this.linkNew.AutoSize = true;
            this.linkNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkNew.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkNew.Location = new System.Drawing.Point(26, 36);
            this.linkNew.Name = "linkNew";
            this.linkNew.Size = new System.Drawing.Size(86, 13);
            this.linkNew.TabIndex = 6;
            this.linkNew.TabStop = true;
            this.linkNew.Text = "New File (Ctrl+N)";
            this.linkNew.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkNew.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOpen_LinkClicked);
            // 
            // pSave
            // 
            this.pSave.BackgroundImage = global::MCSkin3DLanguageEditor.Properties.Resources.saveHS;
            this.pSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pSave.Enabled = false;
            this.pSave.Location = new System.Drawing.Point(234, 35);
            this.pSave.Name = "pSave";
            this.pSave.Size = new System.Drawing.Size(16, 16);
            this.pSave.TabIndex = 5;
            this.pSave.Click += new System.EventHandler(this.pSave_Click);
            // 
            // pOpen
            // 
            this.pOpen.BackgroundImage = global::MCSkin3DLanguageEditor.Properties.Resources._112_ArrowCurve_Blue_Left_16x16_72;
            this.pOpen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pOpen.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pOpen.Location = new System.Drawing.Point(122, 35);
            this.pOpen.Name = "pOpen";
            this.pOpen.Size = new System.Drawing.Size(16, 16);
            this.pOpen.TabIndex = 4;
            this.pOpen.Click += new System.EventHandler(this.pOpen_Click);
            // 
            // linkSave
            // 
            this.linkSave.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(14)))), ((int)(((byte)(151)))));
            this.linkSave.AutoSize = true;
            this.linkSave.Enabled = false;
            this.linkSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkSave.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkSave.Location = new System.Drawing.Point(249, 36);
            this.linkSave.Name = "linkSave";
            this.linkSave.Size = new System.Drawing.Size(88, 13);
            this.linkSave.TabIndex = 3;
            this.linkSave.TabStop = true;
            this.linkSave.Text = "Save File (Ctrl+S)";
            this.linkSave.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkSave.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSave_LinkClicked);
            // 
            // linkOpenFile
            // 
            this.linkOpenFile.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(14)))), ((int)(((byte)(151)))));
            this.linkOpenFile.AutoSize = true;
            this.linkOpenFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkOpenFile.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkOpenFile.Location = new System.Drawing.Point(137, 37);
            this.linkOpenFile.Name = "linkOpenFile";
            this.linkOpenFile.Size = new System.Drawing.Size(90, 13);
            this.linkOpenFile.TabIndex = 2;
            this.linkOpenFile.TabStop = true;
            this.linkOpenFile.Text = "Open File (Ctrl+O)";
            this.linkOpenFile.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(112)))), ((int)(((byte)(238)))));
            this.linkOpenFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkOpenFile_LinkClicked);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(560, 542);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.pTop);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(400, 450);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MCSkin3D - Language Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.pTop.ResumeLayout(false);
            this.pTop.PerformLayout();
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            this.spcMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pTop;
        private System.Windows.Forms.Label label1;
        private Jonas.fixLinkLabel linkOpenFile;
        private Jonas.fixLinkLabel linkSave;
        private Jonas.fixPanel pOpen;
        private Jonas.fixPanel pSave;
        private Jonas.fixPanel pNew;
        private Jonas.fixLinkLabel linkNew;
        private System.Windows.Forms.SplitContainer spcMain;
        private Jonas.fixListView lvItems;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private Jonas.fixListView lvItemsOthers;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;

    }
}

