using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Buffs
{
    public class PinkSalty : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pink Salty");
            Description.SetDefault("Greatly increased life regeneration but decreased defense");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            longerExpertDebuff/* tModPorter Note: Removed. Use BuffID.Sets.LongerExpertDebuff instead */ = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 8;
            player.statDefense -= 6;
        }
    }
}