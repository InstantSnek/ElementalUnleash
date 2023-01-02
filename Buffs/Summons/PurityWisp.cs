using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Buffs.Summons
{
    public class PurityWisp : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purity Wisp");
            Description.SetDefault("The purity wisp will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
            if (player.ownedProjectileCounts[Mod.Find<ModProjectile>("PurityWisp").Type] > 0)
            {
                modPlayer.purityMinion = true;
            }
            if (!modPlayer.purityMinion)
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