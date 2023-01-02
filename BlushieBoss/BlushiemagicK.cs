using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.BlushieBoss
{
    public class BlushiemagicK : BlushiemagicBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("blushiemagic (K)");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            this.Music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Phyrnna - Return of the Snow Queen");
        }

        public override void AI()
        {
            if (BlushieBoss.Timer >= 390 && BlushieBoss.Timer < 600)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(NPC.Center - new Vector2(50f, 50f), 100, 100, Mod.Find<ModDust>("Sparkle").Type, 0f, 0f, 0, new Color(0, 0, 255), 1f);
                }
            }
        }

        public override bool CheckDead()
        {
            if (BlushieBoss.HealthK > 0)
            {
                NPC.life = BlushieBoss.HealthK;
            }
            else
            {
                NPC.active = false;
                if (Main.netMode != 1)
                {
                    BlushieBoss.KylieTalk("I knew it. I'm so useless...");
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("FrostFairyWings").Type);
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (BlushieBoss.Timer >= 600)
            {
                Texture2D texture = Mod.GetTexture("BlushieBoss/BlushiemagicK_Back");
                spriteBatch.Draw(texture, NPC.Center - Main.screenPosition - new Vector2(texture.Width / 2, texture.Height / 2), Color.White);
            }
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (BlushieBoss.ShieldK >= 300 && BlushieBoss.ShieldBuff(NPC))
            {
                Texture2D shield = Mod.GetTexture("BlushieBoss/ShieldK");
                spriteBatch.Draw(shield, NPC.Center - Main.screenPosition - new Vector2(shield.Width / 2, shield.Height / 2), null, Color.White * 0.5f);
            }
        }

        public override double CalculateDamage(Player player, double damage)
        {
            if (BlushieBoss.ShieldK >= 300 && BlushieBoss.ShieldBuff(NPC))
            {
                BlushieBoss.ShieldK = 0;
                return 0;
            }
            float defenseMult = player.statDefense / 200f;
            float resistMult = player.endurance / 0.6f;
            float mult = 0.6f * defenseMult + 0.4f * resistMult;
            damage = mult * 100000;
            if (damage > 100000)
            {
                damage = 100000;
            }
            if (Main.netMode != 2 && NPC.localAI[0] == 0f && damage < 50000)
            {
                Main.NewText("<blushiemagic (K)> Oh yeah, uh, you need lots of defense and damage reduction if you want to damage me. Sorry...", 0, 128, 255);
                NPC.localAI[0] = 1f;
            }
            return damage;
        }

        public override void SetHealth(double damage)
        {
            BlushieBoss.HealthK = NPC.life - (int)damage;
        }
    }
}