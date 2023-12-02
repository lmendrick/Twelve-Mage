/*
 * Lucas Mendrick
 * Twelve Mage
 * Displays scrolling credits in order to attribute assets.
 * Work in progress
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveMage
{
    internal class CreditsManager
    {
        private SpriteFont titleFont;
        private SpriteFont smallFont;
        private float scrollMultiplier;
        private int windowWidth;
        private int windowHeight;
        private float scrollLocY;
        private int ySpacing = 50;
        private float scrollSpeed = 25f;
        private Vector2 endVector;

        public CreditsManager(int windowWidth, int windowHeight, SpriteFont titleFont, SpriteFont smallFont)
        {
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.titleFont = titleFont;
            this.smallFont = smallFont;

            Reset();
        }

        /// <summary>
        /// Updates the location of the credits based on scrollSpeed
        /// </summary>
        /// <param name="gameTime">
        /// GameTime from main
        /// </param>
        public void Update(GameTime gameTime)
        {
            scrollMultiplier -= scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            scrollLocY = windowHeight + scrollMultiplier;

            // Vector to check if the credits are over. Need to adjust integer based on position of last credit
            endVector = new Vector2(0, (scrollLocY + ySpacing * 30));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            /* Still need credits for:
             * Fireball sprite
             */

            // Title
            spriteBatch.DrawString(
                titleFont,
                "Twelve Mage",
                new Vector2(((windowWidth / 2)) - (titleFont.MeasureString("Twelve Mage").X / 2), (scrollLocY - ySpacing)),
                Color.Yellow);

            // Made By
            spriteBatch.DrawString(
                titleFont,
                "Made By",
                new Vector2(((windowWidth / 2)) - (titleFont.MeasureString("Made By").X / 2), (scrollLocY + ySpacing)),
                Color.Blue);

            // Our names
            spriteBatch.DrawString(
                smallFont,
                "Benjamin Fawley" +
                "\nChloe Hall" +
                "\nAnthony Maldonado" +
                "\nLucas Mendrick",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Benjamin Fawley" +
                "\nChloe Hall" +
                "\nAnthony Maldonado" +
                "\nLucas Mendrick").X / 2), (scrollLocY + ySpacing * 2)),
                Color.Yellow);

            // Assets
            spriteBatch.DrawString(
                titleFont,
                "Assets",
                new Vector2(((windowWidth / 2)) - (titleFont.MeasureString("Assets").X / 2), (scrollLocY + ySpacing * 5)),
                Color.Blue);

            // Wizard Protagonist 
            spriteBatch.DrawString(
                smallFont,
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 6)),
                Color.Yellow);

            // Enemy Sprites
            spriteBatch.DrawString(
                smallFont,
                "Enemy Sprites by Cuddlebug" +
                "\ncuddle-bug.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 8)),
                Color.Yellow);

            // Tileset 
            spriteBatch.DrawString(
                smallFont,
                "Tileset by Cainos" +
                "\ncainos.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 10)),
                Color.Yellow);

            // Firearms
            spriteBatch.DrawString(
                smallFont,
                "Firearms by Ivoryred" +
                "\nivoryred.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 12)),
                Color.Yellow);

            // UI Keys
            spriteBatch.DrawString(
                smallFont,
                "UI Keys by Gerald Burke" +
                "\ngerald-burke.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 14)),
                Color.Yellow);

            // Spell Icons
            spriteBatch.DrawString(
                smallFont,
                "Spell Icons by Aleksandr Makarov" +
                "\niknowkingrabbit.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 16)),
                Color.Yellow);

            // Spell UI Frames
            spriteBatch.DrawString(
                smallFont,
                "Spell UI Frames by Batuhan Karagol" +
                "\nandelrodis.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 18)),
                Color.Yellow);

            // Health Bars
            spriteBatch.DrawString(
                smallFont,
                "Health Bars by Cethiel" +
                "\nopengameart.org/users/cethiel",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 20)),
                Color.Yellow);

            // Flame Sprites
            spriteBatch.DrawString(
                smallFont,
                "Flame Sprites by Max1Truc" +
                "\nmax1truc.itch.io",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 22)),
                Color.Yellow);

            // Title Font
            spriteBatch.DrawString(
                smallFont,
                "Alagard Font by Pix3M" +
                "\ndeviantart.com/pix3m",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Wizard Protagonist by Penzilla" +
                "\npenzilla.itch.io").X / 2), (scrollLocY + ySpacing * 24)),
                Color.Yellow);

            // End
            spriteBatch.DrawString(
                smallFont,
                "Made for IGME 106-01 with Professor Bierre",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Made for IGME 106-01 with Professor Bierre").X / 2), (scrollLocY + ySpacing * 28)),
                Color.Yellow);

            spriteBatch.DrawString(
                smallFont,
                "Copyright 2023 Team Uninstallation Fee, All Rights Reserved",
                new Vector2(((windowWidth / 2)) - (smallFont.MeasureString(
                "Copyright 2023 Team Uninstallation Fee, All Rights Reserved").X / 2), (scrollLocY + ySpacing * 29)),
                Color.Yellow);

            
            
        }

        /// <summary>
        /// Resets the location of all the credits.
        /// Called whenever the credits button is clicked from the main menu.
        /// </summary>
        public void Reset()
        {
            scrollMultiplier = 0;
        }

        /// <summary>
        /// Checks if the credits are over. Used in Game1 to return to main menu.
        /// </summary>
        /// <returns>
        /// True if credits are over (endVector is past the top of the window)
        /// Otherwise false.
        /// </returns>
        public bool IsEnded()
        {
            if (endVector.Y < 0)
            {
                return true;
            }
            return false;
        }
    }
}
