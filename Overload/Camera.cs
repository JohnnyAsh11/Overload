using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overload
{
    public class Camera
    {

        //Fields:
        private Matrix transformationMatrix;
        private Vector2 screenBounds;

        private int playerX;
        private int playerTracker;

        //Properties:
        public Matrix Transform { get { return transformationMatrix; } }

        //Constructors:
        /// <summary>
        /// Default constructor for the Camera class
        /// </summary>
        public Camera()
        {
            transformationMatrix = new Matrix();
            screenBounds = new Vector2(1600, 1000);

            playerX = 0;
        }

        //Methods:
        /// <summary>
        /// Updates the 
        /// </summary>
        /// <param name="gameObj"></param>
        public void FollowObject(GameObject gameObj)
        {
            if (playerX + 1600 < gameObj.Position.X)
            {
                playerX += 1600;

                playerTracker++;
            }
            else if (playerX > gameObj.Position.X)
            {
                playerX -= 1600;

                playerTracker--;
            }

            Matrix position = Matrix.CreateTranslation(
                -playerX,
                0,
                0);

            transformationMatrix = position;
        }

        /// <summary>
        /// method to track down the player's position
        /// </summary>
        /// <returns>the number of times the screen translated</returns>
        public int TrackPlayer()
        {
            return playerTracker;
        }

    }
}
