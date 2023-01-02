using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Bluemagic.Tiles
{
    public class BossTrophy : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 36;
            TileObjectData.addTile(Type);
            DustType = 7;
            disableSmartCursor/* tModPorter Note: Removed. Use TileID.Sets.DisableSmartCursor instead */ = true;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Trophy");
            AddMapEntry(new Color(120, 85, 60), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            int item = 0;
            switch (frameX / 54)
            {
                case 0:
                    item = Mod.Find<ModItem>("AbominationTrophy").Type;
                    break;
                case 1:
                    item = Mod.Find<ModItem>("PuritySpiritTrophy").Type;
                    break;
                case 2:
                    item = Mod.Find<ModItem>("BunnyTrophy").Type;
                    break;
                case 3:
                    item = Mod.Find<ModItem>("TreeTrophy").Type;
                    break;
                case 4:
                    item = Mod.Find<ModItem>("ChaosTrophy").Type;
                    break;
                case 5:
                    item = Mod.Find<ModItem>("CataclysmTrophy").Type;
                    break;
                case 6:
                    item = Mod.Find<ModItem>("PhantomTrophy").Type;
                    break;
            }
            if (item > 0)
            {
                Item.NewItem(i * 16, j * 16, 48, 48, item);
            }
        }
    }
}