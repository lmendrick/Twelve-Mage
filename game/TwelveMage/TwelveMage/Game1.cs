using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveMage
{
    //Anthony Maldonado
    //creating enum for game states
    enum GameState { Menu, Game, Pause, GameOver}
    public class Game1 : Game
    {
        #region FIELDS
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;
        private KeyboardState currentKBState;
        private KeyboardState prevKBState;
        private Player player;
        private Gun gun;
        private FileManager fileManager;
        private Random rng;

        private Texture2D playerSpriteSheet;
        private Texture2D enemySprite;
        private Texture2D bulletSprite;
        private Texture2D fireballSprite;
        private Texture2D healthBar;

        private int playerWidth = 34;
        private int playerHeight = 30;

        private List<GameObject> bullets;
        private List<GameObject> fireBalls;
        private int gunWidth = 45;
        private int gunHeight = 25;


        private SpriteFont titleFont;
        private SpriteFont menuFont;
        private int windowWidth;
        private int windowHeight;
        private bool isPaused;
        private bool hasSaved = false;

        private List<Button> mainMenuButtons = new List<Button>();
        private List<Button> gameOverButtons = new List<Button>();
        private List<Button> pauseMenuButtons = new List<Button>();
        int buttonWidth = 150;
        int buttonHeight = 75;
        int buttonCenterX;
        int buttonCenterY;


        private int wave;
        private int waveIncrease;
        private int score;
        private int highScore;
        private int highWave;
        private Spawner spawner;
        private List<Spawner> spawners;



        // List of enemies to add to for each wave
        private List<Enemy> enemies;
        private Enemy defaultEnemy;


        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            currentState = GameState.Menu;

            // Store the size of the window
            windowWidth = _graphics.GraphicsDevice.Viewport.Width;
            windowHeight = _graphics.GraphicsDevice.Viewport.Height;

            // Reference values to center buttons in window
            buttonCenterX = (windowWidth / 2) - (buttonWidth / 2);
            buttonCenterY = (windowHeight / 2) - (buttonHeight / 2);

            
            

            wave = 1;
            waveIncrease = 2;
            score = 0;
            /*highScore = 0;
            highWave = 0;*/

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Load in fonts (Lucas)
            titleFont = this.Content.Load<SpriteFont>("TitleFont");
            menuFont = this.Content.Load<SpriteFont>("MenuFont");

            // Load in player character sprite sheet (Lucas)
            playerSpriteSheet = this.Content.Load<Texture2D>("CharacterSheet");
            //load in bullet sprite
            bulletSprite = this.Content.Load<Texture2D>("bullet2");

            //Load in gun image
            Texture2D gunSprite = this.Content.Load<Texture2D>("Spas_12");

            rng = new Random();

            // Instantiate player (Lucas)
            Rectangle playerRec = new Rectangle(windowWidth / 2, windowHeight / 2, playerWidth, playerHeight);
            player = new Player(playerRec, playerSpriteSheet, 100);
            //set bullet sprite to bullet property for use
            player.Bullet = bulletSprite;
            //created set of bullets
            bullets = new List<GameObject>();
            // Pass window dimensions to Player
            player.WindowHeight = windowHeight;
            player.WindowWidth = windowWidth;

            // Load Health Bar Assets
            healthBar = this.Content.Load<Texture2D>("HB_1");

            // Create a FileManager (Chloe)
            fileManager = new FileManager();

            Rectangle gunRec = new Rectangle(15, 15, gunWidth, gunHeight);
            gun = new Gun(gunRec, gunSprite, 10, player);
           

            // Load enemy sprite (Lucas)
            enemySprite = this.Content.Load<Texture2D>("ZombieWalkSheet");
            

            // Instantiate single test enemy (Lucas)
            // (position will be randomized in future, may want to add enemyWidth and enemyHeight)
            Rectangle enemyRec = new Rectangle(250, 250, 30, 30);
            

            enemies = new List<Enemy>();
            defaultEnemy = new Enemy(enemyRec, enemySprite, 100, enemies, player);
            enemies.Add(defaultEnemy);
            spawner = new Spawner(
                player.PosVector,
                100,
                100,
                enemies,
                enemySprite,
                100,
                player,
                new Rectangle(0, 0, 20, 20));
            spawners = new List<Spawner>();


            //spawners[0] is top spawner
            //spawners[1] is bottom spawner
            //spawners[2] is left spawner
            //spawners[3] is right spawner

            spawners.Add(new Spawner(
                new Vector2(windowWidth / 2, -40),
                windowWidth / 2,
                0,
                enemies,
                enemySprite,
                100,
                player,
                Rectangle.Empty));

            spawners.Add(new Spawner(
                new Vector2(windowWidth / 2, windowHeight + 40),
                windowWidth / 2,
                0,
                enemies,
                enemySprite,
                100, 
                player,
                Rectangle.Empty));

            spawners.Add(new Spawner(
                new Vector2(-40, windowHeight / 2),
                0,
                windowHeight / 2,
                enemies,
                enemySprite,
                100,
                player,
                Rectangle.Empty));

            spawners.Add(new Spawner(
                new Vector2(windowWidth + 40, windowHeight / 2),
                0,
                windowHeight / 2,
                enemies,
                enemySprite,
                100,
                player,
                Rectangle.Empty));

            // Create a few 100x200 buttons down the left side
            //buttons.Add(new Button(
            //        _graphics.GraphicsDevice,           // device to create a custom texture
            //        new Rectangle(10, 40, 200, 100),    // where to put the button
            //        "Start New Game",                        // button label
            //        menuFont,                               // label font
            //        Color.Purple));                     // button color
            //buttons[0].OnButtonClick += this.NewGame;

            #region Menu Buttons
            // MAIN MENU BUTTONS

            // New Game Button
            mainMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    // where to put the button
            "New Game",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));                     // button color
            mainMenuButtons[0].OnButtonClick += this.NewGame;

            // Load Game Button
            mainMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    // where to put the button
            "Load Game",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));                     // button color
            mainMenuButtons[1].OnButtonClick += this.LoadGame;

            // GAME OVER MENU BUTTONS

            // New Game Button
            gameOverButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    // where to put the button
            "New Game",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));                     // button color
            gameOverButtons[0].OnButtonClick += this.NewGame;

            // Main Menu Button
            gameOverButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    // where to put the button
            "Main Menu",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));
            gameOverButtons[1].OnButtonClick += this.MainMenu;

            // PAUSE MENU BUTTONS

            // Resume button
            pauseMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    // where to put the button
            "Resume",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));                     // button color
            pauseMenuButtons[0].OnButtonClick += this.Resume;

            // Save button
            pauseMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           // device to create a custom texture
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    // where to put the button
            "Save",                        // button label
            menuFont,                               // label font
            Color.DarkBlue));
            pauseMenuButtons[1].OnButtonClick += this.Save;
            #endregion

            // Load highest scores
            int[] stats = fileManager.LoadStats();
            highScore = stats[1];
            highWave = stats[3];


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKBState = Keyboard.GetState();

            // Update the player sprite animation every frame (Lucas)
            player.UpdateAnimation(gameTime);
            
            // TODO: Add your update logic here
            //Anthony Maldonado
            //switches made for the game states 
            switch (currentState)
            {
                case GameState.Menu:

                    foreach(Button button in mainMenuButtons.ToList())
                    {
                        button.Update();
                    }

                    // DEPRECATED
                    // File loading (Chloe)
                    /*if (SingleKeyPress(Keys.L, currentKBState))
                    {
                        player = fileManager.LoadPlayer(playerSpriteSheet);
                        player.Bullet = bulletSprite;
                        enemies = fileManager.LoadEnemies(enemySprite);
                        //enemy = enemies[0]; // Temporary

                        currentState = GameState.Game;
                    }

                    if (currentKBState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }*/

                    break;
                case GameState.Game:

                    // *PUT GAME LOGIC HERE*
                    // If game is not paused, update game logic (Lucas)
                    if (!isPaused)
                    {
                        // Player movement (Lucas)
                        player.Update(gameTime, bullets);

                        spawner.Position = player.PosVector;

                        if(SingleKeyPress(Keys.U, currentKBState))
                        {
                            spawner.SpawnEnemy();

                            //spawners[rng.Next(0,4)].SpawnEnemy();
                        }

                        if(SingleKeyPress(Keys.C, currentKBState))
                        {
                            enemies.Clear();
                        }

                        // Enemy movement (Lucas)
                        // Pass current player position to enemies (Lucas)
                        // Set enemy TimeFreeze status according to if player used ability
                        foreach(Enemy enemy in enemies)
                        {
                            enemy.Update(gameTime, bullets);
                            enemy.PlayerPos = player.PosVector;
                            enemy.IsFrozen = player.IsFrozen;

                            enemy.UpdateAnimation(gameTime);

                        }
                        

                        //for each game object in bullets make a bullet (AJ)
                        foreach (GameObject project in bullets)
                        {
                            Projectile bullet;
                            //check that its a projectile if it is update(AJ)
                            if(project is Projectile)
                            {
                                bullet = (Projectile)project;
                                project.Update(gameTime, bullets);
                            }
                            
                        }

                        //for bullets after timespan ends remove(AJ)
                        for (int i = 0; i < bullets.Count; i++)
                        {
                            if (bullets[i].IsRemoved)
                            {
                                bullets.RemoveAt(i); ;
                                i--;
                            }
                        }


                        //Enemy damage logic moved to enemy class

                        //if enemy collides with player health goes down
                        //have to tweak to tick multiple times for one player instance
                       

                        foreach (Enemy enemy in enemies)
                        {
                            //Anthony if player is invulnerbale count down timer
                            if (player.Invulnerable > 0)
                            {
                                player.Invulnerable -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            if (player.CheckCollision(enemy) && enemy.IsActive)
                            {
                                //Anthony if player collides with enemy tick health down and add to invulnerbale timer
                                if(player.Invulnerable <= 0)
                                {
                                    player.Health -= gun.Health;
                                    player.Invulnerable = 4f;
                                }
                                    

                            }

                        }
                            
                        for(int i = enemies.Count - 1; i >= 0; i--)
                        {
                            if (!enemies[i].IsActive)
                            {
                                enemies.RemoveAt(i);
                            }
                        }

                        
                        
                        if(enemies.Count == 0)
                        {
                            //enemies.Add(new Enemy(new Rectangle(250, 250, rng.Next(windowWidth), rng.Next(windowHeight)), enemySprite, 100));

                            

                            for(int i = 0; i < wave * waveIncrease; i++)
                            {
                                spawners[rng.Next(0, 4)].SpawnEnemy();
                                enemies[i].OnDeath += IncreaseScore;
                            }
                            wave++;
                        }
                        
                        
                        

                        //Addded gun but since its not tweaked fully so commented out for now(AJ)
                        //gun.Update(gameTime);
                        //gun.PosVector = player.PosVector;
                    }

                    // Pause game logic and switch to pause state (Lucas)
                    // Note: May be simpler to not have a pause state, and just make an overlay
                    if (SingleKeyPress(Keys.P, currentKBState))
                    {
                        isPaused = true;
                        currentState = GameState.Pause;
                    }
                    //Anthony if health is 0 game over
                    if (player.Health <= 0)
                    {
                        EndGame();
                    }

                    // Press Q to go to Game Over state for testing (Lucas)
                    if (currentKBState.IsKeyDown(Keys.Q))
                    {
                        EndGame();
                    }
                    break;

                case GameState.Pause:

                    foreach(Button button in pauseMenuButtons.ToList()) // Update all pause menu buttons
                    {
                        button.Update();
                    }

                    // Save Player & Enemy data (Chloe)
                    /*if (SingleKeyPress(Keys.S, currentKBState))
                    {
                        fileManager.SavePlayer(player);
                        fileManager.SaveEnemies(enemies);
                    }

                    // Unpause game and return to game state (Lucas)
                    if (SingleKeyPress(Keys.P, currentKBState))
                    {
                        isPaused = false;
                        currentState = GameState.Game;
                    }*/
                    break;

                case GameState.GameOver:

                    foreach(Button button in gameOverButtons.ToList()) // Update all game over menu buttons
                    {
                        button.Update();
                    }

                    // Return to main menu (Lucas)
                    // Note: Single key press is needed otherwise will start new game
                    /*if (SingleKeyPress(Keys.Enter, currentKBState))
                    {
                        currentState = GameState.Menu;
                    }*/
                    break;

                default:
                    break;
            }


            /*// Checks each button in list for clicks
            // NOTE: ".ToList()" is necessary to avoid error
            foreach (Button button in pauseMenuButtons.ToList())
            {
                button.Update();
            }*/


            prevKBState = currentKBState;
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //switch to ensure what happens in each state
            //will add more after discussion and more work
            switch (currentState)

            {
                case GameState.Menu:

                    // Temp main menu display, can replace these with buttons later (Lucas)

                    // Title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Twelve-Mage",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Twelve-Mage").X / 2),
                        75),
                        Color.Yellow);

                    // Enter to Start
                    //_spriteBatch.DrawString(
                    //    menuFont,
                    //    "Press Enter to start",
                    //    new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press Enter to start").X / 2),
                    //    ((windowHeight / 2)) - 50),
                    //    Color.Black);

                    // L to load
                    //_spriteBatch.DrawString(
                    //    menuFont,
                    //    "Press L to load a game",
                    //    new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press L to load a game").X / 2),
                    //    ((windowHeight / 2))),
                    //    Color.Black);

                    // Controls
                    _spriteBatch.DrawString(
                        menuFont,
                        "Instructions: Use WASD to move and click to shoot enemies.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Instructions: Use WASD to move and click to shoot enemies.").X / 2),
                        ((windowHeight / 2) + 150)),
                        Color.Black);

                    // Instructions
                    _spriteBatch.DrawString(
                        menuFont,
                        "Survive as long as you can. If your health reaches 0, you lose.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Survive as long as you can. If your health reaches 0, you lose.").X / 2),
                        ((windowHeight / 2) + 175)),
                        Color.Black);

                    foreach(Button button in mainMenuButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }
                    break;

                case GameState.Game:

                    // Player sprite/animations (Lucas)
                    player.Draw(_spriteBatch);
                    
                    


                    // Enemy sprite (Lucas)
                    foreach(Enemy enemy in enemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }

                    //draw bullets as long as its a projectile(AJ)
                    foreach (GameObject project in bullets)
                    {
                        if(project is Projectile)
                        {
                            project.Draw(_spriteBatch);
                        }
                        
                    }

                    //gun sprite
                    //gun.Draw(_spriteBatch);

                    //enenmy health display(AJ
                    if (enemies != null && enemies.Count != 0) // Only do this if enemies has an enemy
                    {
                        _spriteBatch.DrawString(
                        menuFont,
                        "Enemy Health: " + enemies[0].Health,
                        new Vector2(10, 10),
                        Color.Black);
                    }
                    //player health display(AJ)
                    _spriteBatch.DrawString(
                        menuFont,
                        "Player Health: " + player.Health,
                        new Vector2(10, 50),
                        Color.Black);
                    //Blink timer display (Ben)
                    string timerString = String.Format("{0:0.00}", player.BlinkTimer);
                    _spriteBatch.DrawString(
                        menuFont,
                        "Blink: " + timerString,
                        new Vector2(10, 90),
                        Color.Black);
                    // Freeze cooldown display (Lucas)
                    string freezeString = String.Format("{0:0.00}", player.FreezeCD);
                    _spriteBatch.DrawString(
                        menuFont,
                        "Time Freeze: " + freezeString,
                        new Vector2(10, 130),
                        Color.Black);
                    string hasteString = string.Format("{0:0.00}", player.HasteTimer);
                    _spriteBatch.DrawString(menuFont,
                        "Haster CoolDown: " + hasteString,
                        new Vector2(10, 170), 
                        Color.Black);
                    //Wave counter display
                    _spriteBatch.DrawString(
                        menuFont,
                        "Wave: " + wave,
                        new Vector2(10, 210),
                        Color.Black);

                    // Pause button (P) (Lucas)
                    _spriteBatch.DrawString(
                        menuFont,
                        "Pause (P)",
                        new Vector2((windowWidth) - (2 * menuFont.MeasureString("Pause (P)").X / 2),
                        (10)),
                        Color.Black);

                    // Player Health Bar Background (Lucas)
                    _spriteBatch.Draw(healthBar,                // Texture
                        new Vector2(windowWidth / 3, 30),       // Location
                        null,                                   // Texture Region Rectangle
                        Color.Black,                             // Color
                        0,                                      // Rotation
                        Vector2.Zero,                           // Center of the rotation
                        new Vector2(1f, 0.5f),                  // Scale 
                        SpriteEffects.None,                     // Effects
                        0);                                     // Layer depth;

                    // Player Health Bar (Lucas)
                    // Scales Health Bar based on player health
                    _spriteBatch.Draw(healthBar,                    // Texture
                        new Vector2(windowWidth / 3, 30),           // Location
                        null,                                       // Texture Region Rectangle
                        Color.White,                                // Color
                        0,                                          // Rotation
                        Vector2.Zero,                               // Center of the rotation
                        new Vector2(1f * player.Health/100, 0.5f),  // Scale 
                        SpriteEffects.None,                         // Effects
                        0);                                         // Layer depth
                    break;

                case GameState.Pause:

                    // Temp pause menu display (Lucas)

                    // Game Paused title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Paused",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Game Paused").X / 2),
                        75),
                        Color.DarkBlue);

                    if (hasSaved)
                    {
                        _spriteBatch.DrawString(
                        menuFont,
                        "Game Saved.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Game Saved.").X / 2),
                        ((windowHeight - 50))),
                        Color.Black);
                    }
                    

                    // Press P to unpause
                    //_spriteBatch.DrawString(
                    //    menuFont,
                    //    "Press P to unpause",
                    //    new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press P to unpause").X / 2),
                    //    ((windowHeight / 2)) - 50),
                    //    Color.Black);

                    // Press S to save game
                    //_spriteBatch.DrawString(
                    //    menuFont,
                    //    "Press S to save game",
                    //    new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press S to save game").X / 2),
                    //    ((windowHeight / 2))),
                    //    Color.Black);

                    foreach(Button button in pauseMenuButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }

                    break;

                case GameState.GameOver:

                    // Temp GameOver display (Lucas)

                    //Game over title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Over",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("GAME OVER").X / 2),
                        75),
                        Color.DarkBlue);

                    // Enter to return to menu
                    //_spriteBatch.DrawString(
                    //    menuFont,
                    //    "Press Enter to return to menu",
                    //    new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press Enter to return to menu").X / 2),
                    //    ((windowHeight / 2)) - 50),
                    //    Color.Black);

                    // Game score
                    _spriteBatch.DrawString(
                        menuFont,
                        "Score: " + score,
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Score: ").X / 2),
                        ((windowHeight  - 75))),
                        Color.Black);

                    // Highscore
                    _spriteBatch.DrawString(
                        menuFont,
                        "Highscore: " + highScore,
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Highscore: ").X / 2),
                        ((windowHeight - 50))),
                        Color.Black);

                    foreach(Button button in gameOverButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }

                    break;

            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Lucas
        /// Checks if a given key was pressed only once
        /// </summary>
        /// <param name="key">
        /// The key to check for
        /// </param>
        /// <param name="currentKbState">
        /// The keyboard state to use
        /// </param>
        /// <returns></returns>
        private bool SingleKeyPress(Keys key, KeyboardState currentKbState)
        {
            if (currentKbState.IsKeyUp(key) && prevKBState.IsKeyDown(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// When new game button is clicked, changes GameState to Game, and does other startup tasks
        /// </summary>
        private void NewGame()
        {
            currentState = GameState.Game;
            wave = 1;
            score = 0;
            int[] stats = fileManager.LoadStats();
            if(stats[1] > highScore) highScore = stats[1];
            if(stats[3] > highWave) highWave = stats[3];
            player.Reset();
            enemies.Clear();
            enemies.Add(defaultEnemy.Clone());
            enemies[0].OnDeath += IncreaseScore;
            //DeactivateButtons();
        }

        /// <summary>
        /// Loads a game from a file, advances game state, and clears buttons.
        /// </summary>
        private void LoadGame()
        {
            player = fileManager.LoadPlayer(playerSpriteSheet);
            player.Bullet = bulletSprite;
            enemies.Clear();
            enemies = fileManager.LoadEnemies(enemySprite);
            int[] stats = fileManager.LoadStats();
            score = stats[0];
            wave = stats[2];
            if(stats[1] > highScore) highScore = stats[2];
            if(stats[3] > highWave) highWave = stats[3];

            foreach(Spawner spawner in spawners)
            {
                spawner.Enemies = enemies;
            }

            foreach(Enemy enemy in enemies)
            {
                enemy.OnDeath += IncreaseScore;
            }

            currentState = GameState.Game;
            //DeactivateButtons();
        }

        /// <summary>
        /// Resumes gameplay from pause menu.
        /// </summary>
        private void Resume()
        {
            isPaused = false;
            currentState = GameState.Game;

            // Sets save status back to false, disabling "Game Saved" text
            hasSaved = false;
            //DeactivateButtons();
        }

        /// <summary>
        /// Saves Player and Enemy Data to a file.
        /// </summary>
        private void Save()
        {
            if(score > highScore) highScore = score;
            if(wave > highWave) highWave = wave;
            if(fileManager.SavePlayer(player) &&
            fileManager.SaveEnemies(enemies) &&
            fileManager.SaveStats(score, wave) &&
            fileManager.SavePersistentStats(highScore, highWave))
            { hasSaved = true; } // Enables "Game Saved" text notification
        }

        /// <summary>
        /// Sets the game state to the Menu.
        /// </summary>
        private void MainMenu()
        {
            currentState = GameState.Menu;
            /*foreach(Button button in mainMenuButtons.ToList())
            {
                button.Active = true; // Activate all butons
            }*/
        }

        /// <summary>
        /// Deactivates every menu button.
        /// </summary>
        private void DeactivateButtons()
        {
            foreach(Button button in mainMenuButtons.ToList())
            {
                button.Active = false;
            }
            foreach(Button button in pauseMenuButtons.ToList())
            {
                button.Active = false;
            }
            foreach(Button button in gameOverButtons.ToList())
            {
                button.Active = false;
            }
        }

        /// <summary>
        /// Increases the player's score by 10; to be used with Enemy's OnDeath event (or other events in future?)
        /// </summary>
        private void IncreaseScore()
        {
            score += 10;
        }

        /// <summary>
        /// (Chloe) Ends the game
        /// </summary>
        private void EndGame()
        {
            currentState = GameState.GameOver;
            if(score > highScore) highScore = score;
            if(wave > highWave) wave = highWave;
            fileManager.SavePersistentStats(highScore, highWave);
        }
    }
}