using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

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
			List<TreeNode> nodes = new List<TreeNode>();

			for (var n = node; n != null; n = n.Parent)
				nodes.Add(n);

			return nodes;
		}
	}
}
