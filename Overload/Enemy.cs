using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Overload
{
    public delegate Rectangle GetPlayerAttack();

    public class Enemy : GameObject
    {

        //Fields:
        private int movementSpeed;

        private int leftDistance;
        private int rightDistance;
        private bool isMovingLeft;
        private bool isDead;

        private int xAnimation;
        private int animationTimer;

        private int deathAnimationTimer;
        private int xDeathAnimation;
        private bool isDeathDone;

        public event GetRectangles GetCollidableTiles;

        public event GetPlayerAttack GetAttack;

        //Properties: 
        public bool IsDeathDone { get { return isDeathDone; } }
        public bool IsDead { get { return isDead; } }

        //Constuctors:
        public Enemy(Rectangle position, int left, int right)
            : base(position, Globals.GameTextures["EnemyRun"])
        {
            this.movementSpeed = 10;

            this.leftDistance = position.X - left;
            this.rightDistance = position.X + right;
            this.isMovingLeft = false;
            this.isDead = false;

            this.xDeathAnimation = 0;
            this.deathAnimationTimer = 5;
            this.isDeathDone = false;

            this.xAnimation = 0;
            this.animationTimer = 10;
        }

        //Methods:
        /// <summary>
        /// per frame update method for the Enemy class
        /// </summary>
        public override void Update()
        {
            if (!isDead)
            {
                if (isMovingLeft)
                {
                    position.X -= movementSpeed;

                    if (position.X <= leftDistance)
                    {
                        isMovingLeft = false;
                    }
                }
                else
                {
                    position.X += movementSpeed;

                    if (position.X >= rightDistance)
                    {
                        isMovingLeft = true;
                    }
                }
            }
            position.Y -= (int)Globals.Gravity;

            if (GetAttack != null)
            {
                Rectangle playerAttack = GetAttack();

                if (position.Intersects(playerAttack))
                {
                    isDead = true;
                }
            }

            #region Collisions
            if (GetCollidableTiles != null)
            {
                List<Rectangle> collidables = GetCollidableTiles();

                Point top = new Point(
                        position.X + (position.Width / 2),
                        position.Y);
                Point bottom = new Point(
                    position.X + (position.Width / 2),
                    position.Y + position.Height);

                Point left = new Point(
                    position.X,
                    position.Y + (position.Height / 2));
                Point right = new Point(
                    position.X + position.Width,
                    position.Y + (position.Height / 2));

                foreach (Rectangle collidable in collidables)
                {
                    if (collidable.Contains(top))
                    {
                        position.Y = collidable.Y + collidable.Height;
                    }
                    else if (collidable.Contains(bottom))
                    {
                        position.Y = collidable.Y - 51;
                    }
                    else if (collidable.Contains(left))
                    {
                        position.X = collidable.X + collidable.Width;
                    }
                    else if (collidable.Contains(right))
                    {
                        position.X = collidable.X - 50;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Draw method for the Enemy class
        /// </summary>
        public override void Draw()
        {
            Rectangle animationPosition = new Rectangle(
                position.X - 175,
                position.Y - 200,
                400, 400);

            if (!isDead)
            {
                if (isMovingLeft)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, 200, 200),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
                        0f);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(xAnimation, 0, 200, 200),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0f);
                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 10;
                    xAnimation += 200;
                    if (xAnimation >= 1600)
                    {
                        xAnimation = 0;
                    }
                }

            }
            else
            {
                Rectangle glitchPosition = new Rectangle(
                position.X - 50,
                position.Y - 70,
                150, 150);

                Globals.SB.Draw(
                    Globals.GameTextures["Glitch"],
                    glitchPosition,
                    new Rectangle(xDeathAnimation, 320, 64, 64),
                    Color.White);

                deathAnimationTimer--;
                if (deathAnimationTimer <= 0)
                {
                    xDeathAnimation += 64;
                    deathAnimationTimer = 5;

                    if (xDeathAnimation == 576)
                    {
                        isDeathDone = true;
                    }
                }
            }
        }

    }
}
