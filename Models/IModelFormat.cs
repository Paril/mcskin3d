using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paril.OpenGL;

namespace MCSkin3D.Models
{
	public interface IModelFormat
	{
		Model Load(string fileName);
	}
}
