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
        Texture2D enemySprite;
        private Texture2D bulletSprite;

        private int playerWidth = 34;
        private int playerHeight = 30;

        private List<GameObject> bullets;

        private int gunWidth = 45;
        private int gunHeight = 25;


        private SpriteFont titleFont;
        private SpriteFont menuFont;
        private int windowWidth;
        private int windowHeight;
        private bool isPaused;

        private List<Button> buttons = new List<Button>();


       

        // Single enemy for testing
        private Enemy enemy;

        // List of enemies to add to for each wave
        private List<Enemy> enemies;
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
            Rectangle playerRec = new Rectangle(30, 30, playerWidth, playerHeight);
            player = new Player(playerRec, playerSpriteSheet, 100);
            //set bullet sprite to bullet property for use
            player.Bullet = bulletSprite;
            //created set of bullets
            bullets = new List<GameObject>();
            
            // Create a FileManager (Chloe)
            fileManager = new FileManager();

            Rectangle gunRec = new Rectangle(15, 15, gunWidth, gunHeight);
            gun = new Gun(gunRec, gunSprite, 10, player, GunState.FaceRight);
           

            // Load enemy sprite (Lucas)
            enemySprite = this.Content.Load<Texture2D>("enemy");

            // Instantiate single test enemy (Lucas)
            // (position will be randomized in future, may want to add enemyWidth and enemyHeight)
            Rectangle enemyRec = new Rectangle(250, 250, 30, 30);
            enemy = new Enemy(enemyRec, enemySprite, 100);

            enemies = new List<Enemy> { enemy };

            // Create a few 100x200 buttons down the left side
            //buttons.Add(new Button(
            //        _graphics.GraphicsDevice,           // device to create a custom texture
            //        new Rectangle(10, 40, 200, 100),    // where to put the button
            //        "Start New Game",                        // button label
            //        menuFont,                               // label font
            //        Color.Purple));                     // button color
            //buttons[0].OnButtonClick += this.NewGame;

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

                    // File loading (Chloe)
                    if (SingleKeyPress(Keys.L, currentKBState))
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
                    }

                    // New Game Button
                    // Create a new 100x200 button tied to NewGame method
                    // NOTE: Could make this button somewhere else (such as LoadContent) and then add
                    // it to the list here for better readability in future
                    buttons.Add(new Button(
                    _graphics.GraphicsDevice,           // device to create a custom texture
                    new Rectangle(10, 40, 200, 100),    // where to put the button
                    "Start New Game",                        // button label
                    menuFont,                               // label font
                    Color.DarkBlue));                     // button color
                    buttons[0].OnButtonClick += this.NewGame;

                    break;
                case GameState.Game:

                    // *PUT GAME LOGIC HERE*
                    // If game is not paused, update game logic (Lucas)
                    if (!isPaused)
                    {
                        // Player movement (Lucas)
                        player.Update(gameTime, bullets);

                        // Enemy movement (Lucas)
                        // Pass current player position to enemies (Lucas)
                        foreach(Enemy enemy in enemies)
                        {
                            enemy.Update(gameTime, bullets);
                            enemy.PlayerPos = player.PosVector;
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
                            if (player.CheckCollision(enemy) && enemy.IsActive)
                            {
                                player.Health -= gun.Health;
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
                            enemies.Add(new Enemy(new Rectangle(250, 250, rng.Next(windowWidth), rng.Next(windowHeight)), enemySprite, 100));
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

                    // Press Q to go to Game Over state for testing (Lucas)
                    if (currentKBState.IsKeyDown(Keys.Q))
                    {
                        currentState = GameState.GameOver;
                    }
                    break;

                case GameState.Pause:

                    // Save Player & Enemy data (Chloe)
                    if (SingleKeyPress(Keys.S, currentKBState))
                    {
                        fileManager.SavePlayer(player);
                        fileManager.SaveEnemies(enemies);
                    }

                    // Unpause game and return to game state (Lucas)
                    if (SingleKeyPress(Keys.P, currentKBState))
                    {
                        isPaused = false;
                        currentState = GameState.Game;
                    }
                    break;

                case GameState.GameOver:

                    // Return to main menu (Lucas)
                    // Note: Single key press is needed otherwise will start new game
                    if (SingleKeyPress(Keys.Enter, currentKBState))
                    {
                        currentState = GameState.Menu;
                    }
                    break;

                default:
                    break;
            }


            // Checks each button in list for clicks
            // NOTE: ".ToList()" is necessary to avoid error
            foreach (Button button in buttons.ToList())
            {
                button.Update();
            }


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
                        (windowHeight / 2) - 100),
                        Color.DarkBlue);

                    // Enter to Start
                    _spriteBatch.DrawString(
                        menuFont,
                        "Press Enter to start",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press Enter to start").X / 2),
                        ((windowHeight / 2)) - 50),
                        Color.Black);

                    // L to load
                    _spriteBatch.DrawString(
                        menuFont,
                        "Press L to load a game",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press L to load a game").X / 2),
                        ((windowHeight / 2))),
                        Color.Black);

                    // Controls
                    _spriteBatch.DrawString(
                        menuFont,
                        "Instructions: Use WASD to move and click to shoot enemies.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Instructions: Use WASD to move and click spacebar to shoot enemies.").X / 2),
                        ((windowHeight / 2) + 50)),
                        Color.Black);

                    // Instructions
                    _spriteBatch.DrawString(
                        menuFont,
                        "Survive as long as you can. If your health reaches 0, you lose.",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Survive as long as you can. If your health reaches 0, you lose.").X / 2),
                        ((windowHeight / 2) + 70)),
                        Color.Black);
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
                    _spriteBatch.DrawString(
                        menuFont,
                        "Enemy Health: " + enemies[0].Health,
                        new Vector2(10, 10),
                        Color.Black);
                    //player health display(AJ)
                    _spriteBatch.DrawString(
                        menuFont,
                        "Player Health: " + player.Health,
                        new Vector2(10, 50),
                        Color.Black);

                    // Pause button (P) (Lucas)
                    _spriteBatch.DrawString(
                        menuFont,
                        "Pause (P)",
                        new Vector2((windowWidth) - (2 * menuFont.MeasureString("Pause (P)").X / 2),
                        (10)),
                        Color.Black);
                    break;

                case GameState.Pause:

                    // Temp pause menu display (Lucas)

                    // Game Paused title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Paused",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Game Paused").X / 2),
                        (windowHeight / 2) - 100),
                        Color.DarkBlue);

                    // Press P to unpause
                    _spriteBatch.DrawString(
                        menuFont,
                        "Press P to unpause",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press P to unpause").X / 2),
                        ((windowHeight / 2)) - 50),
                        Color.Black);

                    // Press S to save game
                    _spriteBatch.DrawString(
                        menuFont,
                        "Press S to save game",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press S to save game").X / 2),
                        ((windowHeight / 2))),
                        Color.Black);
                    break;

                case GameState.GameOver:

                    // Temp GameOver display (Lucas)

                    //Game over title
                    _spriteBatch.DrawString(
                        titleFont,
                        "Game Over",
                        new Vector2((windowWidth / 2) - (titleFont.MeasureString("Game Over").X / 2),
                        (windowHeight / 2) - 100),
                        Color.DarkBlue);

                    // Enter to return to menu
                    _spriteBatch.DrawString(
                        menuFont,
                        "Press Enter to return to menu",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Press Enter to return to menu").X / 2),
                        ((windowHeight / 2)) - 50),
                        Color.Black);

                    // Game score
                    _spriteBatch.DrawString(
                        menuFont,
                        "Score: ",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Score: ").X / 2),
                        ((windowHeight / 2)) - 20),
                        Color.Black);

                    // Highscore
                    _spriteBatch.DrawString(
                        menuFont,
                        "Highscore: ",
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Highscore: ").X / 2),
                        ((windowHeight / 2))),
                        Color.Black);
                    break;

            }


            // Draws each button in the list
            // NOTE: ".ToList()" is necessary to avoid error
            foreach (Button button in buttons.ToList())
            {
                button.Draw(_spriteBatch);
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
        /// When new game button is clicked, changes GameState to Game and clears buttons list
        /// </summary>
        private void NewGame()
        {
            currentState = GameState.Game;
            buttons.Clear();
        }
    }
}