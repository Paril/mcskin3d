#if CONVERT_MODELS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static MCSkin3D.ModelLoader;

namespace MCSkin3D.Models.Convert
{
	public static class ConversionInterface
	{
		static System.Collections.Generic.List<Tuple<string, ModelBase, string>> _models = new System.Collections.Generic.List<Tuple<string, ModelBase, string>>();
		
		static void Add(string name, ModelBase model, string textureRef = null)
		{
			_models.Add(Tuple.Create(name, model, textureRef));
		}

		static ConversionInterface()
		{
			Add("Entities/Armor Stand", new ModelArmorStand(), "armorstand/wood.png");
			Add("Blocks/Banner", new ModelBanner(), "banner_base.png");
			Add("Mobs/Bat", new ModelBat(), "bat.png");

			{
				var blaze = new ModelBlaze();
				blaze.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Blaze", blaze, "blaze.png");
			}

			{
				var boat = new ModelBoat();
				boat.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Entities/Boat", boat, "boat/boat_oak.png");
			}

			{
				var book = new ModelBook();
				book.setRotationAngles(0, 3, 5, 1, 0, 0, null);
				Add("Blocks/Book", book, "enchanting_table_book.png");
			}

			Add("Blocks/Chest", new ModelChest(), "chest/normal.png");
			Add("Mobs/Chicken", new ModelChicken(), "chicken.png");

			{
				var cow = new ModelCow();
				cow.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Cow", cow, "cow/cow.png");
			}

			Add("Mobs/Creeper", new ModelCreeper(), "creeper/creeper.png");

			{
				var dragon = new ModelDragon(0);
				dragon.render(new EntityDragon(), 0, 0, 0, 0, 0, 0);
				Add("Mobs/Dragon", dragon, "enderdragon/dragon.png");
			}

			Add("Items/Elytra", new ModelElytra(), "elytra.png");
			Add("Entities/Ender Crystal", new ModelEnderCrystal(0, true), "endercrystal/endercrystal.png");

			{
				var enderman = new ModelEnderman(0);
				enderman.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Enderman", enderman, "enderman/enderman.png");
			}

			Add("Mobs/EnderMite", new ModelEnderMite(), "endermite.png");

			{
				var ghast = new ModelGhast();
				ghast.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Ghast", ghast, "ghast/ghast.png");
			}

			{
				var guardian = new ModelGuardian();
				guardian.setRotationAngles(0, 0, 1, 0, 0, 0, new EntityGuardian());
				Add("Mobs/Guardian", guardian, "guardian.png");
			}

			{
				var horse = new ModelHorse();
				horse.setLivingAnimations(new EntityHorse(), 0, 0, 0);
				Add("Mobs/Horse", horse, "horse/horse_white.png");
			}

			{
				var ironGolem = new ModelIronGolem();
				ironGolem.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Iron Golem", ironGolem, "iron_golem.png");
			}

			Add("Blocks/Large Chest", new ModelLargeChest(), "chest/normal_double.png");

			{
				var leashKnot = new ModelLeashKnot();
				leashKnot.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Entities/Leash Knot", leashKnot, "lead_knot.png");
			}

			{
				var magmaCube = new ModelMagmaCube();
				magmaCube.setLivingAnimations(new EntityMagmaCube(), 0, 0, 1);
				Add("Mobs/Magma Cube", magmaCube, "slime/magmacube.png");
			}

			Add("Entities/Minecart", new ModelMinecart(), "minecart.png");

			{
				var ocelot = new ModelOcelot();
				ocelot.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Ocelot", ocelot, "cat/ocelot.png");
			}

			{
				var pig = new ModelPig();
				pig.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Pig", pig, "pig/pig.png");
			}

			{
				var rabbit = new ModelRabbit();
				rabbit.setRotationAngles(0, 0, 0, 0, 0, 0, new EntityRabbit());
				Add("Mobs/Rabbit", rabbit, "rabbit/white.png");
			}

			{
				var sheep = new ModelSheep1();
				sheep.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Sheep (Wool)", sheep, "sheep/sheep_fur.png");
			}

			{
				var sheepShear = new ModelSheep2();
				sheepShear.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Sheep", sheepShear, "sheep/sheep.png");
			}

			Add("Items/Shield", new ModelShield(), "shield_base.png");

			{
				var shulker = new ModelShulker();
				shulker.setRotationAngles(0, 0, 0, 0, 0, 0, new EntityShulker());
				Add("Mobs/Shulker", shulker, "shulker/endergolem.png");
			}

			{
				var shulkerBullet = new ModelShulkerBullet();
				shulkerBullet.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Shulker Bullet", shulkerBullet, "shulker/spark.png");
			}

			Add("Blocks/Sign", new ModelSign(), "sign.png");

			{
				var silverfish = new ModelSilverfish();
				silverfish.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Silverfish", silverfish, "silverfish.png");
			}

			Add("Mobs/Skeleton", new ModelSkeleton(), "skeleton/skeleton.png");
			Add("Mobs/Slime", new ModelSlime(16), "slime/slime.png");

			{
				var snowman = new ModelSnowMan();
				snowman.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/SnowMan", snowman, "snowman.png");
			}

			{
				var spider = new ModelSpider();
				spider.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Spider", spider, "spider/spider.png");
			}

			{
				var squid = new ModelSquid();
				squid.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Squid", squid, "squid.png");
			}

			{
				var villager = new ModelVillager(0);
				villager.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Villager", villager, "villager/villager.png");
			}

			{
				var witch = new ModelWitch(0);
				witch.setRotationAngles(0, 0, 0, 0, 0, 0, new Entity());
				Add("Mobs/Witch", witch, "witch.png");
			}

			{
				var wither = new ModelWither(0);
				wither.setRotationAngles(0, 0, 0, 0, 0, 0, null);
				Add("Mobs/Wither", wither, "wither/wither.png");
			}

			{
				var wolf = new ModelWolf();
				wolf.setRotationAngles(0, 0, 1000, 0, 0, 0, null);
				wolf.setLivingAnimations(new EntityWolf(), 0, 0, 0);
				Add("Mobs/Wolf", wolf, "wolf/wolf.png");
			}

			{
				var zombieVillager = new ModelZombieVillager();
				zombieVillager.setRotationAngles(0, 0, 0, 0, 0, 0, new EntityZombie());
				Add("Mobs/Zombie Villager", zombieVillager, "zombie_villager/zombie_villager.png");
			}

			Add("Players/Cape", new ModelCape(), "cape.png");

			{
				var steve = new ModelPlayer(0, false);
				steve.setRotationAngles(0, 0, 0, 0, 0, 0, new Entity());
				Add("Players/Steve", steve, "steve.png");
			}

			{
				var alex = new ModelPlayer(0, true);
				alex.setRotationAngles(0, 0, 0, 0, 0, 0, new Entity());
				Add("Players/Alex", alex, "alex.png");
			}

			{
				var steve = new ModelPlayer(0, false, false);
				steve.setRotationAngles(0, 0, 0, 0, 0, 0, new Entity());
				Add("Players/Steve (Minimal)", steve, "steve_min.png");
			}

			{
				var alex = new ModelPlayer(0, true, false);
				alex.setRotationAngles(0, 0, 0, 0, 0, 0, new Entity());
				Add("Players/Alex (Minimal)", alex, "alex_min.png");
			}
		}

		public static void Convert()
		{
			Directory.Delete("Models", true);

			foreach (var m in _models)
				m.Item2.Save(Path.GetFileNameWithoutExtension(m.Item1), 1, "Models/" + m.Item1 + ".xml", m.Item3);
		}
	}
}
#endif