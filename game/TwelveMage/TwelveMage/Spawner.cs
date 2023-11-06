using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveMage
{
    internal class Spawner
    {
        //The location of the spawner
        private Vector2 position;

        //The radius in either the X or Y direction that the spawner can spawn an enemy into
        private int xRadius;
        private int yRadius;

        private int upperXRange;
        private int upperYRange;
        private int lowerXRange;
        private int lowerYRange;

        //Enemies list
        private List<Enemy> enemies;

        private Texture2D enemyTexture;
        private int enemyHealth;

        private Player player;

        //Random
        private Random rng;

        // Property to re-reference enemies if it becomes a new List
        public List<Enemy> Enemies
        {
            set { enemies = value; }
        }

        /// <summary>
        /// Create a new spawner
        /// </summary>
        /// <param name="position">Where the spawner is centered</param>
        /// <param name="xRadius">The X spawn radius</param>
        /// <param name="yRadius">The Y spawn radius</param>
        public Spawner(Vector2 position, int xRadius, int yRadius, List<Enemy> enemies, Texture2D enemyTexture, int enemyHealth, Player player)
        {
            this.position = position;
            this.xRadius = xRadius;
            this.yRadius = yRadius;
            this.enemies = enemies;
            this.enemyTexture = enemyTexture;
            this.enemyHealth = enemyHealth;
            this.player = player;
            rng = new Random();

            upperXRange = (int)position.X + xRadius;
            upperYRange = (int)position.Y + YRadius;
            lowerXRange = (int)position.X - xRadius;
            lowerYRange = (int)position.Y - YRadius;

        }

        /// <summary>
        /// Get the position
        /// </summary>
        public Vector2 Position { get { return position; } }
        /// <summary>
        /// Get the X radius
        /// </summary>
        public int XRadius { get {  return xRadius; } }
        /// <summary>
        /// Get the Y radius
        /// </summary>
        public int YRadius { get { return yRadius; } }

        public void SpawnEnemy()
        {
            enemies.Add(new Enemy(
                new Rectangle(
                    rng.Next(lowerXRange, upperXRange + 1),
                    rng.Next(lowerYRange, upperYRange + 1),
                    30,
                    30),
                enemyTexture,
                enemyHealth,
                enemies,
                player));



        }
    }
}
