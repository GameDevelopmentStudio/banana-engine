using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace bEngine
{
    public class bGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public static bInput input = new bInput();
        public SpriteFont gameFont;

        // Time flow
        public double millisecondsPerFrame = 17;
        protected double timeSinceLastUpdate = 0;

        // Resolution
        protected int width, height;
        protected uint horizontalZoom, verticalZoom;

        // Gamestate
        public bGameState world;

        public bGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            initSettings();

            Resolution.Init(ref graphics, width, height);
            Resolution.SetVirtualResolution(width, height);
            Resolution.SetResolution((int) (width * horizontalZoom), (int) (height * verticalZoom), false);
        }

        protected virtual void initSettings()
        {
            // default resolution
            horizontalZoom = 3;
            verticalZoom = 3;
            width = 320;
            height = 240;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            input.registerKey("DEBUG", Buttons.Y);
            input.registerKey("DEBUG", Keys.H);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load game-wide available sprite fonts
            gameFont = Content.Load<SpriteFont>("font0");

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
        }


        virtual public void update(GameTime gameTime)
        {
            // Allows the game to exit
            if (input.pressed(Buttons.Back) || input.pressed(Keys.Escape))
                this.Exit();
            // Handles full screen mode
            else if (input.pressed(Keys.F4))
            {
                int rw, rh;
                if (graphics.IsFullScreen)
                {
                    rw = width * (int)horizontalZoom;
                    rh = height * (int)verticalZoom;
                }
                else
                {
                    rw = GraphicsDevice.DisplayMode.Width;
                    rh = GraphicsDevice.DisplayMode.Height;
                }

                Resolution.SetResolution(rw, rh, !graphics.IsFullScreen);
            }
            // Increases milliseconds per frame (slows down game)
            else if (input.pressed(Keys.Add))
            {
                millisecondsPerFrame += 5.0;
            }
            // Decreases milliseconds per frame (speeds up game)
            else if (input.pressed(Keys.Subtract))
            {
                millisecondsPerFrame -= 5.0;
            }
            // Takes a screenshot
            else if (input.pressed(Keys.F12))
            {
                screenshot();
            }

            // Switches between debug on/off state
            if (input.pressed("DEBUG"))
                bConfig.DEBUG = !bConfig.DEBUG;
        }

        protected override void Update(GameTime gameTime)
        {
            // Control time flow (30fps)
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceLastUpdate < millisecondsPerFrame)
                return;
            timeSinceLastUpdate = 0;

            // Update inputstate
            input.update();

            // Game scope relevant step processing
            // by default, just debug keys
            update(gameTime);
			
            // Update current world state (if available)
            if (world != null)
                world.update(gameTime);

            // We shall let XNA have some fun, shan't we?
            base.Update(gameTime);
        }

        virtual public void render(GameTime gameTime)
        {
        }

        protected override void Draw(GameTime gameTime)
        {
            Resolution.BeginDraw();
            // Generate resolution render matrix 
            Matrix matrix = Resolution.getTransformationMatrix();

            spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    RasterizerState.CullCounterClockwise,
                    null,
                    matrix);

            Color bgColor = Color.CornflowerBlue;
            GraphicsDevice.Clear(bgColor);

            render(gameTime);

            // Render world if available
            if (world != null)
                world.render(gameTime, spriteBatch, matrix);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void changeWorld(bGameState newWorld)
        {
            if (world != null)
                world.end();

            world = newWorld;
            world.game = this;

            newWorld.init();
        }

        // Screenshot generation function
        public int count = 0;
        public void screenshot()
        {
            count += 1;
            DateTime debugDate = DateTime.Now;
            string filename = debugDate.Year + "" + debugDate.Month + "" + debugDate.Day + "." +
                              debugDate.Hour + "" + debugDate.Minute + "" + debugDate.Second + ".";
            filename += count.ToString();

            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //force a frame to be drawn (otherwise back buffer is empty) 
            Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            //save to disk 
            Stream stream = File.OpenWrite(filename + ".png");

            texture.SaveAsPng(stream, w, h);
            stream.Dispose();

            texture.Dispose();
        }
    }
}
