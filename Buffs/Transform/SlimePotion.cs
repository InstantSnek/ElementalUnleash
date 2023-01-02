using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Bluemagic;

namespace Bluemagic.Buffs.Transform
{
    public class SlimePotion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Liquified");
            Description.SetDefault("You are a Slime!");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            BluemagicPlayer modPlayer = player.GetModPlayer<BluemagicPlayer>();
            if (modPlayer.liquified == 0)
            {
                modPlayer.liquified = 1 + Main.rand.Next(2);
            }
            else if (modPlayer.liquified < 0)
            {
                modPlayer.liquified *= -1;
            }
            if (player.width == Player.defaultWidth)
            {
                player.width = 32;
                player.position.X -= (player.width - Player.defaultWidth) / 2f;
            }
            if (player.height == Player.defaultHeight)
            {
                player.height = 24;
                player.position.Y += Player.defaultHeight - player.height;
            }
        }
    }
}
