using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TwelveMage
{
    internal class Charger : Enemy
    {
        private bool charging;

        Player player;


        private int attackDistance = 200;
        private float slowTimer = 0.5f;


        public Charger(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player, Texture2D corpseSprite)
            : base(rec, texture, health, enemies, player, corpseSprite)
        {
            this.player = player;
        }


        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            if (!charging)
            {
                base.Update(gameTime, bullets);
            }
            else if(charging)
            {   
                HandleCharging(gameTime);
            }
        }


        private void HandleCharging(GameTime gameTime)
        {
            // Once charging begins, there should be a brief moment where the charger slows down as a warning signal
            if(slowTimer > 0)
            {
                slowTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
            }
            

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
