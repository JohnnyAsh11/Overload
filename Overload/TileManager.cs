using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Overload
{
    public class TileManager
    {
        //Fields
        private string collision_filename;
        private string texture_filename;
        private Tile[,] grid;
        private List<Rectangle> collidable_tiles;

        //Properties
        

        //Constructors
        public TileManager(string collision_filename, string texture_filename)
        {
            this.collision_filename = collision_filename;
            this.texture_filename = texture_filename;
            this.collidable_tiles = new List<Rectangle>();

            Read();
            Read_Textures();
        }

        //Methods
        private void Read()
        {
            // Read from the file and generate tiles based on collision
            StreamReader reader = null;
            int row_counter = 0;
            grid = new Tile[20, 192];
            int x_pos = 0, y_pos = 0;

            try
            {
                reader = new StreamReader(collision_filename);
                string rawData;
                string[] splitData;

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');
                    for(int i = 0; i < splitData.Length; i++)
                    {
                        if (splitData[i] == "0")
                        {
                            // if reads a 0, create a non-colliding tile
                            grid[row_counter, i] = new Tile(new Rectangle(x_pos, y_pos, 50, 50), false);
                        }
                        else if (splitData[i] == "1")
                            // if reads a 1, create a colliding tile and add to list of collidable tiles
                        {
                            grid[row_counter, i] = new Tile(new Rectangle(x_pos, y_pos, 50, 50), true);
                            collidable_tiles.Add(grid[row_counter, i].Position);
                        }
                        else
                        {
                            // unexpected input, throw an error
                            throw new Exception("Unexpected character when parsing " + collision_filename);
                        }
                        x_pos += 50;
                    }
                    row_counter++;
                    y_pos += 50;
                    x_pos = 0;
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
                    //close the file if it isn't null
                    reader.Close();
                }
            }

        }

        public List<Rectangle> GetCollidableTiles()
        {
            return collidable_tiles;
        }


        public void Draw()
        {
            // Draw squares
            for(int row = 0; row < grid.GetLength(0) ; row++)
            {
                for(int col = 0; col < grid.GetLength(1); col++)
                {
                    grid[row, col].Draw();
                }
            }
        }

        private void Read_Textures()
        {
            // Read a file and add texture to created tiles
            StreamReader reader = null;
            int row_counter = 0;

            try
            {
                reader = new StreamReader(texture_filename);
                string rawData;
                string[] splitData;

                while ((rawData = reader.ReadLine()!) != null)
                {
                    splitData = rawData.Split('|');
                    for (int i = 0; i < splitData.Length; i++)
                    {
                        try
                        {
                            int character = int.Parse(splitData[i]);
                            grid[row_counter, i].Texture = (TileTexture)character;
                        }
                        catch
                        {
                            throw new Exception("Unexpected character when parsing " + texture_filename);
                        }
                        
                    }
                    row_counter++;
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
                    //close the file if it isn't null
                    reader.Close();
                }
            }
        }

    }
}
