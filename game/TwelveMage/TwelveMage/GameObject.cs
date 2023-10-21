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
	protected Rectangle rec;
	protected Texture2D texture;
	protected int health;
    protected bool isActive = true;
    public Rectangle Rec {  get { return rec; } }
	public int Health { get { return health; } set { health = value; } }
	

	protected GameObject(Rectangle rec, Texture2D texture, int health)
	{
		this.rec = rec;
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
            if (rec.Intersects(check.Rec))
            {
                isActive = false;
                return true;
            }
            else { return false; }
        }
    }

    // Lucas
    // Purpose: Use a SpriteBatch obejct to draw the GameObject
    // Params: spriteBatch - The SpriteBatch object passed in from the main Draw()
    // Restrictions: Color is always white. May want to add a color field in the future.
    public virtual void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, rec, Color.White);
	}

	// Lucas
	// Purpose: Implemented by child classes so that they can update themselves
	// Params: gameTime - GameTime object passed in from the main Update() 
	public abstract void Update(GameTime gameTime);
}
