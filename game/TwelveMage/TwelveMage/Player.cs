using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using TwelveMage;
using System.Diagnostics;
using System.Collections.Generic;


/*
 * Chloe Hall
 * Twelve-Mage
 * This class handles the player character,
 * including movement, actions, scores, etc.
 * No known issues
 * Anthony: Removed uneeded fields and enum, added projectile capability to player 
 * Lucas: Added Player Movement, States, and Animations
 */

namespace TwelveMage
{

    enum PlayerState
    {
        FaceLeft,
        FaceRight,
        WalkLeft,
        WalkRight
    }

    internal class Player : GameObject
    {
        #region FIELDS
        private const int MAX_HEALTH = 100; // Cap on health (made a constant for future readability/ease of changing)
                                            //private int _health;
                                            //private int _ammo; // Ammunition count (Not yet implemented)
        Color color = Color.White;

        float invulnerable = 0f;

        Texture2D bullet;

        // PROPERTIES
        //public int Health { get { return _health; } }
        //bullet property to get bullet texture
        public Texture2D Bullet
        {
            get { return bullet; }
            set { bullet = value; }
        }

        

        PlayerState state;
        //two new kb states to check if space is clicked only once as to not spam hold
        private KeyboardState currentKB;
        private KeyboardState previousKB;
        

        // Animation
        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int WizardRectOffsetY = 36;   // How far down in the image are the frames?
        const int WizardRectOffsetX = 4;
        const int WizardRectHeight = 30;     // The height of a single frame
        const int WizardRectWidth = 34;      // The width of a single frame

        // Move player with vector2
        Vector2 dir;
        Vector2 pos;
        float speed = 200f;

        private int windowWidth;
        private int windowHeight;

        // Mouse shooting
        private MouseState mState;
        private MouseState prevMState;
        private float shootingTimer;
        private bool hasShot;

        //Spell stuff
        private Spell spell;
        private double blinkTimer;
        private bool blinked;
        private bool isFrozen;
        private double freezeTimer;
        private double freezeCD;
        private bool canFreeze;
        #endregion

        #region PROPERTIES

        public float Invulnerable
        {
            get { return invulnerable; }
            set { invulnerable = value; }
        }

        public Vector2 PosVector 
        { 
            get { return pos; }
            set {  pos = value; }
        }

        public Vector2 DirVector
        {
            get { return dir; }
            set {  dir = value; }
        }

        public PlayerState State
        {
            get { return state; }
            set { state = value; }
        }

        public int WindowWidth
        {
            set { windowWidth = value; }
        }

        public int WindowHeight
        {
            set { windowHeight = value; }
        }

        public double BlinkTimer
        {
            get { return blinkTimer; }
        }

        public bool IsFrozen
        {
            get { return isFrozen; }
        }

        public double FreezeCD
        {
            get { return freezeCD; }
        }

        #endregion

        //removed texture b/c added it as a field in object
        #region CONSTRUCTORS
        public Player(Rectangle rec, Texture2D texture, int health) : base(rec, texture, health)
        {
            this.rec = rec;
            this.pos = new Vector2(rec.X, rec.Y);
            this.health = health;
            spell = new Spell(this);
            blinkTimer = 6.0f;
            blinked = false;
            shootingTimer = 0.25f;
            hasShot = false;

            isFrozen = false;
            freezeTimer = 5;
            freezeCD = 20;
            canFreeze = true;

            // Default sprite direction
            state = PlayerState.FaceRight;

            // Initialize
            fps = 10.0;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image
        }
        #endregion

        #region METHODS

        /// <summary>
        /// Lucas:
        /// Handles user input, player movement, and State change for animations
        /// </summary>
        /// <param name="gameTime">
        /// GameTime passed in from main
        /// </param>
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {

            currentKB = Keyboard.GetState();
            mState = Mouse.GetState();


            #region WASD Processing

            //Spell input
            if(currentKB.IsKeyDown(Keys.Space) && previousKB.IsKeyUp(Keys.Space) && !blinked)
            {
                spell.Blink(DirVector);
                blinked = true;
            }

            //Can only blink once every 6 seconds
            if (blinked)
            {
                blinkTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(blinkTimer <= 0)
                {
                    blinkTimer = 6.0;
                    blinked = false;
                }
            }

            // Time Freeze Spell Input (X Key for now)
            // (Lucas)
            if (currentKB.IsKeyDown(Keys.X) && previousKB.IsKeyUp(Keys.X) && !isFrozen && canFreeze)
            {
                isFrozen = true;
                canFreeze = false;
                freezeCD = 20;
            }

            // Handles how long enemies are frozen (5s)
            if (isFrozen)
            {
                freezeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (freezeTimer <= 0)
                { 
                    isFrozen = false;
                    freezeTimer = 5;
                }
            }

            // Counts down freeze CD (20s)
            if (!canFreeze)
            {
                freezeCD -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (freezeCD <= 0)
                {
                    canFreeze = true;
                    freezeCD = 20;
                }
            }




            // Process W and S keys for vertical movement
            if (currentKB.IsKeyDown(Keys.W))
            {
                dir.Y -= 1; // Move up
            }
            else if (currentKB.IsKeyDown(Keys.S))
            {
                dir.Y += 1; // Move down
            }
            else
            {
                dir.Y = 0; // No vertical movement
            }

            // Process A and D keys for horizontal movement
            if (currentKB.IsKeyDown(Keys.D))
            {
                dir.X += 1; // Move right
            }
            else if (currentKB.IsKeyDown(Keys.A))
            {
                dir.X -= 1; // Move left
            }
            else
            {
                dir.X = 0; // No horizontal movement
            }
            //if space bar shoot bullet
            /*
             * Commented out since mouse shooting has been implemented
            if (currentKB.IsKeyDown(Keys.Space) && previousKB.IsKeyUp(Keys.Space))
            {
                AddBullet(bullets);
                //test
                Debug.WriteLine("AHHHHHHHHH");
            }
            */

            // Mouse shooting (Lucas)
            if (prevMState.LeftButton == ButtonState.Released && mState.LeftButton == ButtonState.Pressed && !hasShot)
            {
                //AddBullet(bullets);
                ShotgunFire(bullets, 5);
                hasShot = true;
            }

            if(hasShot)
            {
                shootingTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(shootingTimer <= 0)
                {
                    shootingTimer = 0.25f;
                    hasShot = false;
                }
            }


            // Normalize the direction vector if there is any movement
            if (dir.X != 0 || dir.Y != 0)
            {
                dir.Normalize();
            }

            // Update the player's position based on direction, time elapsed, and speed
            pos.Y += dir.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            pos.X += dir.X * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            // Set the player's animation state based on movement direction
            if (dir.X < 0)
            {
                state = PlayerState.WalkLeft; // Walking left
            }
            if (dir.X > 0)
            {
                state = PlayerState.WalkRight; // Walking right
            }

            // Handle vertical movement state
            if (dir.Y != 0)
            {
                if (state == PlayerState.WalkLeft || state == PlayerState.FaceLeft)
                {
                    state = PlayerState.WalkLeft; // Walking left
                }
                if (state == PlayerState.WalkRight || state == PlayerState.FaceRight)
                {
                    state = PlayerState.WalkRight; // Walking right
                }
            }

            // Set the player's state to facing left or right when not moving
            if (dir.X == 0 && dir.Y == 0)
            {
                if (state == PlayerState.WalkLeft)
                {
                    state = PlayerState.FaceLeft; // Facing left
                }
                else if (state == PlayerState.WalkRight)
                {
                    state = PlayerState.FaceRight; // Facing right
                }
            }
            //Anthony if player is damaged set invulnerbale 
            //then make color black after set color back to white
            if(invulnerable > 0 )
            {
                color = Color.Black;
            }
            else
            {
                color = Color.White;
            }


            // Update rectangle position
            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);

            previousKB = currentKB;
            prevMState = mState;
            #endregion
        }

        //Anthony Maldonado
        //added bullet method to take the direction of the player and add that to a list of projectiles

        private void AddBullet(List<GameObject> bullets)
        {
            Projectile project = new Projectile(new Rectangle(rec.X,rec.Y, 15,15), bullet, health, 800);

            // Commented out to replace with mouse shooting (Lucas)
            //if(dir == Vector2.Zero)
            //{
            //    project.Direction = Vector2.One;
            //}
            //else
            //{
            //    project.Direction = dir;
            //}

            
            // Mouse shooting (Lucas)
            // Set direction based on mouse cursor position minus player position
            project.Direction = new Vector2(mState.X, mState.Y) - pos;

            bullets.Add(project);

            //var bullet = projectile.Clone() as Projectile;
            //bullet.Direction = dir;
            //bullet.Position = pos;
            //bullet.LinearVelocity = this.LinearVelocity * 2;
            //bullet.LifeSpan = 2f;
            //bullet.Parent = this;
            //sprites.Add(bullet);
        }


        /// <summary>
        /// Gets the mousePos and direction, and than gets a number of different nearby vectors.
        /// These vectors will be used to generate a shotgun blast.
        /// 
        /// - Ben
        /// </summary>
        /// <param name="bullets"></param>
        private void ShotgunFire(List<GameObject> bullets, int numShots)
        {
            Random rng = new Random();
            //When called, get mouse direction
            Vector2 mouseDir = new Vector2(mState.X, mState.Y) - pos;
            mouseDir.Normalize();

            //Changes shot speed
            //Don't turn too low(less than 50) or it breaks
            mouseDir *= 100;


            List<GameObject> shots = new List<GameObject>();
            List<Vector2> shotDirections = GenerateVectors(mouseDir ,numShots);
            
            for(int i = 0; i < numShots; i++)
            {
                shots.Add(new Projectile(
                    new Rectangle(rec.X, rec.Y, 15, 15),
                    bullet,
                    health,
                    rng.Next(25, 40)));
                shots[i].Direction = shotDirections[i];
            }

            foreach(Projectile shot in shots)
            {
                bullets.Add(shot);
            }

            
        }


        /// <summary>
        /// Takes in a Vector2 and generates a list containing a number of nearby vectors.
        /// Use's a fraction of the length of the vector as the radius, in order to ensure no vectors are created going the wrong way
        /// 
        /// - Ben
        /// </summary>
        /// <param name="startingVec">The origin Vector2</param>
        /// <param name="numVecs">How many vectors to generate</param>
        /// <returns>A List containing all the generated vectors</returns>
        private List<Vector2> GenerateVectors(Vector2 startingVec, int numVecs)
        {
            Random rng = new Random();
            float radius = startingVec.Length() / 5;
            List<Vector2> vectors = new List<Vector2>();

            for(int i = 0; i < numVecs; i++)
            {
                float xRadius = (float)rng.NextDouble() * radius;
                xRadius *= RandomizeSign();
                float yRadius = (float)rng.NextDouble() * radius;
                yRadius *= RandomizeSign();
                Vector2 newDir = new Vector2(startingVec.X + xRadius, startingVec.Y + yRadius);
                vectors.Add(newDir);
            }

            return vectors;
        }

        /// <summary>
        /// Takes in a float and randomizes it's sign, multplying by either 1 or -1
        /// </summary>
        private int RandomizeSign()
        {
            Random rng = new Random();
            int signChange;

            do
            {
                signChange = rng.Next(-1, 2);
            }while(signChange == 0);

            return signChange;
        }

        /// <summary>
        /// Lucas:
        /// Updates the player animation
        /// Adapted from Mario Walking PE
        /// </summary>
        /// <param name="gameTime">
        /// GameTime passed in from main
        /// </param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

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

        /// <summary>
        /// Resets the player to default values
        /// </summary>
        public void Reset()
        {
            Center();
            color = Color.White;
            invulnerable = 0f;
            state = PlayerState.FaceRight;
            health = MAX_HEALTH;
        }

        /// <summary>
        /// Lucas:
        /// Draws the animated player sprite based on current State
        /// </summary>
        /// <param name="spriteBatch">
        /// The SpriteBatch passed in from main
        /// </param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            #region State switch
            switch (state)
            {
                case PlayerState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;

                case PlayerState.WalkRight:
                    DrawWalking(SpriteEffects.None, spriteBatch);
                    break;

                case PlayerState.WalkLeft:
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
                    WizardRectOffsetY,
                    WizardRectWidth,
                    WizardRectHeight),
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
                    frame * WizardRectWidth,            // - This rectangle specifies
                    WizardRectOffsetY,                  //	 where "inside" the texture
                    WizardRectWidth,                    //   to get pixels (We don't want to
                    WizardRectHeight),                  //   draw the whole thing)
                color,                            // - The color
                0,                                      // - Rotation (none currently)
                Vector2.Zero,                           // - Origin inside the image (top left)
                1.0f,                                   // - Scale (100% - no change)
                flipSprite,                             // - Can be used to flip the image
                0);                                     // - Layer depth (unused)
        }

        /// <summary>
        /// Centers the player in the middle of the window
        /// </summary>
        public void Center()
        {
            pos.X = (windowWidth / 2) - (rec.Width / 2);
            pos.Y = (windowHeight / 2) - (rec.Height / 2);
        }
        #endregion
    }
}