using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Chloe Hall
 * Twelve-Mage
 * 
 * ICollidable is an interface for collidable GameObjects to consolidate how object-on-object damage functions.
 * Uses two GameObject fields, health and damage, to calculate appropriate health values after a collision.
 */
namespace TwelveMage
{
    internal interface ICollidable
    {
        /// <summary>
        /// Deals source.Damage damage to target.Health
        /// </summary>
        /// <param name="source">GameObject that deals damage</param>
        /// <param name="target">GameObject that takes damage</param>
        void DamagingCollision(GameObject source, GameObject target)
        {
            target.Health -= source.Damage;
        }
    }
}
