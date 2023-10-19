using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
    * Chloe Hall
    * Twelve-Mage
    * This class inherits from Tile, and is specifically for grass tiles
    * No known issues.
    */
namespace TwelveMage
{
    internal class GrassTile : Tile
    {
        public GrassTile(Texture2D texture, Vector2 loc) : base(texture, loc)
        {

        }
    }
}
