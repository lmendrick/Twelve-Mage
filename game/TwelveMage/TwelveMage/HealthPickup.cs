using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    /*
 * Ben Fawley
 * Twelve-Mage
 * Health pickup makes an off chance the enemy drops a health pack for the player
 * 
 * 
 */
    internal class HealthPickup : GameObject
    {
        private Player player;
        private int lifespan;
        private int age;
        private Random rng;

        public int Lifespan { get; }
        public int Age { get; set; }

        public HealthPickup(Rectangle rec, TextureLibrary textureLibrary, int health, Player player, Random rng) 
            : base(rec, textureLibrary, health)
        {
            texture = _textureLibrary.GrabTexture("HealthPickup");
            this.player = player;
            this.rng = rng;
            isActive = true;
            lifespan = rng.Next(3, 6);
            age = 0;
        }
        
        public HealthPickup(Rectangle rec, TextureLibrary textureLibrary, int health, Player player, Random rng, int lifespan, int age)
            : base(rec, textureLibrary, health)
        {
            texture = _textureLibrary.GrabTexture("HealthPickup");
            this.player = player;
            this.rng = rng;
            isActive = true;

            // Lifespan sanity check
            if (lifespan >= 3 && lifespan < 6)
            {
                this.lifespan = lifespan;
            }
            else this.lifespan = rng.Next(3, 6);

            // Age sanity check
            if (age >= 0 && age <= lifespan)
            {
                this.age = age;
            }
            else this.age = 0;
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

            if(age >= lifespan)
            {
                isActive = false;
            }
        }
    }
}
