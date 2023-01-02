using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.NPCs.Night
{
    public class NightSlime : ModNPC
    {
        private static Random sparkleRand = new Random();
        private int sparkleFrame = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 1;
            NPC.width = 36;
            NPC.height = 24;
            NPC.lifeMax = 600;
            NPC.damage = 80;
            NPC.defense = 50;
            NPC.knockBackResist = 0.6f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.value = Item.buyPrice(0, 0, 8, 0);
            NPC.npcSlots = 1;
            NPC.color = new Color(0, 0, 0, 50);
            NPC.alpha = 120;
            AnimationType = 16;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = Mod.Find<ModItem>("NightSlimeBanner").Type;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.type == Mod.Find<ModNPC>("NightSlime").Type && Main.rand.Next(20) == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, Mod.Find<ModDust>("Sparkle").Type, 0f, 0f, 0, Color.White, 1.5f);
            }
            NPC.ai[0] += 2f;
            if (NPC.localAI[0] > 0f)
            {
                NPC.localAI[0] -= 1f;
            }
            Vector2 center = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float shootToX = player.position.X + (float)player.width * 0.5f - center.X;
            float shootToY = player.position.Y - center.Y;
            float distance = (float)System.Math.Sqrt((double)(shootToX * shootToX + shootToY * shootToY));
            if (distance < 480f && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.9f;
                }
                if (Main.netMode != 1 && NPC.localAI[0] == 0f)
                {
                    int projType = Mod.Find<ModProjectile>("StarGel").Type;
                    int damage = 35;
                    if (NPC.type == Mod.Find<ModNPC>("DirtySlime").Type)
                    {
                        projType = Mod.Find<ModProjectile>("DirtGel").Type;
                        damage = 45;
                    }
                    distance = 3f / distance;
                    shootToX *= distance;
                    shootToY *= distance;
                    NPC.localAI[0] = 120f;
                    Projectile.NewProjectile(center.X, center.Y, shootToX, shootToY, projType, damage, 0f, Main.myPlayer, 0f, 0f);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            int chance = 10;
            int time = 240;
            if (NPC.type == Mod.Find<ModNPC>("DirtySlime").Type)
            {
                chance = 9;
                time = 300;
            }
            if (Main.rand.Next(chance) == 0)
            {
                target.AddBuff(Mod.Find<ModBuff>("Liquified").Type, time, true);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.type == Mod.Find<ModNPC>("NightSlime").Type)
            {
                Texture2D sparkleTexture = Mod.GetTexture("NPCs/Night/NightSlimeSparkle");
                frameHeight = sparkleTexture.Height / 3;
                sparkleFrame = (sparkleRand.Next(2) * frameHeight + sparkleFrame) % (3 * frameHeight);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Color color = NPC.color;
            if (NPC.type == Mod.Find<ModNPC>("DirtySlime").Type)
            {
                color = new Color(178, 152, 71);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, (float)(2 * hitDirection), -2f, NPC.alpha, color, 1f);
                    if (k % 5 == 0 && NPC.type == Mod.Find<ModNPC>("NightSlime").Type)
                    {
                        Dust.NewDust(NPC.position, NPC.width, NPC.height, Mod.Find<ModDust>("Sparkle").Type, 0f, 0f, 0, Color.White, 1.5f);
                    }
                }
            }
            else
            {
                int count = 0;
                while (count < damage / NPC.lifeMax * 100)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, (float)hitDirection, -1f, NPC.alpha, color, 1f);
                    if (count % 5 == 0 && NPC.type == Mod.Find<ModNPC>("NightSlime").Type)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Mod.Find<ModDust>("Sparkle").Type, 0f, 0f, 0, Color.White, 1.5f);
                        Main.dust[dust].velocity.X += hitDirection * 3;
                    }
                    count++;
                }
            }
        }

        public override void OnKill()
        {
            Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Gel, Main.rand.Next(4, 7));
            if (Main.rand.Next(Main.expertMode ? 4200 : 6000) == 0)
            {
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.SlimeStaff);
            }
            if (Main.rand.Next(Main.expertMode ? 3 : 2) != 0)
            {
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("SuspiciousGel").Type);
            }
        }

        public override float SpawnChance(NPCSpawnInfo info)
        {
            if (!SpawnHelper.NoBiomeNormalSpawn(info) || !BluemagicWorld.elementalUnleash)
            {
                return 0f;
            }
            if (NPC.type == Mod.Find<ModNPC>("NightSlime").Type)
            {
                return info.SpawnTileY <= Main.worldSurface && !Main.dayTime ? 1f / 6f : 0f;
            }
            if (NPC.type == Mod.Find<ModNPC>("DirtySlime").Type)
            {
                return info.SpawnTileY > Main.worldSurface && info.SpawnTileY < Main.maxTilesY - 190 ? 1f / 6f : 0f;
            }
            return 0f;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.type == Mod.Find<ModNPC>("NightSlime").Type)
            {
                Texture2D sparkleTexture = Mod.GetTexture("NPCs/Night/NightSlimeSparkle");
                spriteBatch.Draw(sparkleTexture, NPC.position - Main.screenPosition, new Rectangle(0, sparkleFrame, sparkleTexture.Width, sparkleTexture.Height / 3), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0f);
            }
        }
    }
}
