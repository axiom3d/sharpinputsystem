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

#region tNamespace Declarations

using System;

using Common.Logging;

using SharpDX.DirectInput;

using MDI = SharpDX.DirectInput;

#endregion Namespace Declarations

namespace SharpInputSystem.DirectX
{
    internal class DirectXKeyboard : Keyboard
    {
        #region Fields and Properties

        private const int BufferSize = 17;
        private static readonly ILog log = LogManager.GetLogger( typeof ( DirectXKeyboard ) );

        private readonly CooperativeLevel _coopSettings;
        private readonly DirectInput _directInput;
        private readonly KeyboardInfo _kbInfo;

        private readonly int[] _keyboardState = new int[ 256 ];
        private SharpDX.DirectInput.Keyboard _keyboard;

        #endregion Fields and Properties

        #region Construction and Destruction

        public DirectXKeyboard( InputManager creator, DirectInput directInput, bool buffered, CooperativeLevel coopSettings )
        {
            Creator = creator;
            this._directInput = directInput;
            IsBuffered = buffered;
            this._coopSettings = coopSettings;
            Type = InputType.Keyboard;
            EventListener = null;

            this._kbInfo = ( KeyboardInfo ) ( ( DirectXInputManager ) Creator ).CaptureDevice<Keyboard>( );

            if ( this._kbInfo == null )
                throw new Exception( "No devices match requested type." );

            log.Debug( "DirectXKeyboard device created." );
        }

        protected override void Dispose( bool disposeManagedResources )
        {
            if ( !IsDisposed )
            {
                if ( disposeManagedResources )
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
                try
                {
                    this._keyboard.Unacquire( );
                }
                catch
                {
                    // NOTE : This is intentional
                }
                finally
                {
                    this._keyboard.Dispose( );
                    this._keyboard = null;
                }

                ( ( DirectXInputManager ) Creator ).ReleaseDevice<Keyboard>( this._kbInfo );

                log.Debug( "DirectXKeyboard device disposed." );
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.Dispose( disposeManagedResources );
        }

        #endregion Construction and Destruction

        #region Methods

        private void Read( )
        {
            KeyboardState state = this._keyboard.GetCurrentState( );
            for ( int i = 0; i < 256; i++ )
                this._keyboardState[ i ] = state.IsPressed( ( Key ) i ) ? i : 0;

            //Set Shift, Ctrl, Alt
            shiftState = 0;
            if ( IsKeyDown( KeyCode.Key_LCONTROL ) || IsKeyDown( KeyCode.Key_RCONTROL ) )
                shiftState |= ShiftState.Ctrl;
            if ( IsKeyDown( KeyCode.Key_LSHIFT ) || IsKeyDown( KeyCode.Key_RSHIFT ) )
                shiftState |= ShiftState.Shift;
            if ( IsKeyDown( KeyCode.Key_LMENU ) || IsKeyDown( KeyCode.Key_RMENU ) )
                shiftState |= ShiftState.Alt;
        }

        private void ReadBuffered( )
        {
            // grab the collection of buffered data
            KeyboardUpdate[] bufferedData = this._keyboard.GetBufferedData( );

            // please tell me why this would ever come back null, rather than an empty collection...
            if ( bufferedData == null )
                return;
            foreach ( KeyboardUpdate packet in bufferedData )
            {
                bool ret = true;

                //foreach (MDI.Key key in packet.PressedKeys)
                //{
                KeyCode keyCode = Convert( packet.Key );

                //Store result in our keyBuffer too
                this._keyboardState[ ( int ) packet.Key ] = 1;

                if ( packet.IsPressed )
                {
                    if ( packet.Key == Key.RightControl || packet.Key == Key.LeftControl )
                        shiftState |= ShiftState.Ctrl;
                    if ( packet.Key == Key.RightAlt || packet.Key == Key.LeftAlt )
                        shiftState |= ShiftState.Alt;
                    if ( packet.Key == Key.RightShift || packet.Key == Key.LeftShift )
                        shiftState |= ShiftState.Shift;

                    if ( EventListener != null )
                        ret = EventListener.KeyPressed( new KeyEventArgs( this, keyCode, 0 ) );
                    if ( ret == false )
                        break;
                }
                else
                {
                    if ( packet.Key == Key.RightControl || packet.Key == Key.LeftControl )
                        shiftState &= ~ShiftState.Ctrl;
                    if ( packet.Key == Key.RightAlt || packet.Key == Key.LeftAlt )
                        shiftState &= ~ShiftState.Alt;
                    if ( packet.Key == Key.RightShift || packet.Key == Key.LeftShift )
                        shiftState &= ~ShiftState.Shift;


                    if ( EventListener != null )
                        ret = EventListener.KeyReleased( new KeyEventArgs( this, keyCode, 0 ) );
                    if ( ret == false )
                        break;
                }
            }
        }

        private KeyCode Convert( Key key )
        {
            switch ( key )
            {
                case Key.A:
                    return KeyCode.Key_A;
                case Key.AbntC1:
                    return KeyCode.Key_ABNT_C1;
                case Key.AbntC2:
                    return KeyCode.Key_ABNT_C2;
                case Key.Apostrophe:
                    return KeyCode.Key_APOSTROPHE;
                case Key.Applications:
                    return KeyCode.Key_APPS;
                case Key.AT:
                    return KeyCode.Key_AT;
                case Key.AX:
                    return KeyCode.Key_AX;
                case Key.B:
                    return KeyCode.Key_B;
                case Key.Backslash:
                    return KeyCode.Key_BACKSLASH;
                case Key.Back:
                    return KeyCode.Key_BACK;
                case Key.C:
                    return KeyCode.Key_C;
                case Key.Calculator:
                    return KeyCode.Key_CALCULATOR;
                case Key.Capital:
                    return KeyCode.Key_CAPITAL;
                case Key.Colon:
                    return KeyCode.Key_COLON;
                case Key.Comma:
                    return KeyCode.Key_COMMA;
                case Key.Convert:
                    return KeyCode.Key_CONVERT;
                case Key.D:
                    return KeyCode.Key_D;
                case Key.D0:
                    return KeyCode.Key_0;
                case Key.D1:
                    return KeyCode.Key_1;
                case Key.D2:
                    return KeyCode.Key_2;
                case Key.D3:
                    return KeyCode.Key_3;
                case Key.D4:
                    return KeyCode.Key_4;
                case Key.D5:
                    return KeyCode.Key_5;
                case Key.D6:
                    return KeyCode.Key_6;
                case Key.D7:
                    return KeyCode.Key_7;
                case Key.D8:
                    return KeyCode.Key_8;
                case Key.D9:
                    return KeyCode.Key_9;
                case Key.Delete:
                    return KeyCode.Key_DELETE;
                case Key.Down:
                    return KeyCode.Key_DOWN;
                case Key.E:
                    return KeyCode.Key_E;
                case Key.End:
                    return KeyCode.Key_END;
                case Key.Equals:
                    return KeyCode.Key_EQUALS;
                case Key.Escape:
                    return KeyCode.Key_ESCAPE;
                case Key.F:
                    return KeyCode.Key_F;
                case Key.F1:
                    return KeyCode.Key_F1;
                case Key.F2:
                    return KeyCode.Key_F2;
                case Key.F3:
                    return KeyCode.Key_F3;
                case Key.F4:
                    return KeyCode.Key_F4;
                case Key.F5:
                    return KeyCode.Key_F5;
                case Key.F6:
                    return KeyCode.Key_F6;
                case Key.F7:
                    return KeyCode.Key_F7;
                case Key.F8:
                    return KeyCode.Key_F8;
                case Key.F9:
                    return KeyCode.Key_F9;
                case Key.F10:
                    return KeyCode.Key_F10;
                case Key.F11:
                    return KeyCode.Key_F11;
                case Key.F12:
                    return KeyCode.Key_F12;
                case Key.F13:
                    return KeyCode.Key_F13;
                case Key.F14:
                    return KeyCode.Key_F14;
                case Key.F15:
                    return KeyCode.Key_F15;
                case Key.G:
                    return KeyCode.Key_G;
                case Key.Grave:
                    return KeyCode.Key_GRAVE;
                case Key.H:
                    return KeyCode.Key_H;
                case Key.Home:
                    return KeyCode.Key_HOME;
                case Key.I:
                    return KeyCode.Key_I;
                case Key.Insert:
                    return KeyCode.Key_INSERT;
                case Key.J:
                    return KeyCode.Key_J;
                case Key.K:
                    return KeyCode.Key_K;
                case Key.L:
                    return KeyCode.Key_L;
                case Key.LeftAlt:
                    return KeyCode.Key_LMENU;
                case Key.Left:
                    return KeyCode.Key_LEFT;
                case Key.LeftBracket:
                    return KeyCode.Key_LBRACKET;
                case Key.LeftControl:
                    return KeyCode.Key_LCONTROL;
                case Key.LeftShift:
                    return KeyCode.Key_LSHIFT;
                case Key.LeftWindowsKey:
                    return KeyCode.Key_LWIN;
                case Key.M:
                    return KeyCode.Key_M;
                case Key.Mail:
                    return KeyCode.Key_MAIL;
                case Key.MediaSelect:
                    return KeyCode.Key_MEDIASELECT;
                case Key.MediaStop:
                    return KeyCode.Key_MEDIASTOP;
                case Key.Minus:
                    return KeyCode.Key_MINUS;
                case Key.Mute:
                    return KeyCode.Key_MUTE;
                case Key.MyComputer:
                    return KeyCode.Key_MYCOMPUTER;
                case Key.N:
                    return KeyCode.Key_N;
                case Key.O:
                    return KeyCode.Key_O;
                case Key.Oem102:
                    return KeyCode.Key_OEM_102;
                case Key.P:
                    return KeyCode.Key_P;
                case Key.PageDown:
                    return KeyCode.Key_PGDOWN;
                case Key.PageUp:
                    return KeyCode.Key_PGUP;
                case Key.Pause:
                    return KeyCode.Key_PAUSE;
                case Key.Period:
                    return KeyCode.Key_PERIOD;
                case Key.PlayPause:
                    return KeyCode.Key_PLAYPAUSE;
                case Key.Power:
                    return KeyCode.Key_POWER;
                case Key.PreviousTrack:
                    return KeyCode.Key_PREVTRACK;
                case Key.PrintScreen:
                    return KeyCode.Key_SYSRQ;
                case Key.Q:
                    return KeyCode.Key_Q;
                case Key.R:
                    return KeyCode.Key_R;
                case Key.Return:
                    return KeyCode.Key_RETURN;
                case Key.RightAlt:
                    return KeyCode.Key_RMENU;
                case Key.Right:
                    return KeyCode.Key_RIGHT;
                case Key.RightBracket:
                    return KeyCode.Key_RBRACKET;
                case Key.RightControl:
                    return KeyCode.Key_RCONTROL;
                case Key.RightShift:
                    return KeyCode.Key_RSHIFT;
                case Key.RightWindowsKey:
                    return KeyCode.Key_RWIN;
                case Key.S:
                    return KeyCode.Key_S;
                case Key.ScrollLock:
                    return KeyCode.Key_SCROLL;
                case Key.Semicolon:
                    return KeyCode.Key_SEMICOLON;
                case Key.Slash:
                    return KeyCode.Key_SLASH;
                case Key.Sleep:
                    return KeyCode.Key_SLEEP;
                case Key.Space:
                    return KeyCode.Key_SPACE;
                case Key.Stop:
                    return KeyCode.Key_STOP;
                case Key.T:
                    return KeyCode.Key_T;
                case Key.Tab:
                    return KeyCode.Key_TAB;
                case Key.U:
                    return KeyCode.Key_U;
                case Key.Underline:
                    return KeyCode.Key_UNDERLINE;
                case Key.Unknown:
                    return KeyCode.Key_UNASSIGNED;
                case Key.Unlabeled:
                    return KeyCode.Key_UNLABELED;
                case Key.Up:
                    return KeyCode.Key_UP;
                case Key.V:
                    return KeyCode.Key_V;
                case Key.VolumeDown:
                    return KeyCode.Key_VOLUMEDOWN;
                case Key.VolumeUp:
                    return KeyCode.Key_VOLUMEUP;
                case Key.W:
                    return KeyCode.Key_W;
                case Key.Wake:
                    return KeyCode.Key_WAKE;
                case Key.WebBack:
                    return KeyCode.Key_WEBBACK;
                case Key.WebFavorites:
                    return KeyCode.Key_WEBFAVORITES;
                case Key.WebForward:
                    return KeyCode.Key_WEBFORWARD;
                case Key.WebHome:
                    return KeyCode.Key_WEBHOME;
                case Key.WebRefresh:
                    return KeyCode.Key_WEBREFRESH;
                case Key.WebSearch:
                    return KeyCode.Key_WEBSEARCH;
                case Key.WebStop:
                    return KeyCode.Key_WEBSTOP;
                case Key.X:
                    return KeyCode.Key_X;
                case Key.Y:
                    return KeyCode.Key_Y;
                case Key.Yen:
                    return KeyCode.Key_YEN;
                case Key.Z:
                    return KeyCode.Key_Z;

                default:
                    return KeyCode.Key_UNLABELED;
            }
        }

        #endregion Methods

        #region InputObject Implementation

        public override void Capture( )
        {
            if ( IsBuffered )
                ReadBuffered( );
            else
                Read( );
        }

        protected override void Initialize( )
        {
            this._keyboard = new SharpDX.DirectInput.Keyboard( this._directInput );

            //_keyboard.SetDataFormat( MDI.DeviceDataFormat.Keyboard );

			if ( this._coopSettings != 0 )
				this._keyboard.SetCooperativeLevel( ( (DirectXInputManager)Creator ).WindowHandle, this._coopSettings );

            if ( IsBuffered )
                this._keyboard.Properties.BufferSize = BufferSize;

            try
            {
                this._keyboard.Acquire( );
            }
            catch ( Exception e )
            {
                throw new Exception( "Failed to aquire keyboard using DirectInput.", e );
            }
        }

        #endregion InputObject Implementation

        #region Keyboard Implementation

        public override int[] KeyStates
        {
            get { return ( int[] ) this._keyboardState.Clone( ); }
        }

        public override bool IsKeyDown( KeyCode key )
        {
            return ( ( this._keyboardState[ ( int ) key ] ) != 0 );
        }

        public override string AsString( KeyCode key )
        {
            return this._keyboard.Properties.GetKeyName( ( Key ) key );
        }

        #endregion Keyboard Implementation
    }
}
