using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using System.Collections;
using System.Collections.Immutable;

namespace Overload
{
    public class EnemyManager
    {

        //Fields:
        private List<Enemy> enemies;
        private List<Rectangle> enemyHitboxes;

        //Properties:
        public int NumOfEnemies { get { return enemies.Count; } }

        //Constructors:
        public EnemyManager(string filepath,Player player, TileManager tManager)
        {
            StreamReader reader = null!;
            enemies = new List<Enemy>();
            enemyHitboxes = new List<Rectangle>();

            try
            {
                reader = new StreamReader(filepath);
                string rawData;
                string[] splitData;

                //reading in the first directions line
                reader.ReadLine();

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');

                    //instantiating the new enemies
                    enemies.Add(new Enemy(
                        new Rectangle(
                            int.Parse(splitData[0]),
                            int.Parse(splitData[1]),
                            50,
                            50),
                            int.Parse(splitData[2]),
                            int.Parse(splitData[3])));
                }

            }
            catch (Exception error)
            {
                Debug.Print(error.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            //event subscription
            foreach (Enemy enemy in enemies)
            {
                enemy.GetAttack += player.AttackRange;
                enemy.GetCollidableTiles += tManager.GetCollidableTiles;
            }
        }

        //Methods:
        /// <summary>
        /// Draw method for the EnemyManager class
        /// </summary>
        public void Draw()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw();
            }
        }

        /// <summary>
        /// per frame update method for the EnemyManager
        /// </summary>
        public void Update()
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update();
            }
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].IsDeathDone)
                {
                    enemies.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// gets a list of the enemy hitboxes so the player can be damaged
        /// </summary>
        /// <returns>a list of rectangle hitboxes</returns>
        public List<Rectangle> EnemyDamage()
        {
            enemyHitboxes.Clear();

            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsDead)
                {
                    enemyHitboxes.Add(enemy.Position);
                }
            }

            return enemyHitboxes;
        }

    }
}
