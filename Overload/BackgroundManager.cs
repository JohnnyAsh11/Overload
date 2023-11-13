using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overload
{

    public delegate Rectangle GetPosition();

    public class BackgroundManager
    {

        //Fields:
        private ParallaxManager pManager;
        private float speed;
        private float movement;
        private KeyboardState kbState;
        private KeyboardState prevKbState;
        private Point prevLocation;

        public event GetPosition GetPlayerPosition;

        //Properties: - NONE -

        //Constructors:
        public BackgroundManager()
        {
            pManager = new ParallaxManager();
            speed = 200f;
        }

        //Methods:
        /// <summary>
        /// used to add layers into the ParallaxManager Field
        /// </summary>
        /// <param name="layer">layer being added</param>
        public void AddLayer(Layer layer)
        {
            pManager.AddLayer(layer);
        }

        /// <summary>
        /// Per frame update method for the GameManager class
        /// </summary>
        /// <param name="gameTime">GameTime obj used to track elapsed time</param>
        public void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();

            if (GetPlayerPosition != null)
            {
                Rectangle playerRect = GetPlayerPosition();

                if (playerRect.Center.X > prevLocation.X)
                {
                    movement = -speed;
                }
                else if (playerRect.Center.X < prevLocation.X)
                {
                    movement = speed;
                }
                else
                {
                    movement = 0;
                }

                prevLocation = playerRect.Center;
            }

            pManager.Update(movement, gameTime);
        }

        /// <summary>
        /// Update method for the menu Parallax
        /// </summary>
        /// <param name="gameTime">GameTime used to track the total elapsed time</param>
        public void MenuUpdate(GameTime gameTime)
        {
            kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Space) &&
                prevKbState.IsKeyUp(Keys.Space))
            {
                if (movement < 0)
                {
                    movement = speed;
                }
                else
                {
                    movement = -speed;
                }
            }
            else if (movement == 0)
            {
                movement = -speed;
            }

            prevKbState = kbState;
            pManager.Update(movement, gameTime);
        }

        /// <summary>
        /// standard draw method for the GameManager class
        /// </summary>
        public void Draw()
        {
            pManager.Draw();
        }

        /// <summary>
        /// resets all layers in the parallax
        /// </summary>
        public void ResetLayers()
        {
            pManager.ResetLayers();
        }

    }
}
