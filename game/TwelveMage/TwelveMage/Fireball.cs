using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
/* Chloe Hall
 * Twelve-Mage
 * This class is for projectiles created by the Fireball spell
 */
namespace TwelveMage
{
    internal class Fireball : Projectile
    {
        #region CONSTRUCTORS
        public Fireball(Rectangle position, TextureLibrary textureLibrary, int health, float range) : base(position, textureLibrary, health, range)
        {
            texture = textureLibrary.GrabTexture("Fireball");
            maxPen = 25; // Fireball goes through more enemies
        }
        #endregion

        #region METHODS
        public override void Update(GameTime gameTime, List<GameObject> bullets)
        {
            rec.Height += 2;
            rec.Width += 2;
            base.Update(gameTime, bullets);
        }
        #endregion
    }
}
