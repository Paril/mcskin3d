using Paril.OpenGL;

namespace MCSkin3D.Models
{
	public interface IModelFormat
	{
		Model Load(string fileName);
	}
}