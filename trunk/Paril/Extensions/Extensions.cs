using System;
using System.IO;

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
	}
}
