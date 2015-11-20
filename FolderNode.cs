//
//    MCSkin3D, a 3d skin management studio for Minecraft
//    Copyright (C) 2013 Altered Softworks & MCSkin3D Team
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace MCSkin3D
{
	[Serializable]
	public class FolderNode : TreeNode
	{
		public FolderNode(string name)
		{
			Name = Text = name;
		}

		string TransformedPath
		{
			get
			{
				if (Parent == null)
				{
					if (Editor.HasOneRoot)
						return Editor.RootFolderString + Text + '\\';

					return Environment.CurrentDirectory + Text + '\\';
				}

				return (Parent as FolderNode).TransformedPath + Text + '\\';
			}
		}

		public DirectoryInfo Directory
		{
			get { return new DirectoryInfo(TransformedPath); }
		}

		public override string ToString()
		{
			return Name;
		}

		public void MoveTo(string newFolderString)
		{
			string folderName = Text;

			var newFolder = new DirectoryInfo(newFolderString);

			if (Directory.FullName == newFolder.FullName)
				return;

			while (System.IO.Directory.Exists(newFolder.FullName))
			{
				newFolderString += " - " + Editor.GetLanguageString("C_MOVED");
				newFolder = new DirectoryInfo(newFolderString);
			}

			FileSystem.MoveDirectory(Directory.FullName, newFolder.FullName);
			//Directory.MoveTo(newFolder.FullName);
			Text = Name = newFolder.Name;
		}
	}
}