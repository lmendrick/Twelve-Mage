/*
 * Lucas Mendrick
 * Twelve-Mage
 * This class serves as a base for all future game objects such as
 * Player, Enemies, and Projectiles.
 * No known issues.
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class GameObject
{
	protected Rectangle position;
	protected Texture2D texture;
	protected int health;
    protected bool isActive = true;
    public Rectangle Position {  get { return position; } }
	public int Health { get { return health; } set { health = value; } }
	

	protected GameObject(Rectangle position, Texture2D texture, int health)
	{
		this.position = position;
		this.texture = texture;
		this.health = health;
	}

    //Anthony
    //Check if game object collides with one another
    public bool CheckCollision(GameObject check)
    {
        //if the object is inactive then nothing happens
        if (isActive == false)
        {
            return false;
        }
        else
        {
            //but if its active check if they instersect and if they do return true
            if (position.Intersects(check.Position))
            {
                isActive = false;
                return true;
            }
            else { return false; }
        }
    }

    // Luke
    // Purpose: Use a SpriteBatch obejct to draw the GameObject
    // Params: spriteBatch - The SpriteBatch object passed in from the main Draw()
    // Restrictions: Color is always white. May want to add a color field in the future.
    public virtual void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, position, Color.White);
	}

	// Luke
	// Purpose: Implemented by child classes so that they can update themselves
	// Params: gameTime - GameTime object passed in from the main Update() 
	public abstract void Update(GameTime gameTime);
}
