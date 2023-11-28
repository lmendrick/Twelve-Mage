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

        private List<Summoner> summoners;
        private List<Enemy> enemies;

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
        private float runningSpeed;
        private bool withinArea = false;
        private int safeDistance = 200;
        private bool playerAvoided = true;
        private int wanderRadius = 100;
        private float playerSummonDistance = 250;
        private float summonTimer = 5.0f;
        private bool hasSummoned = false;
        private int numToSummon = 5;

        /// <summary>
        /// corners[0] = upperLeftCorner
        /// corners[1] = upperRightCorner
        /// corners[2] = lowerLeftCorner
        /// corners[3] = lowerRightCorner
        /// </summary>
        private List<Vector2> corners;


        public Summoner(Rectangle rec, TextureLibrary textureLibrary, int health, List<Enemy> enemies, Player player, int maxEnemies, int windowWidth, int windowHeight, List<Summoner> summoners, Random rng) 
            : base(rec, textureLibrary, health, enemies, player, rng)
        {
            this.rng = rng;
            _textureLibrary = textureLibrary;

            personalSpawner = new Spawner(this.Position, 50, 50, enemies, summoners, textureLibrary, 100, player, Rectangle.Empty, windowWidth, windowHeight, rng);
            playerSpawner = new Spawner(player.Position, 100, 100, enemies, summoners, textureLibrary, 100, player, Rectangle.Empty, windowWidth, windowHeight, rng);
            spawned = new List<Enemy>();

            // Scale of the sprite
            drawScale = 1.5f;
            // Match rectangle scale to draw scale
            rec.X *= (int)drawScale;
            rec.Y *= (int)drawScale;

            this.maxEnemies = maxEnemies;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.player = player;
            this.summoners = summoners;
            this.enemies = enemies;

            speed = 100f;
            corners = new List<Vector2>();
            summoners.Add(this);
            currentEnemies = 0;
            runningSpeed = speed * 2;
            Color = Color.Teal;


            upperLeftCorner = new Vector2(windowWidth / 4, windowHeight / 4);
            upperRightCorner = new Vector2(windowWidth * 0.75f, windowHeight / 4);
            lowerLeftCorner = new Vector2(windowWidth / 4, windowHeight * 0.75f);
            lowerRightCorner = new Vector2(windowWidth * 0.75f, windowHeight * 0.75f);

            corners.Add(upperLeftCorner);
            corners.Add(upperRightCorner);
            corners.Add(lowerLeftCorner);
            corners.Add(lowerRightCorner);

            currentDestination = corners[rng.Next(0,4)];
        }


        /// <summary>
        /// Handles summoner update logic
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="bullets"></param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {

            // Time Freeze Spell
            if (IsFrozen)
            {
                speed = 0;
            }
            else
            {
                speed = 75f;
            }


            // Update state for animations
            // Don't update animation state if freeze spell is active
            if (!isFrozen)
            {
                // Set the enemy's animation state based on movement direction
                if (dir.X < 0)
                {
                    state = EnemyState.WalkLeft; // Walking left
                }
                if (dir.X > 0)
                {
                    state = EnemyState.WalkRight; // Walking right
                }

                // Handle vertical movement state
                if (dir.Y != 0)
                {
                    if (state == EnemyState.WalkLeft || state == EnemyState.FaceLeft)
                    {
                        state = EnemyState.WalkLeft; // Walking left
                    }
                    if (state == EnemyState.WalkRight || state == EnemyState.FaceRight)
                    {
                        state = EnemyState.WalkRight; // Walking right
                    }
                }

                // Set the player's state to facing left or right when not moving
                if (dir.X == 0 && dir.Y == 0)
                {
                    if (state == EnemyState.WalkLeft)
                    {
                        state = EnemyState.FaceLeft; // Facing left
                    }
                    else if (state == EnemyState.WalkRight)
                    {
                        state = EnemyState.FaceRight; // Facing right
                    }
                }
            }

            // Change enemy color based on health (Lucas)
            if (health < 75 && health >= 50)
            {
                color = Color.Orange;
            }
            else if (health < 50 && health >= 25)
            {
                color = Color.IndianRed;
            }
            else if (health < 25)
            {
                color = Color.Red;
            }



            GoToLocation(currentDestination, wanderRadius, gameTime);

            int playerDistance = (int)(player.PosVector - this.Position).Length();

            //If the player comes within a safe distance and the summoner has not already begun to attempt to flee, choose a new location to run to
            if(!SafePlayerDistance(playerDistance) && playerAvoided)
            {
                DetermineDestination();
                playerAvoided = false;
                speed = runningSpeed;
            }
            else if(SafePlayerDistance(playerDistance))
            {
                playerAvoided = true;
                speed = runningSpeed / 2;
            }

            CheckHits(bullets);
            if(health <= 0)
            {
                summoners.Remove(this);
            }

            // remove summoner from list if killed by fireball
            if (state == EnemyState.Dead)
            {
                summoners.Remove(this);
            }

        }



        /// <summary>
        /// Handles summoning logic
        /// </summary>
        /// <param name="gameTime"></param>
        public void Summoning(GameTime gameTime)
        {
            //If the summoner is within its target area, and is a certain distance from the player, and hasn't already summoned, activate player summoning
            if (withinArea && GetDistance(player.PosVector, this.Position) > playerSummonDistance && !hasSummoned)
            {
                PlayerSummon();

            }
            else if (hasSummoned) //If has already summoned, decrement timer
            {
                summonTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


            //If the player is too close to the summoner, and hasn't already summoned, summon enemies using it's personal spawner
            if(GetDistance(player.PosVector, this.Position) < safeDistance && !hasSummoned)
            {
                DefensiveSummon();
            }

            //Check to make sure timer hasn't run out
            if (summonTimer <= 0)
            {
                hasSummoned = false;
                summonTimer = 5.0f;
            }

        }

        /// <summary>
        /// Prompts the playerSpawner to spawn enemies
        /// </summary>
        public void PlayerSummon()
        {
            playerSpawner.Position = player.PosVector;
            hasSummoned = true;

            for(int i = 0; i < numToSummon; i++)
            {
                playerSpawner.SpawnEnemy();
            }
        }

        /// <summary>
        /// Prompts the personalSpawner to spawn enemies
        /// </summary>
        public void DefensiveSummon()
        {
            personalSpawner.Position = this.Position;
            hasSummoned = true;
            for (int i = 0; i < numToSummon * 1.5; i++)
            {
                personalSpawner.SpawnEnemy();
            }
        }



        /// <summary>
        /// Returns the distance between two vectors
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public float GetDistance(Vector2 vec1, Vector2 vec2)
        {
            return (vec2 - vec1).Length();
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
                dir.X += rng.Next((int)(-wanderRadius * 0.75f), (int)(wanderRadius * 0.75f) + 1);
                dir.Y += rng.Next((int)(-wanderRadius * 0.75f), (int)(wanderRadius * 0.75f) + 1);
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
                if (!cornersByDistance.ContainsKey(playerDistance))
                {
                    cornersByDistance.Add(playerDistance, corner);
                }

                
            }

            //Sets the current destination to the corner with the highest distance from the player
            currentDestination = cornersByDistance[highestDistance];
            cornersByDistance.Clear();
        }




    }
}
