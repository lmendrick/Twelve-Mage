using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TwelveMage
{
    internal class WaveManager
    {
        private int currentWave = 1;
        private int specialWaveInterval = 5;
        private int waveIncrease = 3;
        private int corpseLifespan = 2;
        private bool isSpecialWave = false;
        private bool secondWaveArrived = false;
        private double timer = 20.0f;
        private List<Enemy> enemies;
        private List<Spawner> spawners;
        private List<HealthPickup> healthPickups;
        private List<Enemy> deadEnemies;
        private Random rng;
        private List<Enemy> addedEnemies;
        private int startEnemies = 0;
        private int numSpecials = 1;
        private int maxSpecials = 5;


        /// <summary>
        /// Creates a new wave manager entity
        /// </summary>
        /// <param name="enemies">List of enemies</param>
        /// <param name="spawners">List of spawners</param>
        /// <param name="healthPickups">List of pickups</param>
        /// <param name="deadEnemies">List of dead enemies</param>
        public WaveManager(List<Enemy> enemies, List<Spawner> spawners, List<HealthPickup> healthPickups, List<Enemy> deadEnemies) 
        {
            this.enemies = enemies;
            this.spawners = spawners;
            this.healthPickups = healthPickups;
            this.deadEnemies = deadEnemies;
            addedEnemies = new List<Enemy>();

            rng = new Random();
        }

        /// <summary>
        /// Gets the current wave
        /// </summary>
        public int CurrentWave
        {
            get { return currentWave; }
            set { currentWave = value; }
        }

        /// <summary>
        /// Handles wave progression logic
        /// </summary>
        /// <returns></returns>
        public List<Enemy> HandleWaveLogic(GameTime gameTime)
        {

            //If it's a special wave, after 20 seconds or half the enemies dying a second smaller wave will spawn
            if(isSpecialWave)
            {
                if(timer > 0 && enemies.Count > startEnemies / 2 && isSpecialWave && !secondWaveArrived)
                {
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if(isSpecialWave && !secondWaveArrived)
                {
                    SpawnSecondaryWave();

                }
            }
            
            // Wave handling
            if (enemies.Count <= 0)
            {
                if((currentWave + 1) % specialWaveInterval == 0)
                {
                    AdvanceSpecialWave();
                }
                else
                {
                    AdvanceNormalWave();
                }



                enemies[rng.Next(0, enemies.Count())].HasHealthpack = true; // Give two random enemies a healthpack

                // Increment health pickup age
                foreach (HealthPickup healthPickup in healthPickups)
                {
                    healthPickup.Age++;
                }

                // Ages all existing corpses
                foreach (Enemy corpse in deadEnemies)
                {
                    corpse.CorpseAge++;
                }

                // If any corpses are older than the allowed corpseLifespan, remove them
                for (int i = deadEnemies.Count - 1; i >= 0; i--)
                {
                    if (deadEnemies[i].CorpseAge > corpseLifespan)
                    {
                        deadEnemies.RemoveAt(i);
                    }
                }

                currentWave++;
                
            }
            return addedEnemies;
        }

        /// <summary>
        /// Creates a normal wave
        /// </summary>
        public void AdvanceNormalWave()
        {
            // Add a number of regular Enemies equal to wave * waveIncrease
            for (int i = 0; i < currentWave * waveIncrease; i++)
            {
                spawners[rng.Next(0, 4)].SpawnEnemy();
                addedEnemies.Add(enemies[i]);
            }

            timer = 20.0f;
            startEnemies = enemies.Count;
            isSpecialWave = false;
            secondWaveArrived = false;
        }

        /// <summary>
        /// Creates a special wave
        /// </summary>
        public void AdvanceSpecialWave()
        {
            AdvanceNormalWave();

            for (int i = 0; i < numSpecials; i++)
            {
                spawners[rng.Next(0, 4)].SpawnSpecial();
                addedEnemies.Add(enemies[enemies.Count - 1]);
            }

            if(numSpecials < maxSpecials)
            {
                numSpecials++;
            }
            startEnemies = enemies.Count;
            isSpecialWave = true;
        }

        /// <summary>
        /// Creates a smaller secondary wave
        /// </summary>
        public void SpawnSecondaryWave()
        {
            for (int i = 0; i < (currentWave / 2) * waveIncrease; i++)
            {
                spawners[rng.Next(0, 4)].SpawnEnemy();
                addedEnemies.Add(enemies[i]);
            }

            secondWaveArrived = true;
        }

        // Reset to default values (Chloe)
        public void Reset()
        {
            // Clear all relevant Lists
            enemies.Clear();
            healthPickups.Clear();
            deadEnemies.Clear();
            addedEnemies.Clear();

            // Reset relevant values
            currentWave = 1;
            isSpecialWave = false;
            secondWaveArrived = false;
            timer = 20.0f;
            startEnemies = 0;
            numSpecials = 1;
            maxSpecials = 5;
        }
    }
}
