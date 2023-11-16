﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using TwelveMage;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;


/*
 * Chloe Hall
 * Twelve-Mage
 * This class handles the player character,
 * including movement, actions, scores, etc.
 * No known issues
 * Anthony: Removed uneeded fields and enum, added projectile capability to player 
 * Lucas: Added Player Movement, States, and Animations
 * Chloe: Completely reworked spell functionality
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
        Color color = Color.White;

        Texture2D bullet;
        Texture2D fireball;

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
        private Vector2 dir;
        private Vector2 pos;
        private float speed = 200f;

        private int windowWidth;
        private int windowHeight;
        
        // Mouse shooting
        private MouseState mState;
        private MouseState prevMState;
        private float shootingTimer;
        private bool hasShot;
        private int damageGiven = 20;

        // Spell stuff
        private Spell spell;

        // Spell UI drawing rectangles
        private Rectangle blinkRec;
        private Rectangle fireballRec;
        private Rectangle freezeRec;
        private Rectangle hasteRec;

        // Bools to track if spells are possible
        private bool canBlink;
        private bool canFireball;
        private bool canFreeze;
        private bool canHaste;

        // Values for spell effect duration
        private double freezeEffect = 5.0;
        private double hasteEffect = 5.0;

        // Values for spell cooldown duration
        private double blinkCooldown = 6.0;
        private double freezeCooldown = 20.0;
        private double hasteCooldown = 10.0;
        private double fireballCooldown = 10.0;

        // Values for current spell timers
        private double blinkTimer = 0;
        private double fireballTimer = 0;
        private double freezeTimer = 0;
        private double hasteTimer = 0;

        // Invulnerability works similarly to spell effects
        double invulnerableTimer = 0;

        // Note: When a spell is used, its CooldownDuration will be added to its timer.
        //       Spells can only be used when their timer is 0, and each will count down every frame until they reach 0.
        //       For any spell with a lasting effect, it will be in effect when its Timer is >= CooldownDuration - EffectDuration
        //       ex: Freeze is in effect when freezeTimer >= (20.0 - 5.0)
        //       All of this is handled each frame in the UpdateSpells() method.

        #endregion

        #region PROPERTIES
        public int Health
        {
            get { return health; }
            set
            {
                if (value < health && !IsInvulnerable) // Only change health if the player isn't invulnerable
                {
                    invulnerableTimer = 1; // If health decreases, make the add invulnerability
                    health = value;
                }
                if (value > health)
                {
                    health = value;
                }
            }
        }
        public Texture2D Fireball
        {
            get { return fireball; }
            set { fireball = value; }
        }
        public Texture2D Bullet //bullet property to get bullet texture
        {
            get { return bullet; }
            set { bullet = value; }
        }
        public MouseState Mstate
        {
            get { return mState; }
        }
        
        public int DamageGiven
        {
            get { return damageGiven; }
            set {  damageGiven = value; }
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

        public bool IsFrozen // Returns true if FreezeTimer is within the freeze effect's  duration
        {
            get
            {
                if (freezeTimer >= freezeCooldown - freezeEffect) return true;
                else return false;
            }
        }
        public bool IsHastened // Returns true if HasteTimer is within the haste effect's duration
        {
            get
            {
                if (hasteTimer >= hasteCooldown - hasteEffect) return true;
                else return false;
            }
        }
        public bool IsInvulnerable
        {
            get { return (invulnerableTimer > 0); }
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
            blinkTimer = 0;
            fireballTimer = 0;
            freezeTimer = 0;
            hasteTimer = 0;
            
            shootingTimer = 0.25f;
            hasShot = false;

            // Default sprite direction
            state = PlayerState.FaceRight;

            // Initialize animation data
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


            #region Input Processing
            // Process W and S keys for vertical movement (Lucas)
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

            // Process A and D keys for horizontal movement (Lucas)
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

            // Normalize the direction vector if there is any movement (Lucas)
            if (dir.X != 0 || dir.Y != 0)
            {
                dir.Normalize();
            }

            // Update the player's position based on direction, time elapsed, and speed (Lucas)
            pos.Y += dir.Y * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
            pos.X += dir.X * (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

            UpdateSpells(gameTime, spell, bullets); // Update spells

            // Mouse shooting (Lucas)
            if (prevMState.LeftButton == ButtonState.Released && mState.LeftButton == ButtonState.Pressed && !hasShot)
            {
                //AddBullet(bullets);
                ShotgunFire(bullets, 6);
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

            // Set the player's animation state based on movement direction (Lucas)
            if (dir.X < 0)
            {
                state = PlayerState.WalkLeft; // Walking left
            }
            if (dir.X > 0)
            {
                state = PlayerState.WalkRight; // Walking right
            }

            // Handle vertical movement state (Lucas)
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

            // Set the player's state to facing left or right when not moving (Lucas)
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
            
            if(IsInvulnerable && IsHastened)
            {
                color = Color.DarkRed;
            }
            else if (IsInvulnerable)
            {
                color = Color.Black;
            }
            else if(IsHastened)
            {
                color = Color.Orange;
            }
            else color = Color.White;


            // Update rectangle position
            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);

            previousKB = currentKB;
            prevMState = mState;
            #endregion
        }

        //Anthony Maldonado
        //added bullet method to take the direction of the player and add that to a list of projectiles
        public void AddFireBall(List<GameObject> fireBalls)
        {
            
            Projectile project = new Projectile(new Rectangle(rec.X, rec.Y - 50, 150, 150), Fireball, health, 800);
            project.Direction = new Vector2(mState.X, mState.Y) - pos;
            project.Direction.Normalize();
            project.LinearVelocity = .3f;
            project.IsFire = true;
            project.LifeSpan = 5;
            fireBalls.Add(project);
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
                shots[i].IsFire = false;
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
            state = PlayerState.FaceRight;
            health = MAX_HEALTH;

            blinkTimer = 0;
            fireballTimer = 0;
            freezeTimer = 0;
            hasteTimer = 0;
            invulnerableTimer = 0;
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

        private void UpdateSpells(GameTime gameTime, Spell spell, List<GameObject> fire)
        {
            // Check which spells can be used this frame
            canBlink = (blinkTimer <= 0 && !(dir.X == 0 && dir.Y == 0));
            canFireball = (fireballTimer <= 0);
            canFreeze = (freezeTimer <= 0);
            canHaste = (hasteTimer <= 0);

            // Handle spell input
            if (canBlink && currentKB.IsKeyDown(Keys.Space) && previousKB.IsKeyUp(Keys.Space))
            {
                spell.Blink(dir);
                blinkTimer = blinkCooldown;
            }
            if (canFireball && currentKB.IsKeyDown(Keys.F) && previousKB.IsKeyUp(Keys.F))
            {
                LifeSpan = 50f;
                LinearVelocity = .01f;
                
                
                AddFireBall(fire);
                fireballTimer = fireballCooldown;
            }
            if (canFreeze && currentKB.IsKeyDown(Keys.R) && previousKB.IsKeyUp(Keys.R))
            {
                freezeTimer = freezeCooldown;
            }
            if (canHaste && currentKB.IsKeyDown(Keys.E) && previousKB.IsKeyUp(Keys.E))
            {
                hasteTimer = hasteCooldown;
            }

            // Handle spell effects
            if (IsHastened)
            {
                speed = 350f;
            }
            else speed = 200f;

            // Handle spell timers
            if (blinkTimer > 0)
            {
                blinkTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (fireballTimer > 0)
            {
                fireballTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (freezeTimer > 0)
            {
                freezeTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (hasteTimer > 0)
            {
                hasteTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (invulnerableTimer > 0) // And invulnerability
            {
                invulnerableTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Clamp spell timers so they're always >= 0
            if (blinkTimer < 0.0) blinkTimer = 0;
            if (fireballTimer <= 0.0)
            {
                isFire = false;
                damageGiven = 20;
                LifeSpan = 4f;
                LinearVelocity = .5f;
                fireballTimer = 0;
            }
            if (freezeTimer < 0.0) freezeTimer = 0;
            if (hasteTimer < 0.0) hasteTimer = 0;
            if (invulnerableTimer < 0.0) invulnerableTimer = 0;
        }

        /// <summary>
        /// Draws all spell slots, and their timer overlays
        /// </summary>
        public void DrawSpellSlots(SpriteBatch spriteBatch, Texture2D blinkSpellIcon, Texture2D fireballSpellIcon, Texture2D freezeSpellIcon, Texture2D hasteSpellIcon, Texture2D spellSlotOverlay)
        {
            // Rectangles for spells UI
            blinkRec = new Rectangle(20, windowHeight - 100, 50, 80);
            fireballRec = new Rectangle(90, windowHeight - 100, 50, 80);
            freezeRec = new Rectangle(160, windowHeight - 100, 50, 80);
            hasteRec = new Rectangle(230, windowHeight - 100, 50, 80);

            // Draw base icons
            // If the spells are on cooldown, draw the base icons in gray
            if (blinkTimer > 0) spriteBatch.Draw(blinkSpellIcon, blinkRec, Color.Gray * 0.7f);
            else spriteBatch.Draw(blinkSpellIcon, blinkRec, Color.White * 0.7f);
            if (fireballTimer > 0) spriteBatch.Draw(fireballSpellIcon, fireballRec, Color.Gray * 0.7f);
            else spriteBatch.Draw(fireballSpellIcon, fireballRec, Color.White * 0.7f);
            if (freezeTimer > 0) spriteBatch.Draw(freezeSpellIcon, freezeRec, Color.Gray * 0.7f);
            else spriteBatch.Draw(freezeSpellIcon, freezeRec, Color.White * 0.7f);
            if (hasteTimer > 0) spriteBatch.Draw(hasteSpellIcon, hasteRec, Color.Gray * 0.7f);
            else spriteBatch.Draw(hasteSpellIcon, hasteRec, Color.White * 0.7f);

            // Rectangles for cooldown overlays
            Rectangle blinkOverlay = blinkRec;
            Rectangle fireballOverlay = fireballRec;
            Rectangle freezeOverlay = freezeRec;
            Rectangle hasteOverlay = hasteRec;

            // Calculate cooldown overlay size ratios
            // Overlay height : base icon height = spell timer : spell cooldown
            blinkOverlay.Height = (int)(blinkOverlay.Height * (blinkTimer / blinkCooldown));
            fireballOverlay.Height = (int)(fireballOverlay.Height * (fireballTimer / fireballCooldown));
            freezeOverlay.Height = (int)(freezeOverlay.Height * (freezeTimer / freezeCooldown));
            hasteOverlay.Height = (int)(hasteOverlay.Height * (hasteTimer / hasteCooldown));

            spriteBatch.Draw(spellSlotOverlay, blinkOverlay, Color.White * 0.5f);
            spriteBatch.Draw(spellSlotOverlay, fireballOverlay, Color.White * 0.5f);
            spriteBatch.Draw(spellSlotOverlay, freezeOverlay, Color.White * 0.5f);
            spriteBatch.Draw(spellSlotOverlay, hasteOverlay, Color.White * 0.5f);
        }   

        public void OverwriteSpellData(Dictionary<string, double> SpellsDictionary)
        {
            SpellsDictionary.TryGetValue("BlinkTimer", out blinkTimer);
            SpellsDictionary.TryGetValue("BlinkCooldown", out blinkCooldown);
            SpellsDictionary.TryGetValue("FireballTimer", out fireballTimer);
            SpellsDictionary.TryGetValue("FireballCooldown", out fireballCooldown);
            SpellsDictionary.TryGetValue("FreezeTimer", out freezeTimer);
            SpellsDictionary.TryGetValue("FreezeCooldown", out freezeCooldown);
            SpellsDictionary.TryGetValue("FreezeEffect", out freezeEffect);
            SpellsDictionary.TryGetValue("HasteTimer", out hasteTimer);
            SpellsDictionary.TryGetValue("HasteCooldown", out hasteCooldown);
            SpellsDictionary.TryGetValue("HasteEffect", out hasteEffect);
        }

        /// <summary>
        /// Returns a Dictionary with all spell timers, cooldowns, and durations
        /// </summary>
        public Dictionary<String, double> getSpellData()
        {
            Dictionary<String, double> SpellsDictionary = new Dictionary<String, double>();

            // Blink
            SpellsDictionary.Add("BlinkTimer", blinkTimer);
            SpellsDictionary.Add("BlinkCooldown", blinkCooldown);

            // Fireball
            SpellsDictionary.Add("FireballTimer", fireballTimer);
            SpellsDictionary.Add("FireballCooldown", fireballCooldown);

            // Freeze
            SpellsDictionary.Add("FreezeTimer", freezeTimer);
            SpellsDictionary.Add("FreezeCooldown", freezeCooldown);
            SpellsDictionary.Add("FreezeEffect", freezeEffect);

            // Haste
            SpellsDictionary.Add("HasteTimer", hasteTimer);
            SpellsDictionary.Add("HasteCooldown", hasteCooldown);
            SpellsDictionary.Add("HasteEffect", hasteEffect);

            return SpellsDictionary;
        }

        #endregion
    }
}