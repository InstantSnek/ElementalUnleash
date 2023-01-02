using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.PuritySpirit
{
    public class PuritySpiritBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Treasure Bag");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = 11;
            Item.expert = true;
        }

        public override int BossBagNPC => Mod.Find<ModNPC>("PuritySpirit").Type;

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            player.TryGettingDevArmor();
            int choice = Main.rand.Next(7);
            if (choice == 0)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("PuritySpiritMask").Type);
            }
            else if (choice == 1)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("BunnyMask").Type);
            }
            if (choice != 1)
            {
                player.QuickSpawnItem(ItemID.Bunny);
            }
            player.QuickSpawnItem(Mod.Find<ModItem>("InfinityCrystal").Type, 2);
            choice = Main.rand.Next(4);
            int type = 0;
            switch (choice)
            {
            case 0:
                type = Mod.Find<ModItem>("DanceOfBlades").Type;
                break;
            case 1:
                type = Mod.Find<ModItem>("CleanserBeam").Type;
                break;
            case 2:
                type = Mod.Find<ModItem>("PrismaticShocker").Type;
                break;
            case 3:
                type = Mod.Find<ModItem>("VoidEmblem").Type;
                break;
            }
            player.QuickSpawnItem(type);
        }
    }
}