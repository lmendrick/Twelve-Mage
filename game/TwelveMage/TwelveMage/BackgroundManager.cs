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
    enum PropType { Plant, Prop }
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
        private List<BackgroundObject> backgroundObjects = new List<BackgroundObject>();
        private List<BackgroundObject> foregroundObjects = new List<BackgroundObject>();

        // Width and height of tile textures
        private const int tileWidth = 32;
        private const int tileHeight = 32;

        private readonly TextureLibrary _textureLibrary;

        // Tilesets to grab textures from, and associated data
        private Texture2D _grassTileset;
        private Texture2D _flowerTileset;
        private Texture2D _paverTileset;
        private Texture2D _miscTileset;

        private Texture2D _propsTileset;
        private Texture2D _propsShadows;
        private Texture2D _plantsTileset;
        private Texture2D _plantsShadows;

        private Random _myRand;

        #region Source Rectangles
        // Trees (and Tree Shadows)
        Rectangle Tree0Rec = new Rectangle(24, 14, 113, 139);
        Rectangle Tree1Rec = new Rectangle(161, 17, 95, 136);
        Rectangle Tree2Rec = new Rectangle(295, 31, 83, 120);

        // Bushes (and Bush Shadows)
        Rectangle Bush0Rec = new Rectangle(38, 198, 23, 21);
        Rectangle Bush1Rec = new Rectangle(98, 195, 29, 27);
        Rectangle Bush2Rec = new Rectangle(156, 190, 41, 33);
        Rectangle Bush3Rec = new Rectangle(216, 185, 50, 44);
        Rectangle Bush4Rec = new Rectangle(282, 186, 41, 47);
        Rectangle Bush5Rec = new Rectangle(346, 190, 43, 37);

        // Props (and Prop Shadows)
        Rectangle BarrelRec = new Rectangle(162, 152, 31, 38);

        Rectangle CasketEastRec = new Rectangle(288, 87, 67, 36);
        Rectangle CasketNorthRec = new Rectangle(288, 158, 35, 57);

        Rectangle CrateLargeRec = new Rectangle(160, 18, 39, 46);
        Rectangle CrateSmallRec = new Rectangle(163, 86, 31, 39);

        Rectangle Grave0Rec = new Rectangle(225, 239, 35, 41);
        Rectangle Grave1Rec = new Rectangle(227, 303, 34, 40);
        Rectangle Grave2Rec = new Rectangle(289, 251, 32, 29);

        Rectangle ObeliskBrokenRec = new Rectangle(227, 183, 31, 38);
        Rectangle ObeliskShortRec = new Rectangle(227, 9, 32, 52);
        Rectangle ObeliskTallRec = new Rectangle(227, 91, 37, 66);

        Rectangle Pot0Rec = new Rectangle(165, 217, 23, 34);
        Rectangle Pot1Rec = new Rectangle(164, 288, 28, 27);
        Rectangle Pot2Rec = new Rectangle(165, 348, 23, 32);

        Rectangle SignEastRec = new Rectangle(96, 224, 30, 32);
        Rectangle SignWestRec = new Rectangle(99, 160, 29, 32);

        Rectangle StatueRec = new Rectangle(445, 21, 38, 72);
        Rectangle StoneCircleRec = new Rectangle(353, 269, 97, 72);

        // Rocks (and Rock Shadows)
        Rectangle Rock0Rec = new Rectangle(10, 492, 12, 10);
        Rectangle Rock1Rec = new Rectangle(40, 490, 17, 14);
        Rectangle Rock2Rec = new Rectangle(68, 487, 25, 19);
        Rectangle Rock3Rec = new Rectangle(100, 487, 25, 19);
        Rectangle Rock4Rec = new Rectangle(162, 482, 29, 27);
        Rectangle Rock5Rec = new Rectangle(3, 430, 60, 42);
        #endregion

        #endregion

        #region PROPERTIES
        public int LevelWidth  { get { return tiles.GetLength(1); } }
        public int LevelHeight { get { return tiles.GetLength(0); } }
        #endregion

        #region CONSTRUCTORS
        public BackgroundManager(TextureLibrary textureLibrary, Random myRand)
        {
            _textureLibrary = textureLibrary;

            // Assign tile textures
            _grassTileset = _textureLibrary.GrabTexture("GrassTileset");
            _flowerTileset = _textureLibrary.GrabTexture("FlowersTileset");
            _paverTileset = _textureLibrary.GrabTexture("PaversTileset");
            _miscTileset = _textureLibrary.GrabTexture("MiscTileset");

            // Assign prop/plant textures
            _propsTileset = _textureLibrary.GrabTexture("PropsTileset");
            _propsShadows = _textureLibrary.GrabTexture("PropsShadows");
            _plantsTileset = _textureLibrary.GrabTexture("PlantsTileset");
            _plantsShadows = _textureLibrary.GrabTexture("PlantsShadows");

            _myRand = myRand;

            // ~~~~~~~~~~~~~~~~~~BACKGROUND OBJECTS~~~~~~~~~~~~~~~~~~
            // Group of rock, bushe, and sign by the intersection
            AddToBackground(new Point(360, 65), Bush4Rec, PropType.Plant);
            AddToBackground(new Point(375, 95), Rock3Rec, PropType.Prop);
            AddToBackground(new Point(360, 85), SignWestRec, PropType.Prop);

            // Sign the the southwest of the central tree
            AddToBackground(new Point(220, 225), SignEastRec, PropType.Prop);

            // Mysterious stone circle thingy; nearby bushes
            AddToBackground(new Point(435, 290), StoneCircleRec, PropType.Prop);
            AddToBackground(new Point(550, 295), Bush2Rec, PropType.Plant);
            AddToBackground(new Point(425, 345), Rock0Rec, PropType.Prop);
            AddToBackground(new Point(435, 350), Rock2Rec, PropType.Prop);

            // Crates & pots between E/W path and diagonal path
            AddToBackground(new Point(85, 190), CrateSmallRec, PropType.Prop);
            AddToBackground(new Point(115, 180), CrateLargeRec, PropType.Prop);
            AddToBackground(new Point(105, 215), Pot1Rec, PropType.Prop);

            // Misc. bushes
            AddToBackground(new Point(195, 70), Bush4Rec, PropType.Plant);
            AddToBackground(new Point(200, 365), Bush0Rec, PropType.Plant);
            AddToBackground(new Point(20, 180), Bush5Rec, PropType.Plant);

            // Rock in the middle of the path
            AddToBackground(new Point(485, 150), Rock5Rec, PropType.Prop);

            // ~~~~~~~~~~~~~~~~~~FOREGROUND OBJECTS~~~~~~~~~~~~~~~~~~
            // Obelisks
            AddToForeground(new Point(390, 285), ObeliskShortRec, PropType.Prop);
            AddToForeground(new Point(545, 270), ObeliskTallRec, PropType.Prop);
            AddToForeground(new Point(470, 240), ObeliskBrokenRec, PropType.Prop);

            // Trees
            AddToForeground(new Point(250, 100), Tree0Rec, PropType.Plant); // Central tree
            AddToForeground(new Point(640, 30), Tree2Rec, PropType.Plant); // Northeast tree
            AddToForeground(new Point(555, 420), Tree1Rec, PropType.Plant); // South edge tree
            AddToForeground(new Point(-57, 285), Tree2Rec, PropType.Plant); // West edge tree
            AddToForeground(new Point(675, 360), Tree0Rec, PropType.Plant); // Southeast tree
            AddToForeground(new Point(140, 5), Tree2Rec, PropType.Plant); // Northwest tree

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

                            // Misc. Tileset Tiles

                            case "0": // Northwest-corner paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(0, 0, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);

                                break;

                            case "1": // Northeast-corner paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(32, 0, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);

                                break;

                            case "2": // Southeast-corner paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(64, 0, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);

                                break;

                            case "3": // Southwest-corner paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(96, 0, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);

                                break;

                            case "4": // Northeast missing paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(0, 32, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);

                                break;

                            case "5": // Northwest missing paver
                                // Get specific source rectangle
                                sourceRec = new Rectangle(32, 32, tileWidth, tileHeight);

                                // Generate the location of each tile
                                locX = i * tileWidth;
                                locY = j * tileHeight;
                                loc = new Vector2(locX, locY);

                                // Create the tile
                                tiles[i, j] = new Tile("Pavers", _miscTileset, loc, sourceRec);


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
        public void Draw(int windowWidth, int windowHeight, SpriteBatch _spriteBatch)
        {
            if(tiles != null) // If tiles exists,
            {
                foreach(Tile tile in tiles) // Check each tile in tiles
                {
                    if(tile.Loc.X <= windowWidth && tile.Loc.Y <= windowHeight // If this tile is on-screen,
                        && tile.Loc.X >= -32 && tile.Loc.Y >= -32)
                    {
                        tile.Draw(_spriteBatch); // Draw it
                    }
                }
            }

            // Background props drawing
            if(backgroundObjects != null)
            {
                foreach (BackgroundObject prop in backgroundObjects)
                {
                    // Check if this prop is on-screen
                    if (prop.Rec.X <= windowWidth && prop.Rec.Y <= windowHeight
                        && prop.Rec.X >= -prop.Rec.Width && prop.Rec.Y >= -prop.Rec.Height)
                    {
                        prop.Draw(_spriteBatch);
                    }
                }
            }
        }

        /// <summary>
        /// Draws all foreground props
        /// </summary>
        public void DrawForeground(int windowWidth, int windowHeight, SpriteBatch _spriteBatch)
        {
            // Check if there are any foreground props
            if (foregroundObjects != null)
            {
                foreach (BackgroundObject prop in foregroundObjects)
                {
                    // Check if this prop is on-screen
                    if (prop.Rec.X <= windowWidth && prop.Rec.Y <= windowHeight
                        && prop.Rec.X >= -prop.Rec.Width && prop.Rec.Y >= -prop.Rec.Height)
                    {
                        prop.Draw(_spriteBatch);
                    }
                }
            }
        }

        private void AddToBackground(Point location, Rectangle source, PropType type)
        {
            Point size = new Point(source.Width, source.Height);
            Rectangle rec = new Rectangle(location, size);

            switch(type)
            {
                case PropType.Plant:
                    backgroundObjects.Add(new BackgroundObject(source, rec, _plantsTileset, _plantsShadows));
                    break;

                default:
                case PropType.Prop:
                    backgroundObjects.Add(new BackgroundObject(source, rec, _propsTileset, _propsShadows));
                    break;
            }
        }

        private void AddToForeground(Point location, Rectangle source, PropType type)
        {
            Point size = new Point(source.Width, source.Height);
            Rectangle rec = new Rectangle(location, size);

            switch (type)
            {
                case PropType.Plant:
                    foregroundObjects.Add(new BackgroundObject(source, rec, _plantsTileset, _plantsShadows));
                    break;

                default:
                case PropType.Prop:
                    foregroundObjects.Add(new BackgroundObject(source, rec, _propsTileset, _propsShadows));
                    break;
            }
        }
        #endregion
    }
}
