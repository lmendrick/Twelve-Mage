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
         * Lucas: Added Draw and Update methods based on Player states, now rotates around player
         */

    internal class Gun : GameObject
    {
        //fields
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


        // Gun rotation
        private Vector2 mouseDir;
        private MouseState mState;
        private float gunRotation;

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

        public Player Player
        {
            get { return player;}
            set { player = value; }
        }


        //constructors
        public Gun(Rectangle rec, TextureLibrary textureLibrary, int health, Player player) : base(rec, textureLibrary, health)
        {
            texture = _textureLibrary.GrabTexture("Gun");
            this.player = player;
            pos = new Vector2(player.Rec.Center.X, player.Rec.Center.Y);
            gunRotation = 0;
        }

        //methods

        /// <summary>
        /// Lucas:
        /// Update method determines position and rotation of gun based on mouse cursor
        /// </summary>
        /// <param name="gameTime">
        /// Passed in from main
        /// </param>
        /// <param name="bullets">
        /// Unneeded list of bullets since this inherits from GameObject
        /// </param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            mState = Mouse.GetState();

            // Update gun position to be centered on player
            pos = new Vector2(player.Rec.Center.X, player.Rec.Center.Y); 

            // Update vector between gun and mouse cursor
            mouseDir = new Vector2(mState.X, mState.Y) - pos;

            // Calculate rotation
            gunRotation = (float)Math.Atan2(mouseDir.Y, mouseDir.X);

            rec.X = (int)pos.X;
            rec.Y = (int)pos.Y;
        }

        /// <summary>
        /// Lucas:
        /// Draws gun based on rotation
        /// </summary>
        /// <param name="spriteBatch">
        /// spriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // This code will flip gun left or right (if not using rotate code)

            //// Facing left
            //if (player.State == PlayerState.FaceLeft || player.State == PlayerState.WalkLeft)
            //{
            //    DrawLeft(SpriteEffects.FlipHorizontally, spriteBatch);
            //}

            //// Facing right
            //else
            //{
            //    DrawRight(SpriteEffects.None, spriteBatch);
            //}

            // Draw the rotating gun
            DrawRotate(spriteBatch);
            
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
                spriteScale,                           // scale
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
                spriteScale,                           // scale
                flipSprite,
                0);
        }

        /// <summary>
        /// Draws the gun to always point towards the mouse cursor and rotate around the player
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawRotate(SpriteBatch spriteBatch)
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
                gunRotation,                            // Rotation
                new Vector2(0, GunRectHeight / 2),                           // Origin
                spriteScale,                             
                SpriteEffects.None,                     
                0);
        }
    }
}
