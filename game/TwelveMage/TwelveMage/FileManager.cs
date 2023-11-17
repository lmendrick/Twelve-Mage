using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly TextureLibrary library;

        private const string PlayerFilename = "../../../PlayerData.txt";
        private const string EnemiesFilename = "../../../EnemyData.txt";
        private const string StatsFilename = "../../../StatsData.txt";
        private const string PersistentStatsFilename = "../../../PersistentStatsData.txt";
        private const string Level1Filename = "../../../Level1Data.txt";
        private const string HealthPickupsFilename = "../../../HealthPickupsData.txt";
        private const string SpellsFilename = "../../../SpellData.txt";
        private Player player;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTORS
        public FileManager(Player player, TextureLibrary library)
        {
            this.player = player;
            this.library = library;
        }

        #endregion

        #region METHODS

        #region SAVING
        /// <summary>
        /// Saves all Enemies
        /// </summary>
        /// <param name="enemies">The List of every Enemy that currently exists</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SaveEnemies(List<Enemy> enemies)
        {
            bool saved = false;
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(EnemiesFilename);
                // Enemies file format:
                // Line 1   int pos.X,int pos.Y,int health
                // Line 2   int pos.X,int pos.Y,int health
                // ...
                foreach (Enemy enemy in enemies)
                {
                    string line = (int)enemy.Position.X + "," + (int)enemy.Position.Y + "," + enemy.Health;
                    writer.WriteLine(line);
                }
                saved = true;
            }
            catch
            {

            }
            if (writer != null) writer.Close();
            return saved;
        }

        /// <summary>
        /// Saves all HealthPickups
        /// </summary>
        /// <param name="healthpickups">The List of every HealthPickup that currently exists</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SaveHealthPickups(List<HealthPickup> healthpickups)
        {
            bool saved = false;
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(HealthPickupsFilename);
                // HealthPickups file format:
                // Line 1   int pos.X,int pos.Y,int health
                // Line 2   int pos.X,int pos.Y,int health
                // ...
                foreach (HealthPickup pickup in healthpickups)
                {
                    string line = (int)pickup.Rec.X + "," + (int)pickup.Rec.Y + "," + pickup.Health;
                    writer.WriteLine(line);
                }
                saved = true;
            }
            catch
            {

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
            catch
            {

            }
            if (writer != null) writer.Close();
            return saved;
        }

        /// <summary>
        /// Saves the Player data (position, health, state)
        /// </summary>
        /// <param name="player">The current Player</param>
        /// <returns>True if succesfully saved, False if an error occurred</returns>
        public bool SavePlayer(Player player)
        {
            bool saved = false;
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(PlayerFilename);
                // Player file format:
                // Line 1   int pos.X,int pos.Y,int health,string state
                string line = (int)player.PosVector.X + "," + (int)player.PosVector.Y + "," + player.Health + "," + player.State;
                writer.WriteLine(line);
                saved = true;
            }
            catch
            {

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
            catch
            {

            }
            if(writer != null) writer.Close();
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
            catch
            {

            }
            if (writer != null) writer.Close();
            return saved;
        }
        #endregion

        #region LOADING
        /// <summary>
        /// Loads all Enemies from EnemyData.txt
        /// </summary>
        /// <param name="spritesheet">The spritesheet to pass to each Enemy</param>
        /// <returns>A new list of every loaded Enemy</returns>
        public List<Enemy> LoadEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            Texture2D ZombieSpriteSheet = library.GrabTexture("ZombieSheet");

            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(EnemiesFilename);
                // Enemies file format:
                // Line 1   int pos.X,int pos.Y,int health
                // Line 2   int pos.X,int pos.Y,int health
                // ...
                string currentLine = reader.ReadLine(); // Read the first line
                while (!String.IsNullOrEmpty(currentLine)) // Keep going until the next line is empty
                {
                    string[] line = currentLine.Split(','); // Split the current line
                    int.TryParse(line[0].Trim(), out int X);
                    int.TryParse(line[1].Trim(), out int Y);
                    int.TryParse(line[2].Trim(), out int health);
                    Rectangle pos = new Rectangle(X, Y, 30, 30); // Enemy height & width are 30 & 30

                    enemies.Add(new Enemy(pos, ZombieSpriteSheet, health, enemies, player, null));
                    currentLine = reader.ReadLine();
                }
            }
            catch
            {

            }
            if (reader != null) reader.Close();
            return enemies;
        }

        /// <summary>
        /// Loads all HealthPickups from HealthPickupData.txt
        /// </summary>
        /// <param name="sprite">The sprite to pass to each HealthPickup</param>
        /// <returns>A new list of every loaded HealthPickup</returns>
        public List<HealthPickup> LoadHealthPickups(Texture2D sprite, Player player)
        {
            List<HealthPickup> pickups = new List<HealthPickup>();
            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(HealthPickupsFilename);
                // HealthPickups file format:
                // Line 1   int pos.X,int pos.Y,int health
                // Line 2   int pos.X,int pos.Y,int health
                // ...
                string currentLine = reader.ReadLine(); // Read the first line
                while (!String.IsNullOrEmpty(currentLine)) // Keep going until the next line is empty
                {
                    string[] line = currentLine.Split(','); // Split the current line
                    int.TryParse(line[0].Trim(), out int X);
                    int.TryParse(line[1].Trim(), out int Y);
                    int.TryParse(line[2], out int health);
                    Rectangle pos = new Rectangle(X, Y, 16, 16); // HealthPickup height & width are 30 & 30

                    pickups.Add(new HealthPickup(pos, sprite, health, player));
                    currentLine = reader.ReadLine();
                }
            }
            catch
            {

            }
            if (reader != null) reader.Close();
            return pickups;
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
            catch
            {

            }
            if (reader != null) reader.Close();
            return tileTypes;
        }

        /// <summary>
        /// Loads the player from PlayerData.txt
        /// </summary>
        /// <param name="spritesheet">The spritesheet to pass to the new Player</param>
        /// <returns>A new Player with the loaded stats</returns>
        public Player LoadPlayer(Texture2D spritesheet)
        {
            player = null;
            try // Make sure everything works correctly with a try/catch
            {
                reader = new StreamReader(PlayerFilename);
                // Player file format:
                // Line 1   int pos.X,int pos.Y,int health,string state
                string[] line = reader.ReadLine().Split(','); // Split the line into a string[]
                int.TryParse(line[0].Trim(), out int X);
                int.TryParse(line[1].Trim(), out int Y);
                int.TryParse(line[2].Trim(), out int health);
                Rectangle pos = new Rectangle(X, Y, 34, 30); // Player height & width are 34 & 30
                player = new Player(pos, spritesheet, health); // Create a new Player with the loaded data
                player.State = Enum.Parse<PlayerState>(line[3]); // Parse the state data to PlayerState
            }
            catch
            {

            }
            if (reader != null) reader.Close();
            return player;
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
            catch
            {

            }
            if(reader != null) reader.Close();
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
            catch
            {

            }
            if (reader != null) reader.Close();
            return SpellsDictionary;
        }
        
        #endregion

        #endregion
    }
}