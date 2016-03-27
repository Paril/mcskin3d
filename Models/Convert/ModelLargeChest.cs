#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public class ModelLargeChest : ModelChest
	{
		public ModelLargeChest()
		{
			this.boxList.Remove(this.chestLid);
			this.boxList.Remove(this.chestKnob);
			this.boxList.Remove(this.chestBelow);

			this.chestLid = (new ModelRenderer(this, 0, 0)).setTextureSize(128, 64);
			this.chestLid.addBox(0.0F, -5.0F, -14.0F, 30, 5, 14, 0.0F, "Lid");
			this.chestLid.rotationPointX = 1.0F;
			this.chestLid.rotationPointY = 7.0F;
			this.chestLid.rotationPointZ = 15.0F;
			this.chestKnob = (new ModelRenderer(this, 0, 0)).setTextureSize(128, 64);
			this.chestKnob.addBox(-1.0F, -2.0F, -15.0F, 2, 4, 1, 0.0F, "Knob");
			this.chestKnob.rotationPointX = 16.0F;
			this.chestKnob.rotationPointY = 7.0F;
			this.chestKnob.rotationPointZ = 15.0F;
			this.chestBelow = (new ModelRenderer(this, 0, 19)).setTextureSize(128, 64);
			this.chestBelow.addBox(0.0F, 0.0F, 0.0F, 30, 10, 14, 0.0F, "Below");
			this.chestBelow.rotationPointX = 1.0F;
			this.chestBelow.rotationPointY = 6.0F;
			this.chestBelow.rotationPointZ = 1.0F;
		}
	}
}
#endif