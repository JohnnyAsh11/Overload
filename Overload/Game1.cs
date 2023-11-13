using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace Overload
{

    public enum GameState
    {
        MainMenu,
        Game,
        WinScreen,
        LoseScreen,
        Controls
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        private Player player;
        private TileManager tManager;
        private Camera camera;
        private EnemyManager eManager;
        private BackgroundManager bManager;

        private Button startButton;
        private Button controlsButton;
        private Button backButton;
        private BackgroundManager menuBackground;
        private SpriteFont arial40;
        private int timer;
        private int fps;
        private int finalTime;

        private GameState gState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //entire program access to the SpriteBatch object
            Globals.SB = new SpriteBatch(GraphicsDevice);

            fps = 0;
            timer = 0;

            Dictionary<string, Texture2D> gameTextures = new Dictionary<string, Texture2D>();
            Dictionary<TileTexture, Rectangle> tiles = new Dictionary<TileTexture, Rectangle>();
            bManager = new BackgroundManager();
            menuBackground = new BackgroundManager();

            //load all textures here
            #region Textures/Tiles
            gameTextures["DebugImage"] = Content.Load<Texture2D>("WhiteDebugImage");
            arial40 = Content.Load<SpriteFont>("Arial-40");

            gameTextures["PlayerDeath"] = Content.Load<Texture2D>("Death");
            gameTextures["PlayerAttack"] = Content.Load<Texture2D>("Attack2");
            gameTextures["PlayerIdle"] = Content.Load<Texture2D>("Idle");
            gameTextures["PlayerRun"] = Content.Load<Texture2D>("Run");
            gameTextures["PlayerHit"] = Content.Load<Texture2D>("Take Hit - white silhouette");
            gameTextures["PlayerJump"] = Content.Load<Texture2D>("Jump");
            gameTextures["EnemyRun"] = Content.Load<Texture2D>("EnemyRun");
            gameTextures["Heart"] = Content.Load<Texture2D>("heart");

            gameTextures["Glitch"] = Content.Load<Texture2D>("01");
            gameTextures["Tileset"] = Content.Load<Texture2D>("OverloadTiles");

            gameTextures["BackLayer"] = Content.Load<Texture2D>("far-buildings");
            gameTextures["MidLayer"] = Content.Load<Texture2D>("back-buildings");
            gameTextures["FrontLayer"] = Content.Load<Texture2D>("foreground");

            gameTextures["Start"] = Content.Load<Texture2D>("start_button_bright");
            gameTextures["StartHover"] = Content.Load<Texture2D>("start_button_dark");
            gameTextures["Controls"] = Content.Load<Texture2D>("controls_button_bright");
            gameTextures["ControlsHover"] = Content.Load<Texture2D>("controls_button_dark");
            gameTextures["Back"] = Content.Load<Texture2D>("back_button_dark");
            gameTextures["BackHover"] = Content.Load<Texture2D>("back_button_light");

            gameTextures["A"] = Content.Load<Texture2D>("a_button");
            gameTextures["D"] = Content.Load<Texture2D>("d_button");
            gameTextures["Space"] = Content.Load<Texture2D>("space_button");
            gameTextures["Left"] = Content.Load<Texture2D>("left_button");
            gameTextures["Right"] = Content.Load<Texture2D>("right_button");

            gameTextures["Logo"] = Content.Load<Texture2D>("overload_text_orange_thicker_cloud");
            gameTextures["Lose"] = Content.Load<Texture2D>("you_lose");
            gameTextures["Win"] = Content.Load<Texture2D>("you_win");

            tiles[TileTexture.MetalSquare] = new Rectangle(0, 606, 96, 96);
            #endregion

            //entire program access to the game's textures
            Globals.GameTextures = gameTextures;
            Globals.Tiles = tiles;

            //193 x 50
            backButton = new Button(
                Globals.GameTextures["Back"],
                Globals.GameTextures["BackHover"],
                new Rectangle(20, 20, 193, 50));

            startButton = new Button(
                Globals.GameTextures["Start"],
                Globals.GameTextures["StartHover"],
                new Rectangle(680, 600, 274, 75));

            controlsButton = new Button(
                Globals.GameTextures["Controls"],
                Globals.GameTextures["ControlsHover"],
                new Rectangle(680, 700, 271, 75));

            bManager.AddLayer(new Layer(Globals.GameTextures["BackLayer"], 0.1f, 0.5f));
            bManager.AddLayer(new Layer(Globals.GameTextures["MidLayer"], 0.2f, 1f));
            bManager.AddLayer(new Layer(Globals.GameTextures["FrontLayer"], 0.3f, 1.5f));
            menuBackground.AddLayer(new Layer(Globals.GameTextures["BackLayer"], 0.1f, 0.5f));
            menuBackground.AddLayer(new Layer(Globals.GameTextures["MidLayer"], 0.2f, 1f));
            menuBackground.AddLayer(new Layer(Globals.GameTextures["FrontLayer"], 0.3f, 1.5f));

            camera = new Camera();
            player = new Player(camera);
            tManager = new TileManager("../../../Level.txt","../../../Textures.txt");
            eManager = new EnemyManager("../../../Enemies.txt", player, tManager);

            player.GetCollidableTiles += tManager.GetCollidableTiles;
            bManager.GetPlayerPosition += player.GivePosition;
            player.GetEnemyHitboxes += eManager.EnemyDamage;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            switch (gState)
            {
                case GameState.MainMenu:

                    if (startButton.Update())
                    {
                        gState = GameState.Game;
                    }

                    if (controlsButton.Update())
                    {
                        gState = GameState.Controls;
                    }

                    menuBackground.MenuUpdate(gameTime);

                    break;
                case GameState.Game:

                    //updating all objects relevant to the game 
                    bManager.Update(gameTime);
                    player.Update();
                    camera.FollowObject(player);
                    eManager.Update();

                    if (player.Health[0] == 0 &&
                        player.Health[1] == 0 &&
                        player.Health[2] == 0)
                    {
                        gState = GameState.LoseScreen;
                        finalTime = timer;
                    }
                    else if (eManager.NumOfEnemies == 0)
                    {
                        gState = GameState.WinScreen;
                        finalTime = timer;
                    }

                    break;
                case GameState.WinScreen:

                    menuBackground.MenuUpdate(gameTime);

                    if (backButton.Update())
                    {
                        gState = GameState.MainMenu;
                        Reset();
                    }

                    break;
                case GameState.LoseScreen:

                    menuBackground.MenuUpdate(gameTime);

                    if (backButton.Update())
                    {
                        gState = GameState.MainMenu;
                        Reset();
                    }

                    break;
                case GameState.Controls:

                    menuBackground.MenuUpdate(gameTime);

                    if (backButton.Update())
                    {
                        gState = GameState.MainMenu;
                    }

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (gState == GameState.Game)
            {
                OverloadMouse mState = new OverloadMouse();
                mState.PlayerTracker += camera.TrackPlayer;
                Point mouse = mState.GetMousePosition(player);

                bManager.Draw();

                Globals.SB.Begin(transformMatrix: camera.Transform);
                tManager.Draw();
                player.Draw();
                eManager.Draw();

                //drawing the mouse grapple
                if (player.IsGrappling ||
                    player.IsRevolving)
                    Globals.SB.Draw(
                        Globals.GameTextures["Glitch"],
                        new Rectangle(mouse.X - 70, mouse.Y - 65, 150, 150),
                        new Rectangle(0, 0, 64, 64),
                        Color.Yellow);
                Globals.SB.End();

                fps++;
                if (fps >= 60)
                {
                    timer += 1;
                    fps = 0;
                }

                Globals.SB.Begin();
                Globals.SB.DrawString(
                    arial40,
                    $"{timer}",
                    new Vector2(10, 10),
                    Color.White);
                Globals.SB.End();
            }
            else if (gState == GameState.MainMenu)
            {
                menuBackground.Draw();

                Globals.SB.Begin();

                Globals.SB.Draw(
                    Globals.GameTextures["Logo"],
                    new Rectangle(400, 100, 825, 429),
                    Color.White);

                startButton.Draw();
                controlsButton.Draw();
                Globals.SB.End();
            }
            else if (gState == GameState.LoseScreen)
            {
                menuBackground.Draw();

                Globals.SB.Begin();
                backButton.Draw();

                Globals.SB.Draw(
                    Globals.GameTextures["Lose"],
                    new Rectangle(400, 100, 825, 429),
                    Color.White);

                Globals.SB.DrawString(
                    arial40,
                    $"Your Final Time: {finalTime / 60} minutes and {finalTime % 60} seconds",
                    new Vector2(375, 550),
                    Color.White);
                Globals.SB.End();
            }
            else if (gState == GameState.WinScreen)
            {
                menuBackground.Draw();

                Globals.SB.Begin();
                backButton.Draw();

                Globals.SB.Draw(
                    Globals.GameTextures["Win"],
                    new Rectangle(400, 100, 825, 429),
                    Color.White);

                Globals.SB.DrawString(
                    arial40,
                    $"Your Final Time: {finalTime / 60} minutes and {finalTime % 60} seconds",
                    new Vector2(375, 550),
                    Color.White);
                Globals.SB.End();
            }
            else if (gState == GameState.Controls)
            {
                menuBackground.Draw();

                Globals.SB.Begin();
                backButton.Draw();

                Globals.SB.Draw(
                    Globals.GameTextures["A"],
                    new Rectangle(700, 400, 66, 50),
                    Color.White);
                Globals.SB.Draw(
                    Globals.GameTextures["D"],
                    new Rectangle(800, 400, 67, 50),
                    Color.White);
                Globals.SB.Draw(
                    Globals.GameTextures["Space"],
                    new Rectangle(680, 500, 205, 50),
                    Color.White);
                Globals.SB.Draw(
                    Globals.GameTextures["Left"],
                    new Rectangle(700, 600, 66, 50),
                    Color.White);
                Globals.SB.Draw(
                    Globals.GameTextures["Right"],
                    new Rectangle(800, 600, 66, 50),
                    Color.White);

                Globals.SB.End();
            }
            
            base.Draw(gameTime);
        }

        private void Reset()
        {
            camera = new Camera();
            player = new Player(camera);
            tManager = new TileManager("../../../Level.txt", "../../../Textures.txt");
            eManager = new EnemyManager("../../../Enemies.txt", player, tManager);

            player.GetCollidableTiles += tManager.GetCollidableTiles;
            bManager.GetPlayerPosition += player.GivePosition;
            player.GetEnemyHitboxes += eManager.EnemyDamage;

            fps = 0;
            finalTime = 0;
            timer = 0;
        }

    }
}