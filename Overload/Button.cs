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
    public class Button
    {

        //Fields:
        private Texture2D buttonNormal;
        private Texture2D buttonHover;
        private Rectangle position;
        private MouseState prevMState;
        private bool isHover;

        //Properties: - NONE -

        //Constructors:
        public Button(Texture2D button, Texture2D hover, Rectangle position)
        {
            this.buttonNormal = button;
            this.buttonHover = hover;
            this.position = position;
            this.isHover = false;
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the button class
        /// </summary>
        public bool Update()
        {
            MouseState mState = Mouse.GetState();

            if (position.Contains(mState.Position) &&
                mState.LeftButton == ButtonState.Pressed &&
                prevMState.LeftButton == ButtonState.Released)
            {
                return true;
            }

            if (position.Contains(mState.Position))
            {
                isHover = true;
            }
            else
            {
                isHover = false;
            }

            prevMState = mState;
            return false;
        }

        /// <summary>
        /// Draw method for the Button class
        /// </summary>
        public void Draw()
        {
            if (isHover)
            {
                Globals.SB.Draw(
                    buttonHover,
                    position,
                    Color.White);
            }
            else
            {
                Globals.SB.Draw(
                    buttonNormal,
                    position,
                    Color.White);
            }
        }


    }
}
