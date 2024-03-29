﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TwelveMage;

/*
 * Twelve Mage
 * This class was supposed to handle all spells, but was easier to add
 * them in the Player class. Only handles the blink spell.
 */
internal class Spell
{
	private Player player;

	public Spell(Player player)
	{
		this.player = player;	
	}

	public void Fireball() 
    {
		// Handled in Player class as a projectile
	}

	public void Haste()
	{
		// Handled in Player class
	}

	public void TimeFreeze()
	{
		// Handled in Player class 
	}

	/// <summary>
	/// Blinks the player forward in the direction they are moving
	/// </summary>
	/// <param name="direction">The players current direction</param>
	public void Blink(MouseState mState)
	{
		player.Position = mState.Position.ToVector2();
	}
}
