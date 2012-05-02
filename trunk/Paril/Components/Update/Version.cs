using System;

namespace Paril.Components.Update
{
	public struct Version : IComparable<Version>
	{
		public int Build;
		public int Major;
		public int Minor;
		public int Revision;

		public Version(int major = 0, int minor = 0, int revision = 0, int build = 0) :
			this()
		{
			Major = major;
			Minor = minor;
			Revision = revision;
			Build = build;
		}

		public Version(string s) :
			this()
		{
			string[] split = s.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < split.Length; ++i)
				this[i] = int.Parse(split[i]);
		}

		public int this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return Major;
					case 1:
						return Minor;
					case 2:
						return Build;
					case 3:
						return Revision;
				}

				throw new IndexOutOfRangeException();
			}

			set
			{
				switch (index)
				{
					case 0:
						Major = value;
						break;
					case 1:
						Minor = value;
						break;
					case 2:
						Build = value;
						break;
					case 3:
						Revision = value;
						break;
				}
			}
		}

		public override string ToString()
		{
			return Major + "." + Minor + "." + Build + "." + Revision;
		}

		public static Version Parse(string s)
		{
			return new Version(s);
		}

		public static bool operator ==(Version left, Version right)
		{
			return left.Major == right.Major && left.Minor == right.Minor && left.Revision == right.Revision &&
			       left.Build == right.Build;
		}

		public static bool operator !=(Version left, Version right)
		{
			return !(left == right);
		}

		public static bool operator >(Version left, Version right)
		{
			return (left.Major > right.Major) ||
			       (left.Major == right.Major && left.Minor > right.Minor) ||
			       (left.Major == right.Major && left.Minor == right.Minor && left.Revision > right.Revision) ||
			       (left.Major == right.Major && left.Minor == right.Minor && left.Revision == right.Revision &&
			        left.Build > right.Build);
		}

		public static bool operator <(Version left, Version right)
		{
			return left != right && right > left;
		}

		public int CompareTo(Version other)
		{
			if (this < other)
				return -1;
			else if (this > other)
				return 1;

			return 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is Version)
				return this == (Version) obj;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Major.GetHashCode() + Minor.GetHashCode() | Revision.GetHashCode() ^ Build.GetHashCode();
		}
	}
}