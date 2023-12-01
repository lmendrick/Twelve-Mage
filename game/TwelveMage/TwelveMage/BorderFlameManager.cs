using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        // Animation
        private int frame;              // The current animation frame
        private double timeCounter;     // The amount of time that has passed
        private double fps;             // The speed of the animation
        private double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int FireFrameCount = 6;       // The number of frames in the animation
        //const int WizardRectOffsetY = 36;   // How far down in the image are the frames?
        //const int WizardRectOffsetX = 4;
        const int FireRectHeight = 15;     // The height of a single frame
        const int FireRectWidth = 16;      // The width of a single frame

        private int numHorizSprites;
        private int numVertSprites;

        private List<Flame> topFlames;
        private List<Flame> bottomFlames;
        private List<Flame> rightFlames;
        private List<Flame> leftFlames;

        private int horizOffset;
        private int vertOffset;

        private Random rng;

        Flame flame;

        private FlameState currentState;

        private Vector2 extendedTopPosition;
        private Vector2 extendedBottomPosition;
        private Vector2 extendedRightPosition;
        private Vector2 extendedLeftPosition;

        private Vector2 startTopPosition;
        private Vector2 startBottomPosition;
        private Vector2 startRightPosition;
        private Vector2 startLeftPosition;

        private Vector2 moveDirection;
        private float moveSpeed = 50f;

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
            numHorizSprites = windowWidth / FireRectWidth;
            numVertSprites = windowHeight / FireRectHeight;

            Rectangle fireRec = new Rectangle(15, 100, FireRectWidth, FireRectHeight);
            flame = new Flame(fireRec, textureLibrary, 100);

            topFlames = new List<Flame>();
            bottomFlames = new List<Flame>();
            rightFlames = new List<Flame>();
            leftFlames = new List<Flame>();

            rng = new Random();

            // Set starting and ending positions for each border animation
            startTopPosition = new Vector2(-20, -FireRectHeight * flame.Scale);
            startBottomPosition = new Vector2(-20, windowHeight + (FireRectHeight * flame.Scale));
            startRightPosition = new Vector2(windowWidth + (FireRectHeight * flame.Scale), -20);
            startLeftPosition = new Vector2(-FireRectHeight * flame.Scale, -20);

            extendedTopPosition = new Vector2(-20, 0);
            extendedBottomPosition = new Vector2(-20, windowHeight - (FireRectHeight * flame.Scale));
            extendedRightPosition = new Vector2(windowWidth - ((FireRectHeight * flame.Scale)), -20);
            extendedLeftPosition = new Vector2(FireRectHeight * flame.Scale, -20);



            // Fill top list with Flame objects
            for (int i = 0; i < numHorizSprites + 1; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);                        // Set random starting frame to make flames appear dynamic
                topFlames.Add(newFlame);
            }
            // Fill bottom list with Flame objects
            for (int i = 0; i < numHorizSprites + 1; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);                        // Set random starting frame to make flames appear dynamic
                bottomFlames.Add(newFlame);
            }
            // Fill right list with Flame objects
            for (int i = 0; i < numVertSprites + 1; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);
                rightFlames.Add(newFlame);
            }
            // Fill left list with Flame objects
            for (int i = 0; i < numVertSprites + 1; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.Frame = rng.Next(1, 6);
                leftFlames.Add(newFlame);
            }



            // Set positions of each flame
            foreach (Flame flame in topFlames)
            {
                flame.Position = startTopPosition;
                startTopPosition.X += flame.Width;
                extendedTopPosition.X += flame.Width;

            }

            foreach (Flame flame in bottomFlames)
            {
                flame.Position = startBottomPosition;
                startBottomPosition.X += flame.Width;
                extendedBottomPosition.X += flame.Width;
            }


            foreach (Flame flame in rightFlames)
            {
                flame.Position = startRightPosition;
                startRightPosition.Y += flame.Height;
                extendedRightPosition.Y += flame.Height;
            }

            foreach (Flame flame in leftFlames)
            {
                flame.Position = startLeftPosition;
                startLeftPosition.Y += flame.Height;
                extendedLeftPosition.Y += flame.Height;
            }
            currentState = FlameState.Inactive;
            moveDirection = Vector2.UnitX;
            //moveSpeed = 50f;
        }

        public void Update(GameTime gameTime)
        {

            switch (currentState)
            {
                case FlameState.Inactive:

                    break;

                case FlameState.Top:
                    moveDirection = Vector2.UnitY;
                    UpdateFlameMovement(topFlames, gameTime, extendedTopPosition, startTopPosition);
                    break;

                case FlameState.Bottom:
                    foreach (Flame flame in bottomFlames)
                    {
                        moveDirection = Vector2.UnitY * new Vector2(0, -1);
                        flame.UpdateAnimation(gameTime);
                        UpdateFlameMovement(bottomFlames, gameTime, extendedBottomPosition, startBottomPosition);
                    }
                    break;

                case FlameState.Right:
                    foreach (Flame flame in rightFlames)
                    {
                        moveDirection = Vector2.UnitX * new Vector2(-1, 0);
                        flame.UpdateAnimation(gameTime);
                        UpdateFlameMovement(rightFlames, gameTime, extendedRightPosition, startRightPosition);
                    }
                    break;

                case FlameState.Left:
                    foreach (Flame flame in leftFlames)
                    {
                        moveDirection = Vector2.UnitX;
                        flame.UpdateAnimation(gameTime);
                        UpdateFlameMovement(leftFlames, gameTime, extendedLeftPosition, startLeftPosition);
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            switch (currentState)
            {
                case FlameState.Inactive:

                    break;

                case FlameState.Top:
                    foreach (Flame flame in topFlames)
                    {
                        flame.DrawHorizontal(spriteBatch, SpriteEffects.FlipVertically);
                    }
                    break;

                case FlameState.Bottom:
                    foreach (Flame flame in bottomFlames)
                    {
                        flame.DrawHorizontal(spriteBatch, SpriteEffects.None);
                    }
                    break;

                case FlameState.Right:
                    foreach (Flame flame in rightFlames)
                    {
                        flame.DrawVertical(spriteBatch, SpriteEffects.FlipVertically);
                    }
                    break;

                case FlameState.Left:
                    foreach (Flame flame in leftFlames)
                    {
                        flame.DrawVertical(spriteBatch, SpriteEffects.None);
                    }
                    break;
            }
        }

        private void UpdateFlameMovement(List<Flame> flames, GameTime gameTime, Vector2 extendedPosition, Vector2 startPosition)
        {

            foreach (Flame flame in flames)
            {
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
                        currentState = FlameState.Inactive;
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
                        currentState = FlameState.Inactive;
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
                        currentState = FlameState.Inactive;
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
                        currentState = FlameState.Inactive;
                        return;
                    }
                }

            }

        }

        // There's probably a better way to do this....
        private void Reset()
        {
            startTopPosition = new Vector2(-20, -FireRectHeight * flame.Scale);
            startBottomPosition = new Vector2(-20, windowHeight + (FireRectHeight * flame.Scale));
            startRightPosition = new Vector2(windowWidth + (FireRectHeight * flame.Scale), -20);
            startLeftPosition = new Vector2(-FireRectHeight * flame.Scale, -20);

            extendedTopPosition = new Vector2(-20, 0);
            extendedBottomPosition = new Vector2(-20, windowHeight - (FireRectHeight * flame.Scale));
            extendedRightPosition = new Vector2(windowWidth - ((FireRectHeight * flame.Scale)), -20);
            extendedLeftPosition = new Vector2(FireRectHeight * flame.Scale, -20);

            // Set positions of each flame
            foreach (Flame flame in topFlames)
            {
                flame.Position = startTopPosition;
                startTopPosition.X += flame.Width;
                extendedTopPosition.X += flame.Width;

            }

            foreach (Flame flame in bottomFlames)
            {
                flame.Position = startBottomPosition;
                startBottomPosition.X += flame.Width;
                extendedBottomPosition.X += flame.Width;
            }


            foreach (Flame flame in rightFlames)
            {
                flame.Position = startRightPosition;
                startRightPosition.Y += flame.Height;
                extendedRightPosition.Y += flame.Height;
            }

            foreach (Flame flame in leftFlames)
            {
                flame.Position = startLeftPosition;
                startLeftPosition.Y += flame.Height;
                extendedLeftPosition.Y += flame.Height;
            }
        }
    }
}
