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
    */
    internal class FileManager
    {
        #region FIELDS
        private StreamReader reader;
        private StreamWriter writer;

        private const string PlayerFilename = "../../../PlayerData.txt";
        private const string EnemiesFilename = "../../../EnemyData.txt";
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTORS
        #endregion

        #region METHODS
        public void SavePlayer(Player player) // Saves the player data
        {
            try // Make sure everything works correctly with a try/catch
            {
                writer = new StreamWriter(PlayerFilename);
                // Player file format:
                // Line 1   int pos.X,int pos.Y,int health,string state
                string line = (int)player.PosVector.X + "," + (int)player.PosVector.Y + "," + player.Health + "," + player.State;
                writer.WriteLine(line);
            }
            catch
            {
                Console.WriteLine("Failed to write Player data to PlayerData.txt");
            }
            if (writer != null) writer.Close();
        }

        public void SaveEnemies(List<Enemy> enemies) // Saves all the enemy data
        {
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
            }
            catch
            {
                Console.WriteLine("Failed to write Enemy data to EnemiesData.txt");
            }
            if (writer != null) writer.Close();
        }

        public Player LoadPlayer(Texture2D spritesheet)
        {
            Player player = null;
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
                Console.WriteLine("Failed to read Player data from PlayerData.txt");
                //Environment.Exit(1); // If the player can't be loaded, exit the game
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

                    enemies.Add(new Enemy(pos, spritesheet, health));
                    currentLine = reader.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Failed to read Enemy data from EnemiesData.txt");
                //Environment.Exit(1); // If the enemies can't be loaded, exit the game
            }
            if(reader != null) reader.Close();
            return enemies;
        }
        #endregion
    }
}
