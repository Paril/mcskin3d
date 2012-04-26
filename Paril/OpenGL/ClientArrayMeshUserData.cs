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
		public Vector3[] VerticeArray;
		public Vector2[] TexCoordArray;
		public Color4[] ColorArray;
		public int[] IndiceArray;
	}
}
