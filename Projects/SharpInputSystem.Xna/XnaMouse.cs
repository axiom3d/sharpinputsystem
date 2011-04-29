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
using SharpInputSystem.Proxies.Xna;

//using Common.Logging;

#endregion Namespace Declarations

namespace SharpInputSystem
{
	/// <summary>
	/// Xna specialization of the Mouse class
	/// </summary>
	class XnaMouse : Mouse
	{
		#region Fields and Properties
		//private static readonly ILog log = LogManager.GetLogger( typeof( XnaMouse ) );

		// Variables for XnaKeyboard
		private MouseInfo _mInfo;
		private MouseStateProxy previousState;

		#endregion Fields and Properties

		#region Construction and Destruction

		public XnaMouse( InputManager creator, bool buffered )
		{
			Creator = creator;
			IsBuffered = buffered;
			Type = InputType.Mouse;
			EventListener = null;

			_mInfo = ( (XnaInputManager)Creator ).CaptureDevice<Mouse, MouseInfo>();

			if ( _mInfo == null )
			{
				throw new Exception( "No devices match requested type." );
			}

			//log.Debug( "XnaMouse device created." );
			previousState = MouseProxy.GetState();
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

				( (XnaInputManager)Creator ).ReleaseDevice<Mouse>( _mInfo );

				//log.Debug( "XnaMouse device disposed." );

			}
			isDisposed = true;

			// If it is available, make the call to the
			// base class's Dispose(Boolean) method
			base._dispose( disposeManagedResources );
		}

		#endregion Construction and Destruction

		#region Methods

		private bool _doMouseClick( int mouseButton, bool pressed )
		{
			if ( pressed && ( MouseState.Buttons & ( 1 << mouseButton ) ) == 0 )
			{
				MouseState.Buttons |= 1 << mouseButton; //turn the bit flag on
				if ( EventListener != null && IsBuffered )
					return EventListener.MousePressed( new MouseEventArgs( this, MouseState ), (MouseButtonID)mouseButton );
			}
			else if ( !pressed && ( MouseState.Buttons & ( 1 << mouseButton ) ) != 0 )
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

			MouseStateProxy xnaMouseState = MouseProxy.GetState();
			bool axesMoved = false;

			//Accumulate all axis movements for one axesMove message..
			//Buttons are fired off as they are found
			_doMouseClick( 0, xnaMouseState.LeftButton == MouseStateProxy.ButtonState.Pressed );
			_doMouseClick( 1, xnaMouseState.MiddleButton == MouseStateProxy.ButtonState.Pressed );
			_doMouseClick( 2, xnaMouseState.RightButton == MouseStateProxy.ButtonState.Pressed );
			_doMouseClick( 3, xnaMouseState.XButton1 == MouseStateProxy.ButtonState.Pressed );
			_doMouseClick( 4, xnaMouseState.XButton2 == MouseStateProxy.ButtonState.Pressed );

			//for (int i = 0; i < bufferedData.GetButtons().Length; i++)
			//{
			//    if (!_doMouseClick(i, packet))
			//        return;
			//}

			if ( xnaMouseState.X != previousState.X )
			{
				//if ( log.IsDebugEnabled )
				//    log.DebugFormat( "cX({0}):pX({1})", xnaMouseState.X, previousState.X );
				MouseState.X.Absolute = xnaMouseState.X;
				axesMoved = true;
			}

			if ( xnaMouseState.Y != previousState.Y )
			{
				//if ( log.IsDebugEnabled )
				//    log.DebugFormat( "cY({0}):pY({1})", xnaMouseState.Y, previousState.Y );
				MouseState.Y.Absolute = xnaMouseState.Y;
				axesMoved = true;
			}

			if ( xnaMouseState.ScrollWheelValue != previousState.ScrollWheelValue )
			{
				//if ( log.IsDebugEnabled )
				//    log.DebugFormat( "cZ({0}):pZ({1})", xnaMouseState.ScrollWheelValue, previousState.ScrollWheelValue );
				MouseState.Z.Absolute = xnaMouseState.ScrollWheelValue;
				axesMoved = true;
			}


			if ( axesMoved )
			{
				MouseState.X.Relative = previousState.X - xnaMouseState.X;
				MouseState.Y.Relative = previousState.Y - xnaMouseState.Y;
				MouseState.Z.Relative = previousState.ScrollWheelValue - xnaMouseState.ScrollWheelValue;

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
			previousState = xnaMouseState;
		}

		protected override void initialize()
		{
			MouseState.Clear();
		}

		#endregion Mouse Implementation
	}
}
