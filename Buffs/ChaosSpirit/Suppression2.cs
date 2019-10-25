using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Bluemagic;

namespace Bluemagic.Buffs.ChaosSpirit
{
	public class Suppression2 : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Suppression");
			Description.SetDefault("50% reduced damage");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<BluemagicPlayer>().suppression = 0.5f;
			if (player.buffTime[buffIndex] == 1)
			{
				player.buffType[buffIndex] = mod.BuffType("Suppression1");
				player.buffTime[buffIndex] = 300;
			}
		}
	}
}
