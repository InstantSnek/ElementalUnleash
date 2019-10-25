using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Phantom
{
    [AutoloadEquip(EquipType.Shield)]
    public class DungeonShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Knight's Shield");
            Tooltip.SetDefault("Grants immunity to knockback"
                + "\nAbsorbs 25% of damage done to players on your team when above 25% life"
                + "\nYou and players on your team take 7% reduced damage");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.value = Item.buyPrice(0, 20, 0, 0);
            item.rare = 9;
            item.expert = true;
            item.accessory = true;
            item.defense = 6;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            bool enoughLife = player.statLife > player.statLifeMax2 * 0.25f;
            player.noKnockback = true;
            if (enoughLife)
            {
                player.hasPaladinShield = true;
            }
            player.AddBuff(mod.BuffType("PhantomShield"), 5, true);
            if (player.whoAmI != Main.myPlayer && Main.player[Main.myPlayer].team == player.team && player.team != 0)
            {
                if (enoughLife && player.miscCounter % 10 == 0)
                {
                    Main.player[Main.myPlayer].AddBuff(BuffID.PaladinsShield, 20, true);
                }
                Main.player[Main.myPlayer].AddBuff(mod.BuffType("PhantomShield"), 5, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CobaltShield);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(null, "PhantomShield");
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}