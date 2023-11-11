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


        public Summoner(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player, int maxEnemies, int windowWidth, int windowHeight) : base(rec, texture, health, enemies, player)
        {
            rng = new Random();
            personalSpawner = new Spawner(this.Position, 50, 50, enemies, texture, 100, player, Rectangle.Empty, windowWidth, windowHeight);
            playerSpawner = new Spawner(player.Position, 100, 100, enemies, texture, 100, player, Rectangle.Empty, windowWidth, windowHeight);
            spawned = new List<Enemy>();
            this.maxEnemies = maxEnemies;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            currentEnemies = 0;
            

            upperLeftCorner = new Vector2(windowWidth / 5, windowHeight / 5);
            upperRightCorner = new Vector2(windowWidth * (4/5), windowHeight / 5);
            lowerLeftCorner = new Vector2(windowWidth / 5, windowHeight * (4/5));
            lowerRightCorner = new Vector2(windowWidth * (4/5), windowHeight * (4/5));
        }


        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            GoToLocation(upperLeftCorner, 100, gameTime);
        }


        public void GoToLocation(Vector2 location, int wanderRadius, GameTime gameTime)
        {
            int distance = (int)(location - this.Position).Length();

            if(distance > wanderRadius )
            {
                dir = location - this.Position;
                dir.Normalize();
                this.Position += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                withinArea = false;
            }
            else
            {
                if(!withinArea)
                {
                    withinArea = true;
                    dir = new Vector2(rng.Next(-10, 11), rng.Next(-10, 11));
                    dir.Normalize();
                }
                WanderArea(location, wanderRadius, gameTime);
            }
            
        }

        public void WanderArea(Vector2 location, int wanderRadius, GameTime gameTime)
        {
            this.Position += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            if (this.Position.X <= location.X - wanderRadius || this.Position.X >= location.X + wanderRadius || this.Position.Y <= location.Y - wanderRadius || this.Position.Y >= location.Y + wanderRadius)
            {
                dir = location - this.Position;
                dir.X += rng.Next(-wanderRadius / 2, (wanderRadius / 2) + 1);
                dir.Y += rng.Next(-wanderRadius / 2, (wanderRadius / 2) + 1);
            }


            dir.Normalize();
        }




    }
}
