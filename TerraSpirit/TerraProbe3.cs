using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.TerraSpirit
{
    public class TerraProbe3 : TerraProbe
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.lifeMax *= 3;
        }

        public override void Behavior()
        {
            Timer++;
            if (Timer % 60 == 0)
            {
                TerraSpirit spirit = (TerraSpirit)Spirit.ModNPC;
                Vector2 center = NPC.Center;
                spirit.bullets.Add(new BulletPortal2(center, center + new Vector2(-320f, -320f)));
                spirit.bullets.Add(new BulletPortal2(center, center + new Vector2(320f, -320f)));
                spirit.bullets.Add(new BulletPortal2(center, center + new Vector2(-320f, 320f)));
                spirit.bullets.Add(new BulletPortal2(center, center + new Vector2(320f, 320f)));
                Timer = 0;
            }
        }

        public override bool PreKill()
        {
            BluemagicWorld.terraCheckpoint2 = FindHighestLife(BluemagicWorld.terraCheckpoint2);
            Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, Mod.Find<ModItem>("Checkpoint2").Type);
            if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
            return base.PreKill();
        }
    }
}
