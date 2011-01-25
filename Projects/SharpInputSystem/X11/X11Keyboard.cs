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
		public enum Modifier
		{
			LShift,

		}
		/// <summary>
		/// 
		/// </summary>
		enum KeyState
		{
			/// <summary>
			/// Key is pressed and hold down.
			/// </summary>
			Held,
			/// <summary>
			/// Key is currently not pressed
			/// </summary>
			Up,
			/// <summary>
			/// Key was pressed 
			/// </summary>
			Pressed,
		}
		/// <summary>
		/// 
		/// </summary>
		private IntPtr _display;
		/// <summary>
		/// 
		/// </summary>
		private LibX11.XEvent _xEvent;
		/// <summary>
		/// 
		/// </summary>
		private KeyState[] _keyState;
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool Initialize()
		{
			if ( !OpenDisplay() )
				return false;
			if ( !EnableKeyAutoRepeat() )
				return false;
			if ( !GrabKeyboard() )
				return false;

			_xEvent = new LibX11.XEvent();
			_keyState = new KeyState[ Enum.GetNames( typeof( Keys ) ).Length ];
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override bool IsKeyDown( Keys key )
		{
			return _keyState[ (int)key ] == KeyState.Pressed;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override bool IsKeyPressed( Keys key )
		{
			return _keyState[ (int)key ] == KeyState.Pressed;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeSinceLastFrame"></param>
		public override void Update( float timeSinceLastFrame )
		{
			LibX11.XNextEvent( _display, ref _xEvent );
			uint x11KeyCode = 0;
			Keys key = Keys.None;

			switch ( _xEvent.type.ToString() )
			{
				case "KeyPress":
					x11KeyCode = GetX11KeyCode( _xEvent );
					key = LibX11.ToKeys( x11KeyCode );
					_keyState[ (int)key ] = KeyState.Pressed;
					break;
				case "KeyRelease":
					x11KeyCode = GetX11KeyCode( _xEvent );
					key = LibX11.ToKeys( x11KeyCode );
					_keyState[ (int)key ] = KeyState.Up;
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			CloseDisplay();
			UngrabKeyboard();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xevent"></param>
		/// <returns></returns>
		private uint GetX11KeyCode( LibX11.XEvent xevent )
		{
			return LibX11.XKeycodeToKeysym( _display, xevent.KeyEvent.keycode, 0 );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool OpenDisplay()
		{
			_display = LibX11.XOpenDisplay( IntPtr.Zero );
			return _display == IntPtr.Zero ? false : true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool CloseDisplay()
		{
			if ( _display == IntPtr.Zero )
				return false;

			LibX11.XCloseDisplay( _display );
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool EnableKeyAutoRepeat()
		{
			bool detectAuto = false;
			LibX11.XkbSetDetectableAutoRepeat( _display, true, ref detectAuto );
			return detectAuto;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool GrabKeyboard()
		{
			if ( LibX11.XGrabKeyboard( _display, LibX11.XDefaultRootWindow( _display ),
				true, LibX11.GrabModeAsync, LibX11.GrabModeAsync, LibX11.CurrentTime ) != 0 )
				return false;

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool UngrabKeyboard()
		{
			if ( _display == IntPtr.Zero )
				return false;

			LibX11.XUngrabKeyboard( _display, LibX11.CurrentTime );

			return true;
		}
	}
}
