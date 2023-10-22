using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
            Texture2D spriteSheet = this.Content.Load<Texture2D>("CharacterSheet");
            //load in bullet sprite
            bulletSprite = this.Content.Load<Texture2D>("bullet2");

            //Load in gun image
            Texture2D gunSprite = this.Content.Load<Texture2D>("Spas_12");

            // Instantiate player (Lucas)
            Rectangle playerRec = new Rectangle(30, 30, playerWidth, playerHeight);
            player = new Player(playerRec, spriteSheet, 100);
            //set bullet sprite to bullet property for use
            player.Bullet = bulletSprite;
            //created set of bullets
            bullets = new List<GameObject>();
            
            // Create a FileManager (Chloe)
            fileManager = new FileManager();

            Rectangle gunRec = new Rectangle(15, 15, gunWidth, gunHeight);
            gun = new Gun(gunRec, gunSprite, 1000, player, GunState.FaceRight);
           

            // Load enemy sprite (Lucas)
            Texture2D enemySprite = this.Content.Load<Texture2D>("enemy");

            // Instantiate single test enemy (Lucas)
            // (position will be randomized in future, may want to add enemyWidth and enemyHeight)
            Rectangle enemyRec = new Rectangle(250, 250, 30, 30);
            enemy = new Enemy(enemyRec, enemySprite, 100);

            enemies = new List<Enemy> { enemy };

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

                    if (currentKBState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.Game:

                    // *PUT GAME LOGIC HERE*
                    // If game is not paused, update game logic (Lucas)
                    if (!isPaused)
                    {
                        // Player movement (Lucas)
                        player.Update(gameTime, bullets);

                        // Enemy movement (Lucas)
                        enemy.Update(gameTime, bullets);

                        // Pass current player position to enemies (Lucas)
                        enemy.PlayerPos = player.PosVector;

                        //for each game object in bullets make a bullet 
                        foreach (GameObject project in bullets)
                        {
                            Projectile bullet;
                            //check that its a projectile if it is update
                            if(project is Projectile)
                            {
                                bullet = (Projectile)project;
                                project.Update(gameTime, bullets);
                            }
                            
                        }

                        //for bullets after timespan ends remove
                        for (int i = 0; i < bullets.Count; i++)
                        {
                            if (bullets[i].IsRemoved)
                            {
                                bullets.RemoveAt(i); ;
                                i--;
                            }
                        }

                        //Addded gun but since its not tweaked fully commented out
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
                    if (SingleKeyPress(Keys.P, currentKBState))
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
                        new Vector2((windowWidth / 2) - (menuFont.MeasureString("Instructions: Use WASD to move and click to shoot enemies.").X / 2),
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
                    enemy.Draw(_spriteBatch);

                    //draw bullets as long as its a projectile
                    foreach (GameObject project in bullets)
                    {
                        if(project is Projectile)
                        {
                            project.Draw(_spriteBatch);
                        }
                        
                    }

                    //gun sprite
                    //gun.Draw(_spriteBatch);

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
    }
}