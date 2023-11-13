using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Overload
{
    /// <summary>
    /// Static Globals class for anything that the entire program needs access too
    /// </summary>
    public static class Globals
    {

        //Properties:
        public static SpriteBatch SB { get; set; }

        public static Dictionary<string, Texture2D> GameTextures { get; set; }

        public static float Gravity { get { return -25; } }

        public static Dictionary<TileTexture, Rectangle> Tiles { get; set; }

        //Methods: 
        /// <summary>
        /// Normalizes a vector
        /// </summary>
        /// <param name="vector">Vector to be normalized</param>
        /// <returns>a normal vector</returns>
        public static Vector2 Normalize(Vector2 vector)
        {
            Vector2 normalVector = Vector2.Zero;
            float xSquared = (float)Math.Pow(vector.X, 2);
            float ySquared = (float)Math.Pow(vector.Y, 2);
            float length = (float)Math.Sqrt(xSquared + ySquared);

            //creating the normal vector
            normalVector = new Vector2(vector.X / length,
                                       vector.Y / length);

            return normalVector;
        }

        /// <summary>
        /// converts a Degree to a radian
        /// </summary>
        /// <param name="degree">the degree to be converted</param>
        /// <returns>a radian value</returns>
        public static float Deg2Rad(float degree)
        {
            float radian;

            radian = (float)(degree * Math.PI / 180);

            return radian;
        }

    }
}
