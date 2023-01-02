using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Bluemagic.BlushieBoss;
using Bluemagic.PuritySpirit;

namespace Bluemagic
{
    public class BluemagicPlayer : ModPlayer
    {
        public bool eFlames = false;
        public int customMeleeEnchant = 0;
        public bool paladinMinion = false;
        public bool elementShield = false;
        public int elementShields = 0;
        private int elementShieldTimer = 0;
        public int elementShieldPos = 0;
        public bool voidMonolith = false;
        public bool extraAccessory2 = false;
        public bool elementMinion = false;
        public bool saltLamp = false;

        public float puriumShieldChargeMax = 0f;
        public float puriumShieldChargeRate = 1f;
        public float puriumShieldEnduranceMult = 1f;
        public const float puriumShieldDamageEffectiveness = 0.002f;
        public int[] buffImmuneCounter;
        public static List<int> buffImmuneBlacklist = new List<int>(new int[]
        {
            BuffID.Wet,
            BuffID.Lovestruck,
            BuffID.Stinky,
            BuffID.Slimed,
            BuffID.Sunflower,
            BuffID.MonsterBanner,
            BuffID.PeaceCandle
        });
        public const float buffImmuneCost = 50f;
        public const float reviveCost = 1000f;
        private int miscTimer = 0;
        public bool purityMinion = false;
        public int liquified = 0;
        private int transformFrameCounter = 0;

        public int heroLives = 0;
        public int reviveTime = 0;
        public int constantDamage = 0;
        public float percentDamage = 0f;
        public float defenseEffect = -1f;
        public bool chaosDefense = false;
        public bool badHeal = false;
        public int prevLife = -1;
        public int healHurt = 0;
        public bool nullified = false;
        public int purityDebuffCooldown = 0;

        public bool manaMagnet2 = false;
        public bool crystalCloak = false;
        public bool lifeMagnet2 = false;
        public bool voidEmissary = false;
        public bool purityShieldMount = false;

        private int chaosWarningCooldown = 0;
        public int chaosPressure = 0;
        public float suppression = 0f;
        public float ammoCost = 0f;
        public float thrownCost = 0f;
        public int cancelBadRegen = 0;

        internal int terraLives = 0;
        private int terraKill = 0;
        private int terraImmune = 0;
        private Vector2 lastPos;
        public bool triedGodmode = false;
        public bool godmode = false;
        public bool noGodmode = false;
        internal float blushieHealth = 0f;
        internal int origHealth;
        private int blushieImmune = 0;
        public bool frostFairy = false;
        public bool skyDragon = false;
        public int worldReaverCooldown = 0;

        //permanent data
        public float puriumShieldCharge = 0f;
        public CustomStats chaosStats;
        public CustomStats cataclysmStats;

        public override void Initialize()
        {
            buffImmuneCounter = new int[Player.buffImmune.Length];
            chaosStats = CustomStats.CreateChaosStats();
            cataclysmStats = CustomStats.CreateCataclysmStats();
        }

        public override void ResetEffects()
        {
            if (lifeMagnet2)
            {
                Player.potionDelayTime = (int)(Player.potionDelayTime * 0.8f);
                Player.restorationDelayTime = (int)(Player.restorationDelayTime * 0.8f);
            }
            eFlames = false;
            customMeleeEnchant = 0;
            paladinMinion = false;
            elementShield = false;
            elementMinion = false;
            puriumShieldChargeMax = 0f;
            puriumShieldChargeRate = 1f;
            puriumShieldEnduranceMult = 1f;
            purityMinion = false;
            if (liquified < 0)
            {
                liquified = 0;
                Player.position.X -= (Player.width - Player.defaultWidth) / 2f;
                Player.position.Y += Player.height - Player.defaultHeight;
                Player.width = Player.defaultWidth;
                Player.height = Player.defaultHeight;
            }
            else
            {
                liquified *= -1;
            }
            constantDamage = 0;
            percentDamage = 0f;
            defenseEffect = -1f;
            chaosDefense = false;
            badHeal = false;
            healHurt = 0;
            nullified = false;
            manaMagnet2 = false;
            crystalCloak = false;
            lifeMagnet2 = false;
            voidEmissary = false;
            purityShieldMount = false;
            chaosPressure = 0;
            suppression = 0f;
            ammoCost = 0f;
            thrownCost = 0f;
            cancelBadRegen = 0;
            triedGodmode = false;
            godmode = false;
            noGodmode = false;
            frostFairy = false;
            skyDragon = false;
            if (extraAccessory2)
            {
                Player.extraAccessorySlots = 2;
            }
        }

        public override void UpdateDead()
        {
            eFlames = false;
            badHeal = false;
            puriumShieldCharge = 0f;
            reviveTime = 0;
            terraLives = 0;
            terraKill = 0;
            blushieHealth = 0f;
            blushieImmune = 0;
            if (heroLives == 1)
            {
                heroLives = 0;
                if (Main.netMode == 1)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MessageType.HeroLives);
                    packet.Write(Player.whoAmI);
                    packet.Write(heroLives);
                    packet.Send();
                }
            }
        }

        public override void SaveData(TagCompound tag)/* tModPorter Suggestion: Edit tag parameter instead of returning new TagCompound */
        {
            TagCompound tag = new TagCompound();
            tag["version"] = 0;
            tag["extraAccessory2"] = extraAccessory2;
            tag["puriumShieldCharge"] = puriumShieldCharge;
            tag["chaosStats"] = chaosStats.Save();
            tag["cataclysmStats"] = cataclysmStats.Save();
            return tag;
        }

        public override void LoadData(TagCompound tag)
        {
            extraAccessory2 = tag.GetBool("extraAccessory2");
            puriumShieldCharge = tag.GetFloat("puriumShieldCharge");
            TagCompound tagStats = tag.GetCompound("chaosStats");
            if (tagStats != null)
            {
                chaosStats.Load(tagStats);
            }
            tagStats = tag.GetCompound("cataclysmStats");
            if (tagStats != null)
            {
                cataclysmStats.Load(tagStats);
            }
        }

        public override void clientClone(ModPlayer clientClone)
        {
            BluemagicPlayer clone = clientClone as BluemagicPlayer;
            clone.chaosStats = chaosStats.Clone();
            clone.cataclysmStats = cataclysmStats.Clone();
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket(512);
            packet.Write((byte)MessageType.CustomStats);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)0);
            chaosStats.NetSend(packet);
            packet.Send(toWho, fromWho);
            packet = Mod.GetPacket(512);
            packet.Write((byte)MessageType.CustomStats);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)1);
            cataclysmStats.NetSend(packet);
            packet.Send(toWho, fromWho);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            BluemagicPlayer clone = clientPlayer as BluemagicPlayer;
            if (!chaosStats.Equals(clone.chaosStats))
            {
                ModPacket packet = Mod.GetPacket(512);
                packet.Write((byte)MessageType.CustomStats);
                packet.Write((byte)Player.whoAmI);
                packet.Write((byte)0);
                chaosStats.NetSend(packet);
                packet.Send();
            }
            if (!cataclysmStats.Equals(clone.cataclysmStats))
            {
                ModPacket packet = Mod.GetPacket(512);
                packet.Write((byte)MessageType.CustomStats);
                packet.Write((byte)Player.whoAmI);
                packet.Write((byte)1);
                cataclysmStats.NetSend(packet);
                packet.Send();
            }
        }

        private bool AnyChaosSpirit()
        {
            return NPC.AnyNPCs(Mod.Find<ModNPC>("ChaosSpirit").Type) || NPC.AnyNPCs(Mod.Find<ModNPC>("ChaosSpirit2").Type) || NPC.AnyNPCs(Mod.Find<ModNPC>("ChaosSpirit3").Type);
        }

        private bool AnyTerraSpirit()
        {
            return NPC.AnyNPCs(Mod.Find<ModNPC>("TerraSpirit").Type) || NPC.AnyNPCs(Mod.Find<ModNPC>("TerraSpirit2").Type);
        }

        private bool IsChaosSpirit(int type)
        {
            return type == Mod.Find<ModNPC>("ChaosSpirit").Type || type == Mod.Find<ModNPC>("ChaosSpirit2").Type || type == Mod.Find<ModNPC>("ChaosSpirit3").Type;
        }

        private bool IsTerraSpirit(int type)
        {
            return type == Mod.Find<ModNPC>("TerraSpirit").Type || type == Mod.Find<ModNPC>("TerraSpirit2").Type;
        }

        public override void UpdateBiomeVisuals()
        {
            bool useTerra = false;
            bool useChaos = false;
            bool usePurity = false;
            bool useVoidMonolith = false;
            if (BlushieBoss.BlushieBoss.Active)
            {
            }
            else if (AnyTerraSpirit())
            {
                useTerra = true;
            }
            else if (AnyChaosSpirit())
            {
                useChaos = true;
            }
            else if (NPC.AnyNPCs(Mod.Find<ModNPC>("PuritySpirit").Type))
            {
                usePurity = true;
            }
            else if (voidMonolith && !NPC.AnyNPCs(NPCID.MoonLordCore))
            {
                useVoidMonolith = true;
            }
            Player.ManageSpecialBiomeVisuals("Bluemagic:TerraSpirit", useTerra);
            Player.ManageSpecialBiomeVisuals("Bluemagic:ChaosSpirit", useChaos);
            Player.ManageSpecialBiomeVisuals("Bluemagic:PuritySpirit", usePurity);
            Player.ManageSpecialBiomeVisuals("Bluemagic:MonolithVoid", useVoidMonolith);
        }

        public override void UpdateBadLifeRegen()
        {
            if (eFlames)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 16;
            }
            if (healHurt > 0)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                Player.lifeRegen -= 120 * healHurt;
            }
            if (chaosPressure > 0)
            {
                Player.lifeRegenTime -= chaosPressure;
                Player.lifeRegen -= chaosPressure;
                if (Player.lifeRegenTime < 0)
                {
                    Player.lifeRegenTime = 0;
                }
            }
            if (blushieImmune > 0)
            {
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegenTime = 0;
                if (!Bluemagic.testing || Player.statLife > 1)
                {
                    Player.lifeRegen -= 32;
                }
            }
        }

        public override void UpdateLifeRegen()
        {
            if (Player.lifeRegen < 0 && cancelBadRegen > 0)
            {
                Player.lifeRegen += cancelBadRegen;
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
            }
        }

        public override void SetControls()
        {
            for (int k = 0; k < 1000; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Player.whoAmI && Main.projectile[k].type == Mod.Find<ModProjectile>("CleanserBeam").Type)
                {
                    Player.controlLeft = false;
                    Player.controlRight = false;
                    Player.controlUp = false;
                    Player.controlDown = false;
                    Player.controlJump = false;
                    break;
                }
            }
        }

        public override void PreUpdateBuffs()
        {
            if (saltLamp)
            {
                Player.AddBuff(Mod.Find<ModBuff>("SaltLamp").Type, 2, false);
            }
            if (heroLives > 0)
            {
                bool flag = false;
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && npc.type == Mod.Find<ModNPC>("PuritySpirit").Type)
                    {
                        flag = true;
                        PuritySpiritTeleport(npc);
                        break;
                    }
                }
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && IsChaosSpirit(npc.type))
                    {
                        flag = true;
                        ChaosSpiritWarning(npc);
                        break;
                    }
                }
                if (!flag && !Bluemagic.freezeHeroLives)
                {
                    heroLives = 0;
                }
                if (heroLives == 1)
                {
                    Player.AddBuff(Mod.Find<ModBuff>("HeroOne").Type, 2);
                }
                else if (heroLives == 2)
                {
                    Player.AddBuff(Mod.Find<ModBuff>("HeroTwo").Type, 2);
                }
                else if (heroLives == 3)
                {
                    Player.AddBuff(Mod.Find<ModBuff>("HeroThree").Type, 3);
                }
            }
            else
            {
                chaosWarningCooldown = 0;
            }
            if (purityDebuffCooldown > 0)
            {
                purityDebuffCooldown--;
            }
            if (terraLives > 0)
            {
                bool flag = false;
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && IsTerraSpirit(npc.type))
                    {
                        flag = true;
                        TerraSpiritBarrier(npc);
                        break;
                    }
                }
                if (!flag)
                {
                    terraLives = 0;
                }
            }
            if (blushieHealth > 0f)
            {
                if (BlushieBoss.BlushieBoss.Active)
                {
                    BlushieBarrier();
                }
                else
                {
                    blushieHealth = 0f;
                }
            }
            if (CursedMount())
            {
                if (Player.mount.Active)
                {
                    Player.mount.Dismount(Player);
                }
                Player.AddBuff(Mod.Find<ModBuff>("NoMount").Type, 5);
            }
            lastPos = Player.position;
        }

        public bool CursedMount()
        {
            return BlushieBoss.BlushieBoss.Players[Player.whoAmI] && BlushieBoss.BlushieBoss.Phase == 3 && (BlushieBoss.BlushieBoss.Phase3Attack > 7 || (BlushieBoss.BlushieBoss.Phase3Attack == 7 && BlushieBoss.BlushieBoss.Timer >= 2210));
        }

        public void CheckBadHeal()
        {
            if (prevLife >= 0 && badHeal && Player.statLife > prevLife)
            {
                int hurt = 2 * (Player.statLife - prevLife);
                Player.statLife -= hurt;
                CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.DamagedFriendly, hurt.ToString(), false, false);
                if (Player.statLife <= 0 && Player.whoAmI == Main.myPlayer)
                {
                    Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " was dissolved by holy powers"), hurt, 0, false);
                }
            }
            prevLife = -1;
        }

        public void StartBadHeal()
        {
            if (badHeal)
            {
                prevLife = Player.statLife;
            }
        }

        private void PuritySpiritTeleport(NPC npc)
        {
            int halfWidth = PuritySpirit.PuritySpirit.arenaWidth / 2;
            int halfHeight = PuritySpirit.PuritySpirit.arenaHeight / 2;
            Vector2 newPosition = Player.position;
            if (Player.position.X <= npc.Center.X - halfWidth)
            {
                newPosition.X = npc.Center.X + halfWidth - Player.width - 1;
                while (Collision.SolidCollision(newPosition, Player.width, Player.height))
                {
                    newPosition.X -= 16f;
                }
            }
            else if (Player.position.X + Player.width >= npc.Center.X + halfWidth)
            {
                newPosition.X = npc.Center.X - halfWidth + 1;
                while (Collision.SolidCollision(newPosition, Player.width, Player.height))
                {
                    newPosition.X += 16f;
                }
            }
            else if (Player.position.Y <= npc.Center.Y - halfHeight)
            {
                newPosition.Y = npc.Center.Y + halfHeight - Player.height - 1;
                while (Collision.SolidCollision(newPosition, Player.width, Player.height))
                {
                    newPosition.Y -= 16f;
                }
            }
            else if (Player.position.Y + Player.height >= npc.Center.Y + halfHeight)
            {
                newPosition.Y = npc.Center.Y - halfHeight + 1;
                while (Collision.SolidCollision(newPosition, Player.width, Player.height))
                {
                    newPosition.Y += 16f;
                }
            }
            if (newPosition != Player.position)
            {
                Player.Teleport(newPosition, 1, 0);
                NetMessage.SendData(65, -1, -1, null, 0, Player.whoAmI, newPosition.X, newPosition.Y, 1, 0, 0);
                PuritySpiritDebuff();
            }
        }

        internal void TerraSpiritBarrier(NPC npc)
        {
            Vector2 offset = Player.position - lastPos;
            if (offset.Length() > 32f)
            {
                offset.Normalize();
                offset *= 32f;
                Player.position = lastPos + offset;
            }
            int halfWidth = TerraSpirit.TerraSpirit.arenaWidth / 2;
            int halfHeight = TerraSpirit.TerraSpirit.arenaHeight / 2;
            bool spikes = npc.type == Mod.Find<ModNPC>("TerraSpirit2").Type;
            if (Player.position.X <= npc.Center.X - halfWidth)
            {
                Player.position.X = npc.Center.X - halfWidth;
                if (Player.velocity.X < 0f)
                {
                    Player.velocity.X = 0f;
                }
                if (spikes)
                {
                    TerraKill();
                }
            }
            else if (Player.position.X + Player.width >= npc.Center.X + halfWidth)
            {
                Player.position.X = npc.Center.X + halfWidth - Player.width;
                if (Player.velocity.X > 0f)
                {
                    Player.velocity.X = 0f;
                }
                if (spikes)
                {
                    TerraKill();
                }
            }
            if (Player.position.Y <= npc.Center.Y - halfHeight)
            {
                Player.position.Y = npc.Center.Y - halfHeight;
                if (Player.velocity.Y < 0f)
                {
                    Player.velocity.Y = 0f;
                }
                if (spikes)
                {
                    TerraKill();
                }
            }
            else if (Player.position.Y + Player.height >= npc.Center.Y + halfHeight)
            {
                Player.position.Y = npc.Center.Y + halfHeight - Player.height;
                if (Player.velocity.Y > 0f)
                {
                    Player.velocity.Y = 0f;
                }
                if (spikes)
                {
                    TerraKill();
                }
            }
        }

        internal void BlushieBarrier()
        {
            Vector2 offset = Player.position - lastPos;
            if (offset.Length() > 32f)
            {
                offset.Normalize();
                offset *= 32f;
                Player.position = lastPos + offset;
            }
            Vector2 origin = BlushieBoss.BlushieBoss.Origin;
            float arenaSize = BlushieBoss.BlushieBoss.ArenaSize;
            if (Player.position.X <= origin.X - arenaSize)
            {
                Player.position.X = origin.X - arenaSize;
                if (Player.velocity.X < 0f)
                {
                    Player.velocity.X = 0f;
                }
            }
            else if (Player.position.X + Player.width >= origin.X + arenaSize)
            {
                Player.position.X = origin.X + arenaSize - Player.width;
                if (Player.velocity.X > 0f)
                {
                    Player.velocity.X = 0f;
                }
            }
            if (Player.position.Y <= origin.Y - arenaSize)
            {
                Player.position.Y = origin.Y - arenaSize;
                if (Player.velocity.Y < 0f)
                {
                    Player.velocity.Y = 0f;
                }
            }
            else if (Player.position.Y + Player.height >= origin.Y + arenaSize)
            {
                Player.position.Y = origin.Y + arenaSize - Player.height;
                if (Player.velocity.Y > 0f)
                {
                    Player.velocity.Y = 0f;
                }
            }
        }

        public void PuritySpiritDebuff()
        {
            bool flag = true;
            if (Main.rand.Next(2) == 0)
            {
                flag = false;
                for (int k = 0; k < 2; k++)
                {
                    int buffType;
                    int buffTime;
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            buffType = BuffID.Darkness;
                            buffTime = 900;
                            break;
                        case 1:
                            buffType = BuffID.Cursed;
                            buffTime = 300;
                            break;
                        case 2:
                            buffType = BuffID.Confused;
                            buffTime = 600;
                            break;
                        case 3:
                            buffType = BuffID.Slow;
                            buffTime = 900;
                            break;
                        default:
                            buffType = BuffID.Silenced;
                            buffTime = 300;
                            break;
                    }
                    if (Main.expertMode)
                    {
                        buffTime = buffTime * 2 / 3;
                    }
                    if (!Player.buffImmune[buffType])
                    {
                        Player.AddBuff(buffType, buffTime);
                        return;
                    }
                }
            }
            if (flag || Main.expertMode || Main.rand.Next(2) == 0)
            {
                int buffTime = 1800;
                if (Main.expertMode)
                {
                    buffTime = 1200;
                }
                Player.AddBuff(Mod.Find<ModBuff>("Undead").Type, buffTime, false);
            }
            for (int k = 0; k < 25; k++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, Mod.Find<ModDust>("Negative").Type, 0f, -1f, 0, default(Color), 2f);
            }
        }

        private void ChaosSpiritWarning(NPC npc)
        {
            float distance = Vector2.Distance(Player.Center, npc.Center);
            if (distance > ChaosSpirit.ChaosSpirit.killRadius)
            {
                ChaosKill();
            }
            else if (distance > ChaosSpirit.ChaosSpirit.warningRadius)
            {
                if (chaosWarningCooldown <= 0)
                {
                    chaosWarningCooldown = 300;
                    Main.NewText("The air feels heavy too far away from the chaos...", 255, 255, 255);
                }
            }
            if (chaosWarningCooldown > 0)
            {
                chaosWarningCooldown--;
            }
        }

        public void ChaosKill()
        {
            int damage = 100 * Player.statLifeMax2;
            CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.DamagedFriendly, damage.ToString(), true, false);
            if (Player.whoAmI == Main.myPlayer)
            {
                Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " was crushed by chaotic pressure!"), damage, 0, false);
            }
        }

        public override void PostUpdateBuffs()
        {
            if (IsTransformed())
            {
                Nullify();
            }
            if (BlushieBoss.BlushieBoss.Players[Player.whoAmI])
            {
                noGodmode = true;
            }
        }

        public override void PostUpdateEquips()
        {
            if (IsTransformed())
            {
                Nullify();
            }
            if (elementShield)
            {
                if (elementShields > 0)
                {
                    elementShieldTimer--;
                    if (elementShieldTimer < 0)
                    {
                        elementShields--;
                        elementShieldTimer = 600;
                    }
                }
            }
            else
            {
                elementShields = 0;
                elementShieldTimer = 0;
            }
            elementShieldPos++;
            elementShieldPos %= 300;
            if (saltLamp)
            {
                Player.statDefense += 4;
            }
            chaosStats.Update(Player);
            if (Main.expertMode)
            {
                cataclysmStats.Update(Player);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (reviveTime > 0)
            {
                reviveTime--;
            }
            if (puriumShieldChargeMax > 0f)
            {
                ChargePuriumShield(0.001f);
                for (int k = 0; k < Player.MaxBuffs; k++)
                {
                    if (puriumShieldCharge < buffImmuneCost)
                    {
                        break;
                    }
                    if (Player.buffType[k] > 0 && Player.buffTime[k] > 0 && Main.debuff[Player.buffType[k]] && !BuffID.Sets.NurseCannotRemoveDebuff[Player.buffType[k]]/*BuffLoader.CanBeCleared(Player.buffType[k])*//* tModPorter Note: Removed. Use !BuffID.Sets.NurseCannotRemoveDebuff instead */ && !buffImmuneBlacklist.Contains(Player.buffType[k]))
                    {
                        //Check back for issues later
                        buffImmuneCounter[Player.buffType[k]] = 600;
                        puriumShieldCharge -= buffImmuneCost;
                    }
                }
                for (int k = 0; k < buffImmuneCounter.Length; k++)
                {
                    if (buffImmuneCounter[k] > 0)
                    {
                        Player.buffImmune[k] = true;
                        buffImmuneCounter[k]--;
                    }
                }
                Player.endurance += PuriumShieldEndurance();
            }
            CheckBadHeal();
            if (terraImmune > 0)
            {
                terraImmune--;
            }
            if (blushieImmune > 0)
            {
                blushieImmune--;
            }
            if (terraKill > 0)
            {
                terraKill--;
                Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " was torn apart by the force of Terraria!"), Player.statLifeMax2 * 100, 0, false);
            }
            origHealth = Player.statLifeMax2;
            if (blushieHealth > 0f)
            {
                Player.statLifeMax2 = (int)(blushieHealth * Player.statLifeMax2);
            }
        }

        private void ChargePuriumShield(float charge)
        {
            charge *= puriumShieldChargeRate;
            if (puriumShieldCharge + charge > puriumShieldChargeMax)
            {
                charge = puriumShieldChargeMax - puriumShieldCharge;
            }
            if (charge < 0f)
            {
                charge = 0f;
            }
            puriumShieldCharge += charge;
        }

        private float PuriumShieldEndurance()
        {
            float enduranceFill = puriumShieldCharge / reviveCost;
            if (enduranceFill > puriumShieldEnduranceMult)
            {
                enduranceFill = puriumShieldEnduranceMult;
            }
            return 0.2f * enduranceFill;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (liquified > 0)
            {
                if (Player.velocity.Y == 0 || Player.sliding)
                {
                    if (Player.velocity.X != 0f)
                    {
                        Player.velocity.X *= 0.8f;
                        if (Player.velocity.X < 0.1f && Player.velocity.X > -0.1f)
                        {
                            Player.velocity.X = 0f;
                        }
                        transformFrameCounter++;
                    }
                }
                else if (Player.wet)
                {
                    if (Player.controlLeft)
                    {
                        Player.velocity.X = -4f;
                    }
                    else if (Player.controlRight)
                    {
                        Player.velocity.X = 4f;
                    }
                    if (Player.controlJump)
                    {
                        Player.velocity.Y = -4f;
                    }
                }
                else
                {
                    if (Player.velocity.X > 0.1f)
                    {
                        Player.velocity.X = 4f;
                    }
                    if (Player.velocity.X < -0.1f)
                    {
                        Player.velocity.X = -4f;
                    }
                }
                transformFrameCounter++;
                transformFrameCounter %= 16;
            }
        }

        public override void PreUpdateMovement()
        {
            if (purityShieldMount)
            {
                if (Player.controlLeft)
                {
                    Player.velocity.X = -Mounts.PurityShield.speed;
                    Player.direction = -1;
                }
                else if (Player.controlRight)
                {
                    Player.velocity.X = Mounts.PurityShield.speed;
                    Player.direction = 1;
                }
                else
                {
                    Player.velocity.X = 0f;
                }
                if (Player.controlUp)
                {
                    Player.velocity.Y = -Mounts.PurityShield.speed;
                }
                else if (Player.controlDown)
                {
                    Player.velocity.Y = Mounts.PurityShield.speed;
                    Vector2 test = Collision.TileCollision(Player.position, Player.velocity, Player.width, Player.height, true, false, (int)Player.gravDir);
                    if (test.Y == 0f)
                    {
                        Player.velocity.Y = 0.5f;
                    }
                }
                else
                {
                    Player.velocity.Y = 0f;
                }
                if (Player.controlJump)
                {
                    Player.velocity *= 0.5f;
                }
            }
        }

        public override void PostUpdate()
        {
            StartBadHeal();
            miscTimer++;
            miscTimer %= 60;
            if (worldReaverCooldown > 0)
            {
                worldReaverCooldown--;
            }
            if (purityShieldMount)
            {
                Player.hairFrame.Y = 5 * Player.hairFrame.Height;
                Player.headFrame.Y = 5 * Player.headFrame.Height;
                Player.legFrame.Y = 5 * Player.legFrame.Height;
            }
        }

        public override void FrameEffects()
        {
            if (Player.inventory[Player.selectedItem].type == Mod.Find<ModItem>("DarkLightningPack").Type)
            {
                Player.back = EquipLoader.GetEquipSlot(Mod, "DarkLightningPack_Back", EquipType.Back);
            }
            if (IsTransformed())
            {
                Nullify();
            }
        }

        private bool IsTransformed()
        {
            return liquified > 0 || nullified;
        }

        private void Nullify()
        {
            bool saveNullified = this.nullified;
            int saveLiquified = this.liquified;
            Player.ResetEffects();
            Player.head = -1;
            Player.body = -1;
            Player.legs = -1;
            Player.handon = -1;
            Player.handoff = -1;
            Player.back = -1;
            Player.front = -1;
            Player.shoe = -1;
            Player.waist = -1;
            Player.shield = -1;
            Player.neck = -1;
            Player.face = -1;
            Player.balloon = -1;
            this.nullified = saveNullified;
            if (!this.nullified)
            {
                this.liquified = saveLiquified;
            }
        }

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            return !godmode;
        }

        public override bool CanBeHitByProjectile(Projectile projectile)
        {
            return !godmode;
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (npc.type == NPCID.DungeonSpirit && Main.rand.Next(4) == 0)
            {
                Player.AddBuff(Mod.Find<ModBuff>("EtherealFlames").Type, Main.rand.Next(120, 180), true);
            }
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (liquified != 0)
            {
                genGore = false;
                for (int k = 0; k < damage / (double)Player.statLifeMax2 * 100.0; k++)
                {
                    Color color = new Color(0, 220, 40, 100);
                    if (liquified == 2)
                    {
                        color = new Color(0, 80, 255, 100);
                    }
                    Dust.NewDust(Player.position, Player.width, Player.height, 4, hitDirection, -1f, 175, color, 1f);
                }
                playSound = false;
                SoundEngine.PlaySound(SoundID.NPCHit1, Player.position);
                int choice = Main.rand.Next(3);
                string deathText;
                if (choice == 0)
                {
                    deathText = " was reduced to a puddle of gel.";
                }
                else if (choice == 1)
                {
                    deathText = " was squished.";
                }
                else
                {
                    deathText = " was not a very good slime.";
                }
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + deathText);
            }
            if (constantDamage > 0 || percentDamage > 0f)
            {
                int damageFromPercent = (int)(Player.statLifeMax2 * percentDamage);
                damage = Math.Max(constantDamage, damageFromPercent);
                if (chaosDefense)
                {
                    double cap = Main.expertMode ? 75.0 : 50.0;
                    int reduction = (int)(cap * (1.0 - Math.Exp(-Player.statDefense / 150.0)));
                    if (reduction < 0)
                    {
                        reduction = Player.statDefense / 2;
                    }
                    damage -= reduction;
                    if (damage < 0)
                    {
                        damage = 1;
                    }
                }
                customDamage = true;
            }
            else if (defenseEffect >= 0f)
            {
                if (Main.expertMode)
                {
                    defenseEffect *= 1.5f;
                }
                damage -= (int)(Player.statDefense * defenseEffect);
                if (damage < 0)
                {
                    damage = 1;
                }
                customDamage = true;
            }
            constantDamage = 0;
            percentDamage = 0f;
            defenseEffect = -1f;
            chaosDefense = false;
            if (godmode)
            {
                damage = 0;
                customDamage = true;
            }
            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
        }

        public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (elementShield && damage > 1.0)
            {
                if (elementShields < 6)
                {
                    int k;
                    bool flag = false;
                    for (k = 3; k < 8 + Player.extraAccessorySlots; k++)
                    {
                        if (Player.armor[k].type == Mod.Find<ModItem>("SixColorShield").Type)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        Projectile.NewProjectile(Player.Center.X, Player.Center.Y, 0f, 0f, Mod.Find<ModProjectile>("ElementShield").Type, Player.GetWeaponDamage(Player.armor[k]), Player.GetWeaponKnockback(Player.armor[k], 2f), Player.whoAmI, elementShields++);
                    }
                }
                elementShieldTimer = 600;
            }
            if (crystalCloak)
            {
                for (int l = 0; l < 3; l++)
                {
                    Vector2 crystalPos = Player.Center + new Vector2(Main.rand.Next(-400, 401), -Main.rand.Next(500, 800));
                    Vector2 offset = Player.Center - crystalPos;
                    offset.X += (float)Main.rand.Next(-50, 51);
                    offset *= 28f / offset.Length();
                    int proj = Projectile.NewProjectile(crystalPos.X, crystalPos.Y, offset.X, offset.Y, Mod.Find<ModProjectile>("CrystalStar").Type, 100, 6f, Player.whoAmI, 0f, 0f);
                    Main.projectile[proj].ai[1] = Player.position.Y;
                }
            }
            if (puriumShieldChargeMax > 0f)
            {
                float effectiveEndurance = Player.endurance;
                if (effectiveEndurance >= 0.995f)
                {
                    effectiveEndurance = 0.995f;
                }
                double fullDamage = damage / (1f - effectiveEndurance);
                float shieldDamage = (float)(fullDamage * PuriumShieldEndurance() * 0.1f);
                puriumShieldCharge -= shieldDamage;
                if (puriumShieldCharge < 0f)
                {
                    puriumShieldCharge = 0f;
                }
            }
            if (heroLives > 0)
            {
                for (int k = 0; k < 200; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.active && npc.type == Mod.Find<ModNPC>("PuritySpirit").Type)
                    {
                        PuritySpirit.PuritySpirit modNPC = (PuritySpirit.PuritySpirit)npc.ModNPC;
                        if (modNPC.attack >= 0)
                        {
                            double proportion = damage / Player.statLifeMax2;
                            if (proportion > 1.0)
                            {
                                proportion = 1.0;
                            }
                            modNPC.attackWeights[modNPC.attack] += (int)(proportion * 400);
                            if (modNPC.attackWeights[modNPC.attack] > PuritySpirit.PuritySpirit.maxAttackWeight)
                            {
                                modNPC.attackWeights[modNPC.attack] = PuritySpirit.PuritySpirit.maxAttackWeight;
                            }
                            if (nullified && modNPC.attack != 2)
                            {
                                modNPC.attackWeights[2] += (int)(proportion * 200);
                                if (modNPC.attackWeights[2] > PuritySpirit.PuritySpirit.maxAttackWeight)
                                {
                                    modNPC.attackWeights[2] = PuritySpirit.PuritySpirit.maxAttackWeight;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            if (!pvp && crystalCloak)
            {
                int immuneTime = 0;
                if (damage == 1.0)
                {
                    immuneTime = 40;
                }
                else
                {
                    immuneTime = 80;
                }
                if (Player.immuneTime == immuneTime)
                {
                    Player.immuneTime += 20;
                }
                for (int k = 0; k < Player.hurtCooldowns.Length; k++)
                {
                    if (Player.hurtCooldowns[k] == immuneTime)
                    {
                        Player.hurtCooldowns[k] += 20;
                    }
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (godmode)
            {
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            if (puriumShieldChargeMax > 0f && puriumShieldCharge >= reviveCost)
            {
                puriumShieldCharge -= reviveCost;
                if (Player.statLife < 1)
                {
                    Player.statLife = 1;
                }
                StartBadHeal();
                Player.immune = true;
                Player.immuneTime = Player.longInvince ? 180 : 120;
                if (crystalCloak)
                {
                    Player.immuneTime += 60;
                }
                for (int k = 0; k < Player.hurtCooldowns.Length; k++)
                {
                    Player.hurtCooldowns[k] = Player.longInvince ? 180 : 120;
                }
                return false;
            }
            if (heroLives > 1)
            {
                heroLives--;
                if (Main.netMode == 1)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MessageType.HeroLives);
                    packet.Write(Player.whoAmI);
                    packet.Write(heroLives);
                    packet.Send();
                }
                Player.statLife = Player.statLifeMax2;
                Player.HealEffect(Player.statLifeMax2);
                StartBadHeal();
                Player.immune = true;
                Player.immuneTime = Player.longInvince ? 180 : 120;
                if (crystalCloak)
                {
                    Player.immuneTime += 60;
                }
                for (int k = 0; k < Player.hurtCooldowns.Length; k++)
                {
                    Player.hurtCooldowns[k] = Player.longInvince ? 180 : 120;
                }
                SoundEngine.PlaySound(SoundID.Item29, Player.position);
                reviveTime = 60;
                return false;
            }
            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (healHurt > 0)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was dissolved by holy powers");
                }
                else if (chaosPressure > 0)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " was crushed by chaotic pressure");
                }
                else if (blushieImmune > 0)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(Player.name + " succumbed to the might of the blushie!");
                }
            }
            return true;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            Item salt = new Item();
            salt.SetDefaults(Mod.Find<ModItem>("Salt").Type);
            int npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("Abomination").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 25 + 25 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            int count = NPC.CountNPCS(Mod.Find<ModNPC>("CaptiveElement2").Type);
            if (count > 0)
            {
                salt.stack = 50 + 5 * (5 - count);
                Player.GetItem(Player.whoAmI, salt);
            }
            npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("Phantom").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 50 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("PuritySpirit").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 50 + 50 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("ChaosSpirit").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 50 + 25 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("ChaosSpirit2").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 75 + 50 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            npc = NPC.FindFirstNPC(Mod.Find<ModNPC>("ChaosSpirit3").Type);
            if (npc >= 0)
            {
                NPC ent = Main.npc[npc];
                salt.stack = 125 + 25 * (ent.lifeMax - ent.life) / ent.lifeMax;
                Player.GetItem(Player.whoAmI, salt);
            }
            if (NPC.AnyNPCs(Mod.Find<ModNPC>("TerraSpirit").Type))
            {
                salt.SetDefaults(Mod.Find<ModItem>("PureSalt").Type);
                salt.stack = 2 * BluemagicWorld.terraDeaths;
                if (salt.stack > 999)
                {
                    salt.stack = 999;
                }
                Player.GetItem(Player.whoAmI, salt);
            }
        }

        public void TerraKill()
        {
            if (terraLives <= 0 || terraImmune > 0 || Main.netMode == 2)
            {
                return;
            }
            int damage = 100 * Player.statLifeMax2;
            CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.DamagedFriendly, damage.ToString(), true, false);
            terraLives--;
            if (terraLives == 0 && Bluemagic.testing)
            {
                Main.NewText("YOU LOSE! Lucky you're just the dev whose testing her fight.", 255, 25, 25);
                terraLives = 1;
            }
            if (Main.netMode == 1)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)MessageType.TerraLives);
                packet.Write(Player.whoAmI);
                packet.Write(terraLives);
                packet.Send();
            }
            if (Main.netMode == 0)
            {
                string text;
                if (terraLives == 1)
                {
                    text = Language.GetTextValue("Mods.Bluemagic.LifeLeft", Player.name);
                }
                else
                {
                    text = Language.GetTextValue("Mods.Bluemagic.LivesLeft", Player.name, terraLives);
                }
                Main.NewText(text, 255, 25, 25);
            }
            if (terraLives > 0)
            {
                Player.statLife = Player.statLifeMax2;
                Player.HealEffect(Player.statLifeMax2);
                terraImmune = 60;
                if (!Player.immune)
                {
                    Player.immune = true;
                    Player.immuneTime = 60;
                }
            }
            else if (Main.myPlayer == Player.whoAmI)
            {
                terraKill = 10;
                for (int k = 0; k < 10; k++)
                {
                    if (Player.dead)
                    {
                        terraKill = 0;
                        break;
                    }
                    Player.KillMe(PlayerDeathReason.ByCustomReason(Player.name + " was torn apart by the force of Terraria!"), Player.statLifeMax2 * 100, 0, false);
                }
            }
        }

        public void BlushieDamage(float bulletDamage)
        {
            if (blushieHealth <= 0f || blushieImmune > 0 || Main.netMode == 2)
            {
                return;
            }
            int oldHealth = Player.statLife;
            blushieHealth -= bulletDamage;
            Player.statLife = (int)(Player.statLifeMax2 * blushieHealth);
            int constDamage = (int)((200f - Player.statDefense / 2f) * (1f - Player.endurance));
            if (constDamage < 1)
            {
                constDamage = 1;
            }
            if (Player.statLife > oldHealth - constDamage)
            {
                Player.statLife = oldHealth - constDamage;
            }
            int damage = oldHealth - Player.statLife;
            CombatText.NewText(new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), CombatText.DamagedFriendly, damage.ToString(), true, false);
            if (Player.Male)
            {
                SoundEngine.PlaySound(SoundID.PlayerHit, Player.position);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.FemaleHit, Player.position);
            }
            if (blushieHealth > 0f && Player.statLife > 0)
            {
                blushieImmune = 60;
                if (!Player.immune)
                {
                    Player.immune = true;
                    Player.immuneTime = 60;
                }
            }
            else if (Main.myPlayer == Player.whoAmI)
            {
                if (Bluemagic.testing)
                {
                    Main.NewText("YOU LOSE! Lucky that it's not possible for the dev to defeat herself when testing.");
                    blushieHealth = 0.05f;
                    Player.statLife = 1;
                    blushieImmune = 60;
                    return;
                }
                bool playSound = true;
                bool genGore = true;
                PlayerDeathReason damageSource = PlayerDeathReason.ByCustomReason(Player.name + " succumbed to the might of the blushie!");
                PlayerLoader.PreKill(Player, damage, 0, false, ref playSound, ref genGore, ref damageSource);
                damageSource = PlayerDeathReason.ByCustomReason(Player.name + " succumbed to the might of the blushie!");
                Player.lastDeathPostion = Player.Center;
                Player.lastDeathTime = DateTime.Now;
                Player.showLastDeath = true;
                bool flag;
                int coinsOwned = (int)Utils.CoinsCount(out flag, Player.inventory, new int[0]);
                Player.lostCoins = coinsOwned;
                Player.lostCoinString = Main.ValueToCoins(Player.lostCoins);
                Main.mapFullscreen = false;
                Player.trashItem.SetDefaults(0, false);
                if (Player.difficulty == 0)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        Item item = Player.inventory[i];
                        if (item.stack > 0 && ((item.type >= 1522 && item.type <= 1527) || item.type == 3643))
                        {
                            int num = Item.NewItem((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, item.type, 1, false, 0, false, false);
                            Main.item[num].netDefaults(item.netID);
                            Main.item[num].Prefix(item.prefix);
                            Main.item[num].stack = item.stack;
                            Main.item[num].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[num].velocity.X = Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[num].noGrabDelay = 100;
                            Main.item[num].favorited = false;
                            Main.item[num].newAndShiny = false;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(21, -1, -1, null, num, 0f, 0f, 0f, 0, 0, 0);
                            }
                            item.SetDefaults(0, false);
                        }
                    }
                }
                else if (Player.difficulty == 1)
                {
                    Player.DropItems();
                }
                else if (Player.difficulty == 2)
                {
                    Player.DropItems();
                    Player.KillMeForGood();
                }
                if (playSound)
                {
                    SoundEngine.PlaySound(SoundID.PlayerKilled, Player.position);
                }
                Player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
                Player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
                Player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
                Player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f;
                Player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f;
                Player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f;
                if (Player.stoned || !genGore)
                {
                    Player.headPosition = Vector2.Zero;
                    Player.bodyPosition = Vector2.Zero;
                    Player.legPosition = Vector2.Zero;
                }
                if (genGore)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (Player.stoned)
                        {
                            Dust.NewDust(Player.position, Player.width, Player.height, 1, 0f, -2f, 0, default(Color), 1f);
                        }
                        else if (Player.frostArmor)
                        {
                            int num2 = Dust.NewDust(Player.position, Player.width, Player.height, 135, 0f, -2f, 0, default(Color), 1f);
                            Main.dust[num2].shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                        }
                        else if (Player.boneArmor)
                        {
                            int num3 = Dust.NewDust(Player.position, Player.width, Player.height, 26, 0f, -2f, 0, default(Color), 1f);
                            Main.dust[num3].shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                        }
                        else
                        {
                            Dust.NewDust(Player.position, Player.width, Player.height, 5, 0f, -2f, 0, default(Color), 1f);
                        }
                    }
                }
                Player.mount.Dismount(Player);
                Player.dead = true;
                Player.respawnTimer = 600;
                if (Main.expertMode)
                {
                    Player.respawnTimer = 900;
                }
                PlayerLoader.Kill(Player, damage, 0, false, damageSource);
                Player.immuneAlpha = 0;
                Player.palladiumRegen = false;
                Player.iceBarrier = false;
                Player.crystalLeaf = false;
                NetworkText deathText = damageSource.GetDeathText(Player.name);
                if (Main.netMode == 0)
                {
                    Main.NewText(deathText.ToString(), 225, 25, 25, false);
                }
                else if (Main.netMode == 1)
                {
                    NetMessage.SendPlayerDeath(Player.whoAmI, damageSource, damage, 0, false, -1, -1);
                }
                if (Player.difficulty == 0)
                {
                    Player.DropCoins();
                }
                Player.DropTombstone(coinsOwned, deathText, 0);
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (item.CountsAsClass(DamageClass.Melee) && !item.noMelee && !item.noUseGraphic && customMeleeEnchant > 0)
            {
                if (customMeleeEnchant == 1)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Mod.Find<ModDust>("EtherealFlame").Type, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.7f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }
                }
                else if (customMeleeEnchant == 2)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 135, Player.velocity.X * 0.2f + Player.direction * 3f, Player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.7f;
                        Main.dust[dust].velocity.Y -= 0.5f;
                    }
                }
            }
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            if (Main.rand.NextFloat() < ammoCost)
            {
                return false;
            }
            return base.CanConsumeAmmo(weapon, ammo);
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (suppression > 0f)
            {
                damage = (int)(damage * (1f - suppression));
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (customMeleeEnchant == 1)
            {
                target.AddBuff(Mod.Find<ModBuff>("EtherealFlames").Type, 60 * Main.rand.Next(3, 7), false);
            }
            else if (customMeleeEnchant == 2)
            {
                target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 7), false);
            }
            if (puriumShieldChargeMax > 0f && target.lifeMax > 5 && target.type != NPCID.TargetDummy)
            {
                ChargePuriumShield(damage * puriumShieldDamageEffectiveness);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (suppression > 0f)
            {
                damage = (int)(damage * (1f - suppression));
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.CountsAsClass(DamageClass.Melee) && !proj.noEnchantments)
            {
                if (customMeleeEnchant == 1)
                {
                    target.AddBuff(Mod.Find<ModBuff>("EtherealFlames").Type, 60 * Main.rand.Next(3, 7), false);
                }
                else if (customMeleeEnchant == 2)
                {
                    target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 7), false);
                }
            }
            if (puriumShieldChargeMax > 0f && target.lifeMax > 5 && target.type != NPCID.TargetDummy)
            {
                ChargePuriumShield(damage * puriumShieldDamageEffectiveness);
            }
        }

        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if (customMeleeEnchant == 1)
            {
                target.AddBuff(Mod.Find<ModBuff>("EtherealFlames").Type, 60 * Main.rand.Next(3, 7), true);
            }
            else if (customMeleeEnchant == 2)
            {
                target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 7), true);
            }
            if (puriumShieldChargeMax > 0f)
            {
                ChargePuriumShield(damage * puriumShieldDamageEffectiveness);
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if(proj.CountsAsClass(DamageClass.Melee) && !proj.noEnchantments)
            {
                if (customMeleeEnchant == 1)
                {
                    target.AddBuff(Mod.Find<ModBuff>("EtherealFlames").Type, 60 * Main.rand.Next(3, 7), true);
                }
                else if (customMeleeEnchant == 2)
                {
                    target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 7), true);
                }
            }
            if (puriumShieldChargeMax > 0f)
            {
                ChargePuriumShield(damage * puriumShieldDamageEffectiveness);
            }
        }

        public override void ModifyScreenPosition()
        {
            if (BlushieBoss.BlushieBoss.Players[Main.myPlayer] && BlushieBoss.BlushieBoss.CameraFocus)
            {
                Vector2 origin = BlushieBoss.BlushieBoss.Origin;
                Main.screenPosition = origin - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (eFlames)
            {
                if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, Mod.Find<ModDust>("EtherealFlame").Type, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.1f;
                g *= 0.2f;
                b *= 0.7f;
                fullBright = true;
            }
        }

        public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("Bluemagic", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.dead)
            {
                return;
            }
            Mod mod = ModLoader.GetMod("Bluemagic");
            BluemagicPlayer modPlayer = drawPlayer.GetModPlayer<BluemagicPlayer>();
            if (modPlayer.reviveTime > 0)
            {
                Texture2D texture = mod.GetTexture("PuritySpirit/Revive");
                int drawX = (int)(drawInfo.Position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.Position.Y + drawPlayer.height / 4f - 60f + modPlayer.reviveTime - Main.screenPosition.Y);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * (modPlayer.reviveTime / 60f), 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
        });
        public static readonly PlayerLayer MiscEffects = new PlayerLayer("Bluemagic", "MiscEffects", PlayerLayer.MiscEffectsFront, delegate(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.dead)
            {
                return;
            }
            Mod mod = ModLoader.GetMod("Bluemagic");
            BluemagicPlayer modPlayer = drawPlayer.GetModPlayer<BluemagicPlayer>();
            if (modPlayer.badHeal)
            {
                Texture2D texture = mod.GetTexture("Buffs/PuritySpirit/Skull");
                int drawX = (int)(drawInfo.Position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.Position.Y - 4f - Main.screenPosition.Y);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Lighting.GetColor((int)((drawInfo.Position.X + drawPlayer.width / 2f) / 16f), (int)((drawInfo.Position.Y - 4f - texture.Height / 2f) / 16f)), 0f, new Vector2(texture.Width / 2f, texture.Height), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
                for (int k = 0; k < 2; k++)
                {
                    int dust = Dust.NewDust(new Vector2(drawInfo.Position.X + drawPlayer.width / 2f - texture.Width / 2f, drawInfo.Position.Y - 4f - texture.Height), texture.Width, texture.Height, mod.Find<ModDust>("Smoke").Type, 0f, 0f, 0, Color.Black);
                    Main.dust[dust].velocity += drawPlayer.velocity * 0.25f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (modPlayer.puriumShieldChargeMax > 0f && modPlayer.puriumShieldCharge > 0f && !modPlayer.purityShieldMount)
            {
                Texture2D texture = mod.GetTexture("PuriumShield");
                int drawX = (int)(drawInfo.Position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.Position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                float strength = (modPlayer.miscTimer % 30f) / 15f;
                if (strength > 1f)
                {
                    strength = 2f - strength;
                }
                strength = 0.1f + strength * 0.2f;
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * strength, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                data.shader = drawInfo.cBody;
                Main.playerDrawData.Add(data);
            }
            if (modPlayer.purityShieldMount)
            {
                Texture2D texture = mod.GetTexture("Mounts/PurityShield");
                int drawX = (int)(drawInfo.Position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.Position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                float strength = (modPlayer.miscTimer % 30f) / 15f;
                if (strength > 1f)
                {
                    strength = 2f - strength;
                }
                strength = 0.4f + strength * 0.2f;
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White * strength, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                data.shader = drawInfo.drawPlayer.miscDyes[3].dye;
                Main.playerDrawData.Add(data);
            }
            if (modPlayer.blushieHealth > 0f && drawPlayer.whoAmI == Main.myPlayer)
            {
                Texture2D texture = mod.GetTexture(BlushieBoss.BlushieBoss.CameraFocus ? "BlushieBoss/IndicatorBig" : "BlushieBoss/Indicator");
                DrawData data = new DrawData(texture, drawPlayer.Center - new Vector2(texture.Width / 2, texture.Height / 2) - Main.screenPosition, Color.White);
                Main.playerDrawData.Add(data);
            }
        });
        public static readonly PlayerLayer RedLine = new PlayerLayer("Bluemagic", "RedLine", delegate(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.whoAmI != Main.myPlayer || drawPlayer.itemAnimation > 0 || Main.gamePaused)
            {
                return;
            }
            Mod mod = ModLoader.GetMod("Bluemagic");
            if (drawPlayer.inventory[drawPlayer.selectedItem].type == mod.Find<ModItem>("CleanserBeam").Type)
            {
                Texture2D texture = mod.GetTexture("RedLine");
                Vector2 origin = drawPlayer.RotatedRelativePoint(drawPlayer.MountedCenter, true);
                Vector2 mousePos = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                Vector2 unit = mousePos - origin;
                float length = unit.Length();
                unit.Normalize();
                float rotation = unit.ToRotation();
                for (float k = 16f; k <= length; k += 32f)
                {
                    Vector2 drawPos = origin + unit * k - Main.screenPosition;
                    Main.playerDrawData.Add(new DrawData(texture, drawPos, null, Color.White, rotation, new Vector2(2f, 8f), 1f, SpriteEffects.None, 0));
                }
            }
        });
        public static readonly PlayerLayer PreHeldItem = new PlayerLayer("Bluemagic", "PreHeldItem", PlayerLayer.HeldItem, delegate(PlayerDrawSet drawInfo)
        {
            Mod mod = Bluemagic.Instance;
            TextureAssets.Item[mod.Find<ModItem>("ChaosCrystal").Type].Value = mod.GetTexture("Items/ChaosSpirit/ChaosCrystalNoAnim");
            TextureAssets.Item[mod.Find<ModItem>("CataclysmCrystal").Type].Value = mod.GetTexture("Items/ChaosSpirit/CataclysmCrystalNoAnim");
        });
        public static readonly PlayerLayer PostHeldItem = new PlayerLayer("Bluemagic", "PostHeldItem", PlayerLayer.HeldItem, delegate(PlayerDrawSet drawInfo)
        {
            Mod mod = Bluemagic.Instance;
            TextureAssets.Item[mod.Find<ModItem>("ChaosCrystal").Type].Value = mod.GetTexture("Items/ChaosSpirit/ChaosCrystal");
            TextureAssets.Item[mod.Find<ModItem>("CataclysmCrystal").Type].Value = mod.GetTexture("Items/ChaosSpirit/CataclysmCrystal");
        });
        public static readonly PlayerLayer Transform = new PlayerLayer("Bluemagic", "Transform", delegate (PlayerDrawSet info)
        {
            BluemagicPlayer modPlayer = info.drawPlayer.GetModPlayer<BluemagicPlayer>();
            if (modPlayer.liquified > 0)
            {
                if (!Main.NPCLoaded[1])
                {
                    Main.instance.LoadNPC(1);
                }
                Color color = new Color(0, 220, 40, 100);
                if (modPlayer.liquified == 2)
                {
                    color = new Color(0, 80, 255, 100);
                }
                int frame = 0;
                if (modPlayer.transformFrameCounter >= 8)
                {
                    frame = 26;
                }
                float alpha = 175f;
                Color lightColor = Lighting.GetColor((int)(info.drawPlayer.Center.X / 16), (int)(info.drawPlayer.Center.Y / 16));
                float transp = (255f - alpha) / 255f;
                int r = (int)(lightColor.R * transp);
                int g = (int)(lightColor.G * transp);
                int b = (int)(lightColor.B * transp);
                int a = (int)(lightColor.A - alpha);
                if (a < 0)
                {
                    a = 0;
                }
                DrawData item = new DrawData(TextureAssets.Npc[1].Value, info.drawPlayer.position - Main.screenPosition, new Rectangle(0, frame, 32, 26), new Color(r, g, b, a));
                Main.playerDrawData.Add(item);
                r = (int)(lightColor.R - (255 - color.R));
                g = (int)(lightColor.G - (255 - color.G));
                b = (int)(lightColor.B - (255 - color.B));
                a = (int)(lightColor.A - (255 - color.A));
                if (r < 0)
                {
                    r = 0;
                }
                if (g < 0)
                {
                    g = 0;
                }
                if (b < 0)
                {
                    b = 0;
                }
                if (a < 0)
                {
                    a = 0;
                }
                item = new DrawData(TextureAssets.Npc[1].Value, info.drawPlayer.position - Main.screenPosition, new Rectangle(0, frame, 32, 26), new Color(r, g, b, a));
                Main.playerDrawData.Add(item);
            }
        });

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            RedLine.visible = true;
            layers.Add(RedLine);
            for (int k = 0; k < layers.Count; k++)
            {
                if (layers[k] == PlayerLayer.HeldItem)
                {
                    layers.Insert(k, PreHeldItem);
                    k++;
                    layers.Insert(k + 1, PostHeldItem);
                    k++;
                }
            }
            if (!nullified && IsTransformed())
            {
                foreach (PlayerLayer layer in layers)
                {
                    if (layer != PlayerLayer.MiscEffectsBack && layer != PlayerLayer.MiscEffectsFront)
                    {
                        layer.visible = false;
                    }
                }
                Transform.visible = true;
                layers.Add(Transform);
            }
            MiscEffectsBack.visible = true;
            layers.Insert(0, MiscEffectsBack);
            MiscEffects.visible = true;
            layers.Add(MiscEffects);
        }
    }
}
