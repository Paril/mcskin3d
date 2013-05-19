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

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Paril.Extensions
{
	public static class Extensions
	{
		public static FileInfo CopyToParent(this FileInfo me, string newName)
		{
			return me.CopyTo(me.Directory.FullName + '\\' + newName);
		}

		public static void MoveToParent(this FileInfo me, string newName)
		{
			me.MoveTo(me.Directory.FullName + '\\' + newName);
		}

		public static TreeNodeCollection GetParentCollection(this TreeNode node)
		{
			if (node.Parent == null)
				return node.TreeView.Nodes;
			return node.Parent.Nodes;
		}

		public static List<TreeNode> GetNodeChain(this TreeNode node)
		{
			var nodes = new List<TreeNode>();

			for (TreeNode n = node; n != null; n = n.Parent)
				nodes.Add(n);

			return nodes;
		}
	}
}