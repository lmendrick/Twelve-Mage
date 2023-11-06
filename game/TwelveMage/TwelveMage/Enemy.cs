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

    enum EnemyState
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight
    }

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
        private List<Enemy> enemies;

        // Enemy Movement
        private Texture2D sprite;
        private Vector2 dir;
        private Vector2 pos;
        private float speed = 100f;
        private Vector2 playerPos;
        private bool intersectionDetected;
        private AdjustmentDirection direction;

        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 7;       // The number of frames in the animation
        const int EnemyRectOffsetY = 67;   // How far down in the image are the frames?
        const int EnemyRectOffsetX = 4;
        const int EnemyRectHeight = 30;     // The height of a single frame
        const int EnemyRectWidth = 32;      // The width of a single frame

        private EnemyState state;

        private int index;
        private Random rng;

        /// <summary>
        /// Used for determining collision avoidance priority
        /// 
        /// If it is true, this enemy will not move to avoid collisions and will instead allow them to move
        /// True = Don't avoid
        /// False = Avoid
        /// </summary>
        private bool priority;

        /// <summary>
        /// Used for determining which direction to take perpendicularly to the players direction
        /// 
        /// True = Clockwise
        /// False = Counter Clockwise
        /// </summary>
        private bool clockwiseOrCounterClockwise;

        /// <summary>
        /// Determines whether or not all collisions have been resolved
        /// </summary>
        private bool collisionsResolved;

        private Vector2 perpendicularVec;

        //Damage feedback
        private Color color = Color.White;
        private double timer;
        private bool hit;

        private bool isFrozen;
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

        public bool IsFrozen
        {
            get { return isFrozen; }
            set {  isFrozen = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Enemy(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies) : base(rec, texture, health)
        {
            this.rec = rec;
            this.sprite = texture;
            this.pos = new Vector2(rec.X, rec.Y);
            this.health = health;
            this.enemies = enemies;

            rng = new Random();

            intersectionDetected = false;
            collisionsResolved = true;
            priority = false;
            perpendicularVec = Vector2.Zero;
            timer = 1f;

            // Default sprite direction
            state = EnemyState.FaceRight;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
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
            index = enemies.IndexOf(this);

            // TimeFreeze Spell
            TimeFreeze();


            // Set enemy direction based on current player position]
            dir = playerPos - this.pos;
            dir.Normalize();

            //Collision avoidance logic
            AvoidCollisions(enemies, gameTime);

            // Update enemy position to move towards player at set speed
            pos += dir * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            // Update rectangle position
            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);

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


            CheckHits(bullets);

            //Not sure why it isn't drawing the enemy red when they are hit
            if(hit)
            {
                timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                //color = Color.Red;
            }

            if(timer <= 0)
            {
                hit = false;
                timer = 1f;
               // color = Color.White;
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
        } 

        /// <summary>
        /// Lucas:
        /// Draw the enemies based on their current state
        /// </summary>
        /// <param name="spriteBatch">
        /// SpriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {

            #region State switch
            switch (state)
            {
                case EnemyState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case EnemyState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case EnemyState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case EnemyState.WalkLeft:
                    DrawWalking(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
            }
            #endregion
        }


        /// <summary>
        /// Lucas:
        /// Draws the player character idle animation.
        /// Adapted from Mario Walking PE.
        /// </summary>
        /// <param name="flipSprite">
        /// Allows the sprite to be flipped horizontally when facing directions
        /// </param>
        /// <param name="spriteBatch">
        /// The SpriteBatch passed in from main
        /// </param>
        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(
                    0,
                    EnemyRectOffsetY,
                    EnemyRectWidth,
                    EnemyRectHeight),
                    color,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

        /// <summary>
        /// Lucas:
        /// Draws the player character walking animation, based on current frame.
        /// Adapted from Mario Walking PE.
        /// </summary>
        /// <param name="flipSprite">
        /// Allows the sprite to be flipped horizontally when facing directions
        /// </param>
        /// <param name="spriteBatch">
        /// The SpriteBatch passed in from main
        /// </param>
        private void DrawWalking(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,                            // - The texture to draw
                pos,                                    // - The location to draw on the screen
                new Rectangle(                          // - The "source" rectangle
                    frame * EnemyRectWidth,            // - This rectangle specifies
                    EnemyRectOffsetY,                  //	 where "inside" the texture
                    EnemyRectWidth,                    //   to get pixels (We don't want to
                    EnemyRectHeight),                  //   draw the whole thing)
                    color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
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


        public void ResolveIntersection(List<Enemy> enemies)
        {
            /*
             * First determine if the enemy is intersecting with any other enemies
             * Since the solution is the same regardless of which enemy is being intersected, we can break out of searching for intersections once one has been found
             * 
             * Once an intersection has been detected, find the perpendicular Vector to the dir field
             * Then determine what direction is appropriate for the adjustment and apply it to pos
             * This will allow us to move the enemy horizontally, spreading the enemies out to avoid intersection
             */

            //Loop through enemies list looking intersections
            //If detected, set a bool to true and break out of loop
            //Loop should only occur if intersection wasn't already detected
            if (!intersectionDetected)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (this.CheckCollision(enemy))
                    {
                        intersectionDetected = true;
                        break;
                    }
                }

                intersectionDetected = false;
            }
            


            //Once intersection is detected, calculate the perpendicular vector of dir
            //Check to see which direction should be used
            //For this use the position of the intersected enemy

            //Gonna require using a dot product xd


        }

        /// <summary>
        /// Checks list of enemies for any collisions.
        /// 
        /// If any are detected, determine whether or not this enemy should have priority
        /// 
        /// If it doesn't have priority, "flip a coin" to determine which direction to go perpendicular to the dir Vector2
        /// This should remain the same direction until no collisions are detected
        /// 
        /// 
        /// </summary>
        /// <param name="enemies"></param>
        public void AvoidCollisions(List<Enemy> enemies, GameTime gameTime)
        {

            //Check list
            foreach(Enemy enemy in enemies)
            {
                if (this.CheckCollision(enemy))
                {
                    intersectionDetected = true;
                    collisionsResolved = false;
                    DeterminePriority(enemies);
                    break;
                }

            }

            if (collisionsResolved)
            {
                perpendicularVec = Vector2.Zero;
            }

            //If an intersection was found determine which direction to move to avoid it, 
            if (intersectionDetected && !priority)
            {
                if (!collisionsResolved)
                {
                    if (rng.Next(0, 2) == 0)
                    {
                        clockwiseOrCounterClockwise = true;
                    }
                    else
                    {
                        clockwiseOrCounterClockwise = false;
                    }
                }

                
                if (clockwiseOrCounterClockwise)
                {
                    perpendicularVec = new Vector2(dir.Y, -dir.X);
                }
                else
                {
                    perpendicularVec = new Vector2(-dir.Y, dir.X);
                }

                pos += perpendicularVec * (float)gameTime.ElapsedGameTime.TotalSeconds * speed * 2;
            }

            if (!IsCollidingWithAny(enemies))
            {
                collisionsResolved = true;
                intersectionDetected = false;
            }

            
            /*
            foreach(Enemy enemy in enemies)
            {
                if (this.CheckCollision(enemy))
                {
                    intersectionDetected = true;
                    collisionsResolved = false;
                    
                    break;
                }
                intersectionDetected = false;
                collisionsResolved = true;
            }

            if(collisionsResolved)
            {
                perpendicularVec = Vector2.Zero;
                priority = true;
            }
            */


            //pos += perpendicularVec * (float)gameTime.ElapsedGameTime.TotalSeconds * speed * 2;
        }

        /// <summary>
        /// Determines the priority of the enemy
        /// 
        /// If the enemy is the first in the list, it gets priority
        /// 
        /// If the enemy is not intersecting with any enemies lower than it in the list, it gets priority
        /// </summary>
        /// <param name="enemies"></param>
        public void DeterminePriority(List<Enemy> enemies)
        {
            if(index == 0)
            {
                priority = true;
                return;
            }
            else
            {
                //Checks every index lower than it's own index
                //If a collision is detected, set priority to false and break out of the loop
                //If no collision is detected, priority will be true once it exits the loop
                for(int i = index - 1; i >= 0; i--)
                {
                    if (this.CheckCollision(enemies[i]))
                    {
                        priority = false;
                        break;
                    }

                    priority = true;
                }
            }
        }

        public bool IsCollidingWithAny(List<Enemy> enemies)
        {
            foreach(Enemy enemy in enemies)
            {
                if (this.CheckCollision(enemy))
                {
                    return true;
                }
            }

            return false;
        }

        

        /// <summary>
        /// Returns a copy of this Enemy
        /// </summary>
        public Enemy Clone()
        {
            return new Enemy(rec, sprite, health, enemies);
        }

        public event OnDeathDelegate OnDeath; // OnDeath event, for scoring

        /// <summary>
        /// Enemies stop moving during TimeFreeze. See Player class for code.
        /// </summary>
        public void TimeFreeze()
        {
            if (isFrozen)
            {
                speed = 0;
            }
            else
            {
                speed = 100;
            }
        }

        /// <summary>
        /// Lucas:
        /// Updates the enemy animation
        /// Adapted from Mario Walking PE
        /// </summary>
        /// <param name="gameTime">
        /// GameTime passed in from main
        /// </param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Don't update animations if freeze spell is active
            if (!isFrozen)
            {
                // How much time has passed?  
                timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

                // If enough time has passed:
                if (timeCounter >= timePerFrame)
                {
                    frame += 1;                     // Adjust the frame to the next image

                    if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                        frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                    timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                    // This keeps the time passed 
                }
            }
        }
        #endregion
    }
}
