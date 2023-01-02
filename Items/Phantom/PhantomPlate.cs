using System;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Items.Phantom
{
    public class PhantomPlate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 99;
            Item.rare = 8;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }
    }
}