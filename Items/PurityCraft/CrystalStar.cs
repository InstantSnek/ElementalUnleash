using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.PurityCraft
{
    public class CrystalStar : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 5;
            AIType = ProjectileID.HallowStar;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 50;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Mod.Find<ModDust>("CrystalStar").Type, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default(Color), 1.2f);
            }
            for (int k = 0; k < 3; k++)
            {
                int goreType = Mod.GetGoreSlot(Main.rand.Next(2) == 0 ? "Gores/GreenStar" : "Gores/WhiteStar");
                Gore.NewGore(Projectile.position, 0.05f * Projectile.velocity, goreType, 1f);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 center = Projectile.Center;
                float offset = 6f;
                int type = Mod.Find<ModProjectile>("CrystalShard").Type;
                int damage = Projectile.damage - 10;
                Projectile.NewProjectile(center.X - offset, center.Y - offset, -8f, -6f, type, damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(center.X + offset, center.Y - offset, 8f, -6f, type, damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(center.X - offset, center.Y + offset, -10f, -2f, type, damage, Projectile.knockBack, Projectile.owner);
                Projectile.NewProjectile(center.X + offset, center.Y + offset, 10f, -2f, type, damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0);
        }
    }
}