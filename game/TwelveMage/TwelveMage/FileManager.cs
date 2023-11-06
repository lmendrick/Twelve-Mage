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
    */
    internal class FileManager
    {
        #region FIELDS
        private StreamReader reader;
        private StreamWriter writer;

        private const string PlayerFilename = "../../../PlayerData.txt";
        private const string EnemiesFilename = "../../../EnemyData.txt";
        private const string StatsFilename = "../../../StatsData.txt";
        private const string PersistentStatsFilename = "../../../PersistentStatsData.txt";
        private Player player;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTORS
        #endregion

        #region METHODS
        public bool SavePlayer(Player player) // Saves the player data
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

        public bool SaveEnemies(List<Enemy> enemies) // Saves all the enemy data
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

        public bool SaveStats(int score, int wave) // Saves level data (current score, current wave)
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

        public List<Enemy> LoadEnemies(Texture2D spritesheet)
        {
            List<Enemy> enemies = new List<Enemy>();
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

                    enemies.Add(new Enemy(pos, spritesheet, health, enemies, player));
                    currentLine = reader.ReadLine();
                }
            }
            catch
            {

            }
            if(reader != null) reader.Close();
            return enemies;
        }

        public int[] LoadStats() // Returns an int[4] with various scores: level score, high score, level wave, highest wave
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
        #endregion
    }
}
