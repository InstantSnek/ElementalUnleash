using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Tiles
{
    public class PuriumOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileShine[Type] = 800;
            Main.tileShine2[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 750;
            TileID.Sets.Ore[Type] = true;
            HitSound = 21;
            soundStyle/* tModPorter Note: Removed. Integrate into HitSound */ = 1;
            DustType = 128;
            ItemDrop = Mod.Find<ModItem>("PuriumOre").Type;
            MinPick = 225;
            MineResist = 5f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Purium");
            AddMapEntry(new Color(100, 210, 100), name);
        }

        public override void RandomUpdate(int i, int j)
        {
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    WorldGen.Convert(i + x, j + y, 0, 0);
                    Tile tile = Main.tile[i + x, j + y];
                    if (tile.HasTile && (tile.TileType == TileID.Demonite || tile.TileType == TileID.Crimtane) && Main.rand.Next(3) == 0)
                    {
                        tile.TileType = (ushort)Mod.Find<ModTile>("PuriumOre").Type;
                        NetMessage.SendTileRange(Main.myPlayer, i + x, j + y, 1, 1);
                    }
                }
            }
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}