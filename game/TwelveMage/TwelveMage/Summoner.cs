using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    internal class Summoner : Enemy
    {
        private Random rng;

        private Spawner personalSpawner;
        private Spawner playerSpawner;

        private Player player;

        private List<Enemy> spawned;

        private int maxEnemies;
        private int currentEnemies;

        private int windowWidth;
        private int windowHeight;

        private Vector2 upperLeftCorner;
        private Vector2 lowerLeftCorner;
        private Vector2 upperRightCorner;
        private Vector2 lowerRightCorner;

        private Vector2 currentDestination;

        private Vector2 dir;


        private float speed = 75f;
        private bool withinArea = false;
        private int safeDistance = 75;
        private bool playerAvoided = true;

        /// <summary>
        /// corners[0] = upperLeftCorner
        /// corners[1] = upperRightCorner
        /// corners[2] = lowerLeftCorner
        /// corners[3] = lowerRightCorner
        /// </summary>
        private List<Vector2> corners = new List<Vector2>();


        public Summoner(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player, int maxEnemies, int windowWidth, int windowHeight) : base(rec, texture, health, enemies, player)
        {
            rng = new Random();
            personalSpawner = new Spawner(this.Position, 50, 50, enemies, texture, 100, player, Rectangle.Empty, windowWidth, windowHeight);
            playerSpawner = new Spawner(player.Position, 100, 100, enemies, texture, 100, player, Rectangle.Empty, windowWidth, windowHeight);
            spawned = new List<Enemy>();
            this.maxEnemies = maxEnemies;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.player = player;
            currentEnemies = 0;


            upperLeftCorner = new Vector2(windowWidth / 4, windowHeight / 4);
            upperRightCorner = new Vector2(windowWidth * 0.75f, windowHeight / 4);
            lowerLeftCorner = new Vector2(windowWidth / 4, windowHeight * 0.75f);
            lowerRightCorner = new Vector2(windowWidth * 0.75f, windowHeight * 0.75f);

            corners.Add(upperLeftCorner);
            corners.Add(upperRightCorner);
            corners.Add(lowerLeftCorner);
            corners.Add(lowerRightCorner);

            currentDestination = upperLeftCorner;
        }


        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            GoToLocation(currentDestination, 100, gameTime);

            int playerDistance = (int)(player.PosVector - this.Position).Length();

            if(!SafePlayerDistance(playerDistance) && playerAvoided)
            {
                DetermineDestination();
                playerAvoided = false;
            }
            else if(SafePlayerDistance(playerDistance))
            {
                playerAvoided = true;
            }
        }

        /// <summary>
        /// Returns whether or not the enemy is currently a safe distance from the player
        /// </summary>
        /// <param name="distance">The player distance</param>
        /// <returns></returns>
        public bool SafePlayerDistance(int distance)
        {
            return distance > safeDistance;
        }

        /// <summary>
        /// Go to a given location
        /// </summary>
        /// <param name="location">The location to go to</param>
        /// <param name="wanderRadius">The radius of the destination area</param>
        /// <param name="gameTime"></param>
        public void GoToLocation(Vector2 location, int wanderRadius, GameTime gameTime)
        {
            //If the summoner is outside of the zone, move towards it
            int distance = (int)(location - this.Position).Length();

            //If the summoner is outside of the locations area, go towards the destination
            if (distance > wanderRadius)
            {
                dir = location - this.Position;
                dir.Normalize();
                this.Position += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                withinArea = false;
            }
            else
            {
                if (!withinArea)
                {
                    withinArea = true;
                    dir = new Vector2(rng.Next(-10, 11), rng.Next(-10, 11));
                    dir.Normalize();
                }
                WanderArea(location, wanderRadius, gameTime);
            }

            rec.X = (int)this.Position.X;
            rec.Y = (int)this.Position.Y;
        }


        /// <summary>
        /// The summoner will wander around the given area
        /// </summary>
        /// <param name="location">The location to wander around</param>
        /// <param name="wanderRadius">How far around it to wander</param>
        /// <param name="gameTime"></param>
        public void WanderArea(Vector2 location, int wanderRadius, GameTime gameTime)
        {
            //The distance between the summoner and the location
            int distance = (int)(location - this.Position).Length();

            this.Position += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            //If the summoner is farther than the acceptable distance from the location, readjust it's direction
            if (distance > wanderRadius)
            {
                dir = location - this.Position;
                dir.X += rng.Next(-wanderRadius * (3 / 4), (wanderRadius * (3 / 4)) + 1);
                dir.Y += rng.Next(-wanderRadius * (3 / 4), (wanderRadius * (3 / 4)) + 1);
            }


            dir.Normalize();
        }

        /// <summary>
        /// Determines the corner farthest from the player, and sets that as the new currentDestination
        /// </summary>
        public void DetermineDestination()
        {
            //Store the corners by the player's distance from them
            Dictionary<int, Vector2> cornersByDistance = new Dictionary<int, Vector2>();

            //Track the highest distance
            int highestDistance = 0;

            foreach(Vector2 corner in corners)
            {
                //Determines the players distance from the corner
                int playerDistance = (int)(player.PosVector - corner).Length();


                //If this distance is greater than the greatest previously encountered, update tracker
                if(playerDistance > highestDistance)
                {
                    highestDistance = playerDistance;
                }

                //Add to the dictionary
                cornersByDistance.Add(playerDistance, corner);

                
            }

            //Sets the current destination to the corner with the highest distance from the player
            currentDestination = cornersByDistance[highestDistance];
            cornersByDistance.Clear();
        }




    }
}
