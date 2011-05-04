using System;
using System.Collections.Generic;
using System.Text;

using MXF = Microsoft.Xna.Framework;

using SIS = SharpInputSystem;
using Microsoft.Xna.Framework;

namespace SharpInputSystem.Test.Console
{
    public class Camera
    {
        protected float FOV = MXF.MathHelper.Pi / 3;
        protected float aspectRatio = 1;
        protected float nearClip = 1.0f;
        protected float farClip = 10000000.0f;

        protected MXF.Quaternion cameraRotation;
        protected MXF.Vector3 cameraPosition;

        public MXF.Quaternion Rotation
        {
            get
            {
                return this.cameraRotation;
            }
            set
            {
                this.cameraRotation = value;
            }
        }
        public MXF.Vector3 Position
        {
            get
            {
                return this.cameraPosition;
            }
            set
            {
                this.cameraPosition = value;
            }
        }
        public MXF.Matrix Projection
        {
            get
            {
                return MXF.Matrix.CreatePerspectiveFieldOfView(this.FOV, this.aspectRatio, this.nearClip, this.farClip);
            }
        }
        public MXF.Matrix View
        {
            get
            {
                return MXF.Matrix.Invert(MXF.Matrix.CreateFromQuaternion(this.Rotation) * MXF.Matrix.CreateTranslation(this.Position));
            }
        }
        protected float yaw;
        protected float pitch;
        protected float roll;

        public Camera()
        {
            this.cameraRotation = new MXF.Quaternion();
            this.cameraPosition = MXF.Vector3.Zero;

            this.yaw = 0;
            this.pitch = 0;
            this.roll = 0;
        }

        public void Rotate(float xRotation, float yRotation, float zRotation)
        {
            this.yaw += xRotation;
            this.pitch += yRotation;
            this.roll += zRotation;

            MXF.Quaternion q1 = MXF.Quaternion.CreateFromAxisAngle(new MXF.Vector3(0, 1, 0), this.yaw);
            MXF.Quaternion q2 = MXF.Quaternion.CreateFromAxisAngle(new MXF.Vector3(1, 0, 0), this.pitch);
            MXF.Quaternion q3 = MXF.Quaternion.CreateFromAxisAngle(new MXF.Vector3(0, 0, 1), this.roll);
            this.cameraRotation = q1 * q2 * q3;
        }

        public void Translate(MXF.Vector3 distance)
        {
            MXF.Vector3 diff = MXF.Vector3.Transform(distance, MXF.Matrix.CreateFromQuaternion(this.cameraRotation));
            this.cameraPosition += diff;
        }
    }

    public interface ICameraManagerService
    {
        Camera Camera { get; }
    }

    public class CameraManager : MXF.GameComponent, ICameraManagerService
    {
        protected Camera camera;
        IInputManagerService inputManagerService;

        public CameraManager(MXF.Game game)
            : base(game)
        {
            this.camera = new Camera();
            {
                this.camera.Rotation = new MXF.Quaternion(0, 0, 0, 0);
                this.camera.Position = new MXF.Vector3(0.0f, 500.0f, 50.0f);
            }
        }

        public Camera Camera
        {
            get
            {
                return this.camera;
            }
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here
            inputManagerService = (IInputManagerService)this.Game.Services.GetService( typeof( IInputManagerService ) );
        }

        public override void Update( MXF.GameTime gameTime )
        {
            // Retrieve the mousestate
            inputManagerService.Mouse.Capture();
            MouseState ms = inputManagerService.Mouse.MouseState;
            Keyboard kb = inputManagerService.Keyboard;
            Joystick gp = null;
            if ( inputManagerService.GamePads.Count > 0 )
                gp = inputManagerService.GamePads[0];
            kb.Capture();
            gp.Capture();

            // Save the offset between mousecoordinates, and the current mouse pos

            // Check if pressed the left mouse button
            if ( !ms.IsButtonDown( MouseButtonID.Left ) )
            {
                // Rotate camera
                this.camera.Rotate(ms.X.Relative * 0.005f, ms.Y.Relative * 0.005f, 0);
            }

            // Variable that controls the speed
            float speed = 35f;

            if ( ( kb.IsShiftState( Keyboard.ShiftState.Shift ) && kb.IsKeyDown( KeyCode.Key_LSHIFT ) ) ||
                  ( gp != null && gp.JoystickState.IsButtonDown( 2 ) ) )
                speed *= 2;

            // Check for specified key presses
            if ( kb.IsKeyDown( KeyCode.Key_W) || kb.IsKeyDown( KeyCode.Key_UP) || ms.IsButtonDown(MouseButtonID.Left) )
                this.camera.Translate(new MXF.Vector3(0, 0, -1) * speed);

            if ( kb.IsKeyDown(KeyCode.Key_S) || kb.IsKeyDown( KeyCode.Key_DOWN) )
                this.camera.Translate(new MXF.Vector3(0, 0, 1) * speed);

            if ( kb.IsKeyDown(KeyCode.Key_A) || kb.IsKeyDown( KeyCode.Key_LEFT) )
                this.camera.Translate(new MXF.Vector3(-1, 0, 0) * speed);

            if ( kb.IsKeyDown(KeyCode.Key_D) || kb.IsKeyDown( KeyCode.Key_RIGHT) )
                this.camera.Translate(new MXF.Vector3(1, 0, 0) * speed);

            if ( gp != null )
            {
            }
            // TODO: Add your update code here
            base.Update(gameTime);
        }
    }
}
