using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Items.Abomination
{
    [AutoloadEquip(EquipType.Head)]
    public class AbominationMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = 1;
            Item.vanity = true;
        }

        public override bool DrawHead()/* tModPorter Note: Removed. In SetStaticDefaults, use ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false if you returned false */
        {
            return false;
        }
    }
}