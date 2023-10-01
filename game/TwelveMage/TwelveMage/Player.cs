using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Chloe Hall
 * Twelve-Mage
 * This class handles the player character,
 * including movement, actions, scores, etc.
 * No known issues
 */

namespace TwelveMage
{
    enum PlayerState // Enum for Player FSM
    {

    }
    internal class Player : GameObject
    {
            // FIELDS
        // Objects
        private Rectangle position; // Location of the Player; used for position calculations & sprite drawing
        private Texture2D texture;

            // Values
        private const int MAX_HEALTH = 100; // Cap on health (made a constant for future readability/ease of changing)
        private int _health;
        //private int _ammo; // Ammunition count (Not yet implemented)

            // PROPERTIES
        public Rectangle Position { get { return position; } }
        public int Health { get { return _health; } }

            // METHODS
        public override void Update(GameTime gameTime) { } // Necessary for inheriting from GameObject
        public override void Draw(SpriteBatch spriteBatch) { } // Necessary for inheriting from GameObject
        public Boolean CheckCollision(GameObject _gameObject) // Checks collision with a given GameObject
        {
            return false;
        }

            // CONSTRUCTORS
        public Player(Rectangle position, Texture2D texture) : base(position, texture)
        {
            this.position = position;
            this.texture = texture;
        }
    }
}
