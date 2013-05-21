using OpenTK;
using System.Drawing;

namespace Paril.OpenGL
{
	public struct Face
	{
		public bool Downface;
		public int[] Indices;
		public Vector3 Normal;
		public Vector2[] TexCoords;
		public Vector3[] Vertices;

		public Face(Vector3[] vertices, Vector2[] texCoords, int[] indices) :
			this()
		{
			Vertices = vertices;
			TexCoords = texCoords;
			Indices = indices;
		}

		public Rectangle TexCoordsToInteger(int width, int height)
		{
			float minX = 1, minY = 1, maxX = 0, maxY = 0;

			foreach (Vector2 x in TexCoords)
			{
				if (x.X < minX)
					minX = x.X;
				if (x.X > maxX)
					maxX = x.X;

				if (x.Y < minY)
					minY = x.Y;
				if (x.Y > maxY)
					maxY = x.Y;
			}

			return new Rectangle((int)(minX * width), (int)(minY * height), (int)((maxX - minX) * width),
								 (int)((maxY - minY) * height));
		}

		public RectangleF TexCoordsToFloat(int width, int height)
		{
			float minX = 1, minY = 1, maxX = 0, maxY = 0;

			foreach (Vector2 x in TexCoords)
			{
				if (x.X < minX)
					minX = x.X;
				if (x.X > maxX)
					maxX = x.X;

				if (x.Y < minY)
					minY = x.Y;
				if (x.Y > maxY)
					maxY = x.Y;
			}

			return new RectangleF((minX * width), (minY * height), ((maxX - minX) * width), ((maxY - minY) * height));
		}
	}
}
