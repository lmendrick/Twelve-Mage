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
        private Texture2D shadowTexture;
        #endregion

        #region PROPERTIES
        public Rectangle Rec {  get { return rec; } }
        #endregion

        #region CONSTRUCTORS
        public BackgroundObject(Rectangle source, Rectangle rec, Texture2D texture, Texture2D shadowTexture)
        {
            this.rec = rec;
            this.source = source;
            this.texture = texture;
            this.shadowTexture = shadowTexture;
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
            _spriteBatch.Draw(shadowTexture, rec, source, Color.White * 0.5f);
            _spriteBatch.Draw(texture, rec, source, Color.White);
        }
        #endregion
    }
}
