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
 * Zombie Enemy type
 */
namespace TwelveMage
{
    internal class Zombie : Enemy
    {
        double zombieSpeed = 1.0; // Zombie-Specific speed multiplier

        public Zombie(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player, Texture2D corpseSprite) : base(rec, texture, health, enemies, player, corpseSprite)
        {

        }
    }
}
