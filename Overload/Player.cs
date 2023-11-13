using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Overload
{
    public delegate List<Rectangle> GetRectangles();

    public class Player : GameObject
    {

        //Fields:
        #region physics fields
        private Vector2 direction;
        private Vector2 velocity;
        private Vector2 acceleration;
        private Vector2 clickLocation;
        private float deltaTime;
        private Rectangle clickRange;
        private bool isGrappling;
        private MouseState prevMState;

        private bool isRevolving;
        private Vector2 center;
        private Vector2 rotationalVelocity;
        #endregion

        private int xAnimation;
        private int animationTimer;
        private int glitchTimer;
        private int glitchAnimation;
        private bool isMoving;
        private bool isFacingLeft;
        private bool isAttacking;
        private int attackTimer;

        private int[] health;
        private int hitCooldown;

        private OverloadMouse mouse;

        public event GetRectangles GetCollidableTiles;

        public event GetRectangles GetEnemyHitboxes;

        //Properties:
        public bool IsGrappling { get { return isGrappling; } }
        public bool IsRevolving { get { return isRevolving; } }

        public int[] Health { get { return health; } }

        //Constructors:
        /// <summary>
        /// Default constructor for the Player class
        /// </summary>
        public Player(Camera camera)
            : base(new Rectangle(500, 10, 50, 50), Globals.GameTextures["PlayerRun"])
        {
            this.direction = new Vector2(1, 1);
            this.velocity = Vector2.Zero;
            this.acceleration = new Vector2(1, 1);
            this.clickLocation = Vector2.Zero;
            this.clickRange = Rectangle.Empty;

            this.isRevolving = false;
            this.center = Vector2.Zero;
            this.rotationalVelocity = Vector2.Zero;

            this.xAnimation = 0;
            this.animationTimer = 10;
            this.glitchAnimation = 0;
            this.glitchTimer = 5;
            this.isMoving = false;
            this.isFacingLeft = false;

            this.isAttacking = false;
            this.hitCooldown = 0;

            this.health = new int[3];
            for (int i = 0; i < health.Length; i++)
            {
                health[i] = 1;
            }

            this.mouse = new OverloadMouse();
            mouse.PlayerTracker += camera.TrackPlayer;
        }

        //Methods:
        /// <summary>
        /// Per frame update method for the player class
        /// </summary>
        public override void Update()
        {
            MouseState mState = Mouse.GetState();
            Point mousePos = mouse.GetMousePosition(this);

            //basic movment with A and D also associated arrow keys
            KeyboardState kbState = Keyboard.GetState();

            if (!isGrappling || !isRevolving || !isAttacking)
            {
                if (kbState.IsKeyDown(Keys.D) ||
                    kbState.IsKeyDown(Keys.Right))
                {
                    position.X += 10;
                    isMoving = true;

                    isFacingLeft = false;
                }
                else if (kbState.IsKeyDown(Keys.A) ||
                         kbState.IsKeyDown(Keys.Left))
                {
                    position.X -= 10;
                    isMoving = true;

                    isFacingLeft = true;
                }
                else
                {
                    isMoving = false;
                }
            }

            #region grapple state logic
            //checking if the player clicked an area
            if (mState.LeftButton == ButtonState.Pressed &&
                prevMState.LeftButton == ButtonState.Released &&
                !isAttacking)
            {
                //Saving that area to a vector variable
                clickLocation = mousePos.ToVector2();
                clickRange = new Rectangle( 
                                (int)clickLocation.X - 50,
                                (int)clickLocation.Y - 50,
                                100, 100);

                //increasing deltaTime
                deltaTime += .08f * 3;

                //reseting values when not being clicked
                velocity.X = 0;
                velocity.Y = 0;
                
                acceleration.X = 1;
                acceleration.Y = 1;

                isGrappling = true;
            }
            else if (mState.LeftButton == ButtonState.Released)
            {
                //reset the click location
                clickLocation = Vector2.Zero;

                isGrappling = false;

                //reseting deltaTime
                deltaTime = 1f;
            }
            #endregion

            //Grappling logic
            if (isGrappling)
            {
                #region setting direction
                //setting the X direction
                if (clickLocation.X >= position.X)
                {
                    direction.X = 1;
                }
                else if (clickLocation.X < position.X)
                {
                    direction.X = -1;
                }

                //setting the Y direction
                if (clickLocation.Y >= position.Y)
                {
                    direction.Y = 1;
                }
                else if (clickLocation.Y < position.Y)
                {
                    direction.Y = -1;
                }
                #endregion

                //increasing acceleration by time
                acceleration *= deltaTime;

                //change velocity by the normal of the click,
                //  the current zero and the direction
                velocity = acceleration *
                           Globals.Normalize(clickLocation) *
                           direction;

                #region setting max velocity
                //creating a maximum velocity
                int maxVelocity = 50;
                if (velocity.X < -maxVelocity)
                {
                    velocity.X = -maxVelocity;
                }
                else if (velocity.X > maxVelocity)
                {
                    velocity.X = maxVelocity;
                }

                if (velocity.Y < -maxVelocity)
                {
                    velocity.Y = -maxVelocity;
                }
                else if (velocity.Y > maxVelocity)
                {
                    velocity.Y = maxVelocity;
                }
                #endregion

                //Moving the character if the player has not reached the click location
                if (!clickRange.Intersects(position))
                {
                    position.Y += (int)velocity.Y;
                    position.X += (int)velocity.X;
                }
                //if the player is there then set grappling equal to false
                else
                {
                    isRevolving = true;
                }
            }
            else
            {
                position.Y -= (int)Globals.Gravity;
            }

            if (isRevolving)
            {
                #region setting direction
                //setting the X direction
                if (clickLocation.X >= position.X)
                {
                    direction.X = 1;
                }
                else if (clickLocation.X < position.X)
                {
                    direction.X = -1;
                }

                //setting the Y direction
                if (clickLocation.Y >= position.Y)
                {
                    direction.Y = 1;
                }
                else if (clickLocation.Y < position.Y)
                {
                    direction.Y = -1;
                }
                #endregion

                rotationalVelocity = new Vector2(
                    Globals.Deg2Rad(velocity.X),
                    Globals.Deg2Rad(velocity.Y));

                position.X += (int)(rotationalVelocity.X * direction.X);
                position.Y += (int)(rotationalVelocity.Y * direction.Y);

                if (mState.LeftButton == ButtonState.Released)
                {
                    isGrappling = false;
                    isRevolving = false;
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


                    if (!isGrappling && !isRevolving)
                    {
                        if (collidable.Contains(bottom))
                        {
                            position.Y = collidable.Y - 51;
                        }
                        else if (collidable.Contains(top))
                        {
                            position.Y = collidable.Y + collidable.Height;
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
            }
            #endregion

            if (kbState.IsKeyDown(Keys.Space) && !isAttacking)
            {
                isAttacking = true;
                attackTimer = 30;

                animationTimer = 10;
                xAnimation = 600;
            }
            else if (isAttacking)
            {
                attackTimer--;
                if (attackTimer <= 0)
                {
                    isAttacking = false;
                }
            }


            if (GetEnemyHitboxes != null)
            {
                List<Rectangle> hitboxes = GetEnemyHitboxes();

                foreach (Rectangle hitbox in hitboxes)
                {
                    if (position.Intersects(hitbox) &&
                        !isGrappling && !isRevolving &&
                        hitCooldown == 0)
                    {
                        for (int i = health.Length - 1; i >= 0; i--)
                        {
                            if (health[i] == 1)
                            {
                                health[i] = 0;
                                hitCooldown = 60;
                                break;
                            }
                        }
                    }
                }

                hitCooldown--;
                if (hitCooldown <= 0)
                {
                    hitCooldown = 0;
                }
            }
            
            prevMState = mState;
        }

        /// <summary>
        /// Draw method for the Player class
        /// </summary>
        public override void Draw()
        {
            Rectangle animationPosition = new Rectangle(
                position.X - 175, 
                position.Y - 200, 
                400, 400);
            Rectangle glitchPosition = new Rectangle(
                position.X - 50, 
                position.Y - 70, 
                150, 150);

            if (isAttacking)
            {
                if (isFacingLeft)
                {
                    Globals.SB.Draw(
                            Globals.GameTextures["PlayerAttack"],
                            animationPosition,
                            new Rectangle(
                                xAnimation,
                                0,
                                200,
                                200),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            SpriteEffects.FlipHorizontally,
                            0f);
                }
                else
                {
                    Globals.SB.Draw(
                            Globals.GameTextures["PlayerAttack"],
                            animationPosition,
                            new Rectangle(
                                xAnimation,
                                0,
                                200,
                                200),
                            Color.White);

                }

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 15;
                    xAnimation += 200;

                    if (xAnimation >= 1200)
                    {
                        xAnimation = 0;
                    }
                }
            }
            else if (isGrappling || isRevolving)
            {
                Globals.SB.Draw(
                        Globals.GameTextures["PlayerJump"],
                        animationPosition,
                        new Rectangle(
                            xAnimation,
                            0,
                            200,
                            200),
                        Color.White);

                animationTimer--;
                if (animationTimer <= 0)
                {
                    animationTimer = 10;
                    xAnimation += 200;

                    if (xAnimation >= 400)
                    {
                        xAnimation = 0;
                    }
                }
            }
            else if (isMoving)
            {
                if (!isFacingLeft)
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(
                            xAnimation,
                            0,
                            200,
                            200),
                        Color.White);
                }
                else
                {
                    Globals.SB.Draw(
                        asset,
                        animationPosition,
                        new Rectangle(
                            xAnimation,
                            0,
                            200,
                            200),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        SpriteEffects.FlipHorizontally,
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
                Globals.SB.Draw(
                    Globals.GameTextures["PlayerIdle"],
                    animationPosition,
                    new Rectangle(
                        xAnimation, 
                        0, 
                        200, 
                        200),
                    Color.White);

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

            //playing the GLITCH animation
            if (isGrappling || isRevolving)
            {
                Globals.SB.Draw(
                    Globals.GameTextures["Glitch"],
                    glitchPosition,
                    new Rectangle(glitchAnimation, 384, 64, 64),
                    Color.White);

                glitchTimer--;
                if (glitchTimer <= 0)
                {
                    glitchTimer = 5;
                    glitchAnimation += 64;
                    if (glitchAnimation >= 576)
                    {
                        glitchAnimation = 0;
                    }
                }
            }


            int tracker = mouse.GetTracker();
            int start = 120;
            if (tracker > 0)
            {
                start += (tracker * 1600);
            }

            for (int i = 0; i < 3; i++)
            {
                if (health[i] == 1)
                {
                    Globals.SB.Draw(
                            Globals.GameTextures["Heart"],
                            new Rectangle(start, 20, 75, 75),
                            Color.White);
                }
                else
                {
                    Globals.SB.Draw(
                            Globals.GameTextures["Heart"],
                            new Rectangle(start, 20, 75, 75),
                            Color.Black);
                }

                start += 60;
            }
        }

        /// <summary>
        /// gives the player's position to any class that may need it
        /// </summary>
        /// <returns>the player's rectangle position</returns>
        public Rectangle GivePosition()
        {
            return position;
        }

        /// <summary>
        /// creates the attack space for the Player's attack
        /// </summary>
        /// <returns>the attack hitbox</returns>
        public Rectangle AttackRange()
        {
            if (isAttacking && xAnimation >= 800)
            {
                if (isFacingLeft)
                {
                    return new Rectangle(
                            position.X - 150,
                            position.Y - 50,
                            200,
                            100);                    
                }
                else
                {
                    return new Rectangle(
                        position.X,
                        position.Y - 50,
                        200,
                        100);
                }
            }

            return Rectangle.Empty;
        }

    }
}
