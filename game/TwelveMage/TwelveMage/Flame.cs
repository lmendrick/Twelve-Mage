using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

/*
 * Lucas Mendrick
 * Twelve Mage
 * This class creates Flame objects which inherit from GameObject
 * Used in the BorderFlameManager visual feedback
 * Could also be used in various ways in the future (Flame objects that need to be dodged, etc)
 */

namespace TwelveMage
{

    internal class Flame : GameObject
    {
        private TextureLibrary textureLibrary;
        private Texture2D flameSpriteSheet;

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int FireFrameCount = 5;       // The number of frames in the animation
        const int FireRectOffsetY = 0;   // How far down in the image are the frames?
        const int FireRectHeight = 15;     // The height of a single frame
        const int FireRectWidth = 16;      // The width of a single frame

        // Draw scale
        private float scale;

        // Flame color
        private Color color;

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Frame
        {
            get { return frame;}
            set { frame = value; }
        }

        public float Scale
        {
            get { return scale; }
        }

        public Flame(Rectangle rec, TextureLibrary textureLibrary, int health) : base (rec, textureLibrary, health)
        {
            this.textureLibrary = textureLibrary;
            flameSpriteSheet = textureLibrary.GrabTexture("FlameSheet");
            
            // Set position
            position = new Vector2 (rec.X, rec.Y);

            // Set color 
            color = Color.Cyan;         // Cyan + red flame sprites = green

            // Initialize animation data
            scale = 2f;
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image

        }

        /// <summary>
        /// Update the flame object
        /// </summary>
        /// <param name="gameTime">
        /// GameTime from main
        /// </param>
        /// <param name="bullets">
        /// Unused list of bullets
        /// </param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            UpdateAnimation(gameTime);

            // Update rectangle to match vector position
            rec.X = (int)position.X;
            rec.Y = (int)position.Y;
        }

        /// <summary>
        /// Handle animation timing
        /// </summary>
        /// <param name="gameTime">
        /// Update GameTime
        /// </param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > FireFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }

        /// <summary>
        /// Default draw method
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                flameSpriteSheet,                            // - The texture to draw
                position,                                    // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * FireRectWidth,            // - This rectangle specifies
                    FireRectOffsetY,                  //	 where "inside" the texture
                    FireRectWidth,                    //   to get pixels (We don't want to
                    FireRectHeight),                  //   draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                scale,                                   // - Scale (100% - no change)
                SpriteEffects.None,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
        }

        /// <summary>
        /// Draws a flame in its normal vertical orientation
        /// Used with top and bottom borders
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch from main
        /// </param>
        /// <param name="spriteEffects">
        /// Allows sprite to be flipped
        /// </param>
        public void DrawVertical(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(
                flameSpriteSheet,                            // - The texture to draw
                position,                                    // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * FireRectWidth,            // - This rectangle specifies
                    FireRectOffsetY,                  //	 where "inside" the texture
                    FireRectWidth,                    //   to get pixels (We don't want to
                    FireRectHeight),                  //   draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                scale,                                   // - Scale (100% - no change)
                spriteEffects,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
        }

        /// <summary>
        /// Draws a flame horizontally (rotated 90 degrees)
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch from main
        /// </param>
        /// <param name="spriteEffects">
        /// Allows sprite to be flipped
        /// </param>
        public void DrawHorizontal(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(
                flameSpriteSheet,                            // - The texture to draw
                position,                                    // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * FireRectWidth,            // - This rectangle specifies
                    FireRectOffsetY,                  //	 where "inside" the texture
                    FireRectWidth,                    //   to get pixels (We don't want to
                    FireRectHeight),                  //   draw the whole thing)
                color,                            // - The color
                MathHelper.ToRadians(90f),                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                scale,                                   // - Scale (100% - no change)
                spriteEffects,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
        }
    }
}