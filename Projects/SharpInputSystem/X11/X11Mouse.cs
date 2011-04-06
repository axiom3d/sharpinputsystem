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
	public class X11Mouse : Mouse, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		private IntPtr _display;
		private IntPtr _windowHandle;
		private LibX11.XEvent _xEvent;
		private IntPtr _window;
		private int _lastMouseX;
		private int _lastMouseY;
		private int _lastMouseZ;
		private bool[] _buttons;

		public X11Mouse( InputManager creator, bool buffered, IntPtr handle )
		{
			base.IsBuffered = buffered;
			_windowHandle = handle;
		}

		internal override void initialize()
		{
			try
			{
				_display = LibX11.XOpenDisplay( IntPtr.Zero );
				if ( _display == IntPtr.Zero )
					throw new Exception( "" );
				IntPtr window = LibX11.XDefaultRootWindow( _display );
				_window = window;
				//Warp mouse inside window
				LibX11.XWarpPointer( _display, IntPtr.Zero, window, 0, 0, 0, 0, 6, 6 );

				int grabResult = LibX11.XGrabPointer( _display, window, true, LibX11.ButtonPressMask | LibX11.ButtonReleaseMask | LibX11.PointerMotionMask, LibX11.GrabModeAsync,
				LibX11.GrabModeAsync, window, IntPtr.Zero, LibX11.CurrentTime );

				_xEvent = new LibX11.XEvent();

				_buttons = new bool[ Enum.GetNames( typeof( MouseButtonID ) ).Length ];
				LibX11.XWarpPointer( _display, IntPtr.Zero, window, 0, 0, 0, 0, 6, 6 );
			}
			catch
			{
				throw new Exception( "Failed to grab the Mouse pointer" );
			}
		}
		public override void Capture()
		{
			try
			{
				//Clear old relative values
				MouseState.X.Relative = MouseState.Y.Relative = MouseState.Z.Relative = 0;
				if ( _display == IntPtr.Zero )
					throw new Exception( "LOST DISPLAY" );
				while ( LibX11.XPending( _display ) > 0 )
				{
					LibX11.XNextEvent( _display, ref _xEvent );
					MouseButtonID mb = MouseButtonID.Button3;
					bool axisMoved = false;
					bool buttonDown = false;
					bool buttonUp = false;

					switch ( _xEvent.type.ToString() )
					{
						case "MotionNotify":
							axisMoved = true;
							MouseState.X.Absolute = _xEvent.MotionEvent.x;
							MouseState.Y.Absolute = _xEvent.MotionEvent.y;
							int sysX = _xEvent.MotionEvent.x;
							int sysY = _xEvent.MotionEvent.y;
							int dx = sysX - _lastMouseX;
							int dy = sysY - _lastMouseY;
							MouseState.X.Relative = dx;
							MouseState.Y.Relative = dy;

							_lastMouseX = sysX;
							_lastMouseY = sysY;

							break;
						case "ButtonPress":
							mb = ToMouseButton( _xEvent.ButtonEvent.button );

							MouseState.Buttons = (int)mb;
							MouseState.X.Absolute = _xEvent.MotionEvent.x;
							MouseState.Y.Absolute = _xEvent.MotionEvent.y;
							buttonDown = true;
							break;
						case "ButtonRelease":
							mb = ToMouseButton( _xEvent.ButtonEvent.button );
							MouseState.Buttons = (int)mb;
							MouseState.X.Absolute = _xEvent.MotionEvent.x;
							MouseState.Y.Absolute = _xEvent.MotionEvent.y;
							buttonUp = true;
							break;
					}
					//Clip values to window
					if ( MouseState.X.Absolute < 0 )
						MouseState.X.Absolute = 0;
					else if ( MouseState.X.Absolute > MouseState.Width )
						MouseState.X.Absolute = MouseState.Width;
					if ( MouseState.Y.Absolute < 0 )
						MouseState.Y.Absolute = 0;
					else if ( MouseState.Y.Absolute > MouseState.Height )
						MouseState.Y.Absolute = MouseState.Height;

					//Do the move
					if ( EventListener != null && axisMoved )
						EventListener.MouseMoved( new MouseEventArgs( this, MouseState ) );
					if ( EventListener != null && buttonUp )
						EventListener.MouseReleased( new MouseEventArgs( this, MouseState ), mb );
					if ( EventListener != null && buttonDown )
						EventListener.MousePressed( new MouseEventArgs( this, MouseState ), mb );
				}
			}
			catch ( Exception ex )
			{
				System.Console.WriteLine( ex.ToString() );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x11Button"></param>
		/// <returns></returns>
		public static MouseButtonID ToMouseButton( int x11Button )
		{
			switch ( x11Button )
			{
				case 1:
					return MouseButtonID.Left;
				case 3:
					return MouseButtonID.Right;
				default:
					return MouseButtonID.Middle;
			}
		}
		protected override void _dispose( bool managed )
		{
			if ( _display != IntPtr.Zero )
				LibX11.XUngrabPointer( _display, LibX11.CurrentTime );
		}
	}

	public class LinuxMouse
	{
		private IntPtr _window;
		private IntPtr _display;
		private bool _grabMouse;
		private bool _hideMouse;
		private bool _mouseFocusLost;
	}
}
