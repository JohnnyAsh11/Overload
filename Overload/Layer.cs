using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overload
{
    public class Layer
    {

        //Fields:
        private Texture2D asset;
        private Rectangle position1;
        private Rectangle position2;
        private Point point1;
        private Point point2;
        private Point screenSize;
        private float depth;
        private float moveScale;
        private bool isFullscreen;

        private Point originalPosition;

        //Properties:
        public bool IsFullscreen { get { return isFullscreen; } }

        //Constructors:
        /// <summary>
        /// parameterized constructor for the Layer class
        /// </summary>
        /// <param name="asset">Texture asset for this background layer</param>
        /// <param name="depth">Where the layer is on the Z-axis</param>
        /// <param name="moveScale">The scale of how quickly the layer moves</param>
        public Layer(Texture2D asset, float depth, float moveScale)
        {
            this.asset = asset;
            this.moveScale = moveScale;
            this.depth = depth;
            this.isFullscreen = true;

            this.point1 = Point.Zero;
            this.point2 = Point.Zero;
            this.screenSize = new Point(1600, 960);
            this.position1 = new Rectangle(point1, screenSize);
            this.position2 = new Rectangle(point2, screenSize);
        }

        //Methods:
        /// <summary>
        /// per frame update method for individual layers of the parallax background
        /// </summary>
        /// <param name="movement">How much the player has moved</param>
        /// <param name="gameTime">GameTime obj used to track elapsed time</param>
        public void Update(float movement, GameTime gameTime)
        {
            //multiplying the amount of player movement by the
            //movement scaler and the total time passed
            if (isFullscreen)
            {
                position1.X += (int)(movement * moveScale * (float)gameTime.ElapsedGameTime.TotalSeconds);
                position1.X %= position1.Width;

                if (position1.X >= 0)
                {
                    position2.X = position1.X - position1.Width;
                }
                else
                {
                    position2.X = position1.X + position1.Width;
                }
            }
            else
            {
                position1.X += (int)(movement * moveScale * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        /// <summary>
        /// resets the none fullscreen layers back to their original positions
        /// </summary>
        public void ResetPosition()
        {
            if (!isFullscreen)
            {
                position1.X = originalPosition.X;
                position1.Y = originalPosition.Y;
            }
        }

        /// <summary>
        /// Standard Draw method for the Layer class
        /// </summary>
        public void Draw()
        {
            if (isFullscreen)
            {
                Globals.SB.Draw(
                    asset,
                    position1,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    depth);

                Globals.SB.Draw(
                    asset,
                    position2,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    depth);
            }
            else
            {
                Globals.SB.Draw(
                    asset,
                    position1,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    depth);
            }
        }

    }
}
