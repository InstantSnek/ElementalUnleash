using System;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Buffs.Summons
{
    public class MiniCaptiveElement : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Captive Elements");
            Description.SetDefault("The mini captive elements will fight for you.");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("MiniCaptiveElement0").Type] > 0)
            {
                modPlayer.elementMinion = true;
            }
            if (!modPlayer.elementMinion)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}