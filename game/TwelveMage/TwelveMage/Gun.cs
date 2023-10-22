﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    //gunstates
    enum GunState
    {
        FaceLeft,
        FaceRight
    }
    internal class Gun : GameObject
    {
        //fields
        Texture2D gunTexture;
        // Move gun with player
        Vector2 dir;
        Vector2 pos;
        GunState state;
        float speed = 200f;
        const int GunRectHeight = 45;     // The height the image
        const int GunRectWidth = 25;        // The width of the image 
        const int GunRectOffsetY = 1;
        
        //gun face
        public GunState State
        {
            get { return state; } 
            set { state = value; }
        }

        //properties
        public Vector2 PosVector
        {
            get { return pos; }
            set {  pos = value; }
        }

        public Vector2 DirVector
        {
            get { return dir; }
        }


        //constructors
        public Gun(Rectangle rec, Texture2D texture, int health, Player player, GunState gunState) : base(rec, texture, health)
        {

            this.rec = rec;
            this.gunTexture = texture;
            this.health = health;
            this.pos = player.PosVector;
            //default positioning
            this.state = gunState;


        }

        //methods

        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
           KeyboardState current = Keyboard.GetState();
            if(current.IsKeyDown(Keys.D))
            {
                state = GunState.FaceRight;
            }
            if(current.IsKeyDown(Keys.A))
            {
                state = GunState.FaceLeft;
            }

            rec.X = (int)(pos.X);
            rec.Y = (int)(pos.Y);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case GunState.FaceRight:
                    DrawStanding(SpriteEffects.None, spriteBatch);
                    break;

                case GunState.FaceLeft:
                    DrawStanding(SpriteEffects.FlipHorizontally, spriteBatch);
                    break;
            } // Necessary for inheriting from GameObject
        }

        private void DrawStanding(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                pos,
                new Rectangle(
                    0,
                    GunRectOffsetY,
                    GunRectWidth,
                    GunRectHeight),
                Color.White,
                0,
                Vector2.Zero,
                1.0f,
                flipSprite,
                0);
        }

    }
}