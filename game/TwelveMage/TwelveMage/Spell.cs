using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwelveMage;
internal class Spell
{
	private Player player;

	public Spell(Player player)
	{
		this.player = player;
	}

	public void Fireball()
	{

	}

	public void Haste()
	{

	}

	public void TimeFreeze()
	{

	}

	/// <summary>
	/// Blinks the player forward in the direction they are moving
	/// </summary>
	/// <param name="direction">The players current direction</param>
	public void Blink(Vector2 direction)
	{
		Vector2 blinkDistance = new Vector2(direction.X * 100, direction.Y * 100);

		player.PosVector += blinkDistance;
		

	}
}
