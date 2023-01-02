using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Abomination
{
    public class AbominationBag : ModItem
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
            Item.rare = 9;
            Item.expert = true;
        }

        public override int BossBagNPC => Mod.Find<ModNPC>("Abomination").Type;

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            if (Main.rand.Next(7) == 0)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("AbominationMask").Type);
            }
            player.QuickSpawnItem(Mod.Find<ModItem>("MoltenDrill").Type);
            player.QuickSpawnItem(Mod.Find<ModItem>("DimensionalChest").Type);
            player.QuickSpawnItem(Mod.Find<ModItem>("MoltenBar").Type, 5);
            player.QuickSpawnItem(Mod.Find<ModItem>("SixColorShield").Type);
        }
    }
}