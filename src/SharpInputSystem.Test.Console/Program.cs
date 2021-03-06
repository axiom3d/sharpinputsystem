﻿#region MIT/X11 License

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
using System.Collections.Generic;
using System.Windows.Forms;
using Common.Logging;

namespace SharpInputSystem.Test.Console
{
    internal class EventHandler : IKeyboardListener, IMouseListener, IJoystickListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

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

        public bool KeyPressed(KeyEventArgs e)
        {
            log.Info(String.Format("KeyPressed : {0} {1}", e.Key, e.Text));
            return true;
        }

        public bool KeyReleased(KeyEventArgs e)
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
            return true;
        }

        public bool MouseReleased(MouseEventArgs arg, MouseButtonID id)
        {
            log.Info(String.Format("MouseReleased : {0}", arg.State.Buttons));
            return true;
        }

        #endregion
    }

    internal partial class Program
    {
        private static readonly EventHandler _handler = new EventHandler();

        private static InputManager _inputManager;
        private static Keyboard _kb;
        private static Mouse _m;

        private static readonly List<Joystick> _joys = new List<Joystick>();
        private static readonly List<ForceFeedback> _ff = new List<ForceFeedback>();

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private static Main _window;

        private static void DoStartup()
        {
            _window = new Main();
            _window.FormClosed += (sender, e) =>
            {
                if (_handler != null)
                    _handler.appRunning = false;
            };
            _window.Show();

            var pl = new ParameterList
            {
                new Parameter("WINDOW", _window.Handle),

                //Default mode is foreground exclusive..but, we want to show mouse - so nonexclusive
                new Parameter("w32_mouse", "CLF_BACKGROUND"),
                new Parameter("w32_mouse", "CLF_NONEXCLUSIVE"),
                new Parameter("dx_mouse_hide", true),
                new Parameter("x11_keyboard_grab", false),
                new Parameter("x11_mouse_grab", false),
                new Parameter("x11_mouse_hide", false)
            };

            CreatInputSystem(pl);

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
                _m.EventListener = _handler;

                MouseState ms = _m.MouseState;
                ms.Width = 100;
                ms.Height = 100;
            }

            if (_inputManager.DeviceCount<Keyboard>() > 0)
            {
                _kb = _inputManager.CreateInputObject<Keyboard>(buffered, "");
                log.Info(String.Format("Created {0}buffered keyboard", buffered ? "" : "un"));
                _kb.EventListener = _handler;
            }

            //This demo only uses at max 4 joys
            int numSticks = _inputManager.DeviceCount<Joystick>();
            if (numSticks > 4)
                numSticks = 4;

            for (int i = 0; i < numSticks; ++i)
            {
                _joys.Insert(i, _inputManager.CreateInputObject<Joystick>(true, ""));
                _joys[i].EventListener = _handler;

                _ff.Insert(i, (ForceFeedback)_joys[i].QueryInterface<ForceFeedback>());
                if (_ff[i] != null)
                {
                    log.Info(String.Format("Created buffered joystick with ForceFeedback support."));
                    //TODO: Dump out all the supported effects
                }
                else
                    log.Info(String.Format("Created buffered joystick. **without** FF support"));
            }
        }

        static partial void CreatInputSystem(ParameterList pl);

        private static void Main(string[] args)
        {
            log.Info("SharpInputSystem Console Application");
            try
            {
                DoStartup();

                while (_handler.appRunning)
                {
                    //Throttle down CPU usage
                    Application.DoEvents();

                    if (_m != null)
                    {
                        _m.Capture();
                        if (!_m.IsBuffered)
                            HandleNonBufferedMouse();
                    }

                    if (_kb != null)
                    {
                        _kb.Capture();
                        if (!_kb.IsBuffered)
                            HandleNonBufferedKeys();
                    }

                    foreach (Joystick joy in _joys)
                    {
                        if (joy != null)
                        {
                            joy.Capture();
                            if (!joy.IsBuffered)
                                HandleNonBufferedJoystick(joy);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("SIS Exception Caught!");
                Exception logEx = e;
                while (logEx != null)
                {
                    log.Error("", logEx);
                    logEx = logEx.InnerException;
                }
                log.Info("Press any key to exit.");
                System.Console.ReadKey();
            }

            if (_inputManager != null)
            {
                _inputManager.DestroyInputObject(_kb);
                _inputManager.DestroyInputObject(_m);

                foreach (Joystick joy in _joys)
                    _inputManager.DestroyInputObject(joy);
            }

            _window.Close();
            _window = null;

            log.Info("Goodbye");
            return;
        }

        private static void HandleNonBufferedKeys()
        {
            if (_kb.IsKeyDown(KeyCode.Key_ESCAPE) || _kb.IsKeyDown(KeyCode.Key_Q))
                _handler.appRunning = false;
            if (_kb.IsShiftState(Keyboard.ShiftState.Alt))
                System.Console.Write(" ALT ");
            if (_kb.IsShiftState(Keyboard.ShiftState.Shift))
                System.Console.Write(" SHIFT ");
            if (_kb.IsShiftState(Keyboard.ShiftState.Ctrl))
                System.Console.Write(" CTRL ");

            int[] ks = _kb.KeyStates;
            for (int i = 0; i < ks.Length; i++)
            {
                if (ks[i] != 0)
                    log.Info(String.Format("KeyPressed : {0} {1}", (KeyCode)i, _kb.AsString((KeyCode)i)));
            }
        }

        private static void HandleNonBufferedMouse() { }

        private static void HandleNonBufferedJoystick(Joystick joy) { }
    }
}
