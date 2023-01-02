using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.TerraSpirit
{
    public class Checkpoint3 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Third Ritual");
            Tooltip.SetDefault("Enrages the Spirit of Purity at 33% health"
                + "\nCan be reused infinitely"
                + "\nEach player starts with {0} lives");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = 12;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = 4;
            Item.UseSound = SoundID.Item44;
        }

        public override bool CanUseItem(Player player)
        {
            return BluemagicWorld.terraCheckpoint3 > 0 && !NPC.AnyNPCs(Mod.Find<ModNPC>("TerraSpirit").Type) && !NPC.AnyNPCs(Mod.Find<ModNPC>("TerraSpirit2").Type);
        }

        public override bool? UseItem(Player player)/* tModPorter Suggestion: Return null instead of false */
        {
            if (Main.netMode != 1)
            {
                NPC.NewNPC((int)player.Center.X, (int)player.Center.Y, Mod.Find<ModNPC>("TerraSpirit").Type, 0, 8f);
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> lines)
        {
            for (int k = 0; k < lines.Count; k++)
            {
                if (lines[k].Mod == "Terraria" && lines[k].Name == "Tooltip2")
                {
                    lines[k].Text = string.Format(lines[k].Text, BluemagicWorld.terraCheckpoint3);
                }
            }
        }
    }
}