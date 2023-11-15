using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        WalkRight,
        Dead
    }

    public delegate void OnDeathDelegate();

    internal class Enemy : GameObject
    {

        #region FIELDS
        private const int MAX_Health = 200;
        private int damage;
        private List<Enemy> enemies;
        private bool hasHealthpack;

        private Player player;

        // Enemy Movement
        private Texture2D sprite;
        private Texture2D corpseSprite;
        private Vector2 dir;
        private Vector2 pos;
        private float speed = 100f;

        // Collision
        private Vector2 playerPos;
        private bool intersectionDetected;
        private int damageTaken;

        //Knockback
        private float knockbackDistance = 10;
        private float distanceTraveled = 0;
        private Vector2 knockbackVec;
        private bool knockbackCompleted = true;
        private bool knockbackReady = true;
        private bool knocked = false;
        

        // Animation
        int frame;              
        double timeCounter;     
        double fps;             
        double timePerFrame;    

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 7;       // The number of frames in the animation
        const int EnemyRectOffsetY = 67;   // How far down in the image are the frames?
        const int EnemyRectOffsetX = 4;
        const int EnemyRectHeight = 30;     // The height of a single frame
        const int EnemyRectWidth = 32;      // The width of a single frame

        protected EnemyState state;

        private int corpseAge = 0;

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


        private Vector2 adjustment;
        private double adjustmentTimer;

        //Damage feedback
        protected Color color = Color.White;
        private double timer;
        private bool hit;

        // TimeFreeze spell
        protected bool isFrozen;

        protected float drawScale;
        #endregion

        #region PROPERTIES
        public Vector2 Position // Property to set the current player position
        {
            get { return pos; }
            set { pos = value; }
        }
        public int X // Property to set X coord
        {
            get { return (int)pos.X; }
            set { pos.X = value; }
        }
        public int DamageTaken
        {
            get { return damageTaken; }
            set { damageTaken = value; }
        }
        public int Y // Property to set Y coord
        {
            get { return (int)pos.Y; }
            set { pos.Y = value; }
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

        public bool HasHealthpack
        {
            get { return hasHealthpack; }
            set { hasHealthpack = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public int CorpseAge
        {
            get { return corpseAge; }
            set { corpseAge = value; }
        }

        public Texture2D CorpseSprite
        {
            get { return corpseSprite; }
            set { corpseSprite = value; }
        }
        #endregion

        #region CONSTRUCTORS

        public Enemy(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player, Texture2D corpseSprite) : base(rec, texture, health)
        {
            this.rec = rec;
            this.sprite = texture;
            this.pos = new Vector2(rec.X, rec.Y);
            this.health = health;
            this.enemies = enemies;
            this.player = player;

            rng = new Random();

            intersectionDetected = false;
            collisionsResolved = true;
            priority = false;
            adjustmentTimer = 1.0f;
            timer = 1f;

            // Default sprite direction
            state = EnemyState.FaceRight;

            // Initialize animation data
            fps = 10.0;                     
            timePerFrame = 1.0 / fps;       

            drawScale = 1f;
            this.corpseSprite = corpseSprite;
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

            // TimeFreeze Spell (Lucas)
            CheckIfFrozen();


            // Set enemy direction based on current player position (Lucas)
            dir = playerPos - this.pos;
            dir.Normalize();

            //Collision avoidance logic
            adjustment = GetAdjustmentVector(enemies, gameTime);
            pos += adjustment * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;


            //Knockback
            HandleKnockback(gameTime);

            if (knocked)
            {
                dir *= 0.5f;
            }
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
                knockbackReady = true;
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

                case EnemyState.Dead:
                    spriteBatch.Draw(corpseSprite, rec, color);
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Handles the enemy knockback effect
        /// </summary>
        /// <param name="gameTime">
        /// gameTime passed in from main
        /// </param>
        public void HandleKnockback(GameTime gameTime)
        {
            if(knocked)
            {
                knockbackVec = dir * -5;
                knockbackCompleted = false;
            }

            if (!knockbackCompleted)
            {
                pos += knockbackVec * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                distanceTraveled += (knockbackVec * (float)gameTime.ElapsedGameTime.TotalSeconds * speed).Length();
            }

            if(distanceTraveled >= knockbackDistance)
            {
                distanceTraveled = 0;
                knockbackCompleted = true;
                knocked = false;
            }

        }


        /// <summary>
        /// Lucas:
        /// Draws the enemy idle animation.
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
                drawScale,
                flipSprite,
                0);
        }

        /// <summary>
        /// Lucas:
        /// Draws the enemy character walking animation, based on current frame.
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
                drawScale,                                   // - Scale (100% - no change)
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
                    
                    health -= DamageTaken;
                    
                    
                    hit = true;
                    knocked = true;
                    if(isFire == true)
                    {
                        bulletList.RemoveAt(i);
                    }
                }
            }

            if(health <= 0)
            {
                if (OnDeath != null) OnDeath(); // Use OnDeath event
                isActive = false;
                state = EnemyState.Dead;
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

        /// <summary>
        /// Checks list of enemies for any collisions.
        /// 
        /// If any are detected, determine whether or not this enemy should have priority
        /// 
        /// If it doesn't have priority, and the adjustmentTimer hasn't run out,
        /// decrement the timer, and determine which perpendicular directional vector to use
        /// 
        /// Otherwise, return the 0 vector
        /// 
        /// </summary>
        /// <param name="enemies"></param>
        public Vector2 GetAdjustmentVector(List<Enemy> enemies, GameTime gameTime)
        {
            Enemy intersected = null;
            
            //Check list
            foreach(Enemy enemy in enemies)
            {
                if (this.CheckCollision(enemy))
                {
                    intersectionDetected = true;
                    collisionsResolved = false;
                    intersected = enemy;
                    DeterminePriority(enemies);
                    break;
                }
                intersectionDetected = false;
            }

            //Determine Perpendicular Vectors
            if (intersectionDetected && !priority && adjustmentTimer >= 0) 
            {
                adjustmentTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 playerDirection = GetPlayerDirection(playerPos);

                Vector2 intersectedDirection = intersected.Position - this.pos;

                Vector2 CCWVector = new Vector2(-playerDirection.Y, playerDirection.X);
                CCWVector.Normalize();
                Vector2 CWVector = new Vector2(playerDirection.Y, -playerDirection.X);
                CWVector.Normalize();

                if (SignsAreEqual(CWVector, intersectedDirection))
                {
                    return CCWVector;
                }
                else
                {
                    return CWVector;
                }

            }
            else
            {
                adjustmentTimer = 1.0f;
                return Vector2.Zero;
            }
  
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

        /// <summary>
        /// Loops through enemies list and checks for collisions
        /// </summary>
        /// <param name="enemies"></param>
        /// <returns></returns>
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
        /// Returns the player directions
        /// </summary>
        /// <param name="playerPos">The players position</param>
        /// <returns></returns>
        public Vector2 GetPlayerDirection(Vector2 playerPos)
        {
            return playerPos - this.pos;
        }

        /// <summary>
        /// Returns whether or not the signs of the X and Y
        /// components of two vectors are of identical signs
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool SignsAreEqual(Vector2 a, Vector2 b)
        {
            if (a.Equals(b))
            {
                return true;
            }

            if(a.X == b.X && a.Y == b.Y)
            {
                return true;
            }

            bool XSignsEqual = (a.X >= 0 && b.X >= 0) || (a.X <  0 && b.X < 0);
            bool YSignsEqual = (a.Y >= 0 && b.Y >= 0) || (a.Y < 0 && b.Y < 0);

            return XSignsEqual && YSignsEqual;
        }

        

        /// <summary>
        /// Returns a copy of this Enemy
        /// </summary>
        public Enemy Clone()
        {
            return new Enemy(rec, sprite, health, enemies, player, corpseSprite);
        }

        public event OnDeathDelegate OnDeath; // OnDeath event, for scoring

        /// <summary>
        /// Enemies stop moving during TimeFreeze. See Player class for code.
        /// </summary>
        public void CheckIfFrozen()
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
