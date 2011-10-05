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

using SlimDX.DirectInput;

using SWF = System.Windows.Forms;
using System.Runtime.InteropServices;

using MDI = SlimDX.DirectInput;
using Common.Logging;
#endregion Namespace Declarations

namespace SharpInputSystem.DirectX
{
	class DirectXMouse : SharpInputSystem.Mouse
	{
		[StructLayout( LayoutKind.Sequential )]
		private struct POINT
		{
			public int X;
			public int Y;

			public POINT( int x, int y )
			{
				this.X = x;
				this.Y = y;
			}
		}

		#region Fields and Properties
		
		private static readonly ILog log = LogManager.GetLogger( typeof( DirectXMouse ) );

		private const int _BUFFER_SIZE = 64;

		private MDI.CooperativeLevel _coopSettings;
		private MDI.DirectInput _directInput;
		private MDI.Mouse _mouse;
		private MouseInfo _msInfo;

		private IntPtr _window;

		[DllImport( "user32.dll" )]
		private static extern bool GetCursorPos( out POINT lpPoint );
		[DllImport( "user32.dll" )]
		private static extern bool ScreenToClient( IntPtr hWnd, ref POINT lpPoint );

		#endregion Fields and Properties

		#region Construction and Destruction

		public DirectXMouse( InputManager creator, MDI.DirectInput device, bool buffered, MDI.CooperativeLevel coopSettings )
		{
			Creator = creator;
			_directInput = device;
			IsBuffered = buffered;
			_coopSettings = coopSettings;
			Type = InputType.Mouse;
			EventListener = null;

			_msInfo = (MouseInfo)( (DirectXInputManager)Creator ).CaptureDevice<Mouse>();

			if ( _msInfo == null )
			{
				throw new Exception( "No devices match requested type." );
			}
			
			log.Debug( "DirectXMouse device created." );

		}

		protected override void _dispose( bool disposeManagedResources )
		{
			if ( !isDisposed )
			{
				if ( disposeManagedResources )
				{
					// Dispose managed resources.

					if ( _mouse != null )
					{
						try
						{
							_mouse.Unacquire();
						}
						catch
						{
							// NOTE : This is intentional
						}

						finally
						{
							_mouse.Dispose();
							_mouse = null;
						}
					}

					
					( (DirectXInputManager)Creator ).ReleaseDevice<SharpInputSystem.Mouse>( _msInfo );
				}
				// There are no unmanaged resources to release, but
				// if we add them, they need to be released here.

				log.Debug( "DirectXMouse device disposed." );
			}

			// If it is available, make the call to the
			// base class's Dispose(Boolean) method
			base._dispose( disposeManagedResources );
		}

		#endregion Construction and Destruction

		#region Methods

		private bool _doMouseClick( int mouseButton, MDI.MouseState bufferedData )
		{
			if ( bufferedData.IsPressed( mouseButton ) && ( MouseState.Buttons & ( 1 << mouseButton ) ) == 0 )
			{
				MouseState.Buttons |= 1 << mouseButton; //turn the bit flag on
				if ( EventListener != null && IsBuffered )
					return EventListener.MousePressed( new MouseEventArgs( this, MouseState ), (MouseButtonID)mouseButton );
			}
			else if ( bufferedData.IsReleased( mouseButton ) && ( MouseState.Buttons & ( 1 << mouseButton ) ) != 0 )
			{
				MouseState.Buttons &= ~( 1 << mouseButton ); //turn the bit flag off
				if ( EventListener != null && IsBuffered )
					return EventListener.MouseReleased( new MouseEventArgs( this, MouseState ), (MouseButtonID)mouseButton );
			}

			return true;
		}

		#endregion Methods

		#region Mouse Implementation

		public override void Capture()
		{
			// Clear Relative movement
			MouseState.X.Relative = MouseState.Y.Relative = MouseState.Z.Relative = 0;
			if ( SlimDX.Result.Last.IsFailure )
				return;

			IEnumerable<MDI.MouseState> bufferedData = null;
			try
			{
				bufferedData = _mouse.GetBufferedData();
			}
			catch ( Exception ex ) {}

			if ( SlimDX.Result.Last.IsFailure || bufferedData == null )
			{
				if ( _mouse.Acquire().IsFailure )
					return;
				if ( _mouse.Poll().IsFailure )
					return;

				try
				{
					bufferedData = _mouse.GetBufferedData();
					if ( SlimDX.Result.Last.IsFailure || bufferedData == null )
						return;
				}
				catch ( Exception ex )
				{
					return;
				}
			}
			bool axesMoved = false;

			//Accumulate all axis movements for one axesMove message..
			//Buttons are fired off as they are found
			foreach ( MDI.MouseState packet in bufferedData )
			{
				for ( int i = 0; i < packet.GetButtons().Length; i++ )
				{
					if ( !_doMouseClick( i, packet ) )
						return;
				}

				if ( packet.X != 0 )
				{
					MouseState.X.Relative = packet.X;
					axesMoved = true;
				}

				if ( packet.Y != 0 )
				{
					MouseState.Y.Relative = packet.Y;
					axesMoved = true;
				}

				if ( packet.Z != 0 )
				{
					MouseState.Z.Relative = packet.Z;
					axesMoved = true;
				}

			}

			if ( axesMoved )
			{
				if ( ( this._coopSettings & MDI.CooperativeLevel.Nonexclusive ) == MDI.CooperativeLevel.Nonexclusive )
				{
					//DirectInput provides us with meaningless values, so correct that
					POINT point;
					GetCursorPos( out point );
					ScreenToClient( _window, ref point );
					MouseState.X.Absolute = point.X;
					MouseState.Y.Absolute = point.Y;
				}
				else
				{
					MouseState.X.Absolute += MouseState.X.Relative;
					MouseState.Y.Absolute += MouseState.Y.Relative;
				}
				MouseState.Z.Absolute += MouseState.Z.Relative;

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
				if ( EventListener != null && IsBuffered )
					EventListener.MouseMoved( new MouseEventArgs( this, MouseState ) );
			}
		}

		protected override void initialize()
		{
			MouseState.Clear();

			_mouse = new MDI.Mouse( _directInput );

			_mouse.Properties.AxisMode = DeviceAxisMode.Relative;

			_window = ( (DirectXInputManager)Creator ).WindowHandle;

			_mouse.SetCooperativeLevel( _window, _coopSettings );

			if ( IsBuffered )
			{
				_mouse.Properties.BufferSize = _BUFFER_SIZE;
			}

			try
			{
				_mouse.Acquire();
			}
			catch ( Exception e )
			{
				throw new Exception( "Failed to acquire mouse using DirectInput.", e );
			}
		}

		#endregion Mouse Implementation

	}
}
