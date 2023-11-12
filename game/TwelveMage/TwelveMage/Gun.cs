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
    /*
         * Anthony Maldonado
         * Twelve-Mage
         * This class handles the gun being used by the player
         * might remove in place of having the sprite put directly on the wizard
         * Lucas: Added Draw and Update methods based on Player states
         */

    internal class Gun : GameObject
    {
        //fields
        Texture2D gunTexture;
        // Move gun with player
        private Vector2 dir;
        private Vector2 pos;

        // Reference to current player
        private Player player;

        // Set scale of gun sprite
        private float spriteScale = 0.4f;
        
        private const int GunRectHeight = 24;     // The height the image
        private const int GunRectWidth = 73;        // The width of the image 
        private const int GunRectOffsetY = 0;

        // Offset gun from player origin
        private Vector2 posOffset;
        

        //properties
        public Vector2 PosVector
        {
            get { return pos; }
            set {  pos = value; }
        }

        public Vector2 DirVector
        {
            get { return dir; }
            set {  dir = value; }
        }


        //constructors
        public Gun(Rectangle rec, Texture2D texture, int health, Player player) : base(rec, texture, health)
        {

            this.rec = rec;
            this.gunTexture = texture;
            this.health = health;
            this.pos = new Vector2(rec.X, rec.Y);
            this.player = player;
            posOffset = new Vector2(5, 15);

        }

        //methods

        /// <summary>
        /// Lucas:
        /// Update method determines position and orientation of gun relative to player
        /// </summary>
        /// <param name="gameTime">
        /// Passed in from main
        /// </param>
        /// <param name="bullets">
        /// Unneeded list of bullets since this inherits from GameObject
        /// </param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {

            // Set offset for facing left
            if (player.State == PlayerState.FaceLeft || player.State == PlayerState.WalkLeft)
            {
                posOffset = new Vector2(-5, 15);
            }

            // Set offset for facing right
            else
            {
                posOffset = new Vector2(5, 15);
            }

            // Calculate positon of gun
            pos = player.PosVector + posOffset;
        }

        /// <summary>
        /// Lucas:
        /// Draws gun by referencing current player state
        /// </summary>
        /// <param name="spriteBatch">
        /// spriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {

            // Facing left
            if (player.State == PlayerState.FaceLeft || player.State == PlayerState.WalkLeft)
            {
                DrawLeft(SpriteEffects.FlipHorizontally, spriteBatch);
            }

            // Facing right
            else
            {
                DrawRight(SpriteEffects.None, spriteBatch);
            }
            
        }

        /// <summary>
        /// Lucas:
        /// Draws gun facing left
        /// </summary>
        /// <param name="flipSprite">
        /// Flips the sprite
        /// </param>
        /// <param name="spriteBatch">
        /// Passed in from Draw
        /// </param>
        public void DrawLeft(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(
                    0,
                    GunRectOffsetY,
                    GunRectWidth,
                    GunRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                spriteScale,                           // Sprite scale
                flipSprite,
                0);
        }

        /// <summary>
        /// Draws gun facing right
        /// </summary>
        /// <param name="flipSprite">
        /// Flips the gun (unused)
        /// </param>
        /// <param name="spriteBatch">
        /// Passed in from Draw
        /// </param>
        public void DrawRight(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(
                    0,
                    GunRectOffsetY,
                    GunRectWidth,
                    GunRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                spriteScale,                           // Sprite scale
                flipSprite,
                0);
        }

    }

}
