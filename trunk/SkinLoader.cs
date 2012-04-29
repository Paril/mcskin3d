using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace MCSkin3D
{
	public static class SkinLoader
	{
		static TreeNode _tempToSelect;

		static void RecurseAddDirectories(string path, IList nodes, List<Skin> skins)
		{
			var di = new DirectoryInfo(path);

			foreach (FileInfo file in di.GetFiles("*.png", SearchOption.TopDirectoryOnly))
			{
				var skin = new Skin(file);
				nodes.Add(skin);

				if (_tempToSelect == null)
					_tempToSelect = skin;
				else if (GlobalSettings.LastSkin == skin.File.ToString())
					_tempToSelect = skin;

				skins.Add(skin);
			}

			foreach (DirectoryInfo dir in di.GetDirectories())
			{
				if ((dir.Attributes & FileAttributes.Hidden) != 0)
					continue;

				var folderNode = new FolderNode(dir.FullName);
				RecurseAddDirectories(dir.FullName, folderNode.Nodes, skins);
				nodes.Add(folderNode);
			}
		}

		public static void LoadSkins()
		{
			var skins = new List<Skin>();
			var rootNodes = new List<TreeNode>();

			if (Editor.HasOneRoot)
				RecurseAddDirectories(Editor.RootFolderString, rootNodes, skins);
			else
			{
				foreach (string x in GlobalSettings.SkinDirectories)
				{
					var folder = new FolderNode(x);
					RecurseAddDirectories(x, folder.Nodes, skins);
					rootNodes.Add(folder);
				}
			}

			Editor.MainForm.Invoke((Action<List<Skin>, List<TreeNode>, TreeNode>)Editor.MainForm.FinishedLoadingSkins, skins, rootNodes, _tempToSelect);
		}
	}
}
