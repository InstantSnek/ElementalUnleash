using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Purium.Tools
{
    public class PuriumChainsaw : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 110;
            item.melee = true;
            item.width = 20;
            item.height = 12;
            item.useTime = 7;
            item.useAnimation = 15;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.axe = 170 / 5;
            item.tileBoost += 4;
            item.useStyle = 5;
            item.knockBack = 7;
            item.value = Item.sellPrice(0, 8, 0, 0);
            item.rare = 11;
            item.UseSound = SoundID.Item23;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("PuriumChainsaw");
            item.shootSpeed = 46f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PuriumBar", 10);
            recipe.AddTile(null, "PuriumAnvil");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}