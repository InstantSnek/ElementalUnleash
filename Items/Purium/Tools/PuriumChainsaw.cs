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
            Item.damage = 110;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 20;
            Item.height = 12;
            Item.useTime = 7;
            Item.useAnimation = 15;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.axe = 170 / 5;
            Item.tileBoost += 4;
            Item.useStyle = 5;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = 11;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("PuriumChainsaw").Type;
            Item.shootSpeed = 46f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(null, "PuriumBar", 10);
            recipe.AddTile(null, "PuriumAnvil");
            recipe.Register();
        }
    }
}