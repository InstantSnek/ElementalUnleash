using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Tiles
{
    public class Shroomstone : BaseMushroomTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            HitSound = 21;
            soundStyle/* tModPorter Note: Removed. Integrate into HitSound */ = 1;
            DustType = 17;
            ItemDrop = Mod.Find<ModItem>("Shroomstone").Type;
            MinPick = 65;
            AddMapEntry(new Color(93, 127, 255));
            TileID.Sets.Conversion.Stone[Type] = true;
            TileID.Sets.Stone[Type] = true;
        }
    }
}