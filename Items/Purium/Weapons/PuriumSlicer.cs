using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Purium.Weapons
{
    public class PuriumSlicer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1.2f;
            Item.useStyle = 1;
            Item.useAnimation = 20;
            Item.useTime = 12;
            Item.damage = 502;
            Item.knockBack = 4.5f;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.rare = 11;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.value = Item.sellPrice(0, 12, 0, 0);
            Item.UseSound = SoundID.Item1;
            Item.shoot = Mod.Find<ModProjectile>("PuriumSlice").Type;
            Item.shootSpeed = 0.5f;
        }

        public override bool OnlyShootOnSwing/* tModPorter Note: Removed. If you returned true, set Item.useTime to a multiple of Item.useAnimation */ => true;

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(null, "PuriumBar", 12);
            recipe.AddTile(null, "PuriumAnvil");
            recipe.Register();
        }
    }
}