using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TwelveMage
{
    //Anthony Maldonado
    //creating enum for game states
    enum GameState { Menu, Game, Pause, GameOver, Credits}
    public class Game1 : Game
    {
        #region FIELDS
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextureLibrary _textureLibrary;
        private GameState currentState;
        private KeyboardState currentKBState;
        private KeyboardState prevKBState;
        private Player player;
        private Gun gun;
        private FileManager fileManager;
        private Random rng;

        private Texture2D healthBar;
        private Texture2D buttonBackground;

        private int playerWidth = 34;
        private int playerHeight = 30;

        private List<GameObject> bullets;
        private List<GameObject> fireBalls;
        private int gunWidth = 73;
        private int gunHeight = 24;


        private SpriteFont titleFont;
        private SpriteFont smallFont;
        private SpriteFont menuFont;
        private SpriteFont titleBorderFont;
        private int windowWidth;
        private int windowHeight;
        private bool isPaused;
        private bool hasSaved = false;

        private List<Button> mainMenuButtons = new List<Button>();
        private List<Button> gameOverButtons = new List<Button>();
        private List<Button> pauseMenuButtons = new List<Button>();
        private List<Button> creditsButtons = new List<Button>();
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

        // BackgroundManager stuff
        private BackgroundManager backgroundManager;

        // List of enemies to add to for each wave
        private List<Enemy> enemies;
        private List<Enemy> deadEnemies;
        private List<Summoner> summoners;
        private Enemy defaultEnemy;
        private bool enemiesActive;
        private int corpseLifespan = 2;


        private List<HealthPickup> healthPickups;

        private CreditsManager creditsManager;

        private bool cheatsActive;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //added fullscreen capability
           // _graphics.IsFullScreen = true;
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
            waveIncrease = 3;
            score = 0;
            /*highScore = 0;
            highWave = 0;*/

            cheatsActive = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _textureLibrary = new TextureLibrary(this.Content); // Initialize the texture library

            rng = new Random();

            // Load in fonts (Lucas)
            //titleFont = this.Content.Load<SpriteFont>("TitleFont");
            titleFont = this.Content.Load<SpriteFont>("AlagardFont");
            menuFont = this.Content.Load<SpriteFont>("AlagardFontMed");
            smallFont = this.Content.Load<SpriteFont>("AlagardFontSmall");


            // Instantiate player (Lucas)
            Rectangle playerRec = new Rectangle(windowWidth / 2, windowHeight / 2, playerWidth, playerHeight);
            player = new Player(playerRec, _textureLibrary, 100, windowWidth, windowHeight);
            
            //created set of bullets
            bullets = new List<GameObject>();
            fireBalls = new List<GameObject>();
            // Pass window dimensions to Player (Lucas)
            player.WindowHeight = windowHeight;
            player.WindowWidth = windowWidth;

            // Pass font to Player for spell CD
            player.TimerFont = menuFont;
            
            // Load Health Bar Assets (Lucas)
            healthBar = _textureLibrary.GrabTexture("HealthBar");

            buttonBackground = _textureLibrary.GrabTexture("ButtonBackground");

            // Create a FileManager (Chloe)
            fileManager = new FileManager(player, _textureLibrary, windowHeight, windowWidth, rng);

            // Do BackgroundManager initialization
            backgroundManager = new BackgroundManager(_textureLibrary, rng);
            backgroundManager.LoadLevel(fileManager);

            // Instantiate gun
            Rectangle gunRec = new Rectangle(15, 15, gunWidth, gunHeight);
            gun = new Gun(gunRec, _textureLibrary, 10, player);


            enemiesActive = true;

            creditsManager = new CreditsManager(windowWidth, windowHeight, titleFont, menuFont, smallFont);

            // Instantiate single default enemy (Lucas)
            Rectangle enemyRec = new Rectangle(250, 250, 30, 30);

            healthPickups = new List<HealthPickup>();

            deadEnemies = new List<Enemy>();
            enemies = new List<Enemy>();
            summoners = new List<Summoner>();
            defaultEnemy = new Enemy(enemyRec, _textureLibrary, 100, enemies, player, rng);
            enemies.Add(defaultEnemy);
            spawner = new Spawner(
                player.PosVector,
                100,
                100,
                enemies,
                summoners,
                _textureLibrary,
                100,
                player,
                new Rectangle(0, 0, 20, 20),
                windowWidth,
                windowHeight,
                rng);
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
                summoners,
                _textureLibrary,
                100,
                player,
                Rectangle.Empty,
                windowWidth,
                windowHeight,
                rng));

            spawners.Add(new Spawner(
                new Vector2(windowWidth / 2, windowHeight + 40),
                windowWidth / 2,
                0,
                enemies,
                summoners,
                _textureLibrary,
                100, 
                player,
                Rectangle.Empty,
                windowWidth,
                windowHeight,
                rng));

            spawners.Add(new Spawner(
                new Vector2(-40, windowHeight / 2),
                0,
                windowHeight / 2,
                enemies,
                summoners,
                _textureLibrary,
                100,
                player,
                Rectangle.Empty,
                windowWidth,
                windowHeight,
                rng));

            spawners.Add(new Spawner(
                new Vector2(windowWidth + 40, windowHeight / 2),
                0,
                windowHeight / 2,
                enemies,
                summoners,
                _textureLibrary,
                100,
                player,
                Rectangle.Empty,
                windowWidth,
                windowHeight,
                rng));

            #region Menu Buttons
            // MAIN MENU BUTTONS

            // New Game Button
            mainMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    // where to put the button
            "New Game",                        // button text
            menuFont,                               // font
            Color.DarkBlue));                     // color
            mainMenuButtons[0].OnButtonClick += this.NewGame;

            // Load Game Button
            mainMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    
            "Load Game",                        
            menuFont,                               
            Color.DarkBlue));                     
            mainMenuButtons[1].OnButtonClick += this.LoadGame;

            // Credits Button
            mainMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,
            new Rectangle(10, 10, buttonWidth / 2, buttonHeight / 2),
            "Credits",
            smallFont,
            Color.DarkBlue));
            mainMenuButtons[2].OnButtonClick += this.Credits;


            // Exit Button
            Button ExitButton = new Button(
            _graphics.GraphicsDevice,
            new Rectangle(710, 10, buttonWidth / 2, buttonHeight / 2),
            "Quit",
            smallFont,
            Color.DarkBlue);
            mainMenuButtons.Add(ExitButton);
            mainMenuButtons[3].OnButtonClick += this.Exit;
            

            // CREDITS BUTTONS

            // Return to Main Menu
            Button MenuButton = new Button(
            _graphics.GraphicsDevice,
            new Rectangle(10, 10, buttonWidth / 2, buttonHeight / 2),
            "Menu",
            smallFont,
            Color.DarkBlue);
            creditsButtons.Add(MenuButton);
            creditsButtons[0].OnButtonClick += this.MainMenu;

            // GAME OVER MENU BUTTONS

            // New Game Button
            gameOverButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    
            "New Game",                        
            menuFont,                               
            Color.DarkBlue));                     
            gameOverButtons[0].OnButtonClick += this.NewGame;

            // Main Menu Button
            gameOverButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    
            "Main Menu",                        
            menuFont,                               
            Color.DarkBlue));
            gameOverButtons[1].OnButtonClick += this.MainMenu;

            //exit gameover
            gameOverButtons.Add(ExitButton);
            gameOverButtons[2].OnButtonClick += this.Exit;

            // PAUSE MENU BUTTONS

            // Resume button
            pauseMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY - 50, buttonWidth, buttonHeight),    
            "Resume",                        
            menuFont,                               
            Color.DarkBlue));                     
            pauseMenuButtons[0].OnButtonClick += this.Resume;

            // Save button
            pauseMenuButtons.Add(new Button(
            _graphics.GraphicsDevice,           
            new Rectangle(buttonCenterX, buttonCenterY + 50, buttonWidth, buttonHeight),    
            "Save",                        
            menuFont,                               
            Color.DarkBlue));
            pauseMenuButtons[1].OnButtonClick += this.Save;

            //exit
            pauseMenuButtons.Add(ExitButton);
            pauseMenuButtons[2].OnButtonClick += this.Exit;

            pauseMenuButtons.Add(MenuButton);
            pauseMenuButtons[3].OnButtonClick += this.MainMenu;
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

                    // Update menu buttons
                    foreach(Button button in mainMenuButtons.ToList())
                    {
                        button.Update();
                    }
                    break;

                case GameState.Credits:

                    // Update credits
                    creditsManager.Update(gameTime);

                    // Update buttons
                    foreach (Button button in creditsButtons.ToList())
                    {
                        button.Update();
                    }

                    // Check if credits are over and return to main menu
                    if (creditsManager.IsEnded())
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                case GameState.Game:

                    // *PUT GAME LOGIC HERE*
                    // If game is not paused, update game logic (Lucas)
                    if (!isPaused)
                    {
                        // Player movement (Lucas)
                        player.Update(gameTime, bullets);

                        spawner.Position = player.PosVector;

                        // Update gun rotation
                        gun.Update(gameTime, bullets);
                        
                        // Cheats
                        if (cheatsActive)
                        {
                            // Spawn a test enemy (U)
                            if (SingleKeyPress(Keys.U, currentKBState))
                            {
                                spawner.SpawnCharger();

                                //spawners[rng.Next(0,4)].SpawnEnemyd
                            }

                            // Activate/Deactivate enemy Update (X)
                            if (SingleKeyPress(Keys.X, currentKBState))
                            {
                                enemiesActive = !enemiesActive;
                            }

                            // Clear all enemies (C)
                            if (SingleKeyPress(Keys.C, currentKBState))
                            {
                                enemies.Clear();
                                summoners.Clear();
                                deadEnemies.Clear();
                            }

                            // Go to GameOver state (Q)
                            if (SingleKeyPress(Keys.Q, currentKBState))
                            {
                                EndGame();
                            }

                            // Heal player to 100 (H)
                            if (SingleKeyPress(Keys.H, currentKBState))
                            {
                                player.Health = 100;
                            }

                            // Toggle God Mode (G)
                            if (SingleKeyPress(Keys.G, currentKBState))
                            {
                                ToggleGodMode();
                            }
                        }                       

                        foreach (Enemy enemy in enemies)
                        {
                            // Pass current player position to enemies
                            enemy.PlayerPos = player.PosVector;

                            // Set TimeFreeze status according to if player used ability
                            enemy.IsFrozen = player.IsFrozen;
                            enemy.DamageTaken = player.DamageGiven;

                            // Update enemy logic and animation
                            if (enemiesActive)
                            {
                                enemy.Update(gameTime, bullets);
                                enemy.UpdateAnimation(gameTime);
                            }
                            
                        }

                        // Make sure summoners stop summoning enemies while frozen
                        foreach(Summoner summoner in summoners)
                        {
                            if (!summoner.IsFrozen)
                            {
                                summoner.Summoning(gameTime);
                            }
                        }

                        // Update HealthPickup collisions and despawning
                        foreach (HealthPickup healthPickup in healthPickups)
                        {
                            healthPickup.Update(gameTime, bullets);
                        }

                        // Remove health objects if no longer active
                        for(int i = healthPickups.Count - 1; i >= 0; i--)
                        {
                            if (!healthPickups[i].IsActive)
                            {
                                healthPickups.RemoveAt(i);
                            }
                        }
                        
                        // Update fireball spell
                        foreach (GameObject fire in fireBalls)
                        {
                            Projectile fireball;
                            if(fire is Projectile)
                            {
                                fireball = (Projectile)fire;
                                fire.Update(gameTime, fireBalls);
                            }
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

                        //if enemy collides with player health goes down
                        foreach (Enemy enemy in enemies)
                        {
                            if (player.CheckCollision(enemy) && enemy.IsActive)
                            {
                                //Anthony if player collides with enemy tick health down and add to invulnerbale timer
                                if (!(enemy is Charger))
                                {
                                    player.Health -= gun.Health;
                                }
                                else if(enemy is Charger)
                                {
                                    player.Health -= gun.Health * 2;
                                }
                            }
                        }
                            
                        for(int i = enemies.Count - 1; i >= 0; i--) // Remove every inactive enemy
                        {
                            if (!enemies[i].IsActive)
                            {
                                if (enemies[i].HasHealthpack) // If the now-dead enemy has a healthpack, spawn it
                                {
                                    healthPickups.Add(new HealthPickup(
                                        new Rectangle(enemies[i].X, enemies[i].Y, 16, 16),
                                        _textureLibrary,
                                        10,
                                        player,
                                        rng));
                                }

                                deadEnemies.Add(enemies[i]);
                                if(deadEnemies.Count > 150)
                                {
                                    for(int j = 29; j >= 0; j--)
                                    {
                                        deadEnemies.RemoveAt(j);
                                    }
                                }

                                enemies[i].Color = Color.White;
                                enemies.RemoveAt(i);
                            }
                        }
                        
                        // Wave handling
                        if(enemies.Count == 0)
                        {
                            // Add a number of regular Enemies equal to wave * waveIncrease
                            for(int i = 0; i < wave * waveIncrease; i++)
                            {
                                spawners[rng.Next(0, 4)].SpawnEnemy();
                                enemies[i].OnDeath += IncreaseScore;
                            }

                            // On a wave that's a multiple of 5,
                            if((wave + 1) % 5 == 0)
                            {
                                if (wave + 1 >= 15) // If the wave is 15 or higher,
                                {
                                    for (int i = 0; i < (wave + 1) / 10 && i < 4; i++) // Add another special enemy for every 10 waves
                                    {
                                        spawners[rng.Next(0, 4)].SpawnSpecial(); // Randomly selects from one of the special types
                                        enemies[enemies.Count - 1].OnDeath += IncreaseScore;
                                        enemies[enemies.Count - 1].OnDeath += IncreaseScore;
                                    }
                                } // Otherwise, add a summoner
                                else {
                                    spawners[rng.Next(0, 4)].SpawnSummoner();
                                    enemies[enemies.Count - 1].OnDeath += IncreaseScore;
                                    enemies[enemies.Count - 1].OnDeath += IncreaseScore;
                                }
                            }

                            enemies[rng.Next(0, enemies.Count())].HasHealthpack = true; // Give two random enemies a healthpack

                            // Increment health pickup age
                            foreach (HealthPickup healthPickup in healthPickups)
                            {
                                healthPickup.Age++;
                            }

                            // Ages all existing corpses
                            foreach(Enemy corpse in deadEnemies)
                            {
                                corpse.CorpseAge++;
                            }

                            // If any corpses are older than the allowed corpseLifespan, remove them
                            for(int i = deadEnemies.Count - 1; i >= 0; i--)
                            {
                                if (deadEnemies[i].CorpseAge > corpseLifespan)
                                {
                                    deadEnemies.RemoveAt(i);
                                }
                            }

                            wave++;
                        }
                       
                    }

                    // Toggle Cheats (~)
                    if (SingleKeyPress(Keys.OemTilde, currentKBState))
                    {
                        ToggleCheats();
                    }

                    // Pause game logic and switch to pause state (Lucas)
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
                  
                    break;

                case GameState.Pause:

                    foreach(Button button in pauseMenuButtons.ToList()) // Update all pause menu buttons
                    {
                        button.Update();
                    }
                    break;

                case GameState.GameOver:

                    foreach(Button button in gameOverButtons.ToList()) // Update all game over menu buttons
                    {
                        button.Update();
                    }
                    break;

                default:
                    break;
            }

            // Store last frame's keyboard state
            prevKBState = currentKBState;
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            backgroundManager.Draw(windowWidth, windowHeight, _spriteBatch);

            switch (currentState)
            {
                case GameState.Menu:

                    // Draw BG props (Chloe)
                    backgroundManager.DrawProps(windowWidth, windowHeight, _spriteBatch);

                    // Main Menu display (Lucas)

                    // Title Outline
                    _spriteBatch.DrawString(
                        titleFont,
                        "Twelve Mage",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Twelve Mage").X / 2) + 2,
                        75 + 2),
                        Color.DarkBlue);

                    // Title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Twelve Mage",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Twelve Mage").X / 2),
                        75),
                        Color.Yellow);

                    

                    // Controls
                    _spriteBatch.DrawString(
                        smallFont,
                        "Use WASD to move and click to shoot enemies.",
                        new Vector2((windowWidth / 2) - (smallFont.MeasureString("Use WASD to move and click to shoot enemies.").X / 2),
                        ((windowHeight / 2) + 150)),
                        Color.Black);

                    // Instructions
                    _spriteBatch.DrawString(
                        smallFont,
                        "Survive as long as you can!",
                        new Vector2((windowWidth / 2) - (smallFont.MeasureString("Survive as long as you can!").X / 2),
                        ((windowHeight / 2) + 175)),
                        Color.Black);

                    

                    foreach (Button button in mainMenuButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }
                    break;

                case GameState.Credits:
                    // Draw BG props (Chloe)
                    backgroundManager.DrawProps(windowWidth, windowHeight, _spriteBatch);

                    // Credits text
                    creditsManager.Draw(_spriteBatch);
                    foreach (Button button in creditsButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }
                    break;

                case GameState.Game:

                    // Enemy corpse drawing
                    foreach (Enemy enemy in deadEnemies)
                    {
                        enemy.Draw(_spriteBatch);
                    }

                    // Health pickup drawing
                    foreach (HealthPickup healthPickup in healthPickups)
                    {
                        healthPickup.Draw(_spriteBatch);
                    }

                    // Player sprite/animations (Lucas)
                    player.Draw(_spriteBatch);

                    // Draw the gun over the player (Lucas)
                    gun.Draw(_spriteBatch);

                    // Enemy sprites (Lucas)
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

                    // Draw BG props (Chloe)
                    backgroundManager.DrawProps(windowWidth, windowHeight, _spriteBatch);

                    //Wave counter display
                    _spriteBatch.DrawString(
                        smallFont,
                        "Wave: " + wave + " Score: " + score,
                        new Vector2(10, 10),
                        Color.Black);

                    // Pause button (P) (Lucas)
                    _spriteBatch.DrawString(
                        smallFont,
                        "Pause (P)",
                        new Vector2((windowWidth) - (2 * smallFont.MeasureString("Pause (P)").X / 2),
                        (10)),
                        Color.Black);

                    // Player Health Bar Background (Lucas)
                    _spriteBatch.Draw(healthBar,                // Texture
                        new Vector2(windowWidth / 3.6f, 10),       // Location
                        null,                                   // Texture Region Rectangle
                        Color.Black,                            // Color
                        0,                                      // Rotation
                        Vector2.Zero,                           // Center of the rotation
                        new Vector2(0.75f, 0.5f),                  // Scale 
                        SpriteEffects.None,                     // Effects
                        0);                                     // Layer depth

                    // Player Health Bar (Lucas)
                    // Scales Health Bar based on player health
                    _spriteBatch.Draw(healthBar,                    // Texture
                        new Vector2(windowWidth / 3.6f, 10),           // Location
                        null,                                       // Texture Region Rectangle
                        Color.White,                                // Color
                        0,                                          // Rotation
                        Vector2.Zero,                               // Center of the rotation
                        new Vector2(0.75f * player.Health/100, 0.5f),  // Scale 
                        SpriteEffects.None,                         // Effects
                        0);                                         // Layer depth

                    // Spells UI
                    player.DrawSpellSlots(_spriteBatch);

                    if (cheatsActive)
                    {
                        // Cheats menu
                        _spriteBatch.DrawString(
                            smallFont,
                            "Test Enemy: U" +
                            "\nFreeze Enemies: X" +
                            "\nClear Enemies: C" +
                            "\nGame Over: Q" +
                            "\nHeal: H" +
                            "\nGod Mode: G",
                            new Vector2(10, 100),
                            Color.Yellow);
                    }

                    break;

                case GameState.Pause:

                    // Draw BG props (Chloe)
                    backgroundManager.DrawProps(windowWidth, windowHeight, _spriteBatch);

                    // Pause Menu display (Lucas)

                    // Game Paused title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Paused",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Game Paused").X / 2),
                        75),
                        Color.DarkBlue);

                    // Save notification upon succesfully saving game
                    if (hasSaved)
                    {
                        _spriteBatch.DrawString(
                        menuFont,
                        "Game Saved.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Game Saved.").X / 2),
                        ((windowHeight - 50))),
                        Color.Black);
                    }
                    

                    foreach(Button button in pauseMenuButtons.ToList())
                    {
                        button.Draw(_spriteBatch);
                    }

                    break;

                case GameState.GameOver:

                    // Draw BG props (Chloe)
                    backgroundManager.DrawProps(windowWidth, windowHeight, _spriteBatch);

                    // Game Over display (Lucas)

                    //Game over title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Over",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Game Over").X / 2),
                        75),
                        Color.DarkBlue);

                    // Game score
                    _spriteBatch.DrawString(
                        menuFont,
                        "Score: " + score,
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Score: " + score).X / 2),
                        ((windowHeight  - 75))),
                        Color.Black);

                    // Highscore
                    _spriteBatch.DrawString(
                        menuFont,
                        "Highscore: " + highScore,
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Highscore: " + highScore).X / 2),
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
            player.WindowHeight = windowHeight;     // Reset window width and height so that player is centered correctly (Lucas)
            player.WindowWidth = windowWidth;
            player.BorderFlameManager.Reset();      // Reset the border flame positions
            currentState = GameState.Game; // Set GameState
            wave = 1; // Set to wave 1
            score = 0; // Reset score
            int[] stats = fileManager.LoadStats(); // Load stats
            if(stats[1] > highScore) highScore = stats[1]; // Load the saved highscores
            if(stats[3] > highWave) highWave = stats[3];
            player.Reset(); // Reset the player
            enemies.Clear(); // Clear enemies
            summoners.Clear(); // Clear summoners (Lucas)
            deadEnemies.Clear(); // Clear corpses (Lucas)
            healthPickups.Clear(); // Clears HealthPickups
            enemies.Add(defaultEnemy.Clone()); // Add the default enemy
            enemies[0].OnDeath += IncreaseScore; // Add the score increase method to the enemy's OnDeath event
            enemies[0].HasHealthpack = true; // Give the enemy a healthpack

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
        /// Loads a game from a file, advances game state, and clears buttons.
        /// </summary>
        private void LoadGame()
        {
            // Does all GameObject-related saving
            deadEnemies.Clear();
            player.BorderFlameManager.Reset(); // Reset border flame positions
            fileManager.LoadGameObjects(player, enemies, summoners, healthPickups, spawners);
            player.OverwriteSpellData(fileManager.LoadSpells());
            int[] stats = fileManager.LoadStats();
            score = stats[0];
            wave = stats[2];
            if (stats[1] > highScore) highScore = stats[1];
            if (stats[3] > highWave) highWave = stats[3];

            if (enemies != null && enemies.Count != 0)
            {
                foreach (Enemy enemy in enemies)
                {
                    enemy.OnDeath += IncreaseScore;
                }

                enemies[rng.Next(0, enemies.Count())].HasHealthpack = true; // Give a single, random enemy a healthpack
            }
            currentState = GameState.Game;
        }

        /// <summary>
        /// Saves Player and Enemy Data to a file.
        /// </summary>
        private void Save()
        {
            if(score > highScore) highScore = score;
            if(wave > highWave) highWave = wave;
            if (fileManager.SaveGameObjects(player, enemies, healthPickups) && // Does all GameObject-related loading
            fileManager.SaveStats(score, wave) &&
            fileManager.SavePersistentStats(highScore, highWave) &&
            fileManager.SaveSpells(player.GetSpellData()))
            { hasSaved = true; } // Enables "Game Saved" text notification
        }

        /// <summary>
        /// Sets the game state to the Menu.
        /// </summary>
        private void MainMenu()
        {
            isPaused = false;
            hasSaved = false;
            currentState = GameState.Menu;
            /*foreach(Button button in mainMenuButtons.ToList())
            {
                button.Active = true; // Activate all butons
            }*/
        }

        /// <summary>
        /// Lucas
        /// Activates Credits state and resets them to default position
        /// </summary>
        private void Credits()
        {
            creditsManager.Reset();
            currentState = GameState.Credits;
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
        public void IncreaseScore()
        {
            score += 10;
        }

        /// <summary>
        /// Increases the player's score by a given amount
        /// </summary>
        /// <param name="amount">The amount to increase the score by</param>
        private void IncreaseScore(int amount)
        {
            score += amount;
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

        /// <summary>
        /// Lucas
        /// Toggle dev commands (cheats)
        /// </summary>
        private void ToggleCheats()
        {
            if (!cheatsActive)
            {
                cheatsActive = true;
            }
            else
            {
                cheatsActive = false;
                player.IsGod = false;
            }
        }

        /// <summary>
        /// Lucas
        /// Toggle God Mode cheat.
        /// Infinite health and no spell cooldowns
        /// </summary>
        private void ToggleGodMode()
        {
            if (!player.IsGod)
            {
                player.IsGod = true;
            }
            else
            {
                player.IsGod = false;
            }
        }
    }
}