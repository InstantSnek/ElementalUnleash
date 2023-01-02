using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Phantom
{
    public class PhantomBag : ModItem
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
            Item.rare = 8;
            Item.expert = true;
        }

        public override int BossBagNPC => Mod.Find<ModNPC>("Phantom").Type;

        public override bool CanRightClick()
        {
            return true;
        }

        public override void OpenBossBag(Player player)
        {
            player.TryGettingDevArmor();
            if (Main.rand.Next(7) == 0)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("PhantomMask").Type);
            }
            player.QuickSpawnItem(Mod.Find<ModItem>("PhantomPlate").Type, Main.rand.Next(8, 13));
            int reward = 0;
            switch (Main.rand.Next(4))
            {
            case 0:
                reward = Mod.Find<ModItem>("PhantomBlade").Type;
                break;
            case 1:
                reward = Mod.Find<ModItem>("SpectreGun").Type;
                break;
            case 2:
                reward = Mod.Find<ModItem>("PhantomSphere").Type;
                break;
            case 3:
                reward = Mod.Find<ModItem>("PaladinStaff").Type;
                break;
            }
            player.QuickSpawnItem(reward);
            player.QuickSpawnItem(Mod.Find<ModItem>("PhantomShield").Type);
        }
    }
}