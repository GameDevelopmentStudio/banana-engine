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

using bEngine.Helpers.Transitions;

namespace bEngine
{
    public class bGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public static bInput input = new bInput();
        public SpriteFont gameFont;
        public Color bgColor;

        // Time flow
        public double millisecondsPerFrame = 17;
        protected double timeSinceLastUpdate = 0;

        // Resolution
        protected int width, height;
        public int getWith() { return width; }
        public int getHeight() { return height; }
        protected uint horizontalZoom, verticalZoom;

        // Gamestate
        public bGameState world;
        public bool requestedWorldChange = false;
        public Transition requestedTransition = null;
        public bGameState nextWorld = null;
        // Gamestate transitions
        protected Transition gamestateTransition = null;

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

            bgColor = Color.CornflowerBlue;
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
            // Handle gamestate change
            bool newWorldThisStep = false;
            // Change if requested
            if (requestedWorldChange || gamestateTransition != null)
            {
                // Init new gamestate transition
                if (gamestateTransition == null)
                {
                    gamestateTransition = requestedTransition;
                }
                else if (gamestateTransition.expectingWorldChange())
                {
                    actuallyChangeWorld(nextWorld);
                    newWorldThisStep = true;
                    // Notify
                    gamestateTransition.worldChanged();
                }
                else if (gamestateTransition.finished())
                {
                    gamestateTransition = null;
                }
                else
                {
                    gamestateTransition.update();
                }

                if (!newWorldThisStep)
                    return;
            }

            // Control time flow (30fps)
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
            // Update if timer allows or a the gamestate is new and needs to perform
            // its inital step
            if (!newWorldThisStep && (timeSinceLastUpdate < millisecondsPerFrame))
                return;
            timeSinceLastUpdate = 0;

            // Update inputstate
            input.update();

            // Game scope relevant step processing
            // by default, just debug keys
            update(gameTime);
			
            // Update current world state (if available)
            if (world != null)
                world._update(gameTime);

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

            GraphicsDevice.Clear(bgColor);

            render(gameTime);

            // Render world if available
            if (world != null)
                world.render(gameTime, spriteBatch, matrix);

            // Transition
            if (gamestateTransition != null)
                gamestateTransition.render(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public virtual Transition defaultTransition()
        {
            return new NilTransition(this);
        }

        public void changeWorld(bGameState newWorld)
        {
            changeWorld(newWorld, defaultTransition());
        }

        public void changeWorld(bGameState newWorld, Transition transition)
        {
            if (world == null)
                actuallyChangeWorld(newWorld);
            else
            {
                nextWorld = newWorld;
                requestedWorldChange = true;
                requestedTransition = transition;
            }
        }

        public void actuallyChangeWorld(bGameState newWorld)
        {
            if (newWorld != null)
            {
                if (world != null)
                    world.end();

                world = newWorld;
                world.game = this;

                newWorld.init();

                requestedWorldChange = false;
                nextWorld = null;
            }
            else
            {
                Console.WriteLine("An invalid attemp to change world occured:");
                Console.WriteLine("Request: " + (requestedWorldChange ? "issued " : "not issued; ") +
                                  "Valid instance: " + ((nextWorld != null) ? "yes" : "no"));
            }
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
