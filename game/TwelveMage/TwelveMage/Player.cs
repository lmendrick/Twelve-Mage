using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
 * Anthony: Removed uneeded fields and enum
 */

namespace TwelveMage
{
    internal class Player : GameObject
    {
           

            // Values
        private const int MAX_HEALTH = 100; // Cap on health (made a constant for future readability/ease of changing)
        //private int _health;
        //private int _ammo; // Ammunition count (Not yet implemented)

        // PROPERTIES
        //public int Health { get { return _health; } }


        // Texture and drawing
        Texture2D spriteSheet;  // The single image with all of the animation frames

        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int MarioRectOffsetY = 116;   // How far down in the image are the frames?
        const int MarioRectHeight = 72;     // The height of a single frame
        const int MarioRectWidth = 44;      // The width of a single frame

        // CONSTRUCTORS
        // Luke: Added health inherited from GameObject
        public Player(Rectangle position, Texture2D texture, int health) : base(position, texture, health)
        {
            this.spriteSheet = texture;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
        }

        // METHODS
        public override void Update(GameTime gameTime) // Necessary for inheriting from GameObject
        {
            KeyboardState kbState = Keyboard.GetState();

            // Luke: Movement using WASD, probably should use Vector2 in future with
            // normalized values to have diagonals same speed
            if (kbState.IsKeyDown(Keys.W))
            {
                position.Y -= 1;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                position.Y += 1;
            }
            if (kbState.IsKeyDown (Keys.D))
            {
                position.X += 1;
            }
            if (kbState.IsKeyDown (Keys.A))
            {
                position.X -= 1;
            }

        }

        // Luke: Taken from PE - Mario Walking
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

                if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }
        }

        public override void Draw(SpriteBatch spriteBatch) { } // Necessary for inheriting from GameObject
        public Boolean CheckCollision(GameObject _gameObject) // Checks collision with a given GameObject
        {
            return false;
        }

        
    }
}
