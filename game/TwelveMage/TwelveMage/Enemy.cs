﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TwelveMage
{
    /*
 * Anthony Maldonado
 * Twelve-Mage
 * This class handles the enemy 
 * including movement, actions
 * No known issues
 * might try to make it such that the player and enemy inherit health and max health in the future
 */
    internal class Enemy : GameObject
    {
        #region FIELDS
        //private int health;
        private const int MAX_Health = 200;
        private int damage;
        #endregion

        #region CONSTRUCTORS

        public Enemy(Rectangle rec, Texture2D texture, int health) : base(rec, texture, health)
        {

        }
        #endregion

        #region METHODS
        public override void Update(GameTime gameTime) { } // Necessary for inheriting from GameObject
        public override void Draw(SpriteBatch spriteBatch) { } // Necessary for inheriting from GameObject
        #endregion
    }
}
