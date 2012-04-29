using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace Paril.OpenGL
{
	public class ClientArrayMeshUserData : IMeshUserData
	{
		public float[] VerticeArray;
		public float[] TexCoordArray;
		public float[] ColorArray;
		public byte[] IndiceArray;
	}
}
