using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Tiles
{
    public class SaltBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileLighted[Type] = true;
            HitSound = 21;
            soundStyle/* tModPorter Note: Removed. Integrate into HitSound */ = 1;
            DustType = 13;
            ItemDrop = Mod.Find<ModItem>("SaltBrick").Type;
            AddMapEntry(new Color(200, 200, 255));
        }
    }
}