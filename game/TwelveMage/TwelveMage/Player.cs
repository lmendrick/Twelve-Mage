﻿using Microsoft.Xna.Framework;
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
 * Luke: Added Player movement and Animations
 */

namespace TwelveMage
{

    enum PlayerState
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight
    }

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

        PlayerState state;

        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int WizardRectOffsetY = 36;   // How far down in the image are the frames?
        const int WizardRectOffsetX = 4;
        const int WizardRectHeight = 30;     // The height of a single frame
        const int WizardRectWidth = 34;      // The width of a single frame

        // Move player with vector2
        //Vector2 dir = Vector2.Zero;
        //float speed = 5f;

        Rectangle prevPosition;


        public int PlayerX
        {
            get { return position.X; }
        }

        public int PlayerY
        {
            get { return position.Y; }
        }

        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        // CONSTRUCTORS
        // Luke: Added health inherited from GameObject
        public Player(Rectangle position, Texture2D texture, int health) : base(position, texture, health)
        {
            this.position = position;
            this.spriteSheet = texture;
           // this.health = health;

            // Default sprite direction
            //state = PlayerState.FaceRight;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
        }

        // METHODS
        public override void Update(GameTime gameTime) // Necessary for inheriting from GameObject
        {
            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.W))
            {
                this.position.Y -= 1;
                //  dir.Y -= 1;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                this.position.Y += 1;
                // dir.Y += 1;
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                this.position.X += 1;
                //  dir.X += 1;
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                this.position.X -= 1;
                // dir.X -= 1;
            }

            switch (state)
            {
                case PlayerState.FaceRight:

                    // Face left 
                    if (position.X < prevPosition.X)
                    {
                        state = PlayerState.FaceLeft;
                    }

                    // Walk Right 
                    if (position.X > prevPosition.X || position.Y != prevPosition.Y)
                    {
                        state = PlayerState.WalkRight;
                    }
                    break;

                case PlayerState.FaceLeft:
                    
                    // Face Right 
                    if (position.X > prevPosition.X)
                    {
                        state = PlayerState.FaceRight;
                    }

                    // Walk Left 
                    else if (position.X < prevPosition.X || position.Y != prevPosition.Y)
                    {
                        state = PlayerState.WalkLeft;
                    }
                    break;

                case PlayerState.WalkRight:

                    if (position.X <= prevPosition.X && position.Y == prevPosition.Y)
                    {
                        state = PlayerState.FaceRight;
                    }
                    break;

                case PlayerState.WalkLeft:

                    if (position.X >= prevPosition.X && position.Y == prevPosition.Y)
                    {
                        state = PlayerState.FaceLeft;
                    }
                    break;


            }
            prevPosition = position;

            
            // Luke: Movement using WASD, probably should use Vector2 in future with
            // normalized values to have diagonals same speed
            //if (kbState.IsKeyDown(Keys.W))
            //{
            //    this.position.Y -= 1;
            //  //  dir.Y -= 1;
            //}
            //if (kbState.IsKeyDown(Keys.S))
            //{
            //    this.position.Y += 1;
            //   // dir.Y += 1;
            //}
            //if (kbState.IsKeyDown (Keys.D))
            //{
            //    this.position.X += 1;
            //  //  dir.X += 1;
            //}
            //if (kbState.IsKeyDown (Keys.A))
            //{
            //    this.position.X -= 1;
            //   // dir.X -= 1;
            //}

            //if (dir != Vector2.Zero)
            //{
            //    dir.Normalize();
            //}

            // Set positon of rectangle based on direction vector and speed
            //position.Y += (int)dir.Y * (int)gameTime.ElapsedGameTime.TotalSeconds * (int)speed;
            //position.X += (int)dir.X * (int)gameTime.ElapsedGameTime.TotalSeconds * (int)speed;

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

        // Necessary for inheriting from GameObject
        public override void Draw(SpriteBatch spriteBatch)
        {

            switch (state)
            {
                case PlayerState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.WalkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
            }

            
        } 


        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X, position.Y),
                new Rectangle(
                    0,
                    WizardRectOffsetY,
                    WizardRectWidth,
                    WizardRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        private void DrawWalking(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            // Luke: Pass in vector2 from Game1 instead of creating new vector2?
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(position.X, position.Y),
                new Rectangle(
                    frame * WizardRectWidth,
                    WizardRectOffsetY,
                    WizardRectWidth,
                    WizardRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        public Boolean CheckCollision(GameObject _gameObject) // Checks collision with a given GameObject
        {
            return false;
        }
    }
}