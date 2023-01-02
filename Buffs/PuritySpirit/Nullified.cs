using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Bluemagic;

namespace Bluemagic.Buffs.PuritySpirit
{
    public class Nullified : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nullified");
            Description.SetDefault("Your abilities are nullified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff/* tModPorter Note: Removed. Use BuffID.Sets.LongerExpertDebuff instead */ = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BluemagicPlayer>().nullified = true;
        }
    }
}
