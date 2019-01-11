#region MIT/X11 License

/*
Sharp Input System Library
Copyright © 2007-2019 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to Phillip Castaneda for maintaining such a high quality project.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

*/

#endregion MIT/X11 License

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES11;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using System.Collections.Generic;
using Common.Logging;
using SharpInputSystem.Android;

namespace SharpInputSystem.Test.OpenGL.Android
{
    class GLView1 : AndroidGameView, SharpInputSystem.IKeyboardListener, IMouseListener, IJoystickListener
    {
        private static InputManager _inputManager;
        private static Keyboard _kb;
        private static Mouse _m;

        private static readonly List<Joystick> _joys = new List<Joystick>();
        private static readonly List<ForceFeedback> _ff = new List<ForceFeedback>();

        private static readonly ILog log = LogManager.GetLogger(typeof(GLView1));

        public GLView1(Context context) : base(context)
        {
            _context = context;
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var pl = new ParameterList
            {
                new Parameter("WINDOW", this),
                new Parameter("CONTEXT", this._context)
            };

            //This never returns null.. it will raise an exception on errors
            _inputManager = InputManager.CreateInputSystem(typeof(AndroidInputManagerFactory), pl);

            log.Info(String.Format("SIS Version : {0}", _inputManager.Version));
            log.Info(String.Format("Platform : {0}", _inputManager.InputSystemName));
            log.Info(String.Format("Number of Mice : {0}", _inputManager.DeviceCount<Mouse>()));
            log.Info(String.Format("Number of Keyboards : {0}", _inputManager.DeviceCount<Keyboard>()));
            log.Info(String.Format("Number of Joys/Pads: {0}", _inputManager.DeviceCount<Joystick>()));

            bool buffered = true;

            if (_inputManager.DeviceCount<Mouse>() > 0)
            {
                _m = _inputManager.CreateInputObject<Mouse>(buffered, "");
                log.Info(String.Format("Created {0}buffered mouse", buffered ? "" : "un"));
                _m.EventListener = this;

                MouseState ms = _m.MouseState;
                ms.Width = 100;
                ms.Height = 100;
            }
            
            if (_inputManager.DeviceCount<Keyboard>() > 0)
            {
                _kb = _inputManager.CreateInputObject<Keyboard>(buffered, "");
                log.Info(String.Format("Created {0}buffered keyboard", buffered ? "" : "un"));
                _kb.EventListener = this;
            }
            
            // Run the render loop
            Run();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        //
        // In this particular case, we demonstrate how to set
        // the graphics mode and fallback in case the device doesn't
        // support the defaults
        protected override void CreateFrameBuffer()
        {
            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("GLCube", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLCube", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("GLCube", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLCube", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.MatrixMode(All.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
            GL.MatrixMode(All.Modelview);
            GL.Rotate(3.0f, 0.0f, 0.0f, 1.0f);

            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
            GL.Clear((uint)All.ColorBufferBit);

            GL.VertexPointer(2, All.Float, 0, square_vertices);
            GL.EnableClientState(All.VertexArray);
            GL.ColorPointer(4, All.UnsignedByte, 0, square_colors);
            GL.EnableClientState(All.ColorArray);

            GL.DrawArrays(All.TriangleStrip, 0, 4);

            SwapBuffers();
        }
        public bool appRunning = true;

        #region IJoystickListener Members

        public bool ButtonPressed(JoystickEventArgs arg, int button)
        {
            log.Info(String.Format("Joystick ButtonPressed : {0} )", button));
            return true;
        }

        public bool ButtonReleased(JoystickEventArgs arg, int button)
        {
            log.Info(String.Format("Joystick ButtonReleased : {0} )", button));
            return true;
        }

        public bool AxisMoved(JoystickEventArgs arg, int axis)
        {
            int axisValue = arg.State.Axis[axis].Absolute;
            if (axisValue > 2500 || axisValue < -2500)
                log.Info(String.Format("Joystick AxisMoved : {0} = {1} )", axis, axisValue));
            return true;
        }

        public bool SliderMoved(JoystickEventArgs arg, int slider)
        {
            log.Info(String.Format("Joystick SliderMoved : {0} = {1}, {2} )", slider, arg.State.Sliders[slider].X, arg.State.Sliders[slider].Y));
            return true;
        }

        public bool PovMoved(JoystickEventArgs arg, int pov)
        {
            log.Info(String.Format("Joystick PovMoved : {0} = {1} )", pov, arg.State.Povs[pov].Direction.ToString()));
            return true;
        }

        #endregion

        #region IKeyboardListener Members

        public bool KeyPressed(SharpInputSystem.KeyEventArgs e)
        {
            log.Info(String.Format("KeyPressed : {0} {1}", e.Key, e.Text));
            return true;
        }

        public bool KeyReleased(SharpInputSystem.KeyEventArgs e)
        {
            log.Info(String.Format("KeyReleased : {0} {1}", e.Key, e.Text));
            if (e.Key == KeyCode.Key_ESCAPE || e.Key == KeyCode.Key_Q)
                this.appRunning = false;
            return true;
        }

        #endregion

        #region IMouseListener Members

        public bool MouseMoved(MouseEventArgs arg)
        {
            log.Info(String.Format("MouseMoved : R( {0} , {1} , {4} ) A( {2} , {3}, {5} )", arg.State.X.Relative, arg.State.Y.Relative, arg.State.X.Absolute, arg.State.Y.Absolute, arg.State.Z.Relative, arg.State.Z.Absolute));
            return true;
        }

        public bool MousePressed(MouseEventArgs arg, MouseButtonID id)
        {
            log.Info(String.Format("MousePressed : {0}", arg.State.Buttons));
            ((AndroidKeyboard)_kb).ShowSoftInput();
            return true;
        }

        public bool MouseReleased(MouseEventArgs arg, MouseButtonID id)
        {
            log.Info(String.Format("MouseReleased : {0}", arg.State.Buttons));
            return true;
        }

        #endregion

        float[] square_vertices = {
            -0.5f, -0.5f,
            0.5f, -0.5f,
            -0.5f, 0.5f,
            0.5f, 0.5f,
        };

        byte[] square_colors = {
            255, 255,   0, 255,
            0,   255, 255, 255,
            0,     0,    0,  0,
            255,   0,  255, 255,
        };
        private Context _context;
    }
}
