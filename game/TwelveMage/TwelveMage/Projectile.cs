using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    internal class Projectile : GameObject
    {
        /*
         * Anthony Maldonado
         * Twelve-Mage
         * This class handles the projectile being shot by the player
         */     

        //fields

        private float timer;
        private float range = 5;
        private float displacement;
        public bool isFire = false;
        private int maxPen = 30;
        private int numPen;



        // Properties
        public bool IsFire
        {
            get { return isFire; }
            set { isFire = value; }
        }

        public int MaxPen
        {
            get { return maxPen; }
        }

        public int NumPen
        {
            get { return numPen; }
            set { numPen = value; }
        }


        //methods



        public Projectile(Rectangle position, Texture2D texture, int health, float range) : base(position, texture, health)
        {
            //standard velocity of 4

            this.LinearVelocity = LinearVelocity * .05f;
            this.range = range;
            displacement = 0;
        }


        public override void Update(GameTime gametime, List<GameObject> bullets)
        {
            //timer is based off real time
            timer += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (timer > LifeSpan)
            {
                //after lifespan of 4 remove bullet
                IsRemoved = true;
            }

            
            //base position of direction and velocity of bullet

            Position += Direction * LinearVelocity;
            displacement += (Direction * LinearVelocity).Length();
            rec.X = (int)Position.X + rec.X;
            rec.Y = (int)Position.Y + rec.Y;

            if(displacement > range)
            {
                IsRemoved = true;
            }

            if (isFire)
            {
                this.Width += 2;
                this.Height += 2;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            //spriteBatch.Draw(texture, , null,
            //    Color.White, 0, Origin, Vector2.Zero, SpriteEffects.None, 0);
            //draw sprite
            spriteBatch.Draw(texture, Rec, Color.White);

        } // Necessary for inheriting from GameObject

    }
}
