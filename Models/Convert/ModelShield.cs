#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelShield : ModelBase
	{
		public ModelRenderer field_187063_a;
		public ModelRenderer field_187064_b;

		public ModelShield()
		{
			this.textureWidth = 64;
			this.textureHeight = 64;
			this.field_187063_a = new ModelRenderer(this, 0, 0);
			this.field_187063_a.addBox(-6.0F, -11.0F, -2.0F, 12, 22, 1, 0.0F, "Shield");
			this.field_187064_b = new ModelRenderer(this, 26, 0);
			this.field_187064_b.addBox(-1.0F, -3.0F, -1.0F, 2, 6, 6, 0.0F, "Handle");
		}
	}
}
#endif