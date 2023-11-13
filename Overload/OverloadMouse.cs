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
    public delegate int GetPlayerTracker();

    public class OverloadMouse
    {

        //Fields:
        private MouseState mState;
        private Point screen;

        public event GetPlayerTracker PlayerTracker;

        //Properties: - NONE -

        //Constructors:
        public OverloadMouse()
        {
            this.screen = new Point(1600, 1000);
        }

        //Methods:
        /// <summary>
        /// tracks an object to its exact screen location for the mouse
        /// </summary>
        /// <param name="trackedObj">The GameObject being tracked</param>
        /// <returns>the updated mouse Point</returns>
        public Point GetMousePosition(GameObject trackedObj)
        {
            mState = Mouse.GetState();
            Point mouse = mState.Position;

            int playerTracker = 0;
            if (PlayerTracker != null)
            {
                playerTracker = PlayerTracker();
            }

            mouse.X = mouse.X + (screen.X * playerTracker);

            return mouse;
        }
        public int GetTracker()
        {
            int playerTracker = 0;
            if (PlayerTracker != null)
            {
                playerTracker = PlayerTracker();
            }

            return playerTracker;
        }

    }
}
