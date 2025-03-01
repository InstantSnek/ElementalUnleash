using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.BlushieBoss
{
    public class Blushiemagic : BlushiemagicBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("blushiemagic");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            this.Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Shelter");
            NPC.takenDamageMultiplier = 10f;
        }

        public override void AI()
        {
            if (BlushieBoss.Timer > 300f)
            {
                for (int k = 0; k < 3; k++)
                {
                    float radius = 32f;
                    float rotation = Main.rand.NextFloat() * MathHelper.TwoPi;
                    Vector2 dustPos = NPC.Center + new Vector2(0f, 15f) + radius * rotation.ToRotationVector2();
                    int dust = Dust.NewDust(dustPos, 0, 0, Mod.Find<ModDust>("Particle").Type, 0f, 0f, 0, Color.White);
                    Main.dust[dust].customData = NPC;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.GetTexture("BlushieBoss/Wing");
            Vector2 drawPos = new Vector2(NPC.position.X + NPC.width / 2, NPC.position.Y + NPC.height * 3 / 4) - Main.screenPosition;
            float scale = (BlushieBoss.Timer - 60f) / 90f;
            if (scale < 0f)
            {
                scale = 0f;
            }
            if (scale > 1f)
            {
                scale = 1f;
            }
            float baseRot = MathHelper.Pi / 5f;
            float rotate = (BlushieBoss.Timer - 180f) / 120f;
            if (rotate < 0f)
            {
                rotate = 0f;
            }
            if (rotate > 1f)
            {
                rotate = 1f;
            }
            rotate *= MathHelper.Pi * 0.4f;
            for (int k = 0; k <= 3; k++)
            {
                float rot = rotate * k / 3f;
                spriteBatch.Draw(texture, drawPos, null, Color.White, baseRot - rot, new Vector2(0f, 34f), new Vector2(scale, 1f), SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawPos, null, Color.White, -baseRot + rot, new Vector2(100f, 34f), new Vector2(scale, 1f), SpriteEffects.FlipHorizontally, 0f);
            }
            return true;
        }

        public override bool CheckDead()
        {
            if (BlushieBoss.Timer < 3600)
            {
                NPC.life = NPC.lifeMax;
            }
            else
            {
                NPC.active = false;
                BlushieBoss.StartPhase2();
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D shield = Mod.GetTexture("BlushieBoss/Shield");
            float alpha;
            if (BlushieBoss.Timer < 60)
            {
                alpha = 0f;
            }
            else if (BlushieBoss.Timer < 120)
            {
                int temp = (BlushieBoss.Timer - 60) % 40;
                if (temp > 20)
                {
                    temp = 40 - temp;
                }
                alpha = temp / 20f;
            }
            else if (BlushieBoss.Timer < 3600)
            {
                alpha = 1f;
            }
            else if (BlushieBoss.Timer < 3660)
            {
                int temp = (BlushieBoss.Timer - 3600) % 40;
                if (temp > 20)
                {
                    temp = 40 - temp;
                }
                alpha = 1f - temp / 20f;
            }
            else
            {
                alpha = 0f;
            }
            alpha *= 0.5f;
            if (alpha > 0f)
            {
                spriteBatch.Draw(shield, NPC.Center - Main.screenPosition - new Vector2(shield.Width / 2, shield.Height / 2), null, Color.White * alpha);
            }
            BlushieBoss.DrawBullets(spriteBatch);
        }

        public override bool UseSpecialDamage()
        {
            return false;
        }
    }
}