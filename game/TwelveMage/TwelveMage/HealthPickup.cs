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
        private int lifespan;
        private int age;
        private Random rng;



        public HealthPickup(Rectangle rec, TextureLibrary textureLibrary, int health, Player player) 
            : base(rec, textureLibrary, health)
        {
            texture = _textureLibrary.GrabTexture("HealthPickup");
            this.player = player;
            rng = new Random();
            isActive = true;
            lifespan = rng.Next(3, 6);
            age = 0;
        }
        

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }

        }

        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            if (this.CheckCollision(player) && player.Health < 100)
            {
                player.Health += health;
                isActive = false;
            }

            if(age >= lifespan)
            {
                isActive = false;
            }
        }
    }
}
