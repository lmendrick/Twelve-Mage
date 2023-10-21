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
 * Anthony Maldonado
 * Twelve-Mage
 * This class handles the enemy 
 * including movement, actions
 * No known issues
 * might try to make it such that the player and enemy inherit health and max health in the future
 * Lucas: Added enemy movement
 */
    internal class Enemy : GameObject
    {
        #region FIELDS
        //private int health;
        private const int MAX_Health = 200;
        private int damage;

        // Enemy Movement
        private Texture2D sprite;
        private Vector2 dir;
        private Vector2 pos;
        private float speed = 100f;
        private Vector2 playerPos;

        // Property to set the current player position
        public Vector2 PlayerPos
        {
            get { return playerPos; }
            set { playerPos = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Enemy(Rectangle rec, Texture2D texture, int health) : base(rec, texture, health)
        {
            this.rec = rec;
            this.sprite = texture;
            this.pos = new Vector2(rec.X, rec.Y);
            this.health = health;
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Lucas:
        /// Update enemy position based on player position
        /// </summary>
        /// <param name="gameTime">
        /// GameTime from main
        /// </param>
        public override void Update(GameTime gameTime)
        {
            // Set enemy direction based on current player position
            dir = playerPos - this.pos;
            dir.Normalize();

            // Update enemy position to move towards player at set speed
            pos += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            // Update rectangle position
            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);
        } 

        /// <summary>
        /// Lucas:
        /// Draw the enemies at their updated positions (will eventually be animated)
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isActive)
            {
                spriteBatch.Draw(sprite, pos, Color.White);
            }
        }
        #endregion
    }
}
