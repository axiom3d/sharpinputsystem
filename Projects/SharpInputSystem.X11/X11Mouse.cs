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

namespace SharpInputSystem
{
	public class X11Mouse : Mouse, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		private IntPtr _display;
		private IntPtr _window;
		private int _lastMouseX;
		private int _lastMouseY;
		private int _lastMouseZ;
		private bool[] _buttons;
		private bool grabMouse;
		private bool hideMouse;
		private bool mouseFocusLost;
		private LibX11.XEvent _xEvent;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="handle">
		/// A <see cref="IntPtr"/>
		/// </param>
		public X11Mouse( InputManager creator, IntPtr handle, bool buffered )
		{
			base.Creator = creator;
			base.IsBuffered = buffered;
			
			_window = handle;
			grabMouse = ((X11InputManager)creator).GrabMouse;
		}

		protected override void initialize()
		{
			MouseState = new MouseState();
			
			//moved = false;
			//warped = false;
			
			_lastMouseX = _lastMouseY = 6;
			_lastMouseZ = 0;
			
			if ( _display != IntPtr.Zero ) 
				LibX11.XCloseDisplay( _display );
			_display = IntPtr.Zero;
				
			_display = LibX11.XOpenDisplay( IntPtr.Zero ); 
			if ( _display == IntPtr.Zero )
				throw new Exception( "X11Mouse.Initialize, can not open display" );
			
			if ( LibX11.XSelectInput( _display, _window, LibX11.ButtonPressMask | LibX11.ButtonReleaseMask | LibX11.PointerMotionMask ) == LibX11.BadWindow )
				throw new Exception( "X11Mouse.Initialize, X Error!" );
			
			int grabResult = LibX11.XGrabPointer( _display, _window, true, LibX11.ButtonPressMask | LibX11.ButtonReleaseMask | LibX11.PointerMotionMask, LibX11.GrabModeAsync, LibX11.GrabModeAsync, IntPtr.Zero /* _window */, IntPtr.Zero, LibX11.CurrentTime );

			//Warp mouse inside window
			LibX11.XWarpPointer( _display, IntPtr.Zero, _window, 0, 0, 0, 0, 6, 6 );
			
			_buttons = new bool[ Enum.GetNames( typeof( MouseButtonID ) ).Length ];
		}
		
		public override void Capture()
		{
			try
			{
				//Clear old relative values
				MouseState.X.Relative = MouseState.Y.Relative = MouseState.Z.Relative = 0;
				//int w, h, l = 0, t = 0, d;
				//  Point p = new Point();
				//  Point _position = new Point();
				// Rectangle rwLocation = new Rectangle(0, 0, 100, 100);
				// Point cs = _rwControl.PointToClient(new Point(0, 0));
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
							//	Console.WriteLine(string.Format("DX {0}, DY {1}", dx,dy));
							//	Console.WriteLine(string.Format("OX {0}, OY {1}", _lastMouseX,_lastMouseY));
							_lastMouseX = sysX;
							_lastMouseY = sysY;
							//Console.WriteLine(string.Format("NX {0}, NY {1}", _lastMouseX,_lastMouseY));
							break;
						case "ButtonPress":
							mb = ToMouseButton( _xEvent.ButtonEvent.button );
							// _buttons[(int)mb] = true;
							MouseState.Buttons = (int)mb;
							//  p = new Point(_xEvent.ButtonEvent.x, _xEvent.ButtonEvent.y);
							//  _position = new Point(p.X - l - cs.X, p.Y - t - cs.Y);
							MouseState.X.Absolute = _xEvent.MotionEvent.x;
							//_position.X;
							MouseState.Y.Absolute = _xEvent.MotionEvent.y;
							//_position.Y;
							buttonDown = true;
							//OnMouseDown(this, new MouseEventArgs(mb, 1, (int)_position.x, (int)_position.y, 0));
							break;
						case "ButtonRelease":
							mb = ToMouseButton( _xEvent.ButtonEvent.button );
							//  _buttons[(int)mb] = false;
							MouseState.Buttons = (int)mb;
							//  p = new Point(_xEvent.ButtonEvent.x, _xEvent.ButtonEvent.y);
							//  _position = new Point(p.X - l - cs.X, p.Y - t - cs.Y);
							MouseState.X.Absolute = _xEvent.MotionEvent.x;
							//_position.X;
							MouseState.Y.Absolute = _xEvent.MotionEvent.y;
							//_position.Y;
							buttonUp = true;
							//OnMouseUp(this, new MouseEventArgs(mb, 1, (int)_position.x, (int)_position.y, 0));
							break;
					}
					//LibX11.XWarpPointer( _display, IntPtr.Zero, _window, 0, 0, 0, 0, MouseState.Width >> 1, MouseState.Height >> 1 );
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

