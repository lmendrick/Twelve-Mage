using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TwelveMage
{
    /*
 * Anthony Maldonado
 * Twelve-Mage
 * This class handles the enemy 
 * including movement, actions
 * No known issues
 * might try to make it such that the player and enemy inherit health and max health in the future
 * Lucas: Added enemy movement
 * Chloe: Added public getproperties for health and position, to save the enemies to a file
 * Added Clone() method
 * Added OnDeath event + delegate
 */

    public delegate void OnDeathDelegate();

    internal class Enemy : GameObject
    {
        private enum AdjustmentDirection
        {
            Up,
            Down,
            Left,
            Right
        }


        #region FIELDS
        //private int health;
        private const int MAX_Health = 200;
        private int damage;

        // Enemy Movement
        private Texture2D sprite;
        private Vector2 dir;
        private Vector2 pos;
        private float speed = 100f;
        private Vector2 playerPos;
        private bool intersectionDetected;
        private AdjustmentDirection direction;

        //Damage feedback
        private Color color = Color.White;
        private double timer;
        private bool hit;
        #endregion

        #region PROPERTIES
        public Vector2 Position // Property to set the current player position
        {
            get { return pos; }
        }
        public Vector2 PlayerPos // Property to set this enemy's position
        {
            get { return playerPos; }
            set { playerPos = value; }
        }
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public bool IsActive //Property to access the active field
        {
            get { return isActive; }
            set { isActive = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Enemy(Rectangle rec, Texture2D texture, int health) : base(rec, texture, health)
        {
            this.rec = rec;
            this.sprite = texture;
            this.pos = new Vector2(rec.X, rec.Y);
            this.health = health;
            intersectionDetected = false;
            timer = 1f;
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Lucas:
        /// Update enemy position based on player position
        /// </summary>
        /// <param name="gameTime">
        /// GameTime from main
        /// </param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            // Set enemy direction based on current player position]
            dir = playerPos - this.pos;
            dir.Normalize();

            // Update enemy position to move towards player at set speed
            pos += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            // Update rectangle position
            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);

            CheckHits(bullets);

            //Not sure why it isn't drawing the enemy red when they are hit
            if(hit)
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                color = Color.Red;
            }

            if(timer <= 0)
            {
                hit = false;
                timer = 1f;
                color = Color.White;
            }
        } 

        /// <summary>
        /// Lucas:
        /// Draw the enemies at their updated positions (will eventually be animated)
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.isActive)
            {
                spriteBatch.Draw(sprite, pos, color);
            }
        }


        /// <summary>
        /// Check the list of bullets to see if any have hit this enemy
        /// </summary>
        /// <param name="bulletList"></param>
        public void CheckHits(List<GameObject> bulletList)
        {
            for(int i = bulletList.Count - 1; i >= 0; i--)
            {
                if (bulletList[i].CheckCollision(this))
                {
                    health -= 20;
                    bulletList.RemoveAt(i);
                    hit = true;
                }
            }

            if(health <= 0)
            {
                if (OnDeath != null) OnDeath(); // Use OnDeath event
                isActive = false;
            }
        }


        /// <summary>
        /// If a collision with the player is detected, damage them
        /// </summary>
        /// <param name="player"></param>
        public void DamagePlayer(Player player)
        {
            if(this.CheckCollision(player))
            {
                player.Health -= damage;
            }
        }


        public void AvoidIntersections(List<Enemy> enemies)
        {
            //Determine if an intersection is detected
            //If one is, determine the direction to go in
            //Go in that direction until no intersection is detected
            //Should only determine direction once
            Enemy intersected;
            Rectangle intersection;

            if (!intersectionDetected)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (this.CheckCollision(enemy))
                    {
                        intersected = enemy;
                        
                        intersectionDetected = true;
                        break;
                    }
                }

                if(intersectionDetected)
                {
                    //Determine the shortest direction to resolve the collision
                    //Do this by checking the difference in position
                    //Once a direction is chosen, adjust dir X or Y as appropriate. 
                    //Since this method should be called before movement is calculated, simply setting the X or Y equal to a value should work
                }
            }
        }

        /// <summary>
        /// Returns a copy of this Enemy
        /// </summary>
        public Enemy Clone()
        {
            return new Enemy(rec, sprite, health);
        }

        public event OnDeathDelegate OnDeath; // OnDeath event, for scoring
        #endregion
    }
}
