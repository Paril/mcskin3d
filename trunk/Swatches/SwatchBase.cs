using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;

namespace MCSkin3D.Swatches
{
	public abstract class SwatchBase : ISwatch
	{
		public abstract string Name { get; protected set; }
		public abstract string FilePath { get; protected set; }

		public abstract void Save();
		public abstract void Load();

		// Operations not supported by this interface.
		// These are because dupe colors are allowed.
#region Not supported
		int IList<NamedColor>.IndexOf(NamedColor color)
		{
			throw new InvalidOperationException();
		}

		bool ICollection<NamedColor>.Contains(NamedColor color)
		{
			throw new InvalidOperationException();
		}

		bool ICollection<NamedColor>.Remove(NamedColor color)
		{
			throw new InvalidOperationException();
		}

		bool ICollection<NamedColor>.IsReadOnly
		{
			get { return false; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
#endregion

		// Operations implementors must implement.
		public abstract void Insert(int index, NamedColor color);
		public abstract void RemoveAt(int index);

		public abstract NamedColor this[int index]
		{
			get;
			set;
		}

		public abstract void Add(NamedColor color);
		public abstract void Clear();
		public abstract void CopyTo(NamedColor[] array, int start);

		public abstract int Count
		{
			get;
		}

		public abstract IEnumerator<NamedColor> GetEnumerator();

		public override string ToString()
		{
			return Name;
		}
	}
}
