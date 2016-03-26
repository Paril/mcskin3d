using System;
using System.Windows.Forms;
using MCSkin3D.Languages;
using Szotar.WindowsForms;

namespace MCSkin3D.Controls
{
	public class SkinTreeViewToolBar : NativeToolStrip
	{
		public ToolStripButton TreeZoomOutToolStripButton { get; private set; }
		public ToolStripButton TreeZoomInToolStripButton { get; private set; }

		public ToolStripButton ImportToolStripButton { get; private set; }
		public ToolStripSplitButton NewSkinToolStripButton { get; private set; }
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
			LanguageProvider.BeginInit();

			TreeZoomOutToolStripButton = new ToolStripButton();
			TreeZoomInToolStripButton = new ToolStripButton();

			ImportToolStripButton = new ToolStripButton();
			NewSkinToolStripButton = new ToolStripSplitButton();
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
			Items.AddRange(new ToolStripItem[] {
				TreeZoomOutToolStripButton,
				TreeZoomInToolStripButton,
				new ToolStripSeparator(),
				ImportToolStripButton,
				NewSkinToolStripButton,
				NewFolderToolStripButton,
				new ToolStripSeparator(),
				RenameToolStripButton,
				DeleteToolStripButton,
				CloneToolStripButton,
				new ToolStripSeparator(),
				DecResToolStripButton,
				IncResToolStripButton,
				new ToolStripSeparator(),
				FetchToolStripButton});
			// 
			// treeZoomOutToolStripButton
			// 
			TreeZoomOutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			TreeZoomOutToolStripButton.Image = global::MCSkin3D.Properties.Resources.ZoomOutHS;
			TreeZoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			TreeZoomOutToolStripButton.Name = "treeZoomOutToolStripButton";
			LanguageProvider.SetPropertyNames(TreeZoomOutToolStripButton, "Text");
			TreeZoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
			TreeZoomOutToolStripButton.Text = "T_TREE_ZOOMOUT";
			TreeZoomOutToolStripButton.Click += new System.EventHandler(treeZoomOutToolStripButton_Click);
			// 
			// treeZoomInToolStripButton
			// 
			TreeZoomInToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			TreeZoomInToolStripButton.Image = global::MCSkin3D.Properties.Resources.ZoomInHS;
			TreeZoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			TreeZoomInToolStripButton.Name = "treeZoomInToolStripButton";
			LanguageProvider.SetPropertyNames(TreeZoomInToolStripButton, "Text");
			TreeZoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
			TreeZoomInToolStripButton.Text = "T_TREE_ZOOMIN";
			TreeZoomInToolStripButton.Click += new System.EventHandler(treeZoomInToolStripButton_Click);
			// 
			// importToolStripButton
			// 
			ImportToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ImportToolStripButton.Image = global::MCSkin3D.Properties.Resources._112_ArrowCurve_Blue_Left_16x16_72;
			ImportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			ImportToolStripButton.Name = "importToolStripButton";
			LanguageProvider.SetPropertyNames(ImportToolStripButton, "Text");
			ImportToolStripButton.Size = new System.Drawing.Size(23, 22);
			ImportToolStripButton.Text = "T_TREE_IMPORTHERE";
			ImportToolStripButton.Click += new System.EventHandler(importToolStripButton_Click);
			// 
			// newSkinToolStripButton
			// 
			NewSkinToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			NewSkinToolStripButton.Image = global::MCSkin3D.Properties.Resources.newskin;
			NewSkinToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			NewSkinToolStripButton.Name = "newSkinToolStripButton";
			LanguageProvider.SetPropertyNames(NewSkinToolStripButton, "Text");
			NewSkinToolStripButton.Size = new System.Drawing.Size(23, 22);
			NewSkinToolStripButton.Text = "M_NEWSKIN_HERE";
			NewSkinToolStripButton.ButtonClick += new System.EventHandler(newSkinToolStripButton_Click);
			// 
			// newFolderToolStripButton
			// 
			NewFolderToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			NewFolderToolStripButton.Image = global::MCSkin3D.Properties.Resources.NewFolderHS;
			NewFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			NewFolderToolStripButton.Name = "newFolderToolStripButton";
			LanguageProvider.SetPropertyNames(NewFolderToolStripButton, "Text");
			NewFolderToolStripButton.Size = new System.Drawing.Size(23, 22);
			NewFolderToolStripButton.Text = "T_TREE_NEWFOLDER";
			NewFolderToolStripButton.Click += new System.EventHandler(newFolderToolStripButton_Click);
			// 
			// renameToolStripButton
			// 
			RenameToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			RenameToolStripButton.Image = global::MCSkin3D.Properties.Resources.Rename;
			RenameToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			RenameToolStripButton.Name = "renameToolStripButton";
			LanguageProvider.SetPropertyNames(RenameToolStripButton, "Text");
			RenameToolStripButton.Size = new System.Drawing.Size(23, 22);
			RenameToolStripButton.Text = "T_TREE_RENAME";
			RenameToolStripButton.Click += new System.EventHandler(renameToolStripButton_Click);
			// 
			// deleteToolStripButton
			// 
			DeleteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			DeleteToolStripButton.Image = global::MCSkin3D.Properties.Resources.delete;
			DeleteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			DeleteToolStripButton.Name = "deleteToolStripButton";
			LanguageProvider.SetPropertyNames(DeleteToolStripButton, "Text");
			DeleteToolStripButton.Size = new System.Drawing.Size(23, 22);
			DeleteToolStripButton.Text = "T_TREE_DELETE";
			DeleteToolStripButton.Click += new System.EventHandler(deleteToolStripButton_Click);
			// 
			// cloneToolStripButton
			// 
			CloneToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			CloneToolStripButton.Image = global::MCSkin3D.Properties.Resources.clone;
			CloneToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			CloneToolStripButton.Name = "cloneToolStripButton";
			LanguageProvider.SetPropertyNames(CloneToolStripButton, "Text");
			CloneToolStripButton.Size = new System.Drawing.Size(23, 22);
			CloneToolStripButton.Text = "T_TREE_CLONE";
			CloneToolStripButton.Click += new System.EventHandler(cloneToolStripButton_Click);
			// 
			// decResToolStripButton
			// 
			DecResToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			DecResToolStripButton.Image = global::MCSkin3D.Properties.Resources.incres;
			DecResToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			DecResToolStripButton.Name = "decResToolStripButton";
			LanguageProvider.SetPropertyNames(DecResToolStripButton, "Text");
			DecResToolStripButton.Size = new System.Drawing.Size(23, 22);
			DecResToolStripButton.Text = "T_DECRES";
			DecResToolStripButton.Click += new System.EventHandler(decResToolStripButton_Click);
			// 
			// incResToolStripButton
			// 
			IncResToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			IncResToolStripButton.Image = global::MCSkin3D.Properties.Resources.decres;
			IncResToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			IncResToolStripButton.Name = "incResToolStripButton";
			LanguageProvider.SetPropertyNames(IncResToolStripButton, "Text");
			IncResToolStripButton.Size = new System.Drawing.Size(23, 22);
			IncResToolStripButton.Text = "T_INCRES";
			IncResToolStripButton.Click += new System.EventHandler(incResToolStripButton_Click);
			// 
			// fetchToolStripButton
			// 
			FetchToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			FetchToolStripButton.Image = global::MCSkin3D.Properties.Resources.import_from_mc;
			FetchToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			FetchToolStripButton.Name = "fetchToolStripButton";
			LanguageProvider.SetPropertyNames(FetchToolStripButton, "Text");
			FetchToolStripButton.Size = new System.Drawing.Size(23, 22);
			FetchToolStripButton.Text = "M_FETCH_NAME";
			FetchToolStripButton.Click += new System.EventHandler(fetchToolStripButton_Click);

			LanguageProvider.BaseControl = this;
			LanguageProvider.EndInit();
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
