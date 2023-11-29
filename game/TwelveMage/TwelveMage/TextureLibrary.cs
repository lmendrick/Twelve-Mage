using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
/*
 * Chloe Hall
 * Twelve-Mage
 * This class acts as a unified place to store textures.
 */
namespace TwelveMage
{
    internal class TextureLibrary
    {
        private readonly Dictionary<string, Texture2D> texturesDict = new Dictionary<string, Texture2D>(); // Dictionary of every texture
        private readonly ContentManager contentManager; // ContentManager to load textures
        public readonly Texture2D DefaultTexture; // Default texture in the case of a failed GrabTexture.

        // Constant file names for every texture, for easy editing later. To be used with Content.LoadContent.

        // Characters
        private const string playerSheetFile = "CharacterSheet";
        private const string zombieSheetFile = "ZombieWalkSheet";
        private const string corpseFile = "corpse-back";

        // Non-Character Game Objects
        private const string bulletFile = "bullet2";
        private const string healthPackFile = "medkit";
        private const string gunFile = "Spas_12";
        private const string fireballFile = "truefireball";
        private const string flameSpriteSheetFile = "FlameSprites";

        // UI Textures
        private const string buttonImgFile = ""; // Placeholder for button sprite
        private const string blinkSlotFile = "BlinkSpellSlot";
        private const string fireballSlotFile = "FireballSpellSlot";
        private const string freezeSlotFile = "FreezeSpellSlot";
        private const string hasteSlotFile = "HasteSpellSlot";
        private const string spellOverlayFile = "SpellSlotOverlay";
        private const string healthBarFile = "HB_1";

        // Tilesets
        private const string flowersTilesetFile = "FlowersTileset";
        private const string grassTilesetFile = "GrassTileset";
        private const string horizontalHalfPaversFile = "HorizontalHalfPaversTileset"; // Say that five times fast
        private const string miscTilesetFile = "MiscTileset";
        private const string paversTilesetFile = "PaversTileset";
        private const string verticalHalfPaversFile = "VerticalHalfPaversTileset";

        public TextureLibrary(ContentManager contentManager)
        {
            // Initialize the contentManager and fail-state default texture
            this.contentManager = contentManager;
            DefaultTexture = contentManager.Load<Texture2D>("MissingTexture");

            // Add all texture entries
            AddEntry("BlinkSpellSlot", blinkSlotFile);
            AddEntry("Bullet", bulletFile);
            AddEntry("ZombieCorpse", corpseFile);
            AddEntry("Fireball", fireballFile);
            AddEntry("FireballSpellSlot", fireballSlotFile);
            AddEntry("FlowersTileset", flowersTilesetFile);
            AddEntry("FreezeSpellSlot", freezeSlotFile);
            AddEntry("GrassTileset", grassTilesetFile);
            AddEntry("Gun", gunFile);
            AddEntry("HalfPaversH", horizontalHalfPaversFile);
            AddEntry("HalfPaversV", verticalHalfPaversFile);
            AddEntry("HasteSpellSlot", hasteSlotFile);
            AddEntry("HealthBar", healthBarFile);
            AddEntry("HealthPickup", healthPackFile);
            AddEntry("MiscTileset", miscTilesetFile);
            AddEntry("PaversTileset", paversTilesetFile);
            AddEntry("PlayerSheet", playerSheetFile);
            AddEntry("SpellSlotsOverlay", spellOverlayFile);
            AddEntry("ZombieSheet", zombieSheetFile);
            AddEntry("FlameSheet", flameSpriteSheetFile);
        }

        // Simplifies the constructor to make it more readable
        private void AddEntry(string entryName, string fileName)
        {
            texturesDict.Add(entryName, contentManager.Load<Texture2D>(fileName));
        }

        /// <summary>
        /// Returns a given texture from the dictionary, if it exists. Otherwise, returns a default "missing" texture.
        /// </summary>
        /// <param name="entryName">Dictionary key to search for</param>
        public Texture2D GrabTexture(string entryName)
        {
            if (!string.IsNullOrEmpty(entryName) && texturesDict.ContainsKey(entryName)) // If the requested texture is in the Dictionary,
            {
                return texturesDict[entryName]; // Return it;
            }
            else
            {
                Debug.WriteLine(entryName + " could not be found in the TextureLibrary.");
                return DefaultTexture; // Otherwise, return the default "missing" texture.
            }
        }
    }
}
