using System;
using System.Collections.Generic;
using System.Text;

using Xna = Microsoft.Xna.Framework;

using SIS = SharpInputSystem;

namespace SharpInputSystem.Test.Console
{
    public class Camera : Xna.DrawableGameComponent
    {
        #region Fields and Properties

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof( Camera ) );

        private float speed = 250f;

        SIS.InputManager _inputManager;
        SIS.Keyboard _kb;

        private static Camera activeCamera = null;
        public static Camera ActiveCamera
        {
            get
            {
                return activeCamera;
            }
            set
            {
                activeCamera = value;
            }
        }

        private Xna.Matrix projection = Xna.Matrix.Identity;
        public Xna.Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        private Xna.Matrix view = Xna.Matrix.Identity;
        public Xna.Matrix View
        {
            get
            {
                return view;
            }
        }

        private Xna.Vector3 position = new Xna.Vector3( 0, 0, 1000 );
        public Xna.Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        private Xna.Vector3 angle = new Xna.Vector3();
        public Xna.Vector3 Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
            }
        }

        #endregion Fields and Properties

        #region Construction and Destruction

        public Camera( Xna.Game game )
            : base( game )
        {
            if ( ActiveCamera == null )
                ActiveCamera = this;
        }

        #endregion Construction and Destruction

        #region Methods
        #endregion Methods

        #region DrawableGameComponent Implementation

        public override void Initialize()
        {
            _inputManager = SIS.InputManager.CreateInputSystem( this.Game.Window.Handle );

            bool buffered = false;

            if ( _inputManager.DeviceCount<Keyboard>() > 0 )
            {
                _kb = _inputManager.CreateInputObject<Keyboard>( buffered, "" );
                log.Info( String.Format( "Created {0}buffered keyboard", buffered ? "" : "un" ) );
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            float ratio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            projection = Xna.Matrix.CreatePerspectiveFieldOfView( Xna.MathHelper.PiOver4, ratio, 10, 10000 );
            //
            base.LoadContent();
        }

        public override void Update( Xna.GameTime gameTime )
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _kb.Capture();

            // Allows the game to exit
            if ( _kb.IsKeyDown( KeyCode.Key_ESCAPE ) )
                this.Game.Exit();

            //XInput.MouseState mouse = XInput.Mouse.GetState();

            //angle.X += Xna.MathHelper.ToRadians( ( mouse.Y - centerY ) * turnSpeed * 0.01f ); // pitch
            //angle.Y += Xna.MathHelper.ToRadians( ( mouse.X - centerX ) * turnSpeed * 0.01f ); // yaw

            Xna.Vector3 forward = Xna.Vector3.Normalize( new Xna.Vector3( (float)Math.Sin( -angle.Y ), (float)Math.Sin( angle.X ), (float)Math.Cos( -angle.Y ) ) );
            Xna.Vector3 left = Xna.Vector3.Normalize( new Xna.Vector3( (float)Math.Cos( angle.Y ), 0f, (float)Math.Sin( angle.Y ) ) );

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_UP ) )
                position -= forward * speed * delta;

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_DOWN ) )
                position += forward * speed * delta;

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_RIGHT ) )
                position -= left * speed * delta;

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_LEFT ) )
                position += left * speed * delta;

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_PGUP ) )
                position += Xna.Vector3.Down * speed * delta;

            if ( _kb.IsKeyDown( SIS.KeyCode.Key_PGDOWN ) )
                position += Xna.Vector3.Up * speed * delta;

            view = Xna.Matrix.Identity;
            view *= Xna.Matrix.CreateTranslation( -position );
            view *= Xna.Matrix.CreateRotationZ( angle.Z );
            view *= Xna.Matrix.CreateRotationY( angle.Y );
            view *= Xna.Matrix.CreateRotationX( angle.X );

            base.Update( gameTime );
        }

        #endregion DrawableGameComponent Implementation
    }
}
