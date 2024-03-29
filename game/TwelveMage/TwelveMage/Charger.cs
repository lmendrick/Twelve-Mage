﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Ben Fawley
 * Twelve-Mage
 * Zombie special type charger
 * gives a relentless chase 
 * hints at charging before bursting forward
 * calculates player movement and rushes at them based on it
 * 
 * 
 */

namespace TwelveMage
{
    internal class Charger : Enemy
    {
        private bool charging;
        Player player;


        private int attackDistance = 250;
        private int chargeDistance = 300;
        private int distanceTraveled;
        private float pauseTimer = 0.5f;
        


        public Charger(Rectangle rec, TextureLibrary textureLibrary, int health, List<Enemy> enemies, Player player, Random rng)
            : base(rec, textureLibrary, health, enemies, player, rng)
        {
            _textureLibrary = textureLibrary;
            this.player = player;
            Color = Color.DarkMagenta;
            drawScale = 2.5f;
            speed = 100;
        }


        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            speed = 100;

            if (!charging)
            {
                base.Update(gameTime, bullets);

                CheckPlayerDistance();
            }
            else if(charging)
            {   
                HandleCharging(gameTime, bullets);
            }
        }


        private void HandleCharging(GameTime gameTime, List<GameObject> bullets)
        {
            // Once charging begins, there should be a brief moment where the charger stops a warning signal
            if (!IsFrozen)
            {
                if (pauseTimer > 0)
                {
                    pauseTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Direction = Vector2.Zero;

                    if (pauseTimer <= 0)
                    {
                        Direction = player.PosVector - this.Position;
                        Direction.Normalize();
                        distanceTraveled = 0;
                    }
                }
                else
                {
                    this.Position += Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * (speed * 4);
                    distanceTraveled += (int)(Direction * (float)gameTime.ElapsedGameTime.TotalSeconds * (speed * 4)).Length();

                    rec.X = (int)Position.X;
                    rec.Y = (int)Position.Y;
                }


                if (distanceTraveled > chargeDistance)
                {
                    charging = false;
                    distanceTraveled = 0;
                    pauseTimer = 0.5f;
                }
            }

            CheckHits(bullets);

            

            // Once the timer runs out, it should calculate the direction of the player at that moment, and begin charging rapidly in that direction


            // Once the charge is completed, set charging = false
        }


        private void CheckPlayerDistance()
        {
            int playerDistance = (int)(player.PosVector - this.Position).Length();

            if (playerDistance <= attackDistance)
            {
                charging = true;
            }
        }
    }
}
