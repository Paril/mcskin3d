using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCSkin3D.Swatches
{
	public interface ISwatch : IList<NamedColor>
	{
		string Name { get; }
		string FilePath { get; }

		void Save();
		void Load();
	}
}
