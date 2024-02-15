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
        protected int maxPen = 1;
        protected int numPen = 0;
        protected int maxHit = 1;
        protected int numHit = 0;



        // Properties

        public int MaxPen
        {
            get { return maxPen; }
        }

        public int NumPen
        {
            get { return numPen; }
            set { numPen = value; }
        }

        public int NumHit
        {
            get { return numHit; }
            set { numHit = value; }
        }

        public int MaxHit
        {
            get { return maxHit; }
        }


        //methods
        public Projectile(Rectangle position, TextureLibrary textureLibrary, int health, float range) : base(position, textureLibrary, health)
        {
            this.LinearVelocity = LinearVelocity * .05f;
            this.range = range;
            displacement = 0;
            texture = textureLibrary.GrabTexture("Bullet");
            maxHit = maxPen;
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

            if(numHit >= maxHit)
            {
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
