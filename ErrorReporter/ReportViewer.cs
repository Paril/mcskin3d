using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCSkin3D.ExceptionHandler;

namespace ServerTest
{
	public partial class ReportViewer : UserControl
	{
		public ReportViewer()
		{
			InitializeComponent();
		}

		class NodeBuilder
		{
			TreeNode _node;

			private NodeBuilder() { }

			public static NodeBuilder MakeNode(string text, object tag = null, int image = 0)
			{
				NodeBuilder builder = new NodeBuilder();

				builder._node = new TreeNode();
				builder._node.Text = text;
				builder._node.Tag = tag;
				builder._node.ImageIndex = builder._node.SelectedImageIndex = image;
				
				return builder;
			}

			public NodeBuilder AddChild(TreeNode node)
			{
				_node.Nodes.Add(node);
				NodeBuilder nb = new NodeBuilder();
				nb._node = node;
				return nb;
			}

			public NodeBuilder AddChild(string text, object tag = null, int image = 0)
			{
				return AddChild(MakeNode(text, tag, image));
			}

			public NodeBuilder AddChildren<T>(IEnumerable<T> objects, Func<T, NodeBuilder> func)
			{
				foreach (var c in objects)
					AddChild(func(c));

				return this;
			}

			public static implicit operator TreeNode(NodeBuilder builder)
			{
				for (var node = builder._node; ; node = node.Parent)
					if (node.Parent == null)
						return node;
			}
		}

		public void Populate(ReportWrapper report)
		{
			treeView1.BeginUpdate();

			treeView1.Nodes.Clear();

			treeView1.Nodes.Add
				(
					NodeBuilder.MakeNode("Date")
						.AddChild(report.Received.ToString())
				);

			treeView1.Nodes.Add
				(
					NodeBuilder.MakeNode("Name")
						.AddChild(report.Report.Name)
				);

			treeView1.Nodes.Add
				(
					NodeBuilder.MakeNode("Email")
						.AddChild(report.Report.Email)
				);

			treeView1.Nodes.Add
				(
					NodeBuilder.MakeNode("Description")
						.AddChild(report.Report.Description)
				);

			treeView1.Nodes.Add(
				(
					NodeBuilder.MakeNode("Exceptions")
						.AddChildren(report.Report.Data,
							(e) =>
								{
									return NodeBuilder.MakeNode(e.Type)
										.AddChild("Stack Trace")
										.AddChildren(e.Frames,
											(frame) =>
											{
												var node = NodeBuilder.MakeNode(frame.MethodType + " :: " + frame.Method);

												if (!string.IsNullOrEmpty(frame.FileName))
													node.AddChild(frame.FileName + " : " + frame.FileLine + "," + frame.FileColumn);

												return node;
											});
								})
				));

			treeView1.EndUpdate();
		}
	}
}
