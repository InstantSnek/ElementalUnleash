using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Buffs.Summons
{
    public class VoidEmissary : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Void Emissary");
            Description.SetDefault("The void emissary will fight alongside you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
            if (player.ownedProjectileCounts[mod.ProjectileType("VoidEmissary")] > 0)
            {
                modPlayer.voidEmissary = true;
            }
            if (!modPlayer.voidEmissary)
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