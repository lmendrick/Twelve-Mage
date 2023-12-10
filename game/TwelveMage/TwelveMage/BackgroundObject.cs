using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    internal class BackgroundObject
    {
        #region FIELDS
        private Rectangle rec;
        private Rectangle source;
        private Texture2D texture;
        #endregion

        #region PROPERTIES
        public Rectangle Rec {  get { return rec; } }
        #endregion

        #region CONSTRUCTORS
        public BackgroundObject(Rectangle source, Rectangle rec, Texture2D texture)
        {
            this.rec = rec;
            this.source = source;
            this.texture = texture;
        }
        #endregion

        #region METHODS
        public void Update(GameTime gameTime, List<GameObject> gameObjects)
        {
            
        }

        /// <summary>
        /// Draws this BackgroundObject
        /// </summary>
        public void Draw(SpriteBatch _spriteBatch)
        {
            Draw(_spriteBatch, Color.White);
        }

        /// <summary>
        /// Draws this BackgroundObject with a specific color
        /// </summary>
        public void Draw(SpriteBatch _spriteBatch, Color color)
        {
            _spriteBatch.Draw(texture, rec, source, color);
        }
        #endregion
    }
}
