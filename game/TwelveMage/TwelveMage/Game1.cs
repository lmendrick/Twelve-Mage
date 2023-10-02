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
        private GameState currentState = GameState.Menu;
        private KeyboardState currentKBState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
            //switch to ensure what happens in each state
            //will add more after discussion and more work
            switch (currentState)

            {
                case GameState.Menu:
                    _spriteBatch.Begin();
                    
                    _spriteBatch.End();
                    break;
                case GameState.Game:
                    _spriteBatch.Begin();

                    _spriteBatch.End();
                    break;
                case GameState.Pause:
                    if (currentKBState.IsKeyDown(Keys.P))
                    {
                        currentState = GameState.Game;
                    }
                    break;
                case GameState.GameOver:
                    
                    _spriteBatch.Begin();
                    
                    _spriteBatch.End();
                    break;

            }
            base.Draw(gameTime);
        }
    }
}