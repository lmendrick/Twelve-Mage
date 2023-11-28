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

        private Rectangle noSpawningArea;

        //Enemies list
        private List<Enemy> enemies;
        private List<Summoner> summoners;

        private TextureLibrary _textureLibrary;
        private int enemyHealth;
        private int summonerHealth;
        private int chargerHealth;

        private Player player;

        //Random
        private Random rng;

        private int windowWidth;
        private int windowHeight;

        // Property to re-reference enemies if it becomes a new List
        public List<Enemy> Enemies
        {
            set { enemies = value; }
        }

        public Rectangle NoSpawningArea
        {
            get { return noSpawningArea; }

        }

        /// <summary>
        /// Create a new spawner
        /// </summary>
        /// <param name="position">Where the spawner is centered</param>
        /// <param name="xRadius">The X spawn radius</param>
        /// <param name="yRadius">The Y spawn radius</param>
        /// <param name="noSpawningArea">The rectangle to use as reference when constructing the noSpawnArea. The X/Y coords don't matter, only the width/height</param>
        public Spawner(Vector2 position, int xRadius, int yRadius, List<Enemy> enemies, List<Summoner> summoners, TextureLibrary textureLibrary, int enemyHealth, Player player, Rectangle noSpawningArea, int windowWidth, int windowHeight, Random rng)
        {
            _textureLibrary = textureLibrary;

            this.position = position;
            this.xRadius = xRadius;
            this.yRadius = yRadius;
            this.enemies = enemies;
            this.summoners = summoners;
            this.enemyHealth = enemyHealth;
            summonerHealth = (int)((double)enemyHealth * 4); // Don't look at this
            chargerHealth = (int)((double)enemyHealth * 5);
            this.player = player;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.rng = rng;
            this.noSpawningArea = new Rectangle((int)position.X - (noSpawningArea.Width / 2), (int)position.Y - (noSpawningArea.Height / 2), noSpawningArea.Width, noSpawningArea.Height);


            upperXRange = (int)position.X + xRadius;
            upperYRange = (int)position.Y + YRadius;
            lowerXRange = (int)position.X - xRadius;
            lowerYRange = (int)position.Y - YRadius;

        }

        /// <summary>
        /// Get the position
        /// </summary>
        public Vector2 Position 
        {
            get { return position; }
            set { position = value; }
        }
        /// <summary>
        /// Get the X radius
        /// </summary>
        public int XRadius { get {  return xRadius; } }
        /// <summary>
        /// Get the Y radius
        /// </summary>
        public int YRadius { get { return yRadius; } }


        public Summoner SpawnSummoner()
        {
            upperXRange = (int)position.X + xRadius;
            upperYRange = (int)position.Y + YRadius;
            lowerXRange = (int)position.X - xRadius;
            lowerYRange = (int)position.Y - YRadius;

            noSpawningArea.X = (int)position.X - (noSpawningArea.Width / 2);
            noSpawningArea.Y = (int)position.Y - (noSpawningArea.Height / 2);

            Summoner spawned = new Summoner(
                new Rectangle(
                    rng.Next(lowerXRange, upperXRange + 1),
                    rng.Next(lowerYRange, upperYRange + 1),
                    45,
                    45),
                _textureLibrary,
                summonerHealth,
                enemies,
                player,
                10,
                windowWidth,
                windowHeight,
                summoners,
                rng);


            float xDistanceFromPlayer = player.PosVector.X - spawned.X;
            float yDistanceFromPlayer = player.PosVector.Y - spawned.Y;


            if (Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50)
            {
                do
                {
                    //spawned.X = rng.Next((int)(lowerXRange * 1.25), (int)((upperXRange + 1) * 1.25));
                    //spawned.Y = rng.Next((int)(lowerYRange * 1.25), (int)((upperYRange + 1) * 1.25));
                    spawned.X += rng.Next(-50, 51);
                    spawned.Y += rng.Next(-50, 51);
                    xDistanceFromPlayer = player.Rec.X - spawned.X;
                    yDistanceFromPlayer = player.Rec.Y - spawned.Y;
                } while (Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50);
            }




            enemies.Add(spawned);

            return spawned;
        }


        public Charger SpawnCharger()
        {
            upperXRange = (int)position.X + xRadius;
            upperYRange = (int)position.Y + YRadius;
            lowerXRange = (int)position.X - xRadius;
            lowerYRange = (int)position.Y - YRadius;

            noSpawningArea.X = (int)position.X - (noSpawningArea.Width / 2);
            noSpawningArea.Y = (int)position.Y - (noSpawningArea.Height / 2);

            Charger spawned = new Charger(
                new Rectangle(
                    rng.Next(lowerXRange, upperXRange + 1),
                    rng.Next(lowerYRange, upperYRange + 1),
                    60,
                    60),
                _textureLibrary,
                chargerHealth,
                enemies,
                player,
                rng);



            float xDistanceFromPlayer = player.PosVector.X - spawned.X;
            float yDistanceFromPlayer = player.PosVector.Y - spawned.Y;


            if (Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50)
            {
                do
                {
                    //spawned.X = rng.Next((int)(lowerXRange * 1.25), (int)((upperXRange + 1) * 1.25));
                    //spawned.Y = rng.Next((int)(lowerYRange * 1.25), (int)((upperYRange + 1) * 1.25));
                    spawned.X += rng.Next(-50, 51);
                    spawned.Y += rng.Next(-50, 51);
                    xDistanceFromPlayer = player.Rec.X - spawned.X;
                    yDistanceFromPlayer = player.Rec.Y - spawned.Y;
                } while (Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50);
            }




            enemies.Add(spawned);

            return spawned;
        }

        /// <summary>
        /// Spawns a new enemy within the spawn area;
        /// </summary>
        /// <returns>The spawned enemy</returns>
        public Enemy SpawnEnemy()
        {
            upperXRange = (int)position.X + xRadius;
            upperYRange = (int)position.Y + YRadius;
            lowerXRange = (int)position.X - xRadius;
            lowerYRange = (int)position.Y - YRadius;

            noSpawningArea.X = (int)position.X - (noSpawningArea.Width / 2);
            noSpawningArea.Y = (int)position.Y - (noSpawningArea.Height / 2);

            Enemy spawned = new Enemy(
                new Rectangle(
                    rng.Next(lowerXRange, upperXRange + 1),
                    rng.Next(lowerYRange, upperYRange + 1),
                    30,
                    30),
                _textureLibrary,
                enemyHealth,
                enemies,
                player,
                rng);

            /*
            if(spawned.Rec.Intersects(NoSpawningArea))
            {
                do
                {
                    spawned.X = rng.Next(lowerXRange, upperXRange + 1);
                    spawned.Y = rng.Next(lowerYRange, upperYRange + 1);
                }while(spawned.Rec.Intersects(NoSpawningArea));

            }
            */
            float xDistanceFromPlayer = player.PosVector.X - spawned.X;
            float yDistanceFromPlayer = player.PosVector.Y - spawned.Y;

            
            if(Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50)
            {
                do
                {
                    //spawned.X = rng.Next((int)(lowerXRange * 1.25), (int)((upperXRange + 1) * 1.25));
                    //spawned.Y = rng.Next((int)(lowerYRange * 1.25), (int)((upperYRange + 1) * 1.25));
                    spawned.X += rng.Next(-50, 51);
                    spawned.Y += rng.Next(-50, 51);
                    xDistanceFromPlayer = player.Rec.X - spawned.X;
                    yDistanceFromPlayer = player.Rec.Y - spawned.Y;
                } while (Math.Abs(xDistanceFromPlayer) <= 50 || Math.Abs(yDistanceFromPlayer) <= 50);
            }
            
            
            

            enemies.Add(spawned);

            return spawned;

        }

        public Enemy SpawnSpecial()
        {
            if(rng.Next(0,2) == 0)
            {
                return SpawnSummoner();
            }
            else
            {
                return SpawnCharger();
            }
        }
    }
}
