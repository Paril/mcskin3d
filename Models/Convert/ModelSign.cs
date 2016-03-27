#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelSign : ModelBase
	{
		/** The board on a sign that has the writing on it. */
		public ModelRenderer signBoard;

		/** The stick a sign stands on. */
		public ModelRenderer signStick;

		public ModelSign()
		{
			this.signBoard = new ModelRenderer(this, 0, 0);
			this.signBoard.addBox(-12.0F, -14.0F, -1.0F, 24, 12, 2, 0.0F, "Board");
			this.signStick = new ModelRenderer(this, 0, 14);
			this.signStick.addBox(-1.0F, -2.0F, -1.0F, 2, 14, 2, 0.0F, "Stick");
		}

#if RENDER
		/**
		 * Renders the sign model through TileEntitySignRenderer
		 */
		public void renderSign()
		{
			this.signBoard.render(0.0625F);
			this.signStick.render(0.0625F);
		}
#endif
	}
}
#endif