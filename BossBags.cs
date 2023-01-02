using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic
{
    public class BossBags : GlobalItem
    {
        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            if (context == "bossBag" && arg == ItemID.FishronBossBag)
            {
                player.QuickSpawnItem(Mod.Find<ModItem>("Bubble").Type, Main.rand.Next(8, 13));
            }
        }
    }
}