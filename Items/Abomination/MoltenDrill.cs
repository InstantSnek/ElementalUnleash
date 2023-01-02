using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Abomination
{
    public class MoltenDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Can mine Lihzahrd Bricks");
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
            Item.width = 20;
            Item.height = 12;
            Item.useTime = 7;
            Item.useAnimation = 25;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.pick = 210;
            Item.tileBoost++;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 22, 50, 0);
            Item.rare = 9;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.shoot = Mod.Find<ModProjectile>("MoltenDrill").Type;
            Item.shootSpeed = 40f;
        }
    }
}