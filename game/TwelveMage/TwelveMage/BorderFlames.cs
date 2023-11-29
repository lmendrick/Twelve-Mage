using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        const int FireRectHeight = 30;     // The height of a single frame
        const int FireRectWidth = 34;      // The width of a single frame

        private int numHorizSprites;
        private int numVertSprites;

        public BorderFlames(TextureLibrary textureLibrary, int windowWidth, int windowHeight)
		{
			this.textureLibrary = textureLibrary;
			this.windowWidth = windowWidth;
			this.windowHeight = windowHeight;
            numHorizSprites = windowWidth / FireRectWidth;
            numVertSprites = windowHeight / FireRectHeight;
		}

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
