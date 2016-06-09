#region MIT/X11 License
/*
Sharp Input System Library
Copyright © 2007-2011 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to the Phillip Castaneda for maintaining such a high quality project.

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
using System.Text;
using System.Linq;
using System.Windows.Forms;
using Common.Logging;

namespace SharpInputSystem.SWF
{
    public class SWFKeyboard : Keyboard
    {
        private Control _window;
        private int[] _keyBuffer = new int[ 256 ];
        Dictionary<Keys, KeyCode> keyConversion = new Dictionary<Keys, KeyCode>();


        public SWFKeyboard( InputManager creator, Control window, bool buffered )
        {
            this.Creator = creator;
            this.IsBuffered = buffered;

            _window = window;
        }

        protected override void Dispose( bool disposeManagedResources )
        {
            if ( disposeManagedResources )
            {
                _window.KeyDown -= Handle_windowKeyDown;
                _window.KeyUp -= Handle_windowKeyUp;
                _window = null;
            }
            base.Dispose( disposeManagedResources );
        }

        protected internal override void Initialize()
        {
            _window.KeyDown += Handle_windowKeyDown;
            _window.KeyUp += Handle_windowKeyUp;
            _window.PreviewKeyDown += ( sender, e ) => { e.IsInputKey = true; };

            #region Keys to KeyCode Mappings
            keyConversion.Add( Keys.D1, KeyCode.Key_1 );
            keyConversion.Add( Keys.D2, KeyCode.Key_2 );
            keyConversion.Add( Keys.D3, KeyCode.Key_3 );
            keyConversion.Add( Keys.D4, KeyCode.Key_4 );
            keyConversion.Add( Keys.D5, KeyCode.Key_5 );
            keyConversion.Add( Keys.D6, KeyCode.Key_6 );
            keyConversion.Add( Keys.D7, KeyCode.Key_7 );
            keyConversion.Add( Keys.D8, KeyCode.Key_8 );
            keyConversion.Add( Keys.D9, KeyCode.Key_9 );
            keyConversion.Add( Keys.D0, KeyCode.Key_0 );

            keyConversion.Add( Keys.Back, KeyCode.Key_BACK );
            keyConversion.Add( Keys.OemMinus, KeyCode.Key_MINUS );
            keyConversion.Add( Keys.Oemplus, KeyCode.Key_EQUALS );
            keyConversion.Add( Keys.Space, KeyCode.Key_SPACE );
            keyConversion.Add( Keys.Oemcomma, KeyCode.Key_COMMA );
            keyConversion.Add( Keys.OemPeriod, KeyCode.Key_PERIOD );

            keyConversion.Add( Keys.Oem2, KeyCode.Key_BACKSLASH );
            keyConversion.Add( Keys.OemPipe, KeyCode.Key_SLASH );
            keyConversion.Add( Keys.Oem4, KeyCode.Key_LBRACKET );
            keyConversion.Add( Keys.Oem6, KeyCode.Key_RBRACKET );

            keyConversion.Add( Keys.Escape, KeyCode.Key_ESCAPE );

            keyConversion.Add( Keys.Tab, KeyCode.Key_TAB );

            keyConversion.Add( Keys.Oem1, KeyCode.Key_SEMICOLON );
            keyConversion.Add( Keys.OemQuotes, KeyCode.Key_APOSTROPHE );
            keyConversion.Add( Keys.Oemtilde, KeyCode.Key_GRAVE );


            keyConversion.Add( Keys.A, KeyCode.Key_A );
            keyConversion.Add( Keys.B, KeyCode.Key_B );
            keyConversion.Add( Keys.C, KeyCode.Key_C );
            keyConversion.Add( Keys.D, KeyCode.Key_D );
            keyConversion.Add( Keys.E, KeyCode.Key_E );
            keyConversion.Add( Keys.F, KeyCode.Key_F );
            keyConversion.Add( Keys.G, KeyCode.Key_G );
            keyConversion.Add( Keys.H, KeyCode.Key_H );
            keyConversion.Add( Keys.I, KeyCode.Key_I );
            keyConversion.Add( Keys.J, KeyCode.Key_J );
            keyConversion.Add( Keys.K, KeyCode.Key_K );
            keyConversion.Add( Keys.L, KeyCode.Key_L );
            keyConversion.Add( Keys.M, KeyCode.Key_M );
            keyConversion.Add( Keys.N, KeyCode.Key_N );
            keyConversion.Add( Keys.O, KeyCode.Key_O );
            keyConversion.Add( Keys.P, KeyCode.Key_P );
            keyConversion.Add( Keys.Q, KeyCode.Key_Q );
            keyConversion.Add( Keys.R, KeyCode.Key_R );
            keyConversion.Add( Keys.S, KeyCode.Key_S );
            keyConversion.Add( Keys.T, KeyCode.Key_T );
            keyConversion.Add( Keys.U, KeyCode.Key_U );
            keyConversion.Add( Keys.V, KeyCode.Key_V );
            keyConversion.Add( Keys.W, KeyCode.Key_W );
            keyConversion.Add( Keys.X, KeyCode.Key_X );
            keyConversion.Add( Keys.Y, KeyCode.Key_Y );
            keyConversion.Add( Keys.Z, KeyCode.Key_Z );

            keyConversion.Add( Keys.F1, KeyCode.Key_F1 );
            keyConversion.Add( Keys.F2, KeyCode.Key_F2 );
            keyConversion.Add( Keys.F3, KeyCode.Key_F3 );
            keyConversion.Add( Keys.F4, KeyCode.Key_F4 );
            keyConversion.Add( Keys.F5, KeyCode.Key_F5 );
            keyConversion.Add( Keys.F6, KeyCode.Key_F6 );
            keyConversion.Add( Keys.F7, KeyCode.Key_F7 );
            keyConversion.Add( Keys.F8, KeyCode.Key_F8 );
            keyConversion.Add( Keys.F9, KeyCode.Key_F9 );
            keyConversion.Add( Keys.F10, KeyCode.Key_F10 );
            keyConversion.Add( Keys.F11, KeyCode.Key_F11 );
            keyConversion.Add( Keys.F12, KeyCode.Key_F12 );
            keyConversion.Add( Keys.F13, KeyCode.Key_F13 );
            keyConversion.Add( Keys.F14, KeyCode.Key_F14 );
            keyConversion.Add( Keys.F15, KeyCode.Key_F15 );

            //keypad
            keyConversion.Add( Keys.NumPad0, KeyCode.Key_NUMPAD0 );
            keyConversion.Add( Keys.NumPad1, KeyCode.Key_NUMPAD1 );
            keyConversion.Add( Keys.NumPad2, KeyCode.Key_NUMPAD2 );
            keyConversion.Add( Keys.NumPad3, KeyCode.Key_NUMPAD3 );
            keyConversion.Add( Keys.NumPad4, KeyCode.Key_NUMPAD4 );
            keyConversion.Add( Keys.NumPad5, KeyCode.Key_NUMPAD5 );
            keyConversion.Add( Keys.NumPad6, KeyCode.Key_NUMPAD6 );
            keyConversion.Add( Keys.NumPad7, KeyCode.Key_NUMPAD7 );
            keyConversion.Add( Keys.NumPad8, KeyCode.Key_NUMPAD8 );
            keyConversion.Add( Keys.NumPad9, KeyCode.Key_NUMPAD9 );
            keyConversion.Add( Keys.Add, KeyCode.Key_ADD );
            keyConversion.Add( Keys.Subtract, KeyCode.Key_SUBTRACT );
            keyConversion.Add( Keys.Decimal, KeyCode.Key_DECIMAL );
            keyConversion.Add( Keys.Divide, KeyCode.Key_DIVIDE );
            keyConversion.Add( Keys.Multiply, KeyCode.Key_MULTIPLY );
            keyConversion.Add( Keys.Enter, KeyCode.Key_NUMPADENTER );

            //Keypad with numlock off
//            keyConversion.Add( Keys.Home, KeyCode.Key_NUMPAD7 );
//            keyConversion.Add( Keys.Up, KeyCode.Key_NUMPAD8 );
//            keyConversion.Add( Keys.PageUp, KeyCode.Key_NUMPAD9 );
//            keyConversion.Add( Keys.Left, KeyCode.Key_NUMPAD4 );
//            keyConversion.Add( Keys.Begin, KeyCode.Key_NUMPAD5 );
//            keyConversion.Add( Keys.Right, KeyCode.Key_NUMPAD6 );
//            keyConversion.Add( Keys.End, KeyCode.Key_NUMPAD1 );
//            keyConversion.Add( Keys.Down, KeyCode.Key_NUMPAD2 );
//            keyConversion.Add( Keys.PageDown, KeyCode.Key_NUMPAD3 );
//            keyConversion.Add( Keys.Insert, KeyCode.Key_NUMPAD0 );
//            keyConversion.Add( Keys.Delete, KeyCode.Key_DECIMAL );

            keyConversion.Add( Keys.Up, KeyCode.Key_UP );
            keyConversion.Add( Keys.Down, KeyCode.Key_DOWN );
            keyConversion.Add( Keys.Left, KeyCode.Key_LEFT );
            keyConversion.Add( Keys.Right, KeyCode.Key_RIGHT );

            keyConversion.Add( Keys.PageUp, KeyCode.Key_PGUP );
            keyConversion.Add( Keys.PageDown, KeyCode.Key_PGDOWN );
            keyConversion.Add( Keys.Home, KeyCode.Key_HOME );
            keyConversion.Add( Keys.End, KeyCode.Key_END );

            keyConversion.Add( Keys.NumLock, KeyCode.Key_NUMLOCK );
            keyConversion.Add( Keys.Print, KeyCode.Key_SYSRQ );
            keyConversion.Add( Keys.Scroll, KeyCode.Key_SCROLL );
            keyConversion.Add( Keys.Pause, KeyCode.Key_PAUSE );

            keyConversion.Add( Keys.RShiftKey, KeyCode.Key_RSHIFT );
            keyConversion.Add( Keys.LShiftKey, KeyCode.Key_LSHIFT );
            keyConversion.Add( Keys.Alt, KeyCode.Key_RMENU );
            //keyConversion.Add( Keys.Alt, KeyCode.Key_LMENU );

            keyConversion.Add( Keys.Insert, KeyCode.Key_INSERT );
            keyConversion.Add( Keys.Delete, KeyCode.Key_DELETE );

            keyConversion.Add( Keys.LControlKey, KeyCode.Key_LWIN );
            keyConversion.Add( Keys.RControlKey, KeyCode.Key_RWIN );
            keyConversion.Add( Keys.Menu, KeyCode.Key_APPS );

            #endregion Keys to KeyCode Mappings
        }

        public override void Capture()
        {
            System.Windows.Forms.Application.DoEvents();
        }

        public override string AsString( KeyCode key )
        {
            return string.Empty;
        }

        public override int[] KeyStates
        {
            get { return (int[])_keyBuffer.Clone(); }
        }
        
        public override bool IsKeyDown( KeyCode key )
        {
            return _keyBuffer[ (int)key ] == 1;
        }

        void Handle_windowKeyDown( object sender, System.Windows.Forms.KeyEventArgs e )
        {
            if ( keyConversion.ContainsKey( e.KeyCode ) )
            {
                var key = keyConversion[ e.KeyCode ];
                _keyBuffer[ (int)key ] = 1;
                if ( this.EventListener != null && IsBuffered )
                {
                    this.EventListener.KeyPressed( new SharpInputSystem.KeyEventArgs( this, key, (int)key ) );
                }
            }
        }

        void Handle_windowKeyUp( object sender, System.Windows.Forms.KeyEventArgs e )
        {
            if ( keyConversion.ContainsKey( e.KeyCode ) )
            {
                var key = keyConversion[ e.KeyCode ];
                _keyBuffer[ (int)key ] = 0;
                if ( this.EventListener != null && IsBuffered )
                {
                    this.EventListener.KeyReleased( new SharpInputSystem.KeyEventArgs( this, key, (int)key ) );
                }
            }
        }

    }
}

