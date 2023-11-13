using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Overload
{
    public enum TileTexture
    {
       None,            //0
       MetalSquare      //1
    }
    public class Tile
    {
        //Fields
        protected Rectangle position;
        protected bool is_collidable;
        protected TileTexture texture;

        //Properties
        public TileTexture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        //get position for the position of the Tile
        public Rectangle Position
        {
            get { return position; }
        }

        //Constructors
        /// <summary>
        /// Parameterized constructor for the Tile class
        /// </summary>
        /// <param name="position">the rectangle position for the Tile class</param>
        /// <param name="is_collidable">the bool of whether or not the tile is collidable</param>
        public Tile(Rectangle position, bool is_collidable)
        {
            this.position = position;
            this.is_collidable = is_collidable;
        }

        //Methods
        /// <summary>
        /// Draw method for the Tile class
        /// </summary>
        public virtual void Draw()
        {
            if (texture == TileTexture.MetalSquare)
            {
                Globals.SB.Draw(
                    Globals.GameTextures["Tileset"],
                    position,
                    Globals.Tiles[TileTexture.MetalSquare],
                    Color.White);
            }
        }

    }
}
