using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using TwelveMage;


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
        #region FIELDS
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
        Vector2 dir;
        Vector2 pos;
        float speed = 200f;

        Rectangle prevPosition;
        #endregion

        #region PROPERTIES
        public Rectangle Rec
        {
            get { return position; }
        }

        public Vector2 PosVector 
        { 
            get { return pos; } 
        }

        public Vector2 DirVector
        {
            get { return dir; }
        }

        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }
        #endregion

        #region CONSTRUCTORS
        // Luke: Added health inherited from GameObject
        public Player(Rectangle position, Texture2D texture, int health) : base(position, texture, health)
        {
            this.position = position;
            this.spriteSheet = texture;
            this.pos = new Vector2(position.X, position.Y);
            // this.health = health;

            // Default sprite direction
            //state = PlayerState.FaceRight;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
        }
        #endregion

        #region METHODS
        public override void Update(GameTime gameTime) // Necessary for inheriting from GameObject
        {
            KeyboardState kbState = Keyboard.GetState();

            #region WASD Processing
            if (kbState.IsKeyDown(Keys.W))
                dir.Y -= 1;
            else if (kbState.IsKeyDown(Keys.S))
                dir.Y += 1;
            else
                dir.Y = 0;

            if (kbState.IsKeyDown(Keys.D))
                dir.X += 1;
            else if (kbState.IsKeyDown(Keys.A))
                dir.X -= 1;
            else
                dir.X = 0;

            if (dir.X != 0 || dir.Y != 0)
            {
                dir.Normalize();
            }

            pos.Y += dir.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            pos.X += dir.X * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            if (dir.X < 0)
                state = PlayerState.WalkLeft;
            if (dir.X > 0)
                state = PlayerState.WalkRight;

            if (dir.Y != 0)
                if (state == PlayerState.WalkLeft || state == PlayerState.FaceLeft)
                    state = PlayerState.WalkLeft;
            if (state == PlayerState.WalkRight || state == PlayerState.FaceRight)
                state = PlayerState.WalkRight;

            if (dir.X == 0 && dir.Y == 0)
                if (state == PlayerState.WalkLeft)
                    state = PlayerState.FaceLeft;
                else if (state == PlayerState.WalkRight)
                    state = PlayerState.FaceRight;

            position.X = (int)(pos.X);
            position.Y = (int)(pos.Y);
            prevPosition = position;
            #endregion

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
            #region State switch
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
            #endregion

        }


        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                spriteSheet,
                pos,
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
                pos,
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
        #endregion
    }
}