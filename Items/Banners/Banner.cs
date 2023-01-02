using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Bluemagic.Items.Banners
{
    public class Banner : ModItem
    {
        private string name;
        private int placeStyle;

        public Banner()
        {
            this.name = "";
            this.placeStyle = -1;
        }

        public Banner(string name, int placeStyle)
        {
            this.name = name;
            this.placeStyle = placeStyle;
        }

        public override bool IsLoadingEnabled(Mod mod)/* tModPorter Suggestion: If you return false for the purposes of manual loading, use the [Autoload(false)] attribute on your class instead */
        {
            AddBanner("NightSlime", 0);
            AddBanner("TwinEye", 1);
            return false;
        }

        protected override bool CloneNewInstances => true;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}{$Mods.Bluemagic.NPCName." + this.name + "}");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 24;
            Item.maxStack = 99;
            Item.rare = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = Mod.Find<ModTile>("Banner").Type;
            Item.placeStyle = this.placeStyle;
        }

        private void AddBanner(string name, int placeStyle)
        {
            Mod.AddItem(name + "Banner", new Banner(name, placeStyle));
        }
    }
}
