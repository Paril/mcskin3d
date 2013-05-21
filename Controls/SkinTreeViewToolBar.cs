using MCSkin3D.Languages;
using System;
using System.Windows.Forms;
using Szotar.WindowsForms;

namespace MCSkin3D.Controls
{
	public class SkinTreeViewToolBar : NativeToolStrip
	{
		public ToolStripButton TreeZoomOutToolStripButton { get; private set; }
		public ToolStripButton TreeZoomInToolStripButton { get; private set; }

		public ToolStripButton ImportToolStripButton { get; private set; }
		public ToolStripButton NewSkinToolStripButton { get; private set; }
		public ToolStripButton NewFolderToolStripButton { get; private set; }

		public ToolStripButton RenameToolStripButton { get; private set; }
		public ToolStripButton DeleteToolStripButton { get; private set; }
		public ToolStripButton CloneToolStripButton { get; private set; }

		public ToolStripButton DecResToolStripButton { get; private set; }
		public ToolStripButton IncResToolStripButton { get; private set; }

		public ToolStripButton FetchToolStripButton { get; private set; }
		public LanguageProvider LanguageProvider { get; private set; }
		
		public SkinTreeViewToolBar()
		{
			LanguageProvider = new LanguageProvider();
			TreeZoomOutToolStripButton = new ToolStripButton();
			TreeZoomInToolStripButton = new ToolStripButton();

			ImportToolStripButton = new ToolStripButton();
			NewSkinToolStripButton = new ToolStripButton();
			NewFolderToolStripButton = new ToolStripButton();

			RenameToolStripButton = new ToolStripButton();
			DeleteToolStripButton = new ToolStripButton();
			CloneToolStripButton = new ToolStripButton();

			DecResToolStripButton = new ToolStripButton();
			IncResToolStripButton = new ToolStripButton();

			FetchToolStripButton = new ToolStripButton();

			// 
			// toolStrip2
			// 
			this.Items.AddRange(new ToolStripItem[] {
				this.TreeZoomOutToolStripButton,
				this.TreeZoomInToolStripButton,
				new ToolStripSeparator(),
				this.ImportToolStripButton,
				this.NewSkinToolStripButton,
				this.NewFolderToolStripButton,
				new ToolStripSeparator(),
				this.RenameToolStripButton,
				this.DeleteToolStripButton,
				this.CloneToolStripButton,
				new ToolStripSeparator(),
				this.DecResToolStripButton,
				this.IncResToolStripButton,
				new ToolStripSeparator(),
				this.FetchToolStripButton});
			// 
			// treeZoomOutToolStripButton
			// 
			this.TreeZoomOutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TreeZoomOutToolStripButton.Image = global::MCSkin3D.Properties.Resources.ZoomOutHS;
			this.TreeZoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.TreeZoomOutToolStripButton.Name = "treeZoomOutToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.TreeZoomOutToolStripButton, "Text");
			this.TreeZoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.TreeZoomOutToolStripButton.Text = "T_TREE_ZOOMOUT";
			this.TreeZoomOutToolStripButton.Click += new System.EventHandler(this.treeZoomOutToolStripButton_Click);
			// 
			// treeZoomInToolStripButton
			// 
			this.TreeZoomInToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.TreeZoomInToolStripButton.Image = global::MCSkin3D.Properties.Resources.ZoomInHS;
			this.TreeZoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			this.TreeZoomInToolStripButton.Name = "treeZoomInToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.TreeZoomInToolStripButton, "Text");
			this.TreeZoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.TreeZoomInToolStripButton.Text = "T_TREE_ZOOMIN";
			this.TreeZoomInToolStripButton.Click += new System.EventHandler(this.treeZoomInToolStripButton_Click);
			// 
			// importToolStripButton
			// 
			this.ImportToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.ImportToolStripButton.Image = global::MCSkin3D.Properties.Resources._112_ArrowCurve_Blue_Left_16x16_72;
			this.ImportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ImportToolStripButton.Name = "importToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.ImportToolStripButton, "Text");
			this.ImportToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.ImportToolStripButton.Text = "T_TREE_IMPORTHERE";
			this.ImportToolStripButton.Click += new System.EventHandler(this.importToolStripButton_Click);
			// 
			// newSkinToolStripButton
			// 
			this.NewSkinToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.NewSkinToolStripButton.Image = global::MCSkin3D.Properties.Resources.newskin;
			this.NewSkinToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NewSkinToolStripButton.Name = "newSkinToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.NewSkinToolStripButton, "Text");
			this.NewSkinToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.NewSkinToolStripButton.Text = "M_NEWSKIN_HERE";
			this.NewSkinToolStripButton.Click += new System.EventHandler(this.newSkinToolStripButton_Click);
			// 
			// newFolderToolStripButton
			// 
			this.NewFolderToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.NewFolderToolStripButton.Image = global::MCSkin3D.Properties.Resources.NewFolderHS;
			this.NewFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NewFolderToolStripButton.Name = "newFolderToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.NewFolderToolStripButton, "Text");
			this.NewFolderToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.NewFolderToolStripButton.Text = "T_TREE_NEWFOLDER";
			this.NewFolderToolStripButton.Click += new System.EventHandler(this.newFolderToolStripButton_Click);
			// 
			// renameToolStripButton
			// 
			this.RenameToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.RenameToolStripButton.Image = global::MCSkin3D.Properties.Resources.Rename;
			this.RenameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.RenameToolStripButton.Name = "renameToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.RenameToolStripButton, "Text");
			this.RenameToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.RenameToolStripButton.Text = "T_TREE_RENAME";
			this.RenameToolStripButton.Click += new System.EventHandler(this.renameToolStripButton_Click);
			// 
			// deleteToolStripButton
			// 
			this.DeleteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.DeleteToolStripButton.Image = global::MCSkin3D.Properties.Resources.delete;
			this.DeleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.DeleteToolStripButton.Name = "deleteToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.DeleteToolStripButton, "Text");
			this.DeleteToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.DeleteToolStripButton.Text = "T_TREE_DELETE";
			this.DeleteToolStripButton.Click += new System.EventHandler(this.deleteToolStripButton_Click);
			// 
			// cloneToolStripButton
			// 
			this.CloneToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.CloneToolStripButton.Image = global::MCSkin3D.Properties.Resources.clone;
			this.CloneToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CloneToolStripButton.Name = "cloneToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.CloneToolStripButton, "Text");
			this.CloneToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.CloneToolStripButton.Text = "T_TREE_CLONE";
			this.CloneToolStripButton.Click += new System.EventHandler(this.cloneToolStripButton_Click);
			// 
			// decResToolStripButton
			// 
			this.DecResToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.DecResToolStripButton.Image = global::MCSkin3D.Properties.Resources.incres;
			this.DecResToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.DecResToolStripButton.Name = "decResToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.DecResToolStripButton, "Text");
			this.DecResToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.DecResToolStripButton.Text = "T_DECRES";
			this.DecResToolStripButton.Click += new System.EventHandler(this.decResToolStripButton_Click);
			// 
			// incResToolStripButton
			// 
			this.IncResToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.IncResToolStripButton.Image = global::MCSkin3D.Properties.Resources.decres;
			this.IncResToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.IncResToolStripButton.Name = "incResToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.IncResToolStripButton, "Text");
			this.IncResToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.IncResToolStripButton.Text = "T_INCRES";
			this.IncResToolStripButton.Click += new System.EventHandler(this.incResToolStripButton_Click);
			// 
			// fetchToolStripButton
			// 
			this.FetchToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.FetchToolStripButton.Image = global::MCSkin3D.Properties.Resources.import_from_mc;
			this.FetchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.FetchToolStripButton.Name = "fetchToolStripButton";
			this.LanguageProvider.SetPropertyNames(this.FetchToolStripButton, "Text");
			this.FetchToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.FetchToolStripButton.Text = "M_FETCH_NAME";
			this.FetchToolStripButton.Click += new System.EventHandler(this.fetchToolStripButton_Click);

			this.LanguageProvider.BaseControl = this;
		}

		private void treeZoomOutToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformTreeViewZoomOut();
		}

		private void treeZoomInToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformTreeViewZoomIn();
		}
		
		private void importToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformImportSkin();
		}

		private void newSkinToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformNewSkin();
		}

		private void newFolderToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformNewFolder();
		}

		private void renameToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformNameChange();
		}

		private void deleteToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformDeleteSkin();
		}

		private void cloneToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformCloneSkin();
		}

		private void decResToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformDecreaseResolution();
		}

		private void incResToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformIncreaseResolution();
		}

		private void fetchToolStripButton_Click(object sender, EventArgs e)
		{
			Editor.MainForm.PerformImportFromSite();
		}
	}
}
