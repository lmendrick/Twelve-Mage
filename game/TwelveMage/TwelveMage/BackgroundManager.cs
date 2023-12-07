using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    /*
    * Chloe Hall
    * Twelve-Mage
    * This class manages the tileable background,
    * such as loading it from a file,
    * holding the tiles in memory,
    * and assigning them textures based on their type.
    * No known issues.
    */
    internal class BackgroundManager
    {
        #region FIELDS
        // w x h grid of background tiles
        private Tile[,] tiles;

        // Width and height of tile textures
        private const int tileWidth = 32;
        private const int tileHeight = 32;

        private readonly TextureLibrary _textureLibrary;

        // Tilesets to grab textures from, and associated data
        private Texture2D _grassTileset;
        private Texture2D _flowerTileset;
        private Texture2D _paverTileset;

        private Random _myRand;
        private SpriteBatch _spriteBatch;
        #endregion

        #region PROPERTIES
        public int LevelWidth  { get { return tiles.GetLength(1); } }
        public int LevelHeight { get { return tiles.GetLength(0); } }
        #endregion

        #region CONSTRUCTORS
        public BackgroundManager(TextureLibrary textureLibrary, Random myRand, SpriteBatch spriteBatch)
        {
            _textureLibrary = textureLibrary;
            _grassTileset = _textureLibrary.GrabTexture("GrassTileset");
            _flowerTileset = _textureLibrary.GrabTexture("FlowersTileset");
            _paverTileset = _textureLibrary.GrabTexture("PaversTileset");
            _myRand = myRand;
            _spriteBatch = spriteBatch;
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Uses FileManager to load the level data
        /// </summary>
        public void LoadLevel(FileManager _fileManager)
        {
            string[,] tileTypes = _fileManager.LoadLevel();

            if (tileTypes != null)
            {
                // Get level dimensions
                int width = tileTypes.GetLength(0);
                int height = tileTypes.GetLength(1);

                // Initialize tiles
                tiles = new Tile[width, height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        switch (tileTypes[i, j])
                        {
                            case "F": // Flowers

                                // Pick a random texture from the flower tileset
                                // Generate texture source coordinates by finding the number of tileWidth x tileHeight regions in the texture,
                                // Then picking one at random
                                int randomH = _myRand.Next(0, (int)(_flowerTileset.Height / tileHeight));
                                int randomW = _myRand.Next(0, (int)(_flowerTileset.Width / tileWidth));
                                Rectangle sourceRec = new Rectangle(randomW * tileWidth, randomH * tileHeight, tileWidth, tileHeight);

                                // Generate the location of each tile
                                float locX = (float)(i * tileWidth);
                                float locY = (float)(j * tileHeight);
                                Vector2 loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Flowers", _flowerTileset, loc, sourceRec);

                                break;

                            case "P": // Pavers

                                // Pick a random texture from the pavers tileset
                                // Generate texture source coordinates by finding the number of tileWidth x tileHeight regions in the texture,
                                // Then picking one at random
                                randomH = _myRand.Next(0, (int)(_paverTileset.Height / tileHeight));
                                randomW = _myRand.Next(0, (int)(_paverTileset.Width / tileWidth));
                                sourceRec = new Rectangle(randomW * tileWidth, randomH * tileHeight, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _paverTileset, loc, sourceRec);

                                break;

                            default: // Default goes to Grass
                            case "G": // Grass

                                // Pick a random texture from the grass tileset
                                // Generate texture source coordinates by finding the number of tileWidth x tileHeight regions in the texture,
                                // Then picking one at random
                                randomH = _myRand.Next(0, (int)(_grassTileset.Height / tileHeight));
                                randomW = _myRand.Next(0, (int)(_grassTileset.Width / tileWidth));
                                sourceRec = new Rectangle(randomW * tileWidth, randomH * tileHeight, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Grass", _grassTileset, loc, sourceRec);

                                break;
                        }
                    }
                }
            } // Make sure tileTypes was properly loaded
            else // Default: 1x1 grid of grass
            {
                tiles = new Tile[1, 1];
                Rectangle defaultRec = new Rectangle(0, 0, tileWidth, tileHeight);
                Vector2 defaultLoc = new Vector2(0, 0);

                tiles[0, 0] = new Tile("Grass", _grassTileset, defaultLoc, defaultRec);
            }
        }

        /// <summary>
        /// Draws each tile in tiles
        /// </summary>
        public void Draw(int windowWidth, int windowHeight)
        {
            if(tiles != null) // If tiles exists,
            {
                foreach(Tile tile in tiles) // Check each tile in tiles
                {
                    if(tile.Loc.X <= windowWidth && tile.Loc.Y <= windowHeight) // If this tile is on-screen,
                    {
                        tile.Draw(_spriteBatch); // Draw it
                    }
                }
            }
        }
        #endregion
    }
}
