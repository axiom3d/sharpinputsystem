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
        SIS.Mouse _m;

        List<SIS.Joystick> _joys = new List<SIS.Joystick>();
        List<SIS.ForceFeedback> _ff = new List<SIS.ForceFeedback>();

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

            log.Info( String.Format( "SIS Version : {0}", _inputManager.Version ) );
            log.Info( String.Format( "Platform : {0}", _inputManager.InputSystemName ) );
            log.Info( String.Format( "Number of Mice : {0}", _inputManager.DeviceCount<SIS.Mouse>() ) );
            log.Info( String.Format( "Number of Keyboards : {0}", _inputManager.DeviceCount<SIS.Keyboard>() ) );
            log.Info( String.Format( "Number of GamePads: {0}", _inputManager.DeviceCount<SIS.Joystick>() ) );

            bool buffered = false;

            if ( _inputManager.DeviceCount<SIS.Keyboard>() > 0 )
            {
                _kb = _inputManager.CreateInputObject<SIS.Keyboard>( buffered, "" );
                log.Info( String.Format( "Created {0}buffered keyboard", buffered ? "" : "un" ) );
            }

            if ( _inputManager.DeviceCount<SIS.Mouse>() > 0 )
            {
                //_m = _inputManager.CreateInputObject<SIS.Mouse>( buffered, "" );
                //log.Info( String.Format( "Created {0}buffered mouse", buffered ? "" : "un" ) );
                ////_m.EventListener = _handler;

                //MouseState ms = _m.MouseState;
                //ms.Width = 100;
                //ms.Height = 100;
            }

            ////This demo only uses at max 4 joys
            int numSticks = _inputManager.DeviceCount<SIS.Joystick>();
            if( numSticks > 4 )	numSticks = 4;

            for ( int i = 0; i < numSticks; ++i )
            {
                _joys.Insert( i, _inputManager.CreateInputObject<SIS.Joystick>( true, "" ) );
                //_joys[ i ].EventListener = _handler;

                _ff.Insert( i, (SIS.ForceFeedback)_joys[ i ].QueryInterface<SIS.ForceFeedback>() );
                if ( _ff[ i ] != null )
                {
                    log.Info( String.Format( "Created buffered joystick with ForceFeedback support." ) );
                    //Dump out all the supported effects:
                    //        const ForceFeedback::SupportedEffectList &list = g_ff[i]->getSupportedEffects();
                    //        ForceFeedback::SupportedEffectList::const_iterator i = list.begin(),
                    //            e = list.end();
                    //        for( ; i != e; ++i)
                    //            std::cout << "Force =  " << i->first << " Type = " << i->second << std::endl;
                }
                else
                    log.Info( String.Format( "Created buffered joystick. **without** FF support" ) );
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

            if ( _kb != null )
            {
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
            }

            if ( _joys.Count > 0 ) 
            {
                SIS.Joystick joy = _joys[ 0 ];
                joy.Capture();

                if ( joy.JoystickState.IsButtonDown( 0 ) )
                    this.Game.Exit();

                Xna.Vector3 forward = Xna.Vector3.Normalize( new Xna.Vector3( (float)Math.Sin( -angle.Y ), (float)Math.Sin( angle.X ), (float)Math.Cos( -angle.Y ) ) );

                position -= forward * joy.JoystickState.Axis[ 0 ].Absolute * delta;

            }

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
