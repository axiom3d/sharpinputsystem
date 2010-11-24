using System;
using System.Collections.Generic;
using Xna = Microsoft.Xna.Framework;
using XFG = Microsoft.Xna.Framework.Graphics;
using XInput = Microsoft.Xna.Framework.Input;

using log4net;

namespace SharpInputSystem.Test.Console
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main( string[] args )
        {
            using ( Game1 game = new Game1() )
            {
                game.Run();
            }
        }
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Xna.Game
    {
        Xna.GraphicsDeviceManager graphics;
        XFG.SpriteBatch spriteBatch;

        XFG.Model castle;
        Xna.Vector3 modelPosition = Xna.Vector3.Zero;
        float modelRotation = 0.0f;

        private static readonly ILog log = LogManager.GetLogger( typeof( Game1 ) );

        public Game1()
        {
            graphics = new Xna.GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Create the services
            InputManager inputManager = new InputManager(this);
            this.Services.AddService(typeof(IInputManagerService), inputManager);
            this.Components.Add(inputManager);

            CameraManager cameraManagerService = new CameraManager(this);
            this.Services.AddService(typeof(ICameraManagerService), cameraManagerService);
            this.Components.Add(cameraManagerService);


            // Initialize the Movement System
            //MovementManager movement = new MovementManager( this, main );
            //this.Components.Add( movement );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new XFG.SpriteBatch( GraphicsDevice );
            castle = this.Content.Load<XFG.Model>( "Models\\castle" );
            // TODO: use this.Content to load your game content here

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( Xna.GameTime gameTime )
        {
            // Allows the game to exit
            if ( XInput.GamePad.GetState( Xna.PlayerIndex.One ).Buttons.Back == XInput.ButtonState.Pressed )
                this.Exit();

            // TODO: Add your update logic here

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( Xna.GameTime gameTime )
        {
            ICameraManagerService cameraManagerService = (ICameraManagerService)this.Services.GetService(typeof(ICameraManagerService));

            graphics.GraphicsDevice.Clear( XFG.Color.CornflowerBlue );

            // Copy any parent transforms.
            Xna.Matrix[] transforms = new Xna.Matrix[ castle.Bones.Count ];
            castle.CopyAbsoluteBoneTransformsTo( transforms );

            //// Draw the model. A model can have multiple meshes, so loop.
            foreach ( XFG.ModelMesh mesh in castle.Meshes )
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach ( XFG.BasicEffect effect in mesh.Effects )
                {
                        effect.World = Xna.Matrix.CreateRotationY(this.modelRotation) * Xna.Matrix.CreateTranslation(this.modelPosition);
                        effect.View = cameraManagerService.Camera.View;
                        effect.Projection = cameraManagerService.Camera.Projection;
                        effect.EnableDefaultLighting();
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw( gameTime );
        }
    }
}
