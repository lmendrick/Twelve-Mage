using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;

namespace TwelveMage
{
    internal class Flame : GameObject
    {
        private TextureLibrary textureLibrary;
        private Texture2D flameSpriteSheet;

        // Animation
        public int frame = 1;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int FireFrameCount = 5;       // The number of frames in the animation
        const int FireRectOffsetY = 0;   // How far down in the image are the frames?
        //const int WizardRectOffsetX = 4;
        const int FireRectHeight = 15;     // The height of a single frame
        const int FireRectWidth = 16;      // The width of a single frame

        private int randomFrame = 1;

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public int RandomFrame
        {
            get { return randomFrame;}
            set { randomFrame = value; }
        }

        public Flame(Rectangle rec, TextureLibrary textureLibrary, int health) : base (rec, textureLibrary, health)
        {
            this.textureLibrary = textureLibrary;
            flameSpriteSheet = textureLibrary.GrabTexture("FlameSheet");
            

            position = new Vector2 (rec.X, rec.Y);

            // Initialize animation data
            
            frame = randomFrame;

            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image

            //randomFrame = 0;
        }

        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            //position = new Vector2(rec.X, rec.Y);
            UpdateAnimation(gameTime);
            rec.X = (int)position.X;
            rec.Y = (int)position.Y;
        }

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
                Color.DarkCyan,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                2.0f,                                   // - Scale (100% - no change)
                SpriteEffects.None,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
        }
    }
}