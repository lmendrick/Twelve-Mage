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
    * and assigning them textures based on their type
    * No known issues.
    */
    internal class BackgroundManager
    {
        #region FIELDS
        private Tile[,] tiles;
        private int tilesWidth;
        private int tilesHeight;
        private StreamReader reader;
        #endregion

        #region PROPERTIES
        public int TilesWidth { get {  return tilesWidth; } }
        public int TilesHeight { get {  return tilesHeight; } }
        #endregion

        #region CONSTRUCTORS
        public BackgroundManager()
        {

        }
        #endregion

        #region METHODS
        public void LoadFile(string filename)
        {
            reader = new StreamReader(filename);
        }
        #endregion
    }
}
