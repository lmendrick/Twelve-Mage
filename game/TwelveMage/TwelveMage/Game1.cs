using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private int playerWidth = 34;
        private int playerHeight = 30;
        private SpriteFont titleFont;
        private SpriteFont menuFont;
        private int windowWidth;
        private int windowHeight;
        private bool isPaused;
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

            // Load in fonts (Luke)
            titleFont = this.Content.Load<SpriteFont>("TitleFont");
            menuFont = this.Content.Load<SpriteFont>("MenuFont");

            // Load in player character sprite sheet (Luke)
            Texture2D spriteSheet = this.Content.Load<Texture2D>("CharacterSheet");

            // Instantiate player (Luke)
            Rectangle playerRec = new Rectangle(30, 30, playerWidth, playerHeight);
            player = new Player(playerRec, spriteSheet, 100);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKBState = Keyboard.GetState();

            // Update the player sprite animation every frame (Luke)
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
                    // If game is not paused, update game logic (Luke)
                    if (!isPaused)
                    {
                        // Player movement (Luke)
                        player.Update(gameTime);
                    }

                    // Pause game logic and switch to pause state (Luke)
                    // Note: May be simpler to not have a pause state, and just make an overlay
                    if (SingleKeyPress(Keys.P, currentKBState))
                    {
                        isPaused = true;
                        currentState = GameState.Pause;
                    }

                    // Press Q to go to Game Over state for testing (Luke)
                    if (currentKBState.IsKeyDown(Keys.Q))
                    {
                        currentState = GameState.GameOver;
                    }
                    break;

                case GameState.Pause:

                    // Unpause game and return to game state (Luke)
                    if (SingleKeyPress(Keys.P, currentKBState))
                    {
                        isPaused = false;
                        currentState = GameState.Game;
                    }
                    break;

                case GameState.GameOver:

                    // Return to main menu (Luke)
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

                    // Temp main menu display, can replace these with buttons later (Luke)

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

                    // Instructions
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

                    // Player sprite/animations (Luke)
                    player.Draw(_spriteBatch);

                    // Pause button (P) (Luke)
                    _spriteBatch.DrawString(
                        menuFont,
                        "Pause (P)",
                        new Vector2((windowWidth) - (2 * menuFont.MeasureString("Pause (P)").X / 2),
                        (10)),
                        Color.Black);
                    break;

                case GameState.Pause:

                    // Temp pause menu display (Luke)

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

                    // Temp GameOver display (Luke)

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
        /// Luke
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