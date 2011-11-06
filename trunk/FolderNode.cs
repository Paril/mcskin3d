using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MCSkin3D
{
	[Serializable]
	public class FolderNode : TreeNode
	{
		public FolderNode(string name) :
			base(name)
		{
		}

		public DirectoryInfo Directory
		{
			get
			{
				return new DirectoryInfo("Skins\\" + FullPath);
			}
		}
	}
}
