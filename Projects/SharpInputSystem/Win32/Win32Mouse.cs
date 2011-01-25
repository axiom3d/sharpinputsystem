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
	public class Win32Mouse : Mouse
	{
		/// <summary>
		/// 
		/// </summary>
		private const int KEY_PRESSED = 0x8000;
		/// <summary>
		/// 
		/// </summary>
		private enum VirtualKeyStates : int
		{
			VK_LBUTTON = 0x01,
			VK_RBUTTON = 0x02,
			VK_MBUTTON = 0x04,
			//
			VK_XBUTTON1 = 0x05,
			VK_XBUTTON2 = 0x06,
		}
		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		static extern bool GetCursorPos( out Point lpPoint );
		[DllImport( "user32.dll" )]
		static extern short GetKeyState( VirtualKeyStates nVirtKey );
		[DllImport( "user32.dll" )]
		static extern short GetKeyState( int key );
		Control _rwControl;
		private const int LM_INDEX = 0;
		private const int RM_INDEX = 1;
		private const int MM_INDEX = 2;
		private short[] _currentStates = new short[ 3 ];
		private short[] _lastStates = new short[ 3 ];
		private bool[] _isDown = new bool[ 3 ];
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownMouse"></param>
		public Win32Mouse( bool ownMouse )
			: base( ownMouse )
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override bool Initialize()
		{
			IntPtr ctrPtr = (IntPtr)TutorialInputManager.Instance.RenderWindow.GetCustomAttribute( "WINDOW" );
			_rwControl = Control.FromHandle( ctrPtr );
			_rwControl.MouseWheel += new MouseEventHandler( _rwControl_MouseWheel );
			return true;
		}

		void _rwControl_MouseWheel( object sender, MouseEventArgs e )
		{
			OnMouseWheel( sender, e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeSinceLastFrame"></param>
		public override void Update( float timeSinceLastFrame )
		{
			Point p = new Point();
			GetCursorPos( out p );
			int w, h, l, t, d;

			Input.TutorialInputManager.Instance.RenderWindow.GetMetrics( out w, out h, out d, out l, out t );
			Rectangle rwLocation = new Rectangle( l, t, w, h );
			Point cs = _rwControl.PointToClient( new Point( l, t ) );
			_position = new Axiom.Math.Vector2( p.X - l - cs.X, p.Y - t - cs.Y );
			if ( _position.x != _lastPosition.x || _position.y != _position.y )
			{
				OnMouseMoved( this, new MouseEventArgs( MouseButtons.None, 0, (int)_position.x, (int)_position.y, 0 ) );
			}
			_relativePosition = _position - _lastPosition;
			//early out
			//if (rwLocation.Contains(new Point((int)_position.x, (int)_position.y)))
			{
				_currentStates[ LM_INDEX ] = GetKeyState( VirtualKeyStates.VK_LBUTTON );
				_currentStates[ RM_INDEX ] = GetKeyState( VirtualKeyStates.VK_RBUTTON );
				_currentStates[ MM_INDEX ] = GetKeyState( VirtualKeyStates.VK_MBUTTON );

				int index = LM_INDEX;
				if ( _lastStates[ index ] != _currentStates[ index ] )
				{
					if ( Convert.ToBoolean( GetKeyState( VirtualKeyStates.VK_LBUTTON ) & KEY_PRESSED ) )
					{
						OnMouseDown( this, new MouseEventArgs( MouseButtons.Left, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = true;
					}
					else if ( _isDown[ index ] )
					{
						OnMouseUp( this, new MouseEventArgs( MouseButtons.Left, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = false;
					}
				}
				index = RM_INDEX;
				if ( _lastStates[ index ] != _currentStates[ index ] )
				{
					if ( Convert.ToBoolean( GetKeyState( VirtualKeyStates.VK_RBUTTON ) & KEY_PRESSED ) )
					{
						OnMouseDown( this, new MouseEventArgs( MouseButtons.Right, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = true;
					}
					else if ( _isDown[ index ] )
					{
						OnMouseUp( this, new MouseEventArgs( MouseButtons.Right, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = false;
					}
				}
				index = MM_INDEX;
				if ( _lastStates[ index ] != _currentStates[ index ] )
				{
					if ( Convert.ToBoolean( GetKeyState( VirtualKeyStates.VK_MBUTTON ) & KEY_PRESSED ) )
					{
						OnMouseDown( this, new MouseEventArgs( MouseButtons.Middle, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = true;
					}
					else if ( _isDown[ index ] )
					{
						OnMouseUp( this, new MouseEventArgs( MouseButtons.Middle, 1, (int)_position.x, (int)_position.y, 0 ) );
						_isDown[ index ] = false;
					}
				}
			}
			_lastStates[ LM_INDEX ] = _currentStates[ LM_INDEX ];
			_lastStates[ RM_INDEX ] = _currentStates[ RM_INDEX ];
			_lastStates[ MM_INDEX ] = _currentStates[ MM_INDEX ];
			_lastPosition = _position;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mb"></param>
		/// <returns></returns>
		public override bool IsMousePressed( MouseButtons mb )
		{

			switch ( mb )
			{
				case MouseButtons.Left:
					MouseButtonState state = GetKeyState( mb, VirtualKeyStates.VK_LBUTTON );
					return state.IsPressed;
					//Console.WriteLine(GetKeyState(VirtualKeyStates.VK_LBUTTON));
					//return Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LBUTTON) & KEY_PRESSED);
					//state = VirtualKeyStates.VK_LBUTTON;
					return Convert.ToBoolean( GetKeyState( VirtualKeyStates.VK_LBUTTON ) == 1 );
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mb"></param>
		/// <returns></returns>
		public override bool IsMouseDown( MouseButtons mb )
		{
			switch ( mb )
			{
				case MouseButtons.Left:
					return _isDown[ LM_INDEX ];
				case MouseButtons.Right:
					return _isDown[ RM_INDEX ];
				case MouseButtons.Middle:
					return _isDown[ MM_INDEX ];
				default:
					throw new NotImplementedException();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private MouseButtonState GetKeyState( MouseButtons mb, VirtualKeyStates key )
		{
			short keyState = GetKeyState( key );
			int low = Low( keyState ), high = High( keyState );
			bool toggled = low == 1;
			bool pressed = high == 1;
			return new MouseButtonState( mb, pressed, toggled );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private KeyStateInfo GetKeyState( Keys key )
		{
			short keyState = GetKeyState( (int)key );
			int low = Low( keyState ), high = High( keyState );
			bool toggled = low == 1;
			bool pressed = high == 1;
			return new KeyStateInfo( key, pressed, toggled );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyState"></param>
		/// <returns></returns>
		private int High( int keyState )
		{
			return keyState > 0 ? keyState >> 0x10
					: ( keyState >> 0x10 ) & 0x1;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyState"></param>
		/// <returns></returns>
		private int Low( int keyState )
		{
			return keyState & 0xffff;
		}
	}
}
