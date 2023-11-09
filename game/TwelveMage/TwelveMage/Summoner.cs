using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwelveMage
{
    internal class Summoner : Enemy
    {
        private List<Spawner> personalSpawners;
        private List<Spawner> playerSpawners;

        private List<Enemy> spawned;

        private int maxEnemies;
        private int currentEnemies;


        public Summoner(Rectangle rec, Texture2D texture, int health, List<Enemy> enemies, Player player) : base(rec, texture, health, enemies, player)
        {
        }
    }
}
