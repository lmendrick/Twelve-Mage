using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TwelveMage
{
	internal class BorderFlames
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

        private List<Flame> horizFlames;

        private int horizOffset;

        private Random rng;

        Flame flame;

        public BorderFlames(TextureLibrary textureLibrary, int windowWidth, int windowHeight)
		{
			this.textureLibrary = textureLibrary;
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
            numHorizSprites = windowWidth / FireRectWidth;
            numVertSprites = windowHeight / FireRectHeight;

            Rectangle fireRec = new Rectangle(15, 100, FireRectWidth, FireRectHeight);
            flame = new Flame(fireRec, textureLibrary, 100);

            horizFlames = new List<Flame>();

            rng = new Random();

            for (int i = 0; i < numHorizSprites; i++)
            {
                Flame newFlame = new Flame(fireRec, textureLibrary, 100);
                newFlame.frame = rng.Next(1, 6);
                horizFlames.Add(newFlame);
            }

            

            horizOffset = 0;
            foreach (Flame flame in horizFlames)
            {
                //flame.RandomFrame = rng.Next(1, 6);
                flame.Position = new Vector2(horizOffset, windowHeight - 100);
                horizOffset += flame.Width;
            }

            //horizOffset = 0;
		}

        public void Update(GameTime gameTime)
        {
            //horizOffset = 0;
            foreach (Flame flame in horizFlames)
            {
               // flame.Position = new Vector2(0 + horizOffset, windowHeight - 100);
                flame.UpdateAnimation(gameTime);
               // horizOffset += flame.Width;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Flame flame in horizFlames)
            {
                flame.Draw(spriteBatch);
            }
        }
    }
}
