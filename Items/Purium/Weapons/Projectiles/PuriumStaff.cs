using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Purium.Weapons.Projectiles
{
    public class PuriumStaff : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.soundDelay == 0 && System.Math.Abs(Projectile.velocity.X) + System.Math.Abs(Projectile.velocity.Y) > 2f)
            {
                Projectile.soundDelay = 10;
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }
            for (int num143 = 0; num143 < 1; num143++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, Bluemagic.PureColor, 2.5f);
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].velocity += Projectile.velocity * 0.2f;
                Main.dust[dust].position.X = Projectile.Center.X + 4f + (float)Main.rand.Next(-2, 3);
                Main.dust[dust].position.Y = Projectile.Center.Y + (float)Main.rand.Next(-2, 3);
                Main.dust[dust].noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, Bluemagic.PureColor.ToVector3());
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 0f)
            {
                if (Main.player[Projectile.owner].channel)
                {
                    float num146 = 16f;
                    Vector2 vector10 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num147 = (float)Main.mouseX + Main.screenPosition.X - vector10.X;
                    float num148 = (float)Main.mouseY + Main.screenPosition.Y - vector10.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        num148 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector10.Y;
                    }
                    float num149 = (float)System.Math.Sqrt((double)(num147 * num147 + num148 * num148));
                    num149 = (float)System.Math.Sqrt((double)(num147 * num147 + num148 * num148));
                    if (num149 > num146)
                    {
                        num149 = num146 / num149;
                        num147 *= num149;
                        num148 *= num149;
                        int num150 = (int)(num147 * 1000f);
                        int num151 = (int)(Projectile.velocity.X * 1000f);
                        int num152 = (int)(num148 * 1000f);
                        int num153 = (int)(Projectile.velocity.Y * 1000f);
                        if (num150 != num151 || num152 != num153)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num147;
                        Projectile.velocity.Y = num148;
                    }
                    else
                    {
                        int num154 = (int)(num147 * 1000f);
                        int num155 = (int)(Projectile.velocity.X * 1000f);
                        int num156 = (int)(num148 * 1000f);
                        int num157 = (int)(Projectile.velocity.Y * 1000f);
                        if (num154 != num155 || num156 != num157)
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.velocity.X = num147;
                        Projectile.velocity.Y = num148;
                    }
                }
                else if (Projectile.ai[0] == 0f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                    float num158 = 12f;
                    Vector2 vector11 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num159 = (float)Main.mouseX + Main.screenPosition.X - vector11.X;
                    float num160 = (float)Main.mouseY + Main.screenPosition.Y - vector11.Y;
                    if (Main.player[Projectile.owner].gravDir == -1f)
                    {
                        num160 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector11.Y;
                    }
                    float num161 = (float)System.Math.Sqrt((double)(num159 * num159 + num160 * num160));
                    if (num161 == 0f)
                    {
                        vector11 = new Vector2(Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2), Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2));
                        num159 = Projectile.position.X + (float)Projectile.width * 0.5f - vector11.X;
                        num160 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector11.Y;
                        num161 = (float)System.Math.Sqrt((double)(num159 * num159 + num160 * num160));
                    }
                    num161 = num158 / num161;
                    num159 *= num161;
                    num160 *= num161;
                    Projectile.velocity.X = num159;
                    Projectile.velocity.Y = num160;
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            if (Projectile.velocity.X != 0f || Projectile.velocity.Y != 0f)
            {
                Projectile.rotation = (float)System.Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) - 2.355f;
            }
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int k = 0; k < 20; k++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 66, 0f, 0f, 100, Bluemagic.PureColor, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 4f;
            }
        }
    }
}