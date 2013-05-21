using System;
using System.Collections;
using System.Collections.Generic;

namespace MCSkin3D.Swatches
{
	public abstract class SwatchBase : ISwatch
	{
		#region ISwatch Members

		public abstract string Name { get; set; }
		public abstract string FilePath { get; set; }

		public abstract void Save();
		public abstract void Load();

		// Operations not supported by this interface.
		// These are because dupe colors are allowed.

		// Operations implementors must implement.
		public abstract void Insert(int index, NamedColor color);
		public abstract void RemoveAt(int index);

		public abstract NamedColor this[int index] { get; set; }

		public abstract void Add(NamedColor color);
		public abstract void Clear();
		public abstract void CopyTo(NamedColor[] array, int start);

		public abstract int Count { get; }

		public abstract IEnumerator<NamedColor> GetEnumerator();

		public string Format
		{
			get { return SwatchContainer.GetSwatchFormatName(GetType()); }
		}

		public bool Dirty { get; protected set; }

		#endregion

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

		public override string ToString()
		{
			return Name + " [" + SwatchContainer.GetSwatchFormatName(GetType()) + "]";
		}
	}
}