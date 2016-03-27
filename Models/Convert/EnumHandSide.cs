#if CONVERT_MODELS
namespace MCSkin3D.Models.Convert
{
	public struct EnumHandSide
	{
		public static readonly EnumHandSide LEFT = 0;
		public static readonly EnumHandSide RIGHT = 1;

		int _value;
		
		public static implicit operator int(EnumHandSide side)
		{
			return side._value;
		}

		public static implicit operator EnumHandSide(int side)
		{
			return new EnumHandSide() { _value = side };
		}

		public EnumHandSide opposite()
		{
			return _value == LEFT ? RIGHT : LEFT;
		}
	}
}
#endif