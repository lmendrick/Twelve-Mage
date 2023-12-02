using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TwelveMage
{
    /*
    * Chloe Hall
    * Twelve-Mage
    * This class manages file loading/saving
    * and assigning them textures based on their type
    * No known issues.
    * Added stats save method (score, wave)
    * Added persistent stats save method (highScore, highWave)
    * Added HealthPickup save/load methods
    * Added Spells save/load methods
    */
    internal class FileManager
    {
        #region FIELDS
        private StreamReader reader;
        private StreamWriter writer;
        private readonly TextureLibrary _textureLibrary;
        private readonly Random rng;
        private int windowHeight;
        private int windowWidth;

        private const string PlayerFilename = "../../../PlayerData.txt";
        private const string EnemiesFilename = "../../../EnemyData.txt";
        private const string StatsFilename = "../../../StatsData.txt";
        private const string PersistentStatsFilename = "../../../PersistentStatsData.txt";
        private const string Level1Filename = "../../../Level1Data.txt";
        private const string HealthPickupsFilename = "../../../HealthPickupsData.txt";
        private const string SpellsFilename = "../../../SpellData.txt";
        private const string GameObjectsFilename = "../../../Objects.txt";
        private Player player;
        #endregion

        #region CONSTRUCTORS
        public FileManager(Player player, TextureLibrary textureLibrary, int windowHeight, int windowWidth, Random rng)
        {
            this.player = player;
            this.windowWidth = windowHeight;
            this.windowHeight = windowWidth;
            _textureLibrary = textureLibrary;
            this.rng = rng;
        }

        #endregion

        #region METHODS

        #region SAVING
        /// <summary>
        /// Saves all GameObjects, including the Player, Enemies, and HealthPickups
        /// </summary>
        /// <param name="player">The Player to save</param>
        /// <param name="enemies">The List of Enemies to save</param>
        /// <param name="healthPickups">The List of HealthPickups to save</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SaveGameObjects(Player player, List<Enemy> enemies, List<HealthPickup> healthPickups)
        {
            bool saved = false;
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(GameObjectsFilename);
                // GameObjects file format:
                // Line 1, Player data:             int posX, int posY, int Health, double invulnerableTimer, string PlayerState
                // Lines 2-?, Other GameObject data:
                // Section 1, Enemy data:           string EnemyType, int posX, int posY, int Health
                // Section 2, HealthPickup data:    "HealthPickup", int posX, int posY, int Health, int Lifespan, int Age
                string line = "";

                // Write Player data first
                writer.WriteLine((int)player.PosVector.X + "," + (int)player.PosVector.Y + "," + player.Health + "," + player.InvulnerableTimer + "," + player.State);

                // Write Enemy data
                foreach(Enemy enemy in enemies)
                {
                    line = (int)enemy.Position.X + "," + (int)enemy.Position.Y + "," + enemy.Health; // Add basic data

                    switch(enemy) // Add enemy's type to the front of the line and write it
                    {
                        case Summoner:
                            writer.WriteLine("Summoner," + line);
                            break;

                        case Charger:
                            writer.WriteLine("Charger," + line);
                            break;

                        default: // Default to Enemy
                        case Enemy:
                            writer.WriteLine("Enemy," + line);
                            break;
                    }
                }

                // Write HealthPickup data
                foreach(HealthPickup healthPickup in healthPickups)
                {
                    line = "HealthPickup," + (int)healthPickup.Rec.X + "," + (int)healthPickup.Rec.Y + ","
                        + healthPickup.Health + "," + healthPickup.Lifespan + "," + healthPickup.Age;
                    writer.WriteLine(line);
                }
                saved = true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (writer != null) writer.Close();
            return saved;
        }

        /// <summary>
        /// Saves global, presistent data (high scores)
        /// </summary>
        /// <param name="highScore">The high score, stored in memory</param>
        /// <param name="highWave">The highest wave, stored in memory</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SavePersistentStats(int highScore, int highWave) // Saves global data (high score, highest wave)
        {
            bool saved = false;
            try
            {
                writer = new StreamWriter(PersistentStatsFilename);
                // PStats file format:
                // Line 1   int High Score,int Highest Wave
                string line = highScore + "," + highWave;
                writer.WriteLine(line);
                saved = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while saving Persistent Stats: " + ex.Message);
            }
            if (writer != null) writer.Close();
            return saved;
        }

        /// <summary>
        /// Saves level-based stats
        /// </summary>
        /// <param name="score">The current score</param>
        /// <param name="wave">The current wave</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SaveStats(int score, int wave)
        {
            bool saved = false;
            try
            {
                writer = new StreamWriter(StatsFilename);
                // Stats file format:
                // Line 1   int Score,int Wave #
                string line = score + "," + wave;
                writer.WriteLine(line);
                saved = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while saving Stats: " + ex.Message);
            }
            if (writer != null) writer.Close();
            return saved;
        }

        /// <summary>
        /// Saves all spell data
        /// </summary>
        /// <param name="player">The current Player</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SaveSpells(Dictionary<string, double> SpellsDictionary)
        {
            bool saved = false;
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(SpellsFilename);
                // All data is cast to ints because it'll make the file more readable
                // Being within one second of the saved time is accurate enough ¯\_(ツ)_/¯
                // Even though effect and cooldown durations aren't changeable,
                // This method should still be functional if we add stat upgrades
                // Spells file format:
                // Line 1   "Blink:",int blinkTimer,int blinkCooldownDuration
                // Line 2   "Fireball:",int fireballTimer,int fireballCooldownDuration
                // Line 3   "Freeze:",int freezeTimer,int freezeCooldownDuration,int freezeEffectDuration
                // Line 4   "Haste:",int hasteTimer,int hasteCooldownDuration,int hasteEffectDuration

                String currentLine = "";
                // Blink
                SpellsDictionary.TryGetValue("BlinkTimer", out double blinkTimer);
                SpellsDictionary.TryGetValue("BlinkCooldown", out double blinkCooldown);
                currentLine += "Blink:," + blinkTimer + "," + blinkCooldown;
                writer.WriteLine(currentLine);
                currentLine = "";

                // Fireball
                SpellsDictionary.TryGetValue("FireballTimer", out double fireballTimer);
                SpellsDictionary.TryGetValue("FireballCooldown", out double fireballCooldown);
                currentLine += "Fireball:," + fireballTimer + "," + fireballCooldown;
                writer.WriteLine(currentLine);
                currentLine = "";

                // Freeze
                SpellsDictionary.TryGetValue("FreezeTimer", out double freezeTimer);
                SpellsDictionary.TryGetValue("FreezeCooldown", out double freezeCooldown);
                SpellsDictionary.TryGetValue("FreezeEffect", out double freezeEffect);
                currentLine += "Freeze:," + freezeTimer + "," + freezeCooldown + "," + freezeEffect;
                writer.WriteLine(currentLine);
                currentLine = "";

                // Haste
                SpellsDictionary.TryGetValue("HasteTimer", out double hasteTimer);
                SpellsDictionary.TryGetValue("HasteCooldown", out double hasteCooldown);
                SpellsDictionary.TryGetValue("HasteEffect", out double hasteEffect);
                currentLine += "Haste:," + hasteTimer + "," + hasteCooldown + "," + hasteEffect;
                writer.WriteLine(currentLine);

                saved = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while saving Spells: " + ex.Message);
            }
            if (writer != null) writer.Close();
            return saved;
        }
        #endregion

        #region LOADING
        /// <summary>
        /// Loads all GameObject data, including the Player, Enemies, and HealthPickups
        /// </summary>
        /// <param name="player">The Player object to overwite the data of</param>
        /// <param name="enemies">The List of Enemies to add loaded Enemies to</param>
        /// <param name="summoners">The List of Summoners to add loaded Summoners to</param>
        /// <param name="healthPickups">The List of HealthPickups to add loaded HealthPickups to</param>
        /// <param name="spawners">The List of Spawners to pass to loaded Summoners</param>
        public void LoadGameObjects(Player player, List<Enemy> enemies, List<Summoner> summoners,
            List<HealthPickup> healthPickups, List<Spawner> spawners)
        {
            // Clear all relevant Lists
            enemies.Clear();
            summoners.Clear();
            healthPickups.Clear();

            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(GameObjectsFilename);
                // GameObjects file format:
                // Line 1, Player data:             int posX, int posY, int Health, double invulnerableTimer, string PlayerState
                // Lines 2-?, Other GameObject data:
                // Section 1, Enemy data:           string EnemyType, int posX, int posY, int Health
                // Section 2, HealthPickup data:    "HealthPickup", int posX, int posY, int Health, int Lifespan, int Age
                string[] splitLine;

                // Read Player data
                splitLine = reader.ReadLine().Split(",");

                int.TryParse(splitLine[0], out int X);
                int.TryParse(splitLine[1], out int Y);
                int.TryParse(splitLine[2], out int health);
                double.TryParse(splitLine[3], out double invulnerableTimer);

                // Overwrite Player's data; don't create a new Player
                player.PosVector = new Vector2(X, Y);
                player.OverwriteHealthData(health, invulnerableTimer);
                player.State = Enum.Parse<PlayerState>(splitLine[4]);

                // Read other GameObject data
                string currentLine = reader.ReadLine(); // Read the first line
                while (!String.IsNullOrEmpty(currentLine)) // Keep going until the next line is empty
                {
                    splitLine = currentLine.Split(",");

                    switch(splitLine[0])
                    {
                        case "HealthPickup":

                            int.TryParse(splitLine[1], out X);
                            int.TryParse(splitLine[2], out Y);
                            int.TryParse(splitLine[3], out health);
                            int.TryParse(splitLine[4], out int lifespan);
                            int.TryParse(splitLine[5], out int age);

                            Rectangle HPRec = new Rectangle(X, Y, 16, 16); // Healthpack rectangle is 16x16

                            healthPickups.Add(new HealthPickup(HPRec, _textureLibrary, health, player, rng, lifespan, age));

                            break;

                        case "Summoner":

                            int.TryParse(splitLine[1], out X);
                            int.TryParse(splitLine[2], out Y);
                            int.TryParse(splitLine[3], out health);

                            Rectangle summonerRec = new Rectangle(X, Y, 45, 45); // Summoner rectangle is 45x45

                            // Add the new Summoner to enemies; it adds itself to summoners
                            enemies.Add(new Summoner(summonerRec, _textureLibrary, health, enemies, player, 10, windowWidth, windowHeight, summoners, rng));

                            break;

                        case "Charger":

                            int.TryParse(splitLine[1], out X);
                            int.TryParse(splitLine[2], out Y);
                            int.TryParse(splitLine[3], out health);

                            Rectangle chargerRec = new Rectangle(X, Y, 60, 60); // Charger rectangle is 60x60

                            // Add the new Charger to enemies
                            enemies.Add(new Charger(chargerRec, _textureLibrary, health, enemies, player, rng));

                            break;

                        default: // Default to Enemy / Zombie
                        case "Enemy":
                        case "Zombie":

                            int.TryParse(splitLine[1], out X);
                            int.TryParse(splitLine[2], out Y);
                            int.TryParse(splitLine[3], out health);

                            Rectangle zombieRec = new Rectangle(X, Y, 30, 30); // Enemy rectangle is 30x30

                            // Add the new Charger to enemies
                            enemies.Add(new Enemy(zombieRec, _textureLibrary, health, enemies, player, rng));

                            break;
                    }

                    currentLine = reader.ReadLine(); // Read the next line
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while loading GameObjects: " + ex.ToString());
            }
            if (reader != null) reader.Close();
        }

        /// <summary>
        /// Loads the background tile types from Level1Data.txt
        /// </summary>
        /// <returns>A 2D String array representing each tile's type</returns>
        public String[,] LoadLevel()
        {
            String[,] tileTypes = null;
            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(Level1Filename);
                // Level file format:
                // Line 1   int width, int height (total number of tiles on each side)
                // Line 2   X,X,X,...X w times (char representing tile type)
                // Line 3   X,X,X,...X
                // ...
                // Line h   X,X,X,...X

                // Get width & height
                string[] currentLine = reader.ReadLine().Split(",");
                int.TryParse(currentLine[0], out int width);
                int.TryParse(currentLine[1], out int height);

                // Initialize tileTypes
                tileTypes = new String[width, height];

                // Load each tile type
                for (int i = 0; i < height; i++)
                {
                    currentLine = reader.ReadLine().Split(",");
                    for (int j = 0; j < width; j++)
                    {
                        tileTypes[j, i] = currentLine[j];
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while loading Default Level: " + ex.Message);
            }
            if (reader != null) reader.Close();
            return tileTypes;
        }

        /// <summary>
        /// Loads all stats, global and run-based
        /// </summary>
        /// <returns>An array with each stat: {run score, high score, run wave, highest wave}</returns>
        public int[] LoadStats()
        {
            int[] stats = new int[4];
            try
            {
                reader = new StreamReader(StatsFilename);
                // Stats file format:
                // Line 1   int Score,int Wave #
                string[] currentLine = reader.ReadLine().Split(',');
                int.TryParse(currentLine[0], out stats[0]);
                int.TryParse(currentLine[1], out stats[2]);

                if(reader != null) reader.Close();

                reader = new StreamReader(PersistentStatsFilename);
                // PStats file format:
                // Line 1   int High Score,int Highest Wave
                currentLine = reader.ReadLine().Split(',');
                int.TryParse(currentLine[0], out stats[1]);
                int.TryParse(currentLine[1], out stats[3]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while loading Stats: " + ex.Message);
            }
            if (reader != null) reader.Close();
            return stats;
        }

        /// <summary>
        /// Loads all spell data
        /// </summary>
        /// <param name="player">The current Player</param>
        /// <returns>A Dictionary with spell data</returns>
        public Dictionary<String, double> LoadSpells()
        {
            Dictionary<String, double> SpellsDictionary = new Dictionary<String, double>();
            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(SpellsFilename);
                // All data is cast to ints because it'll make the file more readable
                // Being within one second of the saved time is accurate enough ¯\_(ツ)_/¯
                // Even though effect and cooldown durations aren't changeable,
                // This method should still be functional if we add stat upgrades
                // Spells file format:
                // Line 1   "Blink:",int blinkTimer,int blinkCooldownDuration
                // Line 2   "Fireball:",int fireballTimer,int fireballCooldownDuration
                // Line 3   "Freeze:",int freezeTimer,int freezeCooldownDuration,int freezeEffectDuration
                // Line 4   "Haste:",int hasteTimer,int hasteCooldownDuration,int hasteEffectDuration

                // Blink
                double currentValue = 0;
                string[] currentLine = reader.ReadLine().Split(","); // Line 1:

                double.TryParse(currentLine[1], out currentValue); // Blink Timer,
                SpellsDictionary.Add("BlinkTimer", currentValue);
                double.TryParse(currentLine[2], out currentValue); // Blink Cooldown Duration
                SpellsDictionary.Add("BlinkCooldown", currentValue);

                // Fireball
                currentValue = 0;
                currentLine = reader.ReadLine().Split(","); // Line 2:

                double.TryParse(currentLine[1], out currentValue); // Fireball Timer,
                SpellsDictionary.Add("FireballTimer", currentValue);
                double.TryParse(currentLine[2], out currentValue); // Fireball Cooldown Duration
                SpellsDictionary.Add("FireballCooldown", currentValue);

                // Freeze
                currentValue = 0;
                currentLine = reader.ReadLine().Split(","); // Line 3:

                double.TryParse(currentLine[1], out currentValue); // Freeze Timer,
                SpellsDictionary.Add("FreezeTimer", currentValue);
                double.TryParse(currentLine[2], out currentValue); // Freeze Cooldown Duration,
                SpellsDictionary.Add("FreezeCooldown", currentValue);
                double.TryParse(currentLine[3], out currentValue); // Freeze Effect Duration
                SpellsDictionary.Add("FreezeEffect", currentValue);

                // Haste
                currentValue = 0;
                currentLine = reader.ReadLine().Split(","); // Line 4:

                double.TryParse(currentLine[1], out currentValue); // Haste Timer,
                SpellsDictionary.Add("HasteTimer", currentValue);
                double.TryParse(currentLine[2], out currentValue); // Haste Cooldown Duration,
                SpellsDictionary.Add("HasteCooldown", currentValue);
                double.TryParse(currentLine[3], out currentValue); // Haste Effect Duration
                SpellsDictionary.Add("HasteEffect", currentValue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed while loading Spells: " + ex.Message);
            }
            if (reader != null) reader.Close();
            return SpellsDictionary;
        }
        
        #endregion

        #endregion
    }
}