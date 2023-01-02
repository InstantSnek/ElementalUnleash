using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Salt
{
    public class SaltedSalt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Probably not healthy to eat");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;
            Item.maxStack = 30;
            Item.rare = 8;
            Item.value = 300;
            Item.UseSound = SoundID.Item2;
            Item.useStyle = 2;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
            Item.buffType = Mod.Find<ModBuff>("Salty").Type;
            Item.buffTime = 7200;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod, "Salt", 5);
            recipe.Register();
        }
    }
}