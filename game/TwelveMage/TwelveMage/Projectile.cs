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
        


        //methods
        //check if the collectable is colliding with the player
      

        public Projectile(Rectangle position, Texture2D texture, int health) : base(position, texture, health)
        {
            
        }


        public override void Update(GameTime gameTime) // Necessary for inheriting from GameObject
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch) { } // Necessary for inheriting from GameObject

    }
}
