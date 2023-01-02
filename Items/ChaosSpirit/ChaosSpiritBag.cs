using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.ChaosSpirit
{
    public class ChaosSpiritBag : ModItem
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

        public override int BossBagNPC => Mod.Find<ModNPC>("ChaosSpirit").Type;

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
                player.QuickSpawnItem(Mod.Find<ModItem>("ChaosSpiritMask").Type);
            }
            else if (choice == 1)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("CataclysmMask").Type);
            }
            player.QuickSpawnItem(Mod.Find<ModItem>("ChaosCrystal").Type);
            player.QuickSpawnItem(Mod.Find<ModItem>("CataclysmCrystal").Type);
        }
    }
}