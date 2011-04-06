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
using System.Runtime.InteropServices;

#endregion Namespace Declarations

namespace SharpInputSystem
{
	/// <summary>
	/// 
	/// </summary>
	public class X11Keyboard : Keyboard
	{
		protected IntPtr _display;
		protected IntPtr _windowhandle;
		protected IntPtr _window;

		Dictionary<LibX11.KeySym, KeyCode> keyConversion = new Dictionary<LibX11.KeySym, KeyCode>();
		protected int[] keyBuffer = new int[ 256 ];
		public X11Keyboard()
		{
			keyConversion.Add( LibX11.KeySym.XK_1, KeyCode.Key_1 );
			keyConversion.Add( LibX11.KeySym.XK_2, KeyCode.Key_2 );
			keyConversion.Add( LibX11.KeySym.XK_3, KeyCode.Key_3 );
			keyConversion.Add( LibX11.KeySym.XK_4, KeyCode.Key_4 );
			keyConversion.Add( LibX11.KeySym.XK_5, KeyCode.Key_5 );
			keyConversion.Add( LibX11.KeySym.XK_6, KeyCode.Key_6 );
			keyConversion.Add( LibX11.KeySym.XK_7, KeyCode.Key_7 );
			keyConversion.Add( LibX11.KeySym.XK_8, KeyCode.Key_8 );
			keyConversion.Add( LibX11.KeySym.XK_9, KeyCode.Key_9 );
			keyConversion.Add( LibX11.KeySym.XK_0, KeyCode.Key_0 );

			keyConversion.Add( LibX11.KeySym.XK_backSpace, KeyCode.Key_BACK );
			keyConversion.Add( LibX11.KeySym.XK_minus, KeyCode.Key_MINUS );
			keyConversion.Add( LibX11.KeySym.XK_equal, KeyCode.Key_EQUALS );
			keyConversion.Add( LibX11.KeySym.XK_space, KeyCode.Key_SPACE );
			keyConversion.Add( LibX11.KeySym.XK_comma, KeyCode.Key_COMMA );
			keyConversion.Add( LibX11.KeySym.XK_period, KeyCode.Key_PERIOD );

			keyConversion.Add( LibX11.KeySym.XK_backslash, KeyCode.Key_BACKSLASH );
			keyConversion.Add( LibX11.KeySym.XK_slash, KeyCode.Key_SLASH );
			keyConversion.Add( LibX11.KeySym.XK_bracketleft, KeyCode.Key_LBRACKET );
			keyConversion.Add( LibX11.KeySym.XK_bracketright, KeyCode.Key_RBRACKET );

			keyConversion.Add( LibX11.KeySym.XK_Escape, KeyCode.Key_ESCAPE );
			keyConversion.Add( LibX11.KeySym.XK_Caps_Lock, KeyCode.Key_CAPITAL );

			keyConversion.Add( LibX11.KeySym.XK_Tab, KeyCode.Key_TAB );
			keyConversion.Add( LibX11.KeySym.XK_Return, KeyCode.Key_RETURN );
			keyConversion.Add( LibX11.KeySym.XK_Control_L, KeyCode.Key_LCONTROL );
			keyConversion.Add( LibX11.KeySym.XK_Control_R, KeyCode.Key_RCONTROL );

			keyConversion.Add( LibX11.KeySym.XK_colon, KeyCode.Key_COLON );
			keyConversion.Add( LibX11.KeySym.XK_semicolon, KeyCode.Key_SEMICOLON );
			keyConversion.Add( LibX11.KeySym.XK_apostrophe, KeyCode.Key_APOSTROPHE );
			keyConversion.Add( LibX11.KeySym.XK_grave, KeyCode.Key_GRAVE );


			keyConversion.Add( LibX11.KeySym.XK_a, KeyCode.Key_A );
			keyConversion.Add( LibX11.KeySym.XK_b, KeyCode.Key_B );
			keyConversion.Add( LibX11.KeySym.XK_c, KeyCode.Key_C );
			keyConversion.Add( LibX11.KeySym.XK_d, KeyCode.Key_D );
			keyConversion.Add( LibX11.KeySym.XK_e, KeyCode.Key_E );
			keyConversion.Add( LibX11.KeySym.XK_f, KeyCode.Key_F );
			keyConversion.Add( LibX11.KeySym.XK_g, KeyCode.Key_G );
			keyConversion.Add( LibX11.KeySym.XK_h, KeyCode.Key_H );
			keyConversion.Add( LibX11.KeySym.XK_i, KeyCode.Key_I );
			keyConversion.Add( LibX11.KeySym.XK_j, KeyCode.Key_J );
			keyConversion.Add( LibX11.KeySym.XK_k, KeyCode.Key_K );
			keyConversion.Add( LibX11.KeySym.XK_l, KeyCode.Key_L );
			keyConversion.Add( LibX11.KeySym.XK_m, KeyCode.Key_M );
			keyConversion.Add( LibX11.KeySym.XK_n, KeyCode.Key_N );
			keyConversion.Add( LibX11.KeySym.XK_o, KeyCode.Key_O );
			keyConversion.Add( LibX11.KeySym.XK_p, KeyCode.Key_P );
			keyConversion.Add( LibX11.KeySym.XK_q, KeyCode.Key_Q );
			keyConversion.Add( LibX11.KeySym.XK_r, KeyCode.Key_R );
			keyConversion.Add( LibX11.KeySym.XK_s, KeyCode.Key_S );
			keyConversion.Add( LibX11.KeySym.XK_t, KeyCode.Key_T );
			keyConversion.Add( LibX11.KeySym.XK_u, KeyCode.Key_U );
			keyConversion.Add( LibX11.KeySym.XK_v, KeyCode.Key_V );
			keyConversion.Add( LibX11.KeySym.XK_w, KeyCode.Key_W );
			keyConversion.Add( LibX11.KeySym.XK_x, KeyCode.Key_X );
			keyConversion.Add( LibX11.KeySym.XK_y, KeyCode.Key_Y );
			keyConversion.Add( LibX11.KeySym.XK_z, KeyCode.Key_Z );

			keyConversion.Add( LibX11.KeySym.XK_F1, KeyCode.Key_F1 );
			keyConversion.Add( LibX11.KeySym.XK_F2, KeyCode.Key_F2 );
			keyConversion.Add( LibX11.KeySym.XK_F3, KeyCode.Key_F3 );
			keyConversion.Add( LibX11.KeySym.XK_F4, KeyCode.Key_F4 );
			keyConversion.Add( LibX11.KeySym.XK_F5, KeyCode.Key_F5 );
			keyConversion.Add( LibX11.KeySym.XK_F6, KeyCode.Key_F6 );
			keyConversion.Add( LibX11.KeySym.XK_F7, KeyCode.Key_F7 );
			keyConversion.Add( LibX11.KeySym.XK_F8, KeyCode.Key_F8 );
			keyConversion.Add( LibX11.KeySym.XK_F9, KeyCode.Key_F9 );
			keyConversion.Add( LibX11.KeySym.XK_F10, KeyCode.Key_F10 );
			keyConversion.Add( LibX11.KeySym.XK_F11, KeyCode.Key_F11 );
			keyConversion.Add( LibX11.KeySym.XK_F12, KeyCode.Key_F12 );
			keyConversion.Add( LibX11.KeySym.XK_F13, KeyCode.Key_F13 );
			keyConversion.Add( LibX11.KeySym.XK_F14, KeyCode.Key_F14 );
			keyConversion.Add( LibX11.KeySym.XK_F15, KeyCode.Key_F15 );

			//keypad
			keyConversion.Add( LibX11.KeySym.XK_KP_0, KeyCode.Key_NUMPAD0 );
			keyConversion.Add( LibX11.KeySym.XK_KP_1, KeyCode.Key_NUMPAD1 );
			keyConversion.Add( LibX11.KeySym.XK_KP_2, KeyCode.Key_NUMPAD2 );
			keyConversion.Add( LibX11.KeySym.XK_KP_3, KeyCode.Key_NUMPAD3 );
			keyConversion.Add( LibX11.KeySym.XK_KP_4, KeyCode.Key_NUMPAD4 );
			keyConversion.Add( LibX11.KeySym.XK_KP_5, KeyCode.Key_NUMPAD5 );
			keyConversion.Add( LibX11.KeySym.XK_KP_6, KeyCode.Key_NUMPAD6 );
			keyConversion.Add( LibX11.KeySym.XK_KP_7, KeyCode.Key_NUMPAD7 );
			keyConversion.Add( LibX11.KeySym.XK_KP_8, KeyCode.Key_NUMPAD8 );
			keyConversion.Add( LibX11.KeySym.XK_KP_9, KeyCode.Key_NUMPAD9 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Add, KeyCode.Key_ADD );
			keyConversion.Add( LibX11.KeySym.XK_KP_Subtract, KeyCode.Key_SUBTRACT );
			keyConversion.Add( LibX11.KeySym.XK_KP_Decimal, KeyCode.Key_DECIMAL );
			keyConversion.Add( LibX11.KeySym.XK_KP_Equal, KeyCode.Key_NUMPADEQUALS );
			keyConversion.Add( LibX11.KeySym.XK_KP_Divide, KeyCode.Key_DIVIDE );
			keyConversion.Add( LibX11.KeySym.XK_KP_Multiply, KeyCode.Key_MULTIPLY );
			keyConversion.Add( LibX11.KeySym.XK_KP_Enter, KeyCode.Key_NUMPADENTER );

			//Keypad with numlock off
			keyConversion.Add( LibX11.KeySym.XK_KP_Home, KeyCode.Key_NUMPAD7 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Up, KeyCode.Key_NUMPAD8 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Page_Up, KeyCode.Key_NUMPAD9 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Left, KeyCode.Key_NUMPAD4 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Begin, KeyCode.Key_NUMPAD5 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Right, KeyCode.Key_NUMPAD6 );
			keyConversion.Add( LibX11.KeySym.XK_KP_End, KeyCode.Key_NUMPAD1 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Down, KeyCode.Key_NUMPAD2 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Page_Down, KeyCode.Key_NUMPAD3 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Insert, KeyCode.Key_NUMPAD0 );
			keyConversion.Add( LibX11.KeySym.XK_KP_Delete, KeyCode.Key_DECIMAL );

			keyConversion.Add( LibX11.KeySym.XK_Up, KeyCode.Key_UP );
			keyConversion.Add( LibX11.KeySym.XK_Down, KeyCode.Key_DOWN );
			keyConversion.Add( LibX11.KeySym.XK_Left, KeyCode.Key_LEFT );
			keyConversion.Add( LibX11.KeySym.XK_Right, KeyCode.Key_RIGHT );

			keyConversion.Add( LibX11.KeySym.XK_Page_Up, KeyCode.Key_PGUP );
			keyConversion.Add( LibX11.KeySym.XK_Page_Down, KeyCode.Key_PGDOWN );
			keyConversion.Add( LibX11.KeySym.XK_Home, KeyCode.Key_HOME );
			keyConversion.Add( LibX11.KeySym.XK_End, KeyCode.Key_END );

			keyConversion.Add( LibX11.KeySym.XK_Num_Lock, KeyCode.Key_NUMLOCK );
			keyConversion.Add( LibX11.KeySym.XK_Print, KeyCode.Key_SYSRQ );
			keyConversion.Add( LibX11.KeySym.XK_Scroll_Lock, KeyCode.Key_SCROLL );
			keyConversion.Add( LibX11.KeySym.XK_Pause, KeyCode.Key_PAUSE );

			keyConversion.Add( LibX11.KeySym.XK_Shift_R, KeyCode.Key_RSHIFT );
			keyConversion.Add( LibX11.KeySym.XK_Shift_L, KeyCode.Key_LSHIFT );
			keyConversion.Add( LibX11.KeySym.XK_Alt_R, KeyCode.Key_RMENU );
			keyConversion.Add( LibX11.KeySym.XK_Alt_L, KeyCode.Key_LMENU );

			keyConversion.Add( LibX11.KeySym.XK_Insert, KeyCode.Key_INSERT );
			keyConversion.Add( LibX11.KeySym.XK_Delete, KeyCode.Key_DELETE );

			keyConversion.Add( LibX11.KeySym.XK_Super_L, KeyCode.Key_LWIN );
			keyConversion.Add( LibX11.KeySym.XK_Super_R, KeyCode.Key_RWIN );
			keyConversion.Add( LibX11.KeySym.XK_Menu, KeyCode.Key_APPS );
		}

		internal override void initialize()
		{
			if ( _display != IntPtr.Zero )
				LibX11.XCloseDisplay( _display );
			_display = IntPtr.Zero;

			if ( ( _display = LibX11.XOpenDisplay( IntPtr.Zero ) ) == IntPtr.Zero )
				throw new Exception( "X11Keyboard.Initialize, can not open display" );

			if ( ( LibX11.XSelectInput( _display, _window, LibX11.KeyPressMask | LibX11.KeyReleaseMask ) == LibX11.BadWindow ) )
				throw new Exception( "X11Keyboard.Initialize, X Error!" );
		}

		public override int[] KeyStates
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsKeyDown( KeyCode key )
		{
			throw new NotImplementedException();
		}

		public override string AsString( KeyCode key )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Capture()
		{
			int key = 0;
			LibX11.XEvent xevent = new LibX11.XEvent();
			//X11InputManager linman = null;
			while ( LibX11.XPending( _display ) > 0 )
			{
				LibX11.XNextEvent( _display, ref xevent );
				if ( xevent.type == LibX11.XEventName.KeyPress )
				{
					int character = 0;
					if ( TextMode != TextTranslationMode.Off )
					{
						StringBuilder sb = new StringBuilder( 6 );
						LibX11.XLookupString( ref xevent.KeyEvent, ref sb, 6, out key, IntPtr.Zero );

						if ( TextMode == TextTranslationMode.Unicode )
						{
							character = UTF8toUTF32( sb );
						}
						else if ( TextMode == TextTranslationMode.Ascii )
						{
							character = (int)sb[ 0 ];
						}
					}

					//Mask out the modifier states X11 sets and read again
					xevent.KeyEvent.state &= ~LibX11.ShiftMask;
					xevent.KeyEvent.state &= ~LibX11.LockMask;
					StringBuilder b = new StringBuilder();
					LibX11.XLookupString( ref xevent.KeyEvent, ref b, 0, out key, IntPtr.Zero );

					injectKeyDown( key, character );

					//Check for Alt-Tab
					//if ( ( xevent.KeyEvent.state & LibX11.Mod1Mask ) != 0 && key == (int)LibX11.KeySym.XK_Tab )
					//	linman.GrabsState = false;
				}
				else if ( xevent.type == LibX11.XEventName.KeyRelease )
				{
					int character = 0;

					if ( !IsKeyRepeat( xevent ) )
					{
						//Mask out the modifier states X sets.. or we will get improper values
						xevent.KeyEvent.state &= ~LibX11.ShiftMask;
						xevent.KeyEvent.state &= ~LibX11.LockMask;
						if ( TextMode != TextTranslationMode.Off )
						{
							StringBuilder sb = new StringBuilder( 6 );
							LibX11.XLookupString( ref xevent.KeyEvent, ref sb, 0, out key, IntPtr.Zero );
							if ( TextMode == TextTranslationMode.Unicode )
							{
								character = UTF8toUTF32( sb );
							}
							else if ( TextMode == TextTranslationMode.Ascii )
							{
								character = (int)sb[ 0 ];
							}
						}

						injectKeyUp( key, character );
					}
				}
			}//end while

			//If grabbing mode is on.. Handle focus lost/gained via Alt-Tab and mouse clicks
		}

		private int UTF8toUTF32( StringBuilder buffer )
		{
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private bool injectKeyDown( int key, int text )
		{
			KeyCode kc = keyConversion[ (LibX11.KeySym)key ];
			keyBuffer[ key ] = 1;

			//Turn on modifier flags
			if ( kc == KeyCode.Key_LCONTROL || kc == KeyCode.Key_RCONTROL )
				shiftState |= ShiftState.Ctrl;
			else if ( kc == KeyCode.Key_LSHIFT || kc == KeyCode.Key_RSHIFT )
				shiftState |= ShiftState.Shift;
			else if ( kc == KeyCode.Key_LMENU || kc == KeyCode.Key_RMENU )
				shiftState |= ShiftState.Alt;

			if ( IsBuffered && EventListener != null )
				return EventListener.KeyPressed( new KeyEventArgs( this, kc, text ) );

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private bool injectKeyUp( int key, int text )
		{
			KeyCode kc = keyConversion[ (LibX11.KeySym)key ];
			keyBuffer[ key ] = 0;

			//Turn off modifier flags
			if ( kc == KeyCode.Key_LCONTROL || kc == KeyCode.Key_RCONTROL )
				shiftState &= ~ShiftState.Ctrl;
			else if ( kc == KeyCode.Key_LSHIFT || kc == KeyCode.Key_RSHIFT )
				shiftState &= ~ShiftState.Shift;
			else if ( kc == KeyCode.Key_LMENU || kc == KeyCode.Key_RMENU )
				shiftState &= ~ShiftState.Alt;

			if ( IsBuffered && EventListener != null )
				return EventListener.KeyReleased( new KeyEventArgs( this, kc, text ) );

			return true;
		}

		private bool IsKeyRepeat( LibX11.XEvent even )
		{
			return false;
		}
	}
}
