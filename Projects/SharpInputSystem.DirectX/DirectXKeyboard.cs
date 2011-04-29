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
using System.Collections.Generic;

using MDI = SlimDX.DirectInput;
using Common.Logging;
#endregion Namespace Declarations

namespace SharpInputSystem.DirectX
{
    class DirectXKeyboard : Keyboard
    {
        #region Fields and Properties

        private static readonly ILog log = LogManager.GetLogger( typeof( DirectXKeyboard ) );

        private const int _BUFFER_SIZE = 17;

        private MDI.CooperativeLevel _coopSettings;
        private MDI.DirectInput _directInput;
        private MDI.Keyboard _keyboard;
        private KeyboardInfo _kbInfo;

        private int[] _keyboardState = new int[ 256 ];

        #endregion Fields and Properties

        #region Construction and Destruction

        public DirectXKeyboard( InputManager creator, MDI.DirectInput directInput, bool buffered, MDI.CooperativeLevel coopSettings )
        {
            Creator = creator;
            _directInput = directInput;
            IsBuffered = buffered;
            _coopSettings = coopSettings;
            Type = InputType.Keyboard;
            EventListener = null;

            _kbInfo = (KeyboardInfo)( (DirectXInputManager)Creator ).CaptureDevice<Keyboard>();

            if ( _kbInfo == null )
            {
                throw new Exception( "No devices match requested type." );
            }

            log.Debug( "DirectXKeyboard device created." );
        }

        protected override void _dispose( bool disposeManagedResources )
        {
            if ( !isDisposed )
            {
                if ( disposeManagedResources )
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
                try
                {
                    _keyboard.Unacquire();
                }
                catch
                {
                    // NOTE : This is intentional
                }
                finally
                {
                    _keyboard.Dispose();
                    _keyboard = null;
                }

                ( (DirectXInputManager)Creator ).ReleaseDevice<Keyboard>( _kbInfo );

                log.Debug( "DirectXKeyboard device disposed." );

            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base._dispose( disposeManagedResources );
        }

        #endregion Construction and Destruction

        #region Methods

        private void _read()
        {
            MDI.KeyboardState state = _keyboard.GetCurrentState();
            for ( int i = 0; i < 256; i++ )
                _keyboardState[ i ] = state.IsPressed( (MDI.Key)i ) ? i : 0;

            //Set Shift, Ctrl, Alt
            shiftState = (ShiftState)0;
            if ( IsKeyDown( KeyCode.Key_LCONTROL ) || IsKeyDown( KeyCode.Key_RCONTROL ) )
                shiftState |= ShiftState.Ctrl;
            if ( IsKeyDown( KeyCode.Key_LSHIFT ) || IsKeyDown( KeyCode.Key_RSHIFT ) )
                shiftState |= ShiftState.Shift;
            if ( IsKeyDown( KeyCode.Key_LMENU ) || IsKeyDown( KeyCode.Key_RMENU ) )
                shiftState |= ShiftState.Alt;

        }

        private void _readBuffered()
        {
            // grab the collection of buffered data
            IEnumerable<MDI.KeyboardState> bufferedData = _keyboard.GetBufferedData();
            if (SlimDX.Result.Last.IsFailure)
                return;

            // please tell me why this would ever come back null, rather than an empty collection...
            if ( bufferedData == null )
            {
                return;
            }
            foreach ( MDI.KeyboardState packet in bufferedData )
            {
                bool ret = true;

                foreach (MDI.Key key in packet.PressedKeys)
                {
                    KeyCode keyCode = Convert(key);

                    //Store result in our keyBuffer too
                    _keyboardState[ (int)keyCode ] = 1;


                    if ( packet.IsPressed( MDI.Key.RightControl ) || packet.IsPressed( MDI.Key.LeftControl ) )
                        shiftState |= ShiftState.Ctrl;
                    if ( packet.IsPressed( MDI.Key.RightAlt ) || packet.IsPressed( MDI.Key.LeftAlt ) )
                        shiftState |= ShiftState.Alt;
                    if ( packet.IsPressed( MDI.Key.RightShift) || packet.IsPressed( MDI.Key.LeftShift ) )
                        shiftState |= ShiftState.Shift;

                    if ( this.EventListener != null )
                        ret = this.EventListener.KeyPressed( new KeyEventArgs( this, keyCode, 0 ) );
                    if ( ret == false )
                        break;
                }
                foreach ( MDI.Key key in packet.ReleasedKeys )
                {
                    KeyCode keyCode = Convert( key );

                    //Store result in our keyBuffer too
                    _keyboardState[ (int)key ] = 1;

                    if ( packet.IsPressed( (MDI.Key)KeyCode.Key_RCONTROL ) || packet.IsPressed( (MDI.Key)KeyCode.Key_LCONTROL ) )
                        shiftState |= ShiftState.Ctrl;
                    if ( packet.IsPressed( (MDI.Key)KeyCode.Key_RMENU ) || packet.IsPressed( (MDI.Key)KeyCode.Key_LMENU ) )
                        shiftState |= ShiftState.Alt;
                    if ( packet.IsPressed( (MDI.Key)KeyCode.Key_RSHIFT ) || packet.IsPressed( (MDI.Key)KeyCode.Key_LSHIFT ) )
                        shiftState |= ShiftState.Shift;

                    if ( this.EventListener != null )
                        ret = this.EventListener.KeyReleased( new KeyEventArgs( this, keyCode, 0 ) );
                    if ( ret == false )
                        break;
                }
            }
        }

        private KeyCode Convert( MDI.Key key )
        {
            switch ( key )
            {
                case MDI.Key.A:             return KeyCode.Key_A;
                case MDI.Key.AbntC1:        return KeyCode.Key_ABNT_C1;
                case MDI.Key.AbntC2:        return KeyCode.Key_ABNT_C2;
                case MDI.Key.Apostrophe:    return KeyCode.Key_APOSTROPHE;
                case MDI.Key.Applications:  return KeyCode.Key_APPS;
                case MDI.Key.AT:            return KeyCode.Key_AT;
                case MDI.Key.AX:            return KeyCode.Key_AX;
                case MDI.Key.B:             return KeyCode.Key_B;
                case MDI.Key.Backslash:     return KeyCode.Key_BACKSLASH;
                case MDI.Key.Backspace:     return KeyCode.Key_BACK;
                case MDI.Key.C:return KeyCode.Key_C;
                case MDI.Key.Calculator:return KeyCode.Key_CALCULATOR;
                case MDI.Key.CapsLock:return KeyCode.Key_CAPITAL;
                case MDI.Key.Colon:return KeyCode.Key_COLON;
                case MDI.Key.Comma:return KeyCode.Key_COMMA;
                case MDI.Key.Convert:return KeyCode.Key_CONVERT;
                case MDI.Key.D:return KeyCode.Key_D;
                case MDI.Key.D0:return KeyCode.Key_0;
                case MDI.Key.D1:return KeyCode.Key_1;
                case MDI.Key.D2:return KeyCode.Key_2;
                case MDI.Key.D3:return KeyCode.Key_3;
                case MDI.Key.D4:return KeyCode.Key_4;
                case MDI.Key.D5:return KeyCode.Key_5;
                case MDI.Key.D6:return KeyCode.Key_6;
                case MDI.Key.D7:return KeyCode.Key_7;
                case MDI.Key.D8:return KeyCode.Key_8;
                case MDI.Key.D9: return KeyCode.Key_9;
                case MDI.Key.Delete: return KeyCode.Key_DELETE;
                case MDI.Key.DownArrow: return KeyCode.Key_DOWN;
                case MDI.Key.E:return KeyCode.Key_E;
                case MDI.Key.End: return KeyCode.Key_END;
                case MDI.Key.Equals: return KeyCode.Key_EQUALS;
                case MDI.Key.Escape: return KeyCode.Key_ESCAPE;
                case MDI.Key.F: return KeyCode.Key_F;
                case MDI.Key.F1:
                    return KeyCode.Key_F1;
                case MDI.Key.F2:
                    return KeyCode.Key_F2;
                case MDI.Key.F3:
                    return KeyCode.Key_F3;
                case MDI.Key.F4:
                    return KeyCode.Key_F4;
                case MDI.Key.F5:
                    return KeyCode.Key_F5;
                case MDI.Key.F6:
                    return KeyCode.Key_F6;
                case MDI.Key.F7:
                    return KeyCode.Key_F7;
                case MDI.Key.F8:
                    return KeyCode.Key_F8;
                case MDI.Key.F9:
                    return KeyCode.Key_F9;
                case MDI.Key.F10:
                    return KeyCode.Key_F10;
                case MDI.Key.F11:
                    return KeyCode.Key_F11;
                case MDI.Key.F12:
                    return KeyCode.Key_F12;
                case MDI.Key.F13:
                    return KeyCode.Key_F13;
                case MDI.Key.F14:
                    return KeyCode.Key_F14;
                case MDI.Key.F15:
                    return KeyCode.Key_F15;
                case MDI.Key.G:
                    return KeyCode.Key_G;
                case MDI.Key.Grave:
                    return KeyCode.Key_GRAVE;
                case MDI.Key.H:
                    return KeyCode.Key_H;
                case MDI.Key.Home:
                    return KeyCode.Key_HOME;
                case MDI.Key.I:
                    return KeyCode.Key_I;
                case MDI.Key.Insert:
                    return KeyCode.Key_INSERT;
                case MDI.Key.J:
                    return KeyCode.Key_J;
                case MDI.Key.K: return KeyCode.Key_K;
                case MDI.Key.L: return KeyCode.Key_L;
                case MDI.Key.LeftAlt:
                    return KeyCode.Key_LMENU;
                case MDI.Key.LeftArrow:
                    return KeyCode.Key_LEFT;
                case MDI.Key.LeftBracket:
                    return KeyCode.Key_LBRACKET;
                case MDI.Key.LeftControl:
                    return KeyCode.Key_LCONTROL;
                case MDI.Key.LeftShift:
                    return KeyCode.Key_LSHIFT;
                case MDI.Key.LeftWindowsKey:
                    return KeyCode.Key_LWIN;
                case MDI.Key.M:
                    return KeyCode.Key_M;
                case MDI.Key.Mail:
                    return KeyCode.Key_MAIL;
                case MDI.Key.MediaSelect:
                    return KeyCode.Key_MEDIASELECT;
                case MDI.Key.MediaStop:
                    return KeyCode.Key_MEDIASTOP;
                case MDI.Key.Minus:
                    return KeyCode.Key_MINUS;
                case MDI.Key.Mute:
                    return KeyCode.Key_MUTE;
                case MDI.Key.MyComputer:
                    return KeyCode.Key_MYCOMPUTER;
                case MDI.Key.N:
                    return KeyCode.Key_N;
                case MDI.Key.O: return KeyCode.Key_O;
                case MDI.Key.Oem102:
                    return KeyCode.Key_OEM_102;
                case MDI.Key.P:
                    return KeyCode.Key_P;
                case MDI.Key.PageDown:
                    return KeyCode.Key_PGDOWN;
                case MDI.Key.PageUp:
                    return KeyCode.Key_PGUP;
                case MDI.Key.Pause:
                    return KeyCode.Key_PAUSE;
                case MDI.Key.Period:
                    return KeyCode.Key_PERIOD;
                case MDI.Key.PlayPause:
                    return KeyCode.Key_PLAYPAUSE;
                case MDI.Key.Power:
                    return KeyCode.Key_POWER;
                case MDI.Key.PreviousTrack:
                    return KeyCode.Key_PREVTRACK;
                case MDI.Key.PrintScreen:
                    return KeyCode.Key_SYSRQ;
                case MDI.Key.Q:
                    return KeyCode.Key_Q;
                case MDI.Key.R: return KeyCode.Key_R;
                case MDI.Key.Return:
                    return KeyCode.Key_RETURN;
                case MDI.Key.RightAlt:
                    return KeyCode.Key_RMENU;
                case MDI.Key.RightArrow:
                    return KeyCode.Key_RIGHT;
                case MDI.Key.RightBracket:
                    return KeyCode.Key_RBRACKET;
                case MDI.Key.RightControl:
                    return KeyCode.Key_RCONTROL;
                case MDI.Key.RightShift:
                    return KeyCode.Key_RSHIFT;
                case MDI.Key.RightWindowsKey:
                    return KeyCode.Key_RWIN;
                case MDI.Key.S:
                    return KeyCode.Key_S;
                case MDI.Key.ScrollLock:
                    return KeyCode.Key_SCROLL;
                case MDI.Key.Semicolon:
                    return KeyCode.Key_SEMICOLON;
                case MDI.Key.Slash:
                    return KeyCode.Key_SLASH;
                case MDI.Key.Sleep:
                    return KeyCode.Key_SLEEP;
                case MDI.Key.Space:
                    return KeyCode.Key_SPACE;
                case MDI.Key.Stop:
                    return KeyCode.Key_STOP;
                case MDI.Key.T:
                    return KeyCode.Key_T;
                case MDI.Key.Tab:
                    return KeyCode.Key_TAB;
                case MDI.Key.U:
                    return KeyCode.Key_U;
                case MDI.Key.Underline:
                    return KeyCode.Key_UNDERLINE;
                case MDI.Key.Unknown:
                    return KeyCode.Key_UNASSIGNED;
                case MDI.Key.Unlabeled:
                    return KeyCode.Key_UNLABELED;
                case MDI.Key.UpArrow:
                    return KeyCode.Key_UP;
                case MDI.Key.V:
                    return KeyCode.Key_V;
                case MDI.Key.VolumeDown:
                    return KeyCode.Key_VOLUMEDOWN;
                case MDI.Key.VolumeUp:
                    return KeyCode.Key_VOLUMEUP;
                case MDI.Key.W:
                    return KeyCode.Key_W;
                case MDI.Key.Wake:
                    return KeyCode.Key_WAKE;
                case MDI.Key.WebBack:
                    return KeyCode.Key_WEBBACK;
                case MDI.Key.WebFavorites:
                    return KeyCode.Key_WEBFAVORITES;
                case MDI.Key.WebForward:
                    return KeyCode.Key_WEBFORWARD;
                case MDI.Key.WebHome:
                    return KeyCode.Key_WEBHOME;
                case MDI.Key.WebRefresh:
                    return KeyCode.Key_WEBREFRESH;
                case MDI.Key.WebSearch:
                    return KeyCode.Key_WEBSEARCH;
                case MDI.Key.WebStop:
                    return KeyCode.Key_WEBSTOP;
                case MDI.Key.X:
                    return KeyCode.Key_X;
                case MDI.Key.Y: return KeyCode.Key_Y;
                case MDI.Key.Yen:
                    return KeyCode.Key_YEN;
                case MDI.Key.Z:
                    return KeyCode.Key_Z;
                    
                default: return KeyCode.Key_UNLABELED;
            }
        }

        #endregion Methods

        #region InputObject Implementation

        public override void Capture()
        {
            if ( this.IsBuffered )
                _readBuffered();
            else
                _read();
        }

		protected override void initialize()
		{
			_keyboard = new MDI.Keyboard( _directInput );

			//_keyboard.SetDataFormat( MDI.DeviceDataFormat.Keyboard );

			_keyboard.SetCooperativeLevel( ( (DirectXInputManager)Creator ).WindowHandle, _coopSettings );

			if ( IsBuffered )
			{
				_keyboard.Properties.BufferSize = _BUFFER_SIZE;
			}

			try
			{
				_keyboard.Acquire();
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
            get
            {
                return (int[])_keyboardState.Clone();
            }
        }

        public override bool IsKeyDown( KeyCode key )
        {
            return ( ( _keyboardState[ (int)key ] ) != 0 );
        }

        public override string AsString( KeyCode key )
        {
            return _keyboard.Properties.GetKeyName( (MDI.Key)key );
        }

        public override bool IsShiftState( ShiftState state )
        {
            return base.IsShiftState( state );
        }
        #endregion Keyboard Implementation

    }
}
