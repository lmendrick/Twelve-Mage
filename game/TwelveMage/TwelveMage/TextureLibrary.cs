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
 * 
 * This class acts as a unified place to store textures.
 * It's built around a Dictionary of string keys and Texture2D data, such that any texture the program uses can be quickly retrieved.
 * TextureLibrary is meant to be used as a singleton, and passed to any object that 1.) has a sprite of its own, or 2.) creates objects with sprites.
 * 
 * The primary difference in functionality from a standard Dictionary, is that TextureLibrary has a fail-state "missing" texture that is returned
 * when a requested entry does not exist.
 * 
 * To add a new texture:
 *  1.) Add it to the content folder
 *  2.) Build it with Content.mgcb
 *  3.) Add the texture in the constructor as an AddEntry(Key, Filename) call.
 *      * Please put them under the correct sub-heading, and keep each subheading in alphabetical order!
 *      * If the thing you're adding doesn't match any subheading, feel free to add a new one!
 *  4.) Use TextureLibrary.GrabTexture() with the key you assigned the texture, and assign the retrieved texture to a local Texture2D reference.
 */
namespace TwelveMage
{
    internal class TextureLibrary
    {
        private readonly Dictionary<string, Texture2D> texturesDict = new Dictionary<string, Texture2D>(); // Dictionary of every texture
        private readonly ContentManager contentManager; // ContentManager to load textures
        public readonly Texture2D DefaultTexture; // Default texture in the case of a failed GrabTexture. Can also be accessed publicly.

        public TextureLibrary(ContentManager contentManager)
        {
            // Initialize the contentManager and fail-state default texture
            this.contentManager = contentManager;
            DefaultTexture = contentManager.Load<Texture2D>("MissingTexture");

            // ADD NEW ENTRIES HERE:
            // Characters
            AddEntry("PlayerSheet", "CharacterSheet");
            AddEntry("PlayerSheet2", "GrayWizardSprite");
            AddEntry("ZombieCorpse", "corpse-back");
            AddEntry("ZombieSheet", "ZombieWalkSheet");

            // Non-Character GameObjects
            AddEntry("Bullet", "bullet2");
            AddEntry("Fireball", "truefireball");
            AddEntry("FlameSheet", "FlameSprites");
            AddEntry("Gun", "Spas_12");
            AddEntry("HealthPickup", "medkit");

            // UI Textures
            AddEntry("BlinkSpellSlot", "BlinkSpellSlot");
            AddEntry("ButtonBackground", "buttonBackground");
            AddEntry("FireballSpellSlot", "FireballSpellSlot");
            AddEntry("FreezeSpellSlot", "FreezeSpellSlot");
            AddEntry("HasteSpellSlot", "HasteSpellSlot");
            AddEntry("HealthBar", "HB_1");
            AddEntry("SpellSlotsOverlay", "SpellSlotOverlay");

            // Tilesets
            AddEntry("FlowersTileset", "FlowersTileset");
            AddEntry("GrassTileset", "GrassTileset");
            AddEntry("HalfPaversH", "HorizontalHalfPaversTileset");
            AddEntry("HalfPaversV", "VerticalHalfPaversTileset");
            AddEntry("MiscTileset", "MiscTileset");
            AddEntry("PaversTileset", "PaversTileset");
        }

        // Helper method to simplify loading new textures in the constructor
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
