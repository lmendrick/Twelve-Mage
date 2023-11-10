using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    internal class HealthPickup : GameObject
    {
        private Player player;



        public HealthPickup(Rectangle rec, Texture2D texture, int health, Player player) 
            : base(rec, texture, health)
        {
            this.player = player;
            isActive = true;
        }
        

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            if (this.CheckCollision(player) && player.Health < 100)
            {
                player.Health += health;
                isActive = false;
            }
        }
    }
}
