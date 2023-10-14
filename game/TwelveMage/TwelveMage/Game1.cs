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
        //fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentState;
        private KeyboardState currentKBState;
        private Player player;
        private int playerWidth = 34;
        private int playerHeight = 30;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            
            // Luke: Load in player character sprite sheet
            Texture2D spriteSheet = this.Content.Load<Texture2D>("CharacterSheet");

            // Luke: Instantiate player
            Rectangle playerRec = new Rectangle(30, 30, playerWidth, playerHeight);
            player = new Player(playerRec, spriteSheet, 100);
            player.State = PlayerState.FaceRight;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKBState = Keyboard.GetState();

            // Luke: Update the player sprite animation every frame
            player.UpdateAnimation(gameTime);

            // TODO: Add your update logic here
            //Anthony Maldonado
            //switches made for the game states 
            switch (currentState)
            {
                case GameState.Menu:

                    // Luke: Testing player movement
                    player.Update(gameTime);

                    if (currentKBState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.Game:
                    if (currentKBState.IsKeyDown(Keys.P))
                    {
                        currentState = GameState.Pause;
                    }
                    break;
                case GameState.Pause:
                    if (currentKBState.IsKeyDown(Keys.P))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.GameOver:
                    if (currentKBState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.Menu;
                    }
                    break;
                default:
                    break;
            }
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


                    // Luke: Testing player sprite
                    player.Draw(_spriteBatch);


                    break;
                case GameState.Game:

                    break;
                case GameState.Pause:
                    if (currentKBState.IsKeyDown(Keys.P))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.GameOver:

                    break;

            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}