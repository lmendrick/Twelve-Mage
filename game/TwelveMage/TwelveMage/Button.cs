﻿/*
 * Lucas Mendrick
 * Twelve-Mage
 * This class is responsible for creating buttons, detecting if they are clicked, 
 * and assigning events to them if they are clicked.
 * Adapted from PE - Events and Delegates
 * No known issues.
 * 
 * Chloe Hall
 * Added a field to activate/deactivate buttons
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwelveMage
{

    // Delegate for the button being clicked
    public delegate void OnButtonClickDelegate();

    /// <summary>
    /// Builds, monitors, and draws a customized Button
    /// </summary>
    public class Button
    {

        private SpriteFont font;
        private MouseState prevMState;
        private string text;
        private Rectangle position; // Button position and size
        private Vector2 textLoc;
        private Texture2D buttonImg;
        private Color textColor;
        private bool active; // Button will only draw/work when active

        public bool Active // Allow anything to check if a button is active, and turn it on/off
        {
            get { return active; }
            set { active = value; }
        }

        public Texture2D ButtonImg
        {
            get { return buttonImg; }
        }

        public Rectangle Position
        {
            get { return position;}
        }

        // Event for the button being clicked, tied to the OnButtonClick method
        public event OnButtonClickDelegate OnButtonClick;


        /// <summary>
        /// Create a new custom button
        /// </summary>
        /// <param name="device">The graphics device for this game - needed to create custom button textures.</param>
        /// <param name="position">Where to draw the button's top left corner</param>
        /// <param name="text">The text to draw on the button</param>
        /// <param name="font">The font to use when drawing the button text.</param>
        /// <param name="color">The color to make the button's texture.</param>
        public Button(GraphicsDevice device, Rectangle position, String text, SpriteFont font, Color color)
        {
            // References to button data
            this.font = font;
            this.position = position;
            this.text = text;

            // Calculate where to draw text on button (centered)
            Vector2 textSize = font.MeasureString(text);
            textLoc = new Vector2(
                (position.X + position.Width / 2) - textSize.X / 2,
                (position.Y + position.Height / 2) - textSize.Y / 2
            );

            // Invert the button color for the text color
            textColor = new Color(255 - color.R, 255 - color.G, 255 - color.B);

            // Make a custom 2d texture for the button itself
            // NOTE: Maybe make/find an asset for the buttons in the future rather than making one each time
            buttonImg = new Texture2D(device, position.Width, position.Height, false, SurfaceFormat.Color);
            int[] colorData = new int[buttonImg.Width * buttonImg.Height]; // an array to hold all the pixels of the texture
            Array.Fill<int>(colorData, (int)color.PackedValue); // fill the array with all the same color
            buttonImg.SetData<Int32>(colorData, 0, colorData.Length); // update the texture's data

            active = true; // Every button starts active
        }

        /// <summary>
        /// Each frame, update its status if it's been clicked.
        /// </summary>
        public void Update()
        {
            // Check/capture the mouse state regardless of whether this button
            // is active so that it's up to date next time!
            MouseState mState = Mouse.GetState();
            if (active && mState.LeftButton == ButtonState.Released &&
                prevMState.LeftButton == ButtonState.Pressed &&
                position.Contains(mState.Position))
            {
                if (OnButtonClick != null)
                {
                    // Call ALL methods attached to this button
                    OnButtonClick();
                }
            }

            prevMState = mState;
        }

        /// <summary>
        /// Draw the button and then
        /// overlay it with text.
        /// </summary>
        /// <param name="spriteBatch">
        /// The spriteBatch on which to draw this button. 
        /// </param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                // Draw the button itself
                spriteBatch.Draw(buttonImg, position, Color.White);

                // Draw button text over the button
                spriteBatch.DrawString(font, text, textLoc, textColor);
            }
        }

    }
}
