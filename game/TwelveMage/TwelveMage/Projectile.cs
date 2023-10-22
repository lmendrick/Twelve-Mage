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

        //fields
       
        private float timer;

        //methods



        public Projectile(Rectangle position, Texture2D texture, int health) : base(position, texture, health)
        {
            //standard velocity of 4
            this.LinearVelocity = LinearVelocity * 1;
            
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
            rec.X = (int)Position.X + rec.X;
            rec.Y = (int)Position.Y + rec.Y;
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            //spriteBatch.Draw(texture, , null,
            //    Color.White, 0, Origin, Vector2.Zero, SpriteEffects.None, 0);
            //draw sprite
            spriteBatch.Draw(texture, Rec, null, Color.White);

        } // Necessary for inheriting from GameObject

    }
}
