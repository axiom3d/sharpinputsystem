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
		private bool grabMouse;
		private bool hideMouse;
		private bool mouseFocusLost;
		private LibX11.XEvent _xEvent;
		private bool _moved;
		private bool _warped;
		
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
			hideMouse = ((X11InputManager)creator).HideMouse;
		}

		protected override void initialize()
		{
			MouseState = new MouseState();
			
			_moved = false;
			_warped = false;
			
			_lastMouseX = _lastMouseY = 6;
			
			if ( _display != IntPtr.Zero ) 
				LibX11.XCloseDisplay( _display );
			_display = IntPtr.Zero;
				
			_display = LibX11.XOpenDisplay( IntPtr.Zero ); 
			if ( _display == IntPtr.Zero )
				throw new Exception( "X11Mouse.Initialize, can not open display" );
			var sir = LibX11.XSelectInput( _display, _window, LibX11.ButtonPressMask | LibX11.ButtonReleaseMask | LibX11.PointerMotionMask );
			if ( sir != 0 )
				throw new Exception( "X11Mouse.Initialize, X Error!" );
			
			//Warp mouse inside window
			LibX11.XWarpPointer( _display, IntPtr.Zero, _window, 0, 0, 0, 0, 6, 6 );
			
			grab( grabMouse ); 
			hide( hideMouse );	
						
			mouseFocusLost = false;
		}
		
		public override void Capture()
		{
			try
			{
				//Clear old relative values
				MouseState.X.Relative = MouseState.Y.Relative = MouseState.Z.Relative = 0;

				processXEvents();
				
				_warped = false;

				if ( _moved == true )	//Do the move
				{
						
					if ( EventListener != null && IsBuffered )
					{
						EventListener.MouseMoved( new MouseEventArgs( this, MouseState ) );
					}
					_moved = false;
				}
				
				if ( grabMouse )
				{
					if ( ((X11InputManager)Creator).GrabState == true )
					{
						if ( mouseFocusLost )
						{
							grab( true );
							hide( hideMouse );
							mouseFocusLost = false;
						}
					}
					else
					{
						if ( mouseFocusLost )
						{
							grab( false );
							hide( false );
							mouseFocusLost = true;
						}
					}
				}
					
			}
			catch ( Exception ex )
			{
				System.Console.WriteLine( ex.ToString() );
			}
		}
		
		private void processXEvents()
		{
			while ( LibX11.XPending( _display ) > 0 )
			{
				LibX11.XNextEvent( _display, ref _xEvent );
				MouseButtonID mb = MouseButtonID.Button3;

				switch ( _xEvent.type )
				{
					case LibX11.XEventName.MotionNotify:
						
						int sysX = _xEvent.MotionEvent.x;
						int sysY = _xEvent.MotionEvent.y;

						if ( _warped )
						{
							if ( sysX < 5 || sysX > MouseState.Width - 5 ||
								 sysY < 5 || sysY > MouseState.Height - 5 )
								continue;
						}
						int dx = sysX - _lastMouseX;
						int dy = sysY - _lastMouseY;
						_lastMouseX = sysX;
						_lastMouseY = sysY;
						MouseState.X.Absolute += dx;
						MouseState.Y.Absolute += dy;
						MouseState.X.Relative += dx;
						MouseState.Y.Relative += dy;
					
						if ( grabMouse )
						{
							if ( MouseState.X.Absolute < 0 )
								MouseState.X.Absolute = 0;
							else if ( MouseState.X.Absolute > MouseState.Width )
								MouseState.X.Absolute = MouseState.Width;
							if ( MouseState.Y.Absolute < 0 )
								MouseState.Y.Absolute = 0;
							else if ( MouseState.Y.Absolute > MouseState.Height )
								MouseState.Y.Absolute = MouseState.Height;		
						
							if ( mouseFocusLost == true )
							{
								if ( sysX < 5 || sysX > MouseState.Width - 5 ||
								 	 sysY < 5 || sysY > MouseState.Height - 5 )
								{
									_lastMouseX = MouseState.Width >> 1;
									_lastMouseY = MouseState.Height >> 1;
									LibX11.XWarpPointer( _display, IntPtr.Zero, _window, 0, 0, 0, 0, _lastMouseX, _lastMouseY );
									_warped = true;
								}
							}
						}
						_moved = true;
						break;
					
					case LibX11.XEventName.ButtonPress:
						mb = ToMouseButton( _xEvent.ButtonEvent.button );
						MouseState.Buttons |= (int)mb;
					
						((X11InputManager)Creator).GrabState = true;
	
						if ( _xEvent.ButtonEvent.button < 4 )
						{
							if ( IsBuffered == true && EventListener != null )
								if ( EventListener.MousePressed( new MouseEventArgs( this, MouseState ), mb ) == false )									
									return;
						}
						break;
					case LibX11.XEventName.ButtonRelease:
						mb = ToMouseButton( _xEvent.ButtonEvent.button );
						MouseState.Buttons &= ~(int)mb;
						if ( _xEvent.ButtonEvent.button < 4 )
						{
							if ( IsBuffered == true && EventListener != null )
								if ( EventListener.MouseReleased( new MouseEventArgs( this, MouseState ), mb ) == false )									
									return;
						}
	                    //The Z axis gets pushed/released pair message (this is up)
	                    else if( _xEvent.ButtonEvent.button == 4 )
	                    {
	                            MouseState.Z.Relative += 120;
	                            MouseState.Z.Absolute += 120;
	                            _moved = true;
	                    }
	                    //The Z axis gets pushed/released pair message (this is down)
	                    else if( _xEvent.ButtonEvent.button == 5 )
	                    {
	                            MouseState.Z.Relative -= 120;
	                            MouseState.Z.Absolute -= 120;
	                            _moved = true;
	                    }						
						break;
				}
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
			{
				grab( false );
				hide( false );
			}
		}
		
		private void grab( bool grabPointer )
		{
			if ( grabPointer )
				LibX11.XGrabPointer( _display, _window, true, 0, LibX11.GrabModeAsync, LibX11.GrabModeAsync, _window, IntPtr.Zero, LibX11.CurrentTime );
			else
				LibX11.XUngrabPointer( _display, LibX11.CurrentTime );
		}
		
		private void hide( bool hidePointer )
		{
//			if ( hidePointer )
//				LibX11.XDefineCursor( _display, _window, _cursor );
//			else
//				LibX11.XUndefineCursor( _display, _window );
		}

	}
}

