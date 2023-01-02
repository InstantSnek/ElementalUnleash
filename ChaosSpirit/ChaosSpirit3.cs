using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Bluemagic.ChaosSpirit
{
    public class ChaosSpirit3 : ModNPC
    {
        private const int size = ChaosSpirit.size;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit of Chaos");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 400000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.takenDamageMultiplier = 2f;
            NPC.width = size;
            NPC.height = size;
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.npcSlots = 100f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = null;
            for (int k = 0; k < NPC.buffImmune.Length; k++)
            {
                NPC.buffImmune[k] = true;
            }
            Music = MusicID.Title;
            bossBag/* tModPorter Note: Removed. Spawn the treasure bag alongside other loot via npcLoot.Add(ItemDropRule.BossBag(type)) */ = Mod.Find<ModItem>("ChaosSpiritBag").Type;
        }

        internal List<int> targets = new List<int>();
        private bool syncTargets = false;

        private int stage
        {
            get
            {
                return (int)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = value;
            }
        }

        private int timer
        {
            get
            {
                return (int)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = value;
            }
        }

        private int countdown
        {
            get
            {
                return (int)NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = value;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax / Main.GameModeInfo.EnemyMaxLifeMultiplier * 1.2f * bossLifeScale);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void AI()
        {
            Bluemagic.freezeHeroLives = false;
            FindPlayers();
            if (stage > 0 && targets.Count == 0)
            {
                timer = 0;
                stage = -1;
                NPC.netUpdate = true;
            }
            int debuffType = Mod.Find<ModBuff>("ChaosPressure4").Type;
            foreach (int target in targets)
            {
                Main.player[target].AddBuff(debuffType, 2, false);
            }
            switch (stage)
            {
                case -1:
                    RunAway();
                    break;
                case 0:
                    Initialize();
                    break;
                case 1:
                    Countdown();
                    break;
                case 2:
                    EndAttack();
                    break;
                case 10:
                    FinishFight();
                    break;
                default:
                    break;
            }
            if (stage >= 0 && stage < 10)
            {
                CreateSuppressionSphere();
            }
            NPC.timeLeft = NPC.activeTime;
        }

        public void FindPlayers()
        {
            if (Main.netMode != 1)
            {
                int originalCount = targets.Count;
                targets.Clear();
                for (int k = 0; k < 255; k++)
                {
                    if (Main.player[k].active && Main.player[k].GetModPlayer<BluemagicPlayer>().heroLives > 0)
                    {
                        targets.Add(k);
                    }
                }
                if (Main.netMode == 2 && (syncTargets || targets.Count != originalCount))
                {
                    ModPacket netMessage = GetPacket(ChaosSpiritMessageType.TargetList);
                    netMessage.Write(targets.Count);
                    foreach (int target in targets)
                    {
                        netMessage.Write(target);
                    }
                    netMessage.Send();
                    syncTargets = false;
                }
            }
        }

        public void RunAway()
        {
            timer++;
            if (timer >= 360)
            {
                NPC.active = false;
            }
        }

        private void Initialize()
        {
            countdown = 60 * 60;
            stage++;
            Talk("Mods.Bluemagic.CataclysmCountdown", "60");
        }

        private void Countdown()
        {
            countdown--;
            if (countdown == 60 * 60 - 5)
            {
                syncTargets = true;
            }
            if (countdown == 60 * 45)
            {
                Talk("Mods.Bluemagic.CataclysmCountdown", "45");
            }
            else if (countdown == 60 * 30)
            {
                Talk("Mods.Bluemagic.CataclysmCountdown", "30");
            }
            else if (countdown == 60 * 20)
            {
                Talk("Mods.Bluemagic.CataclysmCountdown", "20");
            }
            else if (countdown == 60 * 10)
            {
                Talk("Mods.Bluemagic.CataclysmCountdown", "10");
            }
            else if (countdown == 60 * 5)
            {
                TalkLiteral("5");
            }
            else if (countdown == 60 * 4)
            {
                TalkLiteral("4");
            }
            else if (countdown == 60 * 3)
            {
                TalkLiteral("3");
            }
            else if (countdown == 60 * 2)
            {
                TalkLiteral("2");
            }
            else if (countdown == 60)
            {
                TalkLiteral("1");
            }
            else if (countdown <= 0)
            {
                countdown = 0;
                stage++;
                Talk("Mods.Bluemagic.ChaosPressureStart");
                NPC.netUpdate = true;
            }
        }

        private void EndAttack()
        {
            if (Main.netMode != 1 && countdown == 0)
            {
                int radius = Main.rand.Next(240, 320);
                float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
                Vector2 offset = radius * angle.ToRotationVector2();
                Projectile.NewProjectile(NPC.Center + offset, Vector2.Zero, Mod.Find<ModProjectile>("HolySphere2").Type, 0, 0f, Main.myPlayer, NPC.whoAmI);
            }
            countdown++;
            if (countdown == 140f)
            {
                PlaySound(29, 104);
            }
            if (countdown >= 180f)
            {
                countdown = 0;
                NPC.netUpdate = true;
            }
        }

        public int RandomTarget()
        {
            if (targets.Count == 0)
            {
                return 255;
            }
            return targets[Main.rand.Next(targets.Count)];
        }

        private void CreateSuppressionSphere()
        {
            timer++;
            if (timer >= 60 || (stage < 2 && timer >= 30))
            {
                if (Main.netMode != 1)
                {
                    Vector2 direction;
                    if (Main.rand.Next(stage == 2 ? 6 : 3) == 0)
                    {
                        direction = Main.player[RandomTarget()].Center - NPC.Center;
                        direction.Normalize();
                    }
                    else
                    {
                        direction = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2();
                    }
                    Projectile.NewProjectile(NPC.Center, 4f * direction, Mod.Find<ModProjectile>("SuppressionSphere").Type, 0, 0f, Main.myPlayer, NPC.whoAmI);
                }
                timer = 0;
            }
        }

        public override bool CheckDead()
        {
            if (stage == 10)
            {
                return true;
            }
            NPC.active = true;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            stage = 10;
            timer = 0;
            NPC.netUpdate = true;
            return false;
        }

        private void FinishFight()
        {
            if (timer == 0)
            {
                if (!Main.dedServ)
                {
                    MoonlordDeathDrama.RequestLight(1f, NPC.Center);
                }
                PlaySound(29, 92);
            }
            if (!Main.dedServ)
            {
                float x = Main.rand.Next(-Main.screenWidth / 2, Main.screenWidth / 2);
                float y = Main.rand.Next(-Main.screenHeight / 2, Main.screenHeight / 2);
                MoonlordDeathDrama.AddExplosion(NPC.Center + new Vector2(x, y));
            }
            timer++;
            if (timer >= 300)
            {
                NPC.dontTakeDamage = false;
                NPC.HitSound = null;
                NPC.takenDamageMultiplier = 1f;
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            return CanBeHitByPlayer(player);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return CanBeHitByPlayer(Main.player[projectile.owner]);
        }

        private bool? CanBeHitByPlayer(Player player)
        {
            if (!targets.Contains(player.whoAmI))
            {
                return false;
            }
            return null;
        }

        public override void OnKill()
        {
            int choice = Main.rand.Next(10);
            int item = 0;
            switch (choice)
            {
                case 0:
                    item = Mod.Find<ModItem>("ChaosTrophy").Type;
                    break;
                case 1:
                    item = Mod.Find<ModItem>("CataclysmTrophy").Type;
                    break;
            }
            if (item > 0)
            {
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, item);
            }
            if (Main.expertMode)
            {
                NPC.DropBossBags();
            }
            else
            {
                choice = Main.rand.Next(7);
                if (choice == 0)
                {
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("ChaosSpiritMask").Type);
                }
                else if (choice == 1)
                {
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("CataclysmMask").Type);
                }
                Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("ChaosCrystal").Type);
            }
            BluemagicWorld.downedChaosSpirit = true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The " + name;
            potionType = ItemID.SuperHealingPotion;
        }

        public override void FindFrame(int frameSize)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter >= 6.0)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameSize;
                NPC.frame.Y %= 5 * frameSize;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        private void Talk(string key, byte r = 255, byte g = 255, byte b = 255)
        {
            if (Main.netMode == 0)
            {
                Main.NewText(Language.GetTextValue(key), r, g, b);
            }
            else
            {
                NetworkText text = NetworkText.FromKey(key);
                ChatHelper.BroadcastChatMessage(text, new Color(r, g, b));
            }
        }

        private void Talk(string key, object arg, byte r = 255, byte g = 255, byte b = 255)
        {
            if (Main.netMode == 0)
            {
                Main.NewText(Language.GetTextValue(key, arg), r, g, b);
            }
            else
            {
                NetworkText text = NetworkText.FromKey(key, arg);
                ChatHelper.BroadcastChatMessage(text, new Color(r, g, b));
            }
        }

        private void TalkLiteral(string literal, byte r = 255, byte g = 255, byte b = 255)
        {
            if (Main.netMode == 0)
            {
                Main.NewText(literal, r, g, b);
            }
            else
            {
                NetworkText text = NetworkText.FromLiteral(literal);
                ChatHelper.BroadcastChatMessage(text, new Color(r, g, b));
            }
        }

        private void PlaySound(int type, int style)
        {
            if (Main.netMode != 2)
            {
                if (targets.Contains(Main.myPlayer))
                {
                    SoundEngine.PlaySound(type, -1, -1, style);
                }
                else
                {
                    SoundEngine.PlaySound(type, (int)NPC.position.X, (int)NPC.position.Y, style);
                }
            }
        }

        private ModPacket GetPacket(ChaosSpiritMessageType type)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.ChaosSpirit);
            packet.Write(NPC.whoAmI);
            packet.Write((byte)type);
            return packet;
        }

        public void HandlePacket(BinaryReader reader)
        {
            ChaosSpiritMessageType type = (ChaosSpiritMessageType)reader.ReadByte();
            if (type == ChaosSpiritMessageType.HeroPlayer)
            {
                Player player = Main.player[Main.myPlayer];
                player.GetModPlayer<BluemagicPlayer>().heroLives = reader.ReadInt32();
            }
            else if (type == ChaosSpiritMessageType.TargetList)
            {
                int numTargets = reader.ReadInt32();
                targets.Clear();
                for (int k = 0; k < numTargets; k++)
                {
                    targets.Add(reader.ReadInt32());
                }
            }
            else if (type == ChaosSpiritMessageType.DeActivate)
            {
                NPC.active = false;
            }
            else if (type == ChaosSpiritMessageType.PlaySound)
            {
                int soundType = reader.ReadInt32();
                int style = reader.ReadInt32();
                if (targets.Contains(Main.myPlayer))
                {
                    SoundEngine.PlaySound(soundType, -1, -1, style);
                }
                else
                {
                    SoundEngine.PlaySound(soundType, (int)NPC.position.X, (int)NPC.position.Y, style);
                }
            }
            else if (type == ChaosSpiritMessageType.Damage)
            {
            }
        }
    }
}