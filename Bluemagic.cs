using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Bluemagic.BlushieBoss;
using Bluemagic.ChaosSpirit;
using Bluemagic.Interface;
using Bluemagic.PuritySpirit;
using Bluemagic.TerraSpirit;
using Bluemagic.Tiles;

namespace Bluemagic
{
    public class Bluemagic : Mod
    {
        public static Mod Instance;
        public static Mod Calamity;
        public static Mod Thorium;
        public static Mod Sushi;
        public static Mod HealthBars;
        public const bool testing = false;

        private static Color pureColor = new Color(100, 255, 100);
        private static int pureColorStyle = 0;

        public const string captiveElementHead = "Bluemagic/Abomination/CaptiveElement_Head_Boss_";
        public const string captiveElement2Head = "Bluemagic/Abomination/CaptiveElement2_Head_Boss_";
        public static bool freezeHeroLives = false;

        public static Color PureColor
        {
            get
            {
                return pureColor;
            }
        }

        public Bluemagic()
        {
            ContentAutoloadingEnabled = true;
            GoreAutoloadingEnabled = true;
            MusicAutoloadingEnabled = true;
            
            //Properties/* tModPorter Note: Removed. Instead, assign the properties directly (ContentAutoloadingEnabled, GoreAutoloadingEnabled, MusicAutoloadingEnabled, and BackgroundAutoloadingEnabled) */ = new ModProperties()
            /*{
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
            */
        }

        public override void Load()
        {
            Instance = this;
            foreach (Mod checkMod in ModLoader.Mods)
            {
                if (checkMod.Name == "ExampleMod")
                {
                    throw new Exception("ExampleMod and Bluemagic cannot be loaded at the same time");
                }
            }
            InterfaceHelper.Initialize();
            EquipLoader.AddEquipTexture(this, "Bluemagic/Blushie/DarkLightningPack_Back", EquipType.Back, name: "DarkLightningPack_Back");
            for (int k = 1; k <= 4; k++)
            {
                AddBossHeadTexture(captiveElementHead + k);
                AddBossHeadTexture(captiveElement2Head + k);
            }
            Filters.Scene["Bluemagic:PuritySpirit"] = new Filter(new PuritySpiritScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.9f, 0.4f).UseOpacity(0.7f), EffectPriority.VeryHigh);
            SkyManager.Instance["Bluemagic:PuritySpirit"] = new PuritySpiritSky();
            Filters.Scene["Bluemagic:MonolithVoid"] = new Filter(new ScreenShaderData("FilterMoonLord"), EffectPriority.Medium);
            SkyManager.Instance["Bluemagic:MonolithVoid"] = new VoidSky();
            Filters.Scene["Bluemagic:ChaosSpirit"] = new Filter(new ChaosSpiritScreenShaderData("FilterMiniTower").UseColor(0.9f, 0.4f, 0.4f).UseOpacity(0.25f), EffectPriority.VeryHigh);
            SkyManager.Instance["Bluemagic:ChaosSpirit"] = new ChaosSpiritSky();
            Filters.Scene["Bluemagic:TerraSpirit"] = new Filter(new TerraSpiritScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0.1f), EffectPriority.VeryHigh);
            SkyManager.Instance["Bluemagic:TerraSpirit"] = new TerraSpiritSky();
            SkyManager.Instance["Bluemagic:BlushieBoss"] = new BlushieSky();
            Overlays.Scene["Bluemagic:WorldReaver"] = new WorldReaverOverlay();
            if (!Main.dedServ)
            {
                Filters.Scene["Bluemagic:WorldReaver"] = new Filter(new ScreenShaderData(new Ref<Effect>(GetEffect("Effects/WorldReaver")), "WorldReaver"), EffectPriority.VeryHigh);
            }
        }

        public override void PostSetupContent()
        {
            Mod bossList = ModLoader.GetMod("BossChecklist");
            if (bossList != null)
            {
                bossList.Call(
                    "AddBoss",
                    12.05f,
                    Bluemagic.Instance.Find<ModNPC>("Phantom").Type,
                    this,
                    "The Phantom",
                    (Func<bool>)(() => BluemagicWorld.downedPhantom),
                    Bluemagic.Instance.Find<ModItem>("PaladinEmblem").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("PhantomTrophy").Type, Bluemagic.Instance.Find<ModItem>("PhantomMask").Type},
                    new List<int> {Bluemagic.Instance.Find<ModItem>("PhantomBag").Type, Bluemagic.Instance.Find<ModItem>("PhantomPlate").Type, Bluemagic.Instance.Find<ModItem>("PhantomBlade").Type, Bluemagic.Instance.Find<ModItem>("SpectreGun").Type, Bluemagic.Instance.Find<ModItem>("PhantomSphere").Type, Bluemagic.Instance.Find<ModItem>("PaladinStaff").Type, ItemID.GreaterHealingPotion},
                    string.Format("Use a [i:{0}] in the Dungeon (Plantera must be defeated)", Find<ModItem>("PaladinEmblem").Type));
                bossList.Call(
                    "AddBoss",
                    12.8f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("Abomination").Type, Bluemagic.Instance.Find<ModNPC>("CaptiveElement").Type, Bluemagic.Instance.Find<ModNPC>("CaptiveElement2").Type},
                    this,
                    "The Abomination",
                    (Func<bool>)(() => BluemagicWorld.downedAbomination),
                    Bluemagic.Instance.Find<ModItem>("FoulOrb").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("AbominationTrophy").Type, Bluemagic.Instance.Find<ModItem>("AbominationMask").Type},
                    new List<int> {Bluemagic.Instance.Find<ModItem>("AbominationBag").Type, Bluemagic.Instance.Find<ModItem>("MoltenDrill").Type, Bluemagic.Instance.Find<ModItem>("DimensionalChest").Type, Bluemagic.Instance.Find<ModItem>("MoltenBar").Type, Bluemagic.Instance.Find<ModItem>("SixColorShield").Type, ItemID.GreaterHealingPotion},
                    string.Format("Use a [i:{0}] in the Underworld (Plantera must be defeated)", Find<ModItem>("FoulOrb").Type));
                bossList.Call(
                    "AddBoss",
                    14.5f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("Abomination").Type, Bluemagic.Instance.Find<ModNPC>("CaptiveElement").Type, Bluemagic.Instance.Find<ModNPC>("CaptiveElement2").Type},
                    this,
                    "The Abomination (Rematch)",
                    (Func<bool>)(() => BluemagicWorld.elementalUnleash),
                    Bluemagic.Instance.Find<ModItem>("FoulOrb").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("AbominationTrophy").Type, Bluemagic.Instance.Find<ModItem>("AbominationMask").Type},
                    new List<int> {Bluemagic.Instance.Find<ModItem>("AbominationBag2").Type, Bluemagic.Instance.Find<ModItem>("MoltenDrill").Type, Bluemagic.Instance.Find<ModItem>("DimensionalChest").Type, Bluemagic.Instance.Find<ModItem>("MoltenBar").Type, Bluemagic.Instance.Find<ModItem>("SixColorShield").Type, Bluemagic.Instance.Find<ModItem>("ElementalEye").Type, Bluemagic.Instance.Find<ModItem>("ElementalYoyo").Type, Bluemagic.Instance.Find<ModItem>("ElementalSprayer").Type, Bluemagic.Instance.Find<ModItem>("EyeballTome").Type, Bluemagic.Instance.Find<ModItem>("ElementalStaff").Type, Bluemagic.Instance.Find<ModItem>("EyeballGlove").Type, ItemID.GreaterHealingPotion},
                    string.Format("Use a [i:{0}] in the Underworld (Moon Lord must be defeated). [c/FF0000:Starts the Elemental Unleash!]", Find<ModItem>("FoulOrb").Type));
                bossList.Call(
                    "AddBoss",
                    16f,
                    Bluemagic.Instance.Find<ModNPC>("PuritySpirit").Type,
                    this,
                    "The Spirit of Purity",
                    (Func<bool>)(() => BluemagicWorld.downedPuritySpirit),
                    Bluemagic.Instance.Find<ModItem>("ElementalPurge").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("PuritySpiritTrophy").Type, Bluemagic.Instance.Find<ModItem>("BunnyTrophy").Type, Bluemagic.Instance.Find<ModItem>("TreeTrophy").Type, Bluemagic.Instance.Find<ModItem>("PuritySpiritMask").Type, Bluemagic.Instance.Find<ModItem>("BunnyMask").Type},
                    new List<int> {Bluemagic.Instance.Find<ModItem>("InfinityCrystal").Type, Bluemagic.Instance.Find<ModItem>("PuritySpiritBag").Type, ItemID.SuperHealingPotion},
                    string.Format("Kill a Bunny while the Bunny is standing in front of a placed [i:{0}]", Find<ModItem>("ElementalPurge").Type));
                bossList.Call(
                    "AddBoss",
                    18f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("ChaosSpirit").Type, Bluemagic.Instance.Find<ModNPC>("ChaosSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("ChaosSpirit3").Type},
                    this,
                    "The Spirit of Chaos",
                    (Func<bool>)(() => BluemagicWorld.downedChaosSpirit),
                    Bluemagic.Instance.Find<ModItem>("RitualOfEndings").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("ChaosTrophy").Type, Bluemagic.Instance.Find<ModItem>("CataclysmTrophy").Type, Bluemagic.Instance.Find<ModItem>("ChaosSpiritMask").Type, Bluemagic.Instance.Find<ModItem>("CataclysmMask").Type},
                    new List<int> {Bluemagic.Instance.Find<ModItem>("ChaosCrystal").Type, Bluemagic.Instance.Find<ModItem>("CataclysmCrystal").Type, ItemID.SuperHealingPotion},
                    string.Format("Use a [i:{0}] anytime, anywhere (has infinite reuses)", Find<ModItem>("RitualOfEndings").Type));
                bossList.Call(
                    "AddBoss",
                    42f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("TerraSpirit").Type, Bluemagic.Instance.Find<ModNPC>("TerraSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe3").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe4").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe5").Type},
                    this,
                    "????? (Phase 1)",
                    (Func<bool>)(() => BluemagicWorld.terraCheckpoint1 > 0),
                    Bluemagic.Instance.Find<ModItem>("RitualOfBunnies").Type,
                    null,
                    Bluemagic.Instance.Find<ModItem>("Checkpoint1").Type,
                    string.Format("Use a [i:{0}] anytime, anywhere, after all previous bosses have been defeated (has infinite reuses)", Find<ModItem>("RitualOfBunnies").Type));
                bossList.Call(
                    "AddBoss",
                    256f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("TerraSpirit").Type, Bluemagic.Instance.Find<ModNPC>("TerraSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe3").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe4").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe5").Type},
                    this,
                    "????? (Phase 2)",
                    (Func<bool>)(() => BluemagicWorld.terraCheckpoint2 > 0),
                    Bluemagic.Instance.Find<ModItem>("Checkpoint1").Type,
                    null,
                    Bluemagic.Instance.Find<ModItem>("Checkpoint2").Type,
                    string.Format("Defeat the previous phase or use a [i:{0}]", Find<ModItem>("Checkpoint1").Type));
                bossList.Call(
                    "AddBoss",
                    666f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("TerraSpirit").Type, Bluemagic.Instance.Find<ModNPC>("TerraSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe3").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe4").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe5").Type},
                    this,
                    "????? (Phase 3)",
                    (Func<bool>)(() => BluemagicWorld.terraCheckpoint3 > 0),
                    Bluemagic.Instance.Find<ModItem>("Checkpoint2").Type,
                    null,
                    Bluemagic.Instance.Find<ModItem>("Checkpoint3").Type,
                    string.Format("Defeat the previous phase or use a [i:{0}]", Find<ModItem>("Checkpoint2").Type));
                bossList.Call(
                    "AddBoss",
                    1337f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("TerraSpirit").Type, Bluemagic.Instance.Find<ModNPC>("TerraSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe3").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe4").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe5").Type},
                    this,
                    "????? (Phase 4)",
                    (Func<bool>)(() => BluemagicWorld.terraCheckpointS > 0),
                    Bluemagic.Instance.Find<ModItem>("Checkpoint3").Type,
                    null,
                    Bluemagic.Instance.Find<ModItem>("CheckpointS").Type,
                    string.Format("Defeat the previous phase or use a [i:{0}]", Find<ModItem>("Checkpoint3").Type));
                bossList.Call(
                    "AddBoss",
                    9001f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("TerraSpirit").Type, Bluemagic.Instance.Find<ModNPC>("TerraSpirit2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe2").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe3").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe4").Type, Bluemagic.Instance.Find<ModNPC>("TerraProbe5").Type},
                    this,
                    "?????",
                    (Func<bool>)(() => BluemagicWorld.downedTerraSpirit),
                    Bluemagic.Instance.Find<ModItem>("CheckpointS").Type,
                    Bluemagic.Instance.Find<ModItem>("BlushieCharm").Type,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("PuriumCoin").Type, Bluemagic.Instance.Find<ModItem>("RainbowStar").Type},
                    "Overcome all phases and defeat the boss once and for all!");
                bossList.Call(
                    "AddBoss",
                    9002f,
                    new List<int> {Bluemagic.Instance.Find<ModNPC>("Blushiemagic").Type, Bluemagic.Instance.Find<ModNPC>("BlushiemagicK").Type, Bluemagic.Instance.Find<ModNPC>("BlushiemagicA").Type, Bluemagic.Instance.Find<ModNPC>("BlushiemagicL").Type, Bluemagic.Instance.Find<ModNPC>("BlushiemagicM").Type, Bluemagic.Instance.Find<ModNPC>("BlushiemagicJ").Type},
                    this,
                    "Blushiemagic",
                    (Func<bool>)(() => BluemagicWorld.downedBlushie),
                    Bluemagic.Instance.Find<ModItem>("BlushieCrystal").Type,
                    null,
                    new List<int> {Bluemagic.Instance.Find<ModItem>("PuriumCoin").Type, Bluemagic.Instance.Find<ModItem>("SkyDragonHeart").Type, Bluemagic.Instance.Find<ModItem>("WorldReaver").Type},
                    string.Format("Use a [i:{0}] anytime, anywhere, after all previous bosses have been defeated (has infinite reuses). Be warned, it will be very difficult.", Find<ModItem>("BlushieCrystal").Type),
                    null,
                    null,
                    null,
                    (Func<bool>)(() => BluemagicWorld.downedTerraSpirit)); //Boss is only shown when Terra Spirit has been defeated
            }
            Calamity = ModLoader.GetMod("CalamityMod");
            Thorium = ModLoader.GetMod("ThoriumMod");
            Sushi = ModLoader.GetMod("imkSushisMod");
            if (!Main.dedServ)
            {
                BlushieBoss.BlushieBoss.Load();
            }

            HealthBars = ModLoader.GetMod("FKBossHealthBar");
            if (HealthBars != null)
            {
                HealthBars.Call("RegisterHealthBarMini", Find<ModNPC>("BlushiemagicK").Type);
                HealthBars.Call("RegisterHealthBarMini", Find<ModNPC>("BlushiemagicA").Type);
                HealthBars.Call("RegisterHealthBarMini", Find<ModNPC>("BlushiemagicL").Type);
            }
        }

        public override void Unload()
        {
            Instance = null;
            Calamity = null;
            Thorium = null;
            Sushi = null;
            HealthBars = null;
            BlushieBoss.BlushieBoss.Unload();
        }

        public override void AddRecipes()/* tModPorter Note: Removed. Use ModSystem.AddRecipes */
        {
            BluemagicRecipes.AddRecipes(this);
        }

        public override object Call(object[] args)
        {
            if (args.Length == 0)
            {
                return null;
            }
            if (args[0] == "Set")
            {
                if (args.Length < 4)
                {
                    return null;
                }
                if (args[1] is Player && args[2] is string)
                {
                    return CallPlayerSet((Player)args[1], (string)args[2], args[3]);
                }
                if (args[1] == "Global" && args[2] is string)
                {
                    return CallSet((string)args[2], args[3]);
                }
                return null;
            }
            if (args[0] == "Get")
            {
                if (args.Length < 2)
                {
                    return null;
                }
                if (args[1] is string)
                {
                    return CallGet((string)args[1]);
                }
                return null;
            }
            return null;
        }

        private object CallPlayerSet(Player player, string command, object arg)
        {
            BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
            if (command == "puriumShieldChargeMax" && arg is float)
            {
                modPlayer.puriumShieldChargeMax = (float)arg;
                return arg;
            }
            if (command == "puriumShieldChargeRate" && arg is float)
            {
                modPlayer.puriumShieldChargeRate = (float)arg;
                return arg;
            }
            if (command == "puriumShieldEnduranceMult" && arg is float)
            {
                modPlayer.puriumShieldEnduranceMult = (float)arg;
                return arg;
            }
            if (command == "manaMagnet2" && arg is bool)
            {
                modPlayer.manaMagnet2 = (bool)arg;
                return arg;
            }
            if (command == "crystalCloak" && arg is bool)
            {
                modPlayer.crystalCloak = (bool)arg;
                return arg;
            }
            if (command == "lifeMagnet2" && arg is bool)
            {
                modPlayer.lifeMagnet2 = (bool)arg;
                return arg;
            }
            if (command == "noGodmode" && arg is bool)
            {
                modPlayer.noGodmode = (bool)arg;
                return arg;
            }
            return null;
        }

        private object CallSet(string command, object arg)
        {
            if (command == "blushieDifficulty")
            {
                BlushieBoss.BlushieBoss.difficultyOverride = (Func<int>)arg;
                return arg;
            }
            return null;
        }

        private object CallGet(string command)
        {
            if (command == "downedPhantom")
            {
                return BluemagicWorld.downedPhantom;
            }
            if (command == "downedAbomination")
            {
                return BluemagicWorld.downedAbomination;
            }
            if (command == "elementalUnleash")
            {
                return BluemagicWorld.elementalUnleash;
            }
            if (command == "downedPuritySpirit")
            {
                return BluemagicWorld.downedPuritySpirit;
            }
            if (command == "downedChaosSpirit")
            {
                return BluemagicWorld.downedChaosSpirit;
            }
            if (command == "downedTerraSpirit")
            {
                return BluemagicWorld.downedTerraSpirit;
            }
            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType type = (MessageType)reader.ReadByte();
            if (type == MessageType.PuritySpirit)
            {
                PuritySpirit.PuritySpirit spirit = Main.npc[reader.ReadInt32()].ModNPC as PuritySpirit.PuritySpirit;
                if (spirit != null && spirit.NPC.active)
                {
                    spirit.HandlePacket(reader);
                }
            }
            else if (type == MessageType.HeroLives)
            {
                Player player = Main.player[reader.ReadInt32()];
                int lives = reader.ReadInt32();
                player.GetModPlayer<BluemagicPlayer>().heroLives = lives;
                if (lives > 0)
                {
                    NetworkText text;
                    if (lives == 1)
                    {
                        text = NetworkText.FromKey("Mods.Bluemagic.LifeLeft", player.name);
                    }
                    else
                    {
                        text = NetworkText.FromKey("Mods.Bluemagic.LivesLeft", player.name, lives);
                    }
                    ChatHelper.BroadcastChatMessage(text, new Color(255, 25, 25));
                }
            }
            else if (type == MessageType.ChaosSpirit)
            {
                NPC npc = Main.npc[reader.ReadInt32()];
                if (npc.active)
                {
                    ChaosSpirit.ChaosSpirit spirit = npc.ModNPC as ChaosSpirit.ChaosSpirit;
                    if (spirit != null)
                    {
                        spirit.HandlePacket(reader);
                    }
                    ChaosSpirit2 spirit2 = npc.ModNPC as ChaosSpirit2;
                    if (spirit2 != null)
                    {
                        spirit2.HandlePacket(reader);
                    }
                    ChaosSpirit3 spirit3 = npc.ModNPC as ChaosSpirit3;
                    if (spirit3 != null)
                    {
                        spirit3.HandlePacket(reader);
                    }
                }
            }
            else if (type == MessageType.PushChaosArm)
            {
                NPC npc = Main.npc[reader.ReadInt32()];
                Vector2 push = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                if (npc.active)
                {
                    ChaosSpiritArm arm = npc.ModNPC as ChaosSpiritArm;
                    if (arm != null)
                    {
                        arm.offset += push;
                        if (Main.netMode == 2)
                        {
                            ModPacket packet = GetPacket();
                            packet.Write((byte)MessageType.PushChaosArm);
                            packet.Write(push.X);
                            packet.Write(push.Y);
                            packet.Send(-1, whoAmI);
                        }
                    }
                }
            }
            else if (type == MessageType.TerraSpirit)
            {
                NPC npc = Main.npc[reader.ReadInt32()];
                if (npc.active)
                {
                    TerraSpirit.TerraSpirit spirit = npc.ModNPC as TerraSpirit.TerraSpirit;
                    if (spirit != null)
                    {
                        spirit.HandlePacket(reader);
                    }
                }
            }
            else if (type == MessageType.TerraLives)
            {
                Player player = Main.player[reader.ReadInt32()];
                int lives = reader.ReadInt32();
                player.GetModPlayer<BluemagicPlayer>().terraLives = lives;
                if (lives > 0)
                {
                    NetworkText text;
                    if (lives == 1)
                    {
                        text = NetworkText.FromKey("Mods.Bluemagic.LifeLeft", player.name);
                    }
                    else
                    {
                        text = NetworkText.FromKey("Mods.Bluemagic.LivesLeft", player.name, lives);
                    }
                    ChatHelper.BroadcastChatMessage(text, new Color(255, 25, 25));
                }
            }
            else if (type == MessageType.GoldBlob)
            {
                NPC npc = Main.npc[reader.ReadByte()];
                float value = reader.ReadByte();
                if (npc.active && npc.type == Find<ModNPC>("GoldBlob").Type)
                {
                    npc.localAI[0] = value;
                }
            }
            else if (type == MessageType.ExtraLives)
            {
                BluemagicPlayer player = Main.player[Main.myPlayer].GetModPlayer<BluemagicPlayer>();
                if (player.terraLives > 0)
                {
                    player.terraLives += 3;
                }
            }
            else if (type == MessageType.BulletNegative)
            {
                NPC npc = Main.npc[reader.ReadByte()];
                if (npc.active && npc.type == Find<ModNPC>("TerraSpirit2").Type && npc.ModNPC is TerraSpirit2)
                {
                    var bullets = ((TerraSpirit2)npc.ModNPC).bullets;
                    int count = reader.ReadByte();
                    for (int k = 0; k < count; k++)
                    {
                        bullets.Add(new BulletNegative(reader.ReadVector2(), reader.ReadVector2()));
                    }
                }
            }
            else if (type == MessageType.CustomStats)
            {
                byte byte1 = reader.ReadByte();
                byte byte2 = reader.ReadByte();
                Player player = Main.player[byte1];
                BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
                CustomStats stats = byte2 == 0 ? modPlayer.chaosStats : modPlayer.cataclysmStats;
                stats.NetReceive(reader);
                if (Main.netMode == 2)
                {
                    ModPacket packet = GetPacket(512);
                    packet.Write(byte1);
                    packet.Write(byte2);
                    stats.NetSend(packet);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (type == MessageType.WorldReaver)
            {
                WorldReaverData.Begin(reader.ReadInt32());
            }
        }

        public override void UpdateMusic(ref int music, ref SceneEffectPriority priority)/* tModPorter Note: Removed. Use ModSceneEffect.Music and .Priority, aswell as ModSceneEffect.IsSceneEffectActive */
        {
            if (!Main.gameMenu && BlushieBoss.BlushieBoss.Active && BlushieBoss.BlushieBoss.Phase >= 3)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Fallen Blood");
                priority = SceneEffectPriority.BossHigh;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)/* tModPorter Note: Removed. Use ModSystem.ModifyInterfaceLayers */
        {
            InterfaceHelper.ModifyInterfaceLayers(layers);
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)/* tModPorter Note: Removed. Use ModSystem.PostDrawInterface */
        {
            if (BlushieBoss.BlushieBoss.Active && BlushieBoss.BlushieBoss.Phase == 3 && BlushieBoss.BlushieBoss.Phase3Attack > 0 && HealthBars != null)
            {
                BlushieBoss.HealthBarDraw.DrawHealthBarDefault(spriteBatch, 1f);
            }
        }

        public static void UpdatePureColor()
        {
            pureColor.R = (byte)(255 - 155f * Math.Abs(Math.Cos(pureColorStyle * Math.PI / 200.0)));
            pureColor.B = pureColor.R;
            pureColorStyle = (pureColorStyle + 1) % 200;
        }

        public static void NewText(string key, int r, int g, int b)
        {
            if (Main.netMode == 0)
            {
                Main.NewText(Language.GetTextValue(key), (byte)r, (byte)g, (byte)b);
            }
            else if (Main.netMode == 2)
            {
                NetworkText text = NetworkText.FromKey(key);
                ChatHelper.BroadcastChatMessage(text, new Color(r, g, b));
            }
        }

        public static void NewText(string key, Color color)
        {
            NewText(key, color.R, color.G, color.B);
        }
    }

    enum MessageType : byte
    {
        PuritySpirit,
        HeroLives,
        ChaosSpirit,
        PushChaosArm,
        TerraSpirit,
        TerraLives,
        GoldBlob,
        ExtraLives,
        BulletNegative,
        CustomStats,
        WorldReaver
    }
}