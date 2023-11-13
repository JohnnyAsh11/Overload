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
    /// Abstract GameObject for all objects displayed in the game
    /// </summary>
    public abstract class GameObject
    {

        //Fields:
        protected Rectangle position;
        protected Texture2D asset;
        protected Enum TileTexture;

        //Properties:
        //get/set property for the GO's position
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        //get/set property for the GO's position X
        public int X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        //get/set property for the GO's position Y
        public int Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        //Constructors:
        /// <summary>
        /// parameterized constructor for the GameObject class
        /// </summary>
        /// <param name="position">Rectangle position for the GameObject</param>
        /// <param name="asset">Texture asset for the GameObject</param>
        public GameObject(Rectangle position, Texture2D asset)
        {
            this.position = position;
            this.asset = asset;
        }

        //Methods:
        /// <summary>
        /// Draw method for the GO class
        /// </summary>
        public virtual void Draw()
        {
            Globals.SB.Begin();
            Globals.SB.Draw(
                asset,
                position,
                Color.White);
            Globals.SB.End();
        }

        /// <summary>
        /// per frame update method for GO's
        /// </summary>
        public abstract void Update();


    }
}
