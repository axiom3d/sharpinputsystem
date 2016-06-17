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

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

//using Common.Logging;

using XInput = Microsoft.Xna.Framework.Input;
using MXF = Microsoft.Xna.Framework;

#endregion Namespace Declarations

namespace SharpInputSystem.Xna
{
	/// <summary>
	/// Xna specialization of the Keyboard
	/// </summary>
	class XnaKeyboard : Keyboard
	{
		#region Fields and Properties
		//private static readonly ILog log = LogManager.GetLogger( typeof( XnaKeyboard ) );

		// Variables for XnaKeyboard
		private Dictionary<XInput.Keys, KeyCode> _keyMap = new Dictionary<XInput.Keys, KeyCode>();
		private KeyboardInfo _kbInfo;
		private int[] _keyboardState = new int[ 256 ];

		#endregion Fields and Properties

		#region Construction and Destruction

		public XnaKeyboard( InputManager creator, bool buffered )
		{
			Creator = creator;
			IsBuffered = buffered;
			Type = InputType.Keyboard;
			EventListener = null;

			_kbInfo = (KeyboardInfo)( (XnaInputManager)Creator ).CaptureDevice<Keyboard>();

			if ( _kbInfo == null )
			{
				throw new Exception( "No devices match requested type." );
			}

			//log.Debug( "XnaKeyboard device created." );

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

				( (XnaInputManager)Creator ).ReleaseDevice<Keyboard>( _kbInfo );

				//log.Debug( "XnaKeyboard device disposed." );

			}

			// If it is available, make the call to the
			// base class's Dispose(Boolean) method
			base.Dispose( disposeManagedResources );
		}

		#endregion Construction and Destruction

		#region InputObject Implementation

		public override void Capture()
		{
			XInput.KeyboardState currentState = XInput.Keyboard.GetState( MXF.PlayerIndex.One );
			XInput.Keys[] pressedKeys = currentState.GetPressedKeys();

			bool keyReleased;
			KeyCode testKey;

			//Process KeyRelease
			for ( int key = 0; key < _keyboardState.Length; key++ )
			{
				if ( _keyboardState[ key ] != 0 )
				{
					keyReleased = true;
					for ( int pressed = 0; pressed < pressedKeys.Length; pressed++ )
					{
						if ( _keyMap.TryGetValue( pressedKeys[ pressed ], out testKey ) && (KeyCode)key == testKey )
						{
							keyReleased = currentState.IsKeyDown( pressedKeys[ pressed ] );
						}
					}
					if ( keyReleased )
					{
						_keyboardState[ key ] = 0;
						if ( IsBuffered && EventListener != null )
						{
							if ( EventListener.KeyReleased( new KeyEventArgs( this, (KeyCode)key, 0 ) ) == false )
								break;
						}
					}
				}
			}

			//Process KeyDowns
			for ( int key = 0; key < pressedKeys.Length; key++ )
			{
				if ( _keyMap.TryGetValue( pressedKeys[ key ], out testKey ) )
				{
					_keyboardState[ (int)testKey ] = 1;
					if ( IsBuffered && EventListener != null )
					{
						if ( testKey != KeyCode.Key_UNASSIGNED )
							if ( EventListener.KeyPressed( new KeyEventArgs( this, (KeyCode)key, (int)pressedKeys[ key ] ) ) == false )
								break;
					}
				}
			}
		}

		protected override void Initialize()
		{
			_keyMap.Add( (XInput.Keys) 0x00, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x09, KeyCode.Key_TAB );
			_keyMap.Add( (XInput.Keys) 0x06, KeyCode.Key_BACK );
			_keyMap.Add( (XInput.Keys) 0x13, KeyCode.Key_RETURN );
			_keyMap.Add( (XInput.Keys) 0x14, KeyCode.Key_CAPITAL );
			_keyMap.Add( (XInput.Keys) 0x1b, KeyCode.Key_ESCAPE );
			_keyMap.Add( (XInput.Keys) 0x20, KeyCode.Key_SPACE );
			_keyMap.Add( (XInput.Keys) 0x21, KeyCode.Key_PGUP );
			_keyMap.Add( (XInput.Keys) 0x22, KeyCode.Key_PGDOWN );
			_keyMap.Add( (XInput.Keys) 0x26, KeyCode.Key_UP );
			_keyMap.Add( (XInput.Keys) 0x27, KeyCode.Key_RIGHT );
			_keyMap.Add( (XInput.Keys) 0x23, KeyCode.Key_END );
			_keyMap.Add( (XInput.Keys) 0x24, KeyCode.Key_HOME );
			_keyMap.Add( (XInput.Keys) 0x25, KeyCode.Key_LEFT );
			_keyMap.Add( (XInput.Keys) 0x28, KeyCode.Key_DOWN );
			_keyMap.Add( (XInput.Keys) 0x2e, KeyCode.Key_DELETE );
			_keyMap.Add( (XInput.Keys) 0x2d, KeyCode.Key_INSERT );
			_keyMap.Add( (XInput.Keys) 0x30, KeyCode.Key_0 );
			_keyMap.Add( (XInput.Keys) 0x31, KeyCode.Key_1 );
			_keyMap.Add( (XInput.Keys) 0x32, KeyCode.Key_2 );
			_keyMap.Add( (XInput.Keys) 0x33, KeyCode.Key_3 );
			_keyMap.Add( (XInput.Keys) 0x34, KeyCode.Key_4 );
			_keyMap.Add( (XInput.Keys) 0x35, KeyCode.Key_5 );
			_keyMap.Add( (XInput.Keys) 0x36, KeyCode.Key_6 );
			_keyMap.Add( (XInput.Keys) 0x37, KeyCode.Key_7 );
			_keyMap.Add( (XInput.Keys) 0x38, KeyCode.Key_8 );
			_keyMap.Add( (XInput.Keys) 0x39, KeyCode.Key_9 );
			_keyMap.Add( (XInput.Keys) 0x41, KeyCode.Key_A );
			_keyMap.Add( (XInput.Keys) 0x42, KeyCode.Key_B );
			_keyMap.Add( (XInput.Keys) 0x43, KeyCode.Key_C );
			_keyMap.Add( (XInput.Keys) 0x44, KeyCode.Key_D );
			_keyMap.Add( (XInput.Keys) 0x45, KeyCode.Key_E );
			_keyMap.Add( (XInput.Keys) 0x46, KeyCode.Key_F );
			_keyMap.Add( (XInput.Keys) 0x47, KeyCode.Key_G );
			_keyMap.Add( (XInput.Keys) 0x48, KeyCode.Key_H );
			_keyMap.Add( (XInput.Keys) 0x49, KeyCode.Key_I );
			_keyMap.Add( (XInput.Keys) 0x4a, KeyCode.Key_J );
			_keyMap.Add( (XInput.Keys) 0x4b, KeyCode.Key_K );
			_keyMap.Add( (XInput.Keys) 0x4c, KeyCode.Key_L );
			_keyMap.Add( (XInput.Keys) 0x4d, KeyCode.Key_M );
			_keyMap.Add( (XInput.Keys) 0x4e, KeyCode.Key_N );
			_keyMap.Add( (XInput.Keys) 0x4f, KeyCode.Key_O );
			_keyMap.Add( (XInput.Keys) 0x50, KeyCode.Key_P );
			_keyMap.Add( (XInput.Keys) 0x51, KeyCode.Key_Q );
			_keyMap.Add( (XInput.Keys) 0x52, KeyCode.Key_R );
			_keyMap.Add( (XInput.Keys) 0x53, KeyCode.Key_S );
			_keyMap.Add( (XInput.Keys) 0x54, KeyCode.Key_T );
			_keyMap.Add( (XInput.Keys) 0x55, KeyCode.Key_U );
			_keyMap.Add( (XInput.Keys) 0x56, KeyCode.Key_V );
			_keyMap.Add( (XInput.Keys) 0x57, KeyCode.Key_W );
			_keyMap.Add( (XInput.Keys) 0x58, KeyCode.Key_X );
			_keyMap.Add( (XInput.Keys) 0x59, KeyCode.Key_Y );
			_keyMap.Add( (XInput.Keys) 0x5a, KeyCode.Key_Z );
			_keyMap.Add( (XInput.Keys) 0x5b, KeyCode.Key_LWIN );
			_keyMap.Add( (XInput.Keys) 0x5c, KeyCode.Key_RWIN );
			_keyMap.Add( (XInput.Keys) 0x5d, KeyCode.Key_APPS );
			_keyMap.Add( (XInput.Keys) 0x5f, KeyCode.Key_SLEEP );
			_keyMap.Add( (XInput.Keys) 0x60, KeyCode.Key_NUMPAD0 );
			_keyMap.Add( (XInput.Keys) 0x61, KeyCode.Key_NUMPAD1 );
			_keyMap.Add( (XInput.Keys) 0x62, KeyCode.Key_NUMPAD2 );
			_keyMap.Add( (XInput.Keys) 0x63, KeyCode.Key_NUMPAD3 );
			_keyMap.Add( (XInput.Keys) 0x64, KeyCode.Key_NUMPAD4 );
			_keyMap.Add( (XInput.Keys) 0x65, KeyCode.Key_NUMPAD5 );
			_keyMap.Add( (XInput.Keys) 0x66, KeyCode.Key_NUMPAD6 );
			_keyMap.Add( (XInput.Keys) 0x67, KeyCode.Key_NUMPAD7 );
			_keyMap.Add( (XInput.Keys) 0x68, KeyCode.Key_NUMPAD8 );
			_keyMap.Add( (XInput.Keys) 0x69, KeyCode.Key_NUMPAD9 );
			_keyMap.Add( (XInput.Keys) 0x6a, KeyCode.Key_MULTIPLY );
			_keyMap.Add( (XInput.Keys) 0x6b, KeyCode.Key_ADD );
			_keyMap.Add( (XInput.Keys) 0x6c, KeyCode.Key_SLASH );
			_keyMap.Add( (XInput.Keys) 0x6d, KeyCode.Key_SUBTRACT );
			_keyMap.Add( (XInput.Keys) 0x6f, KeyCode.Key_DIVIDE );
			_keyMap.Add( (XInput.Keys) 0x70, KeyCode.Key_F1 );
			_keyMap.Add( (XInput.Keys) 0x71, KeyCode.Key_F2 );
			_keyMap.Add( (XInput.Keys) 0x72, KeyCode.Key_F3 );
			_keyMap.Add( (XInput.Keys) 0x73, KeyCode.Key_F4 );
			_keyMap.Add( (XInput.Keys) 0x74, KeyCode.Key_F5 );
			_keyMap.Add( (XInput.Keys) 0x75, KeyCode.Key_F6 );
			_keyMap.Add( (XInput.Keys) 0x76, KeyCode.Key_F7 );
			_keyMap.Add( (XInput.Keys) 0x77, KeyCode.Key_F8 );
			_keyMap.Add( (XInput.Keys) 0x78, KeyCode.Key_F9 );
			_keyMap.Add( (XInput.Keys) 0x79, KeyCode.Key_F10 );
			_keyMap.Add( (XInput.Keys) 0x7a, KeyCode.Key_F11 );
			_keyMap.Add( (XInput.Keys) 0x7b, KeyCode.Key_F12 );
			_keyMap.Add( (XInput.Keys) 0x7c, KeyCode.Key_F13 );
			_keyMap.Add( (XInput.Keys) 0x7d, KeyCode.Key_F14 );
			_keyMap.Add( (XInput.Keys) 0x7e, KeyCode.Key_F15 );
			_keyMap.Add( (XInput.Keys) 0x7f, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x80, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x81, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x82, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x83, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x84, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x85, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x86, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x87, KeyCode.Key_UNASSIGNED );
			_keyMap.Add( (XInput.Keys) 0x90, KeyCode.Key_NUMLOCK );
			_keyMap.Add( (XInput.Keys) 0x91, KeyCode.Key_SCROLL );
			_keyMap.Add( (XInput.Keys) 0xa0, KeyCode.Key_LSHIFT );
			_keyMap.Add( (XInput.Keys) 0xa1, KeyCode.Key_RSHIFT );
			_keyMap.Add( (XInput.Keys) 0xa2, KeyCode.Key_LCONTROL );
			_keyMap.Add( (XInput.Keys) 0xa3, KeyCode.Key_RCONTROL );
			_keyMap.Add( (XInput.Keys) 0xa4, KeyCode.Key_LMENU );
			_keyMap.Add( (XInput.Keys) 0xa5, KeyCode.Key_RMENU );
			_keyMap.Add( (XInput.Keys) 0xa6, KeyCode.Key_WEBBACK );
			_keyMap.Add( (XInput.Keys) 0xa7, KeyCode.Key_WEBFORWARD );
			_keyMap.Add( (XInput.Keys) 0xa8, KeyCode.Key_WEBREFRESH );
			_keyMap.Add( (XInput.Keys) 0xa9, KeyCode.Key_WEBSTOP );
			_keyMap.Add( (XInput.Keys) 0xaa, KeyCode.Key_WEBSEARCH );
			_keyMap.Add( (XInput.Keys) 0xab, KeyCode.Key_WEBFAVORITES );
			_keyMap.Add( (XInput.Keys) 0xac, KeyCode.Key_WEBHOME );
			_keyMap.Add( (XInput.Keys) 0xad, KeyCode.Key_MUTE );
			_keyMap.Add( (XInput.Keys) 0xae, KeyCode.Key_VOLUMEDOWN );
			_keyMap.Add( (XInput.Keys) 0xaf, KeyCode.Key_VOLUMEUP );
			_keyMap.Add( (XInput.Keys) 0xb0, KeyCode.Key_NEXTTRACK );
			_keyMap.Add( (XInput.Keys) 0xb1, KeyCode.Key_PREVTRACK );
			_keyMap.Add( (XInput.Keys) 0xb2, KeyCode.Key_MEDIASTOP );
			_keyMap.Add( (XInput.Keys) 0xb3, KeyCode.Key_PLAYPAUSE );
			_keyMap.Add( (XInput.Keys) 0xb4, KeyCode.Key_MAIL );
			_keyMap.Add( (XInput.Keys) 0xba, KeyCode.Key_SEMICOLON );
			_keyMap.Add( (XInput.Keys) 0xbb, KeyCode.Key_MEDIASELECT );
			_keyMap.Add( (XInput.Keys) 0xbc, KeyCode.Key_COMMA );
			_keyMap.Add( (XInput.Keys) 0xbd, KeyCode.Key_MINUS );
			_keyMap.Add( (XInput.Keys) 0xdb, KeyCode.Key_LBRACKET );
			_keyMap.Add( (XInput.Keys) 0xdd, KeyCode.Key_RBRACKET );
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
			return key.ToString();
		}

		public override bool IsShiftState( ShiftState state )
		{
			return base.IsShiftState( state );
		}

		#endregion Keyboard Implementation
	}
}
