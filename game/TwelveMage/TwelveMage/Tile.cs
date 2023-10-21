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
    * Chloe Hall
    * Twelve-Mage
    * This class handles individual background tiles,
    * such as grass, stones, flowers, etc.
    * No known issues.
    */
    internal abstract class Tile
    {
        #region FIELDS
        protected Texture2D texture; // Tile sprite
        protected Vector2 loc; // Tile location
        protected const int TextureScale = 32; // Scale of the tile's texture, in pixels/side
        #endregion

        #region PROPERTIES
        protected Vector2 Loc
        { 
            get { return loc; }
        }
        #endregion

        #region CONSTRUCTORS
        protected Tile(Texture2D texture, Vector2 loc) // Default constructor; requres a texture and a location to function
        {
            this.texture = texture;
            this.loc = loc;
        }
        #endregion

        #region METHODS
        public void Draw(SpriteBatch _spriteBatch) // Draws this tile in the appropriate location.
        {
            // Cast the tile's float location to a square (Rectangle) with TextureScale length sides
            Rectangle drawLocation = new Rectangle((int)loc.X, (int)loc.Y, TextureScale, TextureScale);
            _spriteBatch.Draw(texture, drawLocation, Color.White); // Draw this tile
        }

        public void Draw(SpriteBatch _spriteBatch, Vector2 offset) // Draws this tile with a Vector2 offset (for later)
        {
            // Add the offset to the original location
            Vector2 drawVector = loc + offset;
            // Cast the offset location to a square (Rectangle) with TextureScale length sides
            Rectangle drawLocation = new Rectangle((int)drawVector.X, (int)drawVector.Y, TextureScale, TextureScale);
            _spriteBatch.Draw(texture, drawLocation, Color.White); // Draw this tile
        }
        #endregion
    }
}
