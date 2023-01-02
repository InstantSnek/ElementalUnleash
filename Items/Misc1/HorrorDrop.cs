using System;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Items.Misc1
{
    public class HorrorDrop : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Red as blood");
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 999;
            Item.rare = 3;
            Item.value = 1000;
        }
    }
}