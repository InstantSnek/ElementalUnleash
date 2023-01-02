using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Walls
{
    public class MushroomBrickWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = 26;
            ItemDrop = Mod.Find<ModItem>("MushroomBrickWall").Type;
            AddMapEntry(new Color(64, 62, 80));
        }
    }
}