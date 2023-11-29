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

            startTopPosition = new Vector2(-20, -FireRectHeight * flame.Scale);
            startBottomPosition = new Vector2(-20, windowHeight + (FireRectHeight * flame.Scale));
            startRightPosition = new Vector2(windowWidth + (FireRectHeight * flame.Scale), -20);
            startLeftPosition = new Vector2(-FireRectHeight * flame.Scale, -20);

            extendedTopPosition = new Vector2(-20, 0);
            extendedBottomPosition = new Vector2(0, windowHeight - (FireRectHeight * flame.Scale));
            extendedRightPosition = new Vector2(windowWidth, 0);
            extendedLeftPosition = new Vector2(FireRectHeight * flame.Scale, 0);


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



            horizOffset = 0;

            foreach (Flame flame in topFlames)
            {
                //flame.RandomFrame = rng.Next(1, 6);
                flame.Position = extendedTopPosition;
                //horizOffset += flame.Width;
                startTopPosition.X += flame.Width;
                extendedTopPosition.X += flame.Width;
                
            }
            horizOffset = 0;
            foreach (Flame flame in bottomFlames)
            {
                //flame.RandomFrame = rng.Next(1, 6);
                flame.Position = extendedBottomPosition;
                //horizOffset += flame.Width;
                startBottomPosition.X += flame.Width;
                extendedBottomPosition.X += flame.Width;
            }

            vertOffset = 0;
            foreach (Flame flame in rightFlames)
            {
                //flame.RandomFrame = rng.Next(1, 6);
                flame.Position = extendedRightPosition;
                //vertOffset += flame.Height;
                startRightPosition.Y += flame.Height;
                extendedRightPosition.Y += flame.Height;
            }
            vertOffset = 0;
            foreach (Flame flame in leftFlames)
            {
                //flame.RandomFrame = rng.Next(1, 6);
                flame.Position = extendedLeftPosition;
                //vertOffset += flame.Height;
                startLeftPosition.Y += flame.Height;
                extendedLeftPosition.Y += flame.Height;
            }
           // currentState = FlameState.Inactive;
        }

        public void Update(GameTime gameTime)
        {

            switch (currentState)
            {
                case FlameState.Inactive:

                    break;

                case FlameState.Top:
                    foreach (Flame flame in topFlames)
                    {
                        Vector2 dir = startTopPosition - extendedTopPosition;
                        dir.Normalize();
                        dir *= 10;
                        flame.UpdateAnimation(gameTime);
                        if (flame.Position == extendedTopPosition)
                        {
                            dir = -dir;
                        }
                    }
                    break;

                case FlameState.Bottom:
                    foreach (Flame flame in bottomFlames)
                    {
                        flame.UpdateAnimation(gameTime);
                    }
                    break;

                case FlameState.Right:
                    foreach (Flame flame in rightFlames)
                    {
                        flame.UpdateAnimation(gameTime);
                    }
                    break;

                case FlameState.Left:
                    foreach (Flame flame in leftFlames)
                    {
                        flame.UpdateAnimation(gameTime);
                    }
                    break;
            }


            foreach (Flame flame in bottomFlames)
            {
                flame.UpdateAnimation(gameTime);
            }
            foreach (Flame flame in rightFlames)
            {
                flame.UpdateAnimation(gameTime);
            }
            foreach (Flame flame in topFlames)
            {
                flame.UpdateAnimation(gameTime);
            }
            foreach (Flame flame in leftFlames)
            {
                flame.UpdateAnimation(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            //switch (currentState)
            //{
            //    case FlameState.Inactive:

            //        break;

            //    case FlameState.Top:
            //        foreach (Flame flame in topFlames)
            //        {
            //            flame.DrawHorizontal(spriteBatch, SpriteEffects.FlipVertically);
            //        }
            //        break;

            //    case FlameState.Bottom:
            //        foreach (Flame flame in bottomFlames)
            //        {
            //            flame.DrawHorizontal(spriteBatch, SpriteEffects.None);
            //        }
            //        break;

            //    case FlameState.Right:
            //        foreach (Flame flame in rightFlames)
            //        {
            //            flame.DrawVertical(spriteBatch, SpriteEffects.FlipVertically);
            //        }
            //        break;

            //    case FlameState.Left:
            //        foreach (Flame flame in leftFlames)
            //        {
            //            flame.DrawVertical(spriteBatch, SpriteEffects.None);
            //        }
            //        break;
            //}
            foreach (Flame flame in topFlames)
            {
                flame.DrawHorizontal(spriteBatch, SpriteEffects.FlipVertically);
            }
            foreach (Flame flame in bottomFlames)
            {
                flame.DrawHorizontal(spriteBatch, SpriteEffects.None);
            }
            foreach (Flame flame in rightFlames)
            {
                flame.DrawVertical(spriteBatch, SpriteEffects.FlipVertically);
            }
            foreach (Flame flame in leftFlames)
            {
                flame.DrawVertical(spriteBatch, SpriteEffects.None);
            }
        }

    }
}
