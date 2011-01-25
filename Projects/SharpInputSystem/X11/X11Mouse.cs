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
	public class X11Mouse : Mouse
	{
		private IntPtr _display;
		private LibX11.XEvent _xEvent;
		private Control _rwControl;
		private bool[] _buttons;

		public X11Mouse()
			: base( false )
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool Initialize()
		{
			_display = LibX11.XOpenDisplay( IntPtr.Zero );
			int grabResult = LibX11.XGrabPointer( _display, LibX11.XDefaultRootWindow( _display ),
				false, LibX11.MouseMovedPressedReleased, LibX11.GrabModeAsync,
				LibX11.GrabModeAsync, IntPtr.Zero, IntPtr.Zero, LibX11.CurrentTime );

			_xEvent = new LibX11.XEvent();
			IntPtr ctrPtr = (IntPtr)TutorialInputManager.Instance.RenderWindow.GetCustomAttribute( "WINDOW" );
			_rwControl = Control.FromHandle( ctrPtr );
			_buttons = new bool[ Enum.GetNames( typeof( MouseButtons ) ).Length ];
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="x11Button"></param>
		/// <returns></returns>
		public static MouseButtons ToMouseButton( int x11Button )
		{
			switch ( x11Button )
			{
				case 1:
					return MouseButtons.Left;
				case 3:
					return MouseButtons.Right;
				default:
					return MouseButtons.None;
			}
		}

		public override bool IsMouseDown( MouseButtons mb )
		{
			return _buttons[ (int)mb ];
		}
		public override bool IsMousePressed( MouseButtons mb )
		{
			return _buttons[ (int)mb ];
		}
		public override void Update( float timeSinceLastFrame )
		{
			return;
			int w, h, l, t, d;
			Point p = new Point();
			TutorialInputManager.Instance.RenderWindow.GetMetrics( out w, out h, out d, out l, out t );
			Rectangle rwLocation = new Rectangle( l, t, w, h );
			Point cs = _rwControl.PointToClient( new Point( l, t ) );
			LibX11.XNextEvent( _display, ref _xEvent );
			MouseButtons mb = MouseButtons.None;
			switch ( _xEvent.type.ToString() )
			{
				case "MotionNotify":
					p = new Point( _xEvent.MotionEvent.x, _xEvent.MotionEvent.y );
					_position = new Math.Vector2( p.X - l - cs.X, p.Y - t - cs.Y );
					OnMouseMoved( this, new MouseEventArgs( mb, 0, (int)_position.x, (int)_position.y, 0 ) );
					break;
				case "ButtonPress":
					mb = ToMouseButton( _xEvent.ButtonEvent.button );
					_buttons[ (int)mb ] = true;
					p = new Point( _xEvent.ButtonEvent.x, _xEvent.ButtonEvent.y );
					_position = new Math.Vector2( p.X - l - cs.X, p.Y - t - cs.Y );
					OnMouseDown( this, new MouseEventArgs( mb, 1, (int)_position.x, (int)_position.y, 0 ) );
					break;
				case "ButtonRelease":
					mb = ToMouseButton( _xEvent.ButtonEvent.button );
					_buttons[ (int)mb ] = false;
					p = new Point( _xEvent.ButtonEvent.x, _xEvent.ButtonEvent.y );
					_position = new Math.Vector2( p.X - l - cs.X, p.Y - t - cs.Y );
					OnMouseUp( this, new MouseEventArgs( mb, 1, (int)_position.x, (int)_position.y, 0 ) );
					break;
			}
		}

		public void Dispose()
		{
			if ( _display != IntPtr.Zero )
				LibX11.XUngrabPointer( _display, LibX11.CurrentTime );
		}
	}
}
