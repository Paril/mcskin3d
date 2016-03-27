#if CONVERT_MODELS
using System;

namespace MCSkin3D.Models.Convert
{
	// all the java compatibility stuff
	public struct boolean
	{
		bool value;

		public boolean(bool v) :
			this()
		{
			value = v;
		}

		public static implicit operator bool(boolean b)
		{
			return b.value;
		}

		public static implicit operator boolean(bool b)
		{
			return new boolean(b);
		}

		public override string ToString()
		{
			return value.ToString();
		}
	}

	public class List<T> : System.Collections.Generic.List<T>
	{
		public T get(int index)
		{
			return this[index];
		}

		public int size()
		{
			return Count;
		}

		public void add(T value)
		{
			Add(value);
		}
	}

	public class ArrayList<T> : List<T>
	{
	}
	
	public class Map<K, V> : System.Collections.Generic.Dictionary<K, V>
	{
		public V get(K key)
		{
			return this[key];
		}

		public void put(K key, V value)
		{
			Add(key, value);
		}
	}

	public class HashMap<K, V> : Map<K, V>
	{
	}

	public static class Lists<T>
	{
		public static List<T> newArrayList()
		{
			return new ArrayList<T>();
		}
	}

	public static class Maps<K, V>
	{
		public static Map<K, V> newHashMap()
		{
			return new HashMap<K, V>();
		}
	}

	public class Entity
	{
		internal double motionX;
		internal double motionY;
		internal double motionZ;
		internal float rotationYaw;
		internal float ticksExisted;

		internal bool isSneaking()
		{
			return false;
		}

		internal int getEntityId()
		{
			return 0;
		}
	}

	public class EntityLivingBase : Entity
	{
		internal float prevRenderYawOffset;
		internal float renderYawOffset;
		internal float prevRotationYawHead;
		internal float rotationYawHead;
		internal int rotationPitch;
		internal float prevRotationPitch;
		internal float ticksExisted;

		internal bool func_184613_cA()
		{
			return false;
		}

		internal int func_184599_cB()
		{
			return 0;
		}

		internal EnumHandSide getPrimaryHand()
		{
			return EnumHandSide.RIGHT;
		}
	}

	public class Random
	{
		System.Random _random;

		public Random()
		{
			_random = new System.Random();
		}

		public Random(long r)
		{
			_random = new System.Random((int)r);
		}

		public int nextInt()
		{
			return _random.Next();
		}

		public int nextInt(int max)
		{
			return _random.Next(max);
		}

		public int nextInt(int min, int max)
		{
			return _random.Next(min, max);
		}
	}

	public struct Vec3d
	{
		OpenTK.Vector3d _vec;

		public Vec3d(double x, double y, double z) :
			this()
		{
			_vec.X = x;
			_vec.Y = y;
			_vec.Z = z;
		}

		internal Vec3d normalize()
		{
			throw new NotImplementedException();
		}

		internal float getX()
		{
			return (float)_vec.X;
		}

		internal float getY()
		{
			return (float)_vec.Y;
		}

		internal float getZ()
		{
			return (float)_vec.Z;
		}

		public double xCoord
		{
			get { return _vec.X; }
			set { _vec.X = value; }
		}

		public double yCoord
		{
			get { return _vec.Y; }
			set { _vec.Y = value; }
		}

		public double zCoord
		{
			get { return _vec.Z; }
			set { _vec.Z = value; }
		}

		public static implicit operator OpenTK.Vector3d(Vec3d val)
		{
			return new OpenTK.Vector3d(val.xCoord, val.yCoord, val.zCoord);
		}

		public static implicit operator Vec3d(OpenTK.Vector3d val)
		{
			return new Vec3d(val.X, val.Y, val.Z);
		}
	}
}
#endif