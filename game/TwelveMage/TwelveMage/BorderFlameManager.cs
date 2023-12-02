using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


/*
 * Lucas Mendrick
 * Twelve Mage
 * This class creates a list of Flame objects for each border (4 in total)
 * The flames are "animated" to extend and then retract everytime the player
 * wraps (moves offscreen)
 * Intended to provide visual feedback for the player being damaged by wrapping.
 */

// Make flames stay at extended position longer
// Make flames only appear near where player is instead of entire border?
// Make flames "reach" out as you go towards them
// Distance that flames are extended could be proportional to how close the player is to the border
namespace TwelveMage
{

    enum FlameState
    {
        Inactive,
        Top,
        Bottom,
        Right,
        Left
    }

    internal class BorderFlameManager
    {
        private TextureLibrary textureLibrary;
        private int windowWidth;
        private int windowHeight;


        const int FireRectHeight = 15;     
        const int FireRectWidth = 16;      

        // Number of sprites needed to span the top and bottom border
        private int numXSprites;
        // Number of sprites needed to span the right and left border
        private int numYSprites;

        // Extra length to the starting position of each "wall" of flames so they start off screen 
        private int borderPadding = -30;

        private List<Flame> topFlames;
        private List<Flame> bottomFlames;
        private List<Flame> rightFlames;
        private List<Flame> leftFlames;


        private Random rng;

        Flame flame;

        private FlameState currentState;

        // Position for each flame when fully extended
        private Vector2 extendedTopPosition;
        private Vector2 extendedBottomPosition;
        private Vector2 extendedRightPosition;
        private Vector2 extendedLeftPosition;

        // Starting position for each flame
        private Vector2 startTopPosition;
        private Vector2 startBottomPosition;
        private Vector2 startRightPosition;
        private Vector2 startLeftPosition;

        private Vector2 moveDirection;

        // Speed at which the flames extend and retract
        private float moveSpeed = 75f;

        public FlameState State
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public BorderFlameManager(TextureLibrary textureLibrary, int windowWidth, int windowHeight)
        {
            this.textureLibrary = textureLibrary;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;

            // Calculate number of sprites needed for each list, and add a couple more to make sure the full border is spanned
            numXSprites = (windowWidth / FireRectWidth) + 2;
            numYSprites = (windowHeight / FireRectHeight) + 2;

            // Default rectangle and flame object
            Rectangle fireRec = new Rectangle(15, 100, FireRectWidth, FireRectHeight);
            flame = new Flame(fireRec, textureLibrary, 100);

            topFlames = new List<Flame>();
            bottomFlames = new List<Flame>();
            rightFlames = new List<Flame>();
            leftFlames = new List<Flame>();

            // Random to make flame animations dynamic
            rng = new Random();

            


            // Fill top list with Flame objects
            for (int i = 0; i < numXSprites; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);                        // Set random starting frame to make flames appear dynamic
                topFlames.Add(newFlame);
            }
            // Fill bottom list with Flame objects
            for (int i = 0; i < numXSprites; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);                        // Set random starting frame to make flames appear dynamic
                bottomFlames.Add(newFlame);
            }
            // Fill right list with Flame objects
            for (int i = 0; i < numYSprites; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);
                rightFlames.Add(newFlame);
            }
            // Fill left list with Flame objects
            for (int i = 0; i < numYSprites; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);
                leftFlames.Add(newFlame);
            }


            // Set each flame's position to default values and set state to inactive
            Reset();
        }

        public void Update(GameTime gameTime)
        {

            switch (currentState)
            {
                case FlameState.Inactive:
                    // Do nothing
                    break;

                // TOP
                case FlameState.Top:

                    // Positive Y direction (flames extend down)
                    moveDirection = Vector2.UnitY;
                    UpdateFlameMovement(topFlames, gameTime, extendedTopPosition, startTopPosition);

                    break;

                // BOTTOM
                case FlameState.Bottom:

                    // Negative Y direction (flames extend up)
                    moveDirection = Vector2.UnitY * new Vector2(0, -1);
                    UpdateFlameMovement(bottomFlames, gameTime, extendedBottomPosition, startBottomPosition);

                    break;

                // RIGHT
                case FlameState.Right:

                    // Negative X direction (flames extend left)
                    moveDirection = Vector2.UnitX * new Vector2(-1, 0);
                    UpdateFlameMovement(rightFlames, gameTime, extendedRightPosition, startRightPosition);

                    break;

                // LEFT
                case FlameState.Left:

                    // Positive X direction (flames extend right)
                    moveDirection = Vector2.UnitX;
                    UpdateFlameMovement(leftFlames, gameTime, extendedLeftPosition, startLeftPosition);

                    break;
            }
        }

        /// <summary>
        /// Draws each flame in the list depending on current state
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            switch (currentState)
            {
                case FlameState.Inactive:
                    // Do nothing
                    break;

                // TOP
                case FlameState.Top:
                    foreach (Flame flame in topFlames)
                    {
                        flame.DrawVertical(spriteBatch, SpriteEffects.FlipVertically);
                    }
                    break;

                // BOTTOM
                case FlameState.Bottom:
                    foreach (Flame flame in bottomFlames)
                    {
                        flame.DrawVertical(spriteBatch, SpriteEffects.None);
                    }
                    break;

                // RIGHT
                case FlameState.Right:
                    foreach (Flame flame in rightFlames)
                    {
                        flame.DrawHorizontal(spriteBatch, SpriteEffects.FlipVertically);
                    }
                    break;

                // LEFT
                case FlameState.Left:
                    foreach (Flame flame in leftFlames)
                    {
                        flame.DrawHorizontal(spriteBatch, SpriteEffects.None);
                    }
                    break;
            }
        }

        /// <summary>
        /// "Animate" flames to extend and then retract.
        /// </summary>
        /// <param name="flames">
        /// The list of flames for the border
        /// </param>
        /// <param name="gameTime">
        /// Update GameTime
        /// </param>
        /// <param name="extendedPosition">
        /// The position at which the flames will start to retract
        /// </param>
        /// <param name="startPosition">
        /// The starting position which the flames will return to
        /// </param>
        private void UpdateFlameMovement(List<Flame> flames, GameTime gameTime, Vector2 extendedPosition, Vector2 startPosition)
        {

            foreach (Flame flame in flames)
            {
                // Update animations
                flame.UpdateAnimation(gameTime);

                // Update flame position based on direction and speed
                flame.Position += moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;


                // Top
                if (currentState == FlameState.Top)
                {
                    // Check if fully extended
                    if (moveDirection.Y > 0 && flame.Position.Y >= extendedPosition.Y)
                    {
                        // Reverse the direction on the Y-axis to move back
                        moveDirection.Y = -moveDirection.Y;
                    }
                    // Flame is back at starting position, reset
                    else if (moveDirection.Y < 0 && flame.Position.Y <= startPosition.Y)
                    {
                        Reset();
                        return;
                    }
                }

                // Bottom
                else if (currentState == FlameState.Bottom)
                {
                    // Check if fully extended
                    if (moveDirection.Y < 0 && flame.Position.Y <= extendedPosition.Y)
                    {
                        // Reverse the direction on the Y-axis to move back
                        moveDirection.Y = -moveDirection.Y;
                    }
                    // Flame is back at starting position, reset
                    else if (moveDirection.Y > 0 && flame.Position.Y >= startPosition.Y)
                    {
                        Reset();
                        return;
                    }
                }

                // Right
                else if (currentState == FlameState.Right)
                {
                    // Check if fully extended
                    if (moveDirection.X < 0 && flame.Position.X <= extendedPosition.X)
                    {
                        // Reverse the direction on the X-axis to move back
                        moveDirection.X = -moveDirection.X;
                    }

                    // Flame is back at starting position, reset
                    else if (moveDirection.X > 0 && flame.Position.X >= startPosition.X)
                    {
                        Reset();
                        return;
                    }
                }

                // Left
                else if (currentState == FlameState.Left)
                {
                    // Check if fully extended
                    if (moveDirection.X > 0 && flame.Position.X >= extendedPosition.X)
                    {
                        // Reverse the direction on the X-axis to move back
                        moveDirection.X = -moveDirection.X;
                    }

                    // Flame is back at starting position, reset
                    else if (moveDirection.X < 0 && flame.Position.X <= startPosition.X)
                    {
                        Reset();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Set FlameState to Inactive and reset each flames starting and extended positon.
        /// </summary>
        public void Reset()
        {
            // Set to inactive
            currentState = FlameState.Inactive;

            // Set starting position for each list
            startTopPosition = new Vector2(borderPadding, -FireRectHeight * flame.Scale);
            startBottomPosition = new Vector2(borderPadding, windowHeight);
            startRightPosition = new Vector2(windowWidth + (FireRectHeight * flame.Scale), borderPadding);
            startLeftPosition = new Vector2(0, borderPadding);

            // Set extended position for each list
            extendedTopPosition = new Vector2(borderPadding, 0);
            extendedBottomPosition = new Vector2(borderPadding, windowHeight - (FireRectHeight * flame.Scale));
            extendedRightPosition = new Vector2(windowWidth, borderPadding);
            extendedLeftPosition = new Vector2(FireRectHeight * flame.Scale, borderPadding);

            // Set positions of each flame in the top list
            foreach (Flame flame in topFlames)
            {
                flame.Position = startTopPosition;
                startTopPosition.X += flame.Width;
                extendedTopPosition.X += flame.Width;

            }

            // Set positions of each flame in the bottom list
            foreach (Flame flame in bottomFlames)
            {
                flame.Position = startBottomPosition;
                startBottomPosition.X += flame.Width;
                extendedBottomPosition.X += flame.Width;
            }

            // Set positions of each flame in the right list
            foreach (Flame flame in rightFlames)
            {
                flame.Position = startRightPosition;
                startRightPosition.Y += flame.Height;
                extendedRightPosition.Y += flame.Height;
            }

            // Set positions of each flame in the left list
            foreach (Flame flame in leftFlames)
            {
                flame.Position = startLeftPosition;
                startLeftPosition.Y += flame.Height;
                extendedLeftPosition.Y += flame.Height;
            }
        }
    }
}
