using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;

namespace Paril.Settings.Serializers
{
	public class EnumSerializer<E> : ITypeSerializer
	{
		public string Serialize(object obj)
		{
			return ((int)obj).ToString();
		}
		
		public object Deserialize(string str)
		{
			return Enum.Parse(typeof(E), str);
		}
	}
}
