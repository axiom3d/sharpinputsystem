#region LGPL License
/*
Sharp Input System Library
Copyright (C) 2007 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to the Phillip Castaneda for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#region Namespace Declarations

using System;
using System.Collections.Generic;

using SlimDX.DirectInput;

using SWF = System.Windows.Forms;
using System.Runtime.InteropServices;

using MDI = SlimDX.DirectInput;
using log4net;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    class DirectXMouse : Mouse
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
        private MDI.Device<MDI.MouseState> _mouse;
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

                    ( (DirectXInputManager)Creator ).ReleaseDevice<Mouse>( _msInfo );
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

        private bool _doMouseClick( int mouseButton, MDI.BufferedData<MDI.MouseState> bufferedData )
        {
            if (  bufferedData.Data.IsPressed( mouseButton ) )
            {
                MouseState.Buttons |= 1 << mouseButton; //turn the bit flag on
                if ( EventListener != null && IsBuffered )
                    return EventListener.MousePressed( new MouseEventArgs( this, MouseState ), (MouseButtonID)mouseButton );
            }
            else
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

            IEnumerable<MDI.BufferedData<MDI.MouseState>> bufferedData = _mouse.GetBufferedData();
            if ( bufferedData == null )
            {
                try
                {
                    _mouse.Acquire();
                    bufferedData = _mouse.GetBufferedData();

                    if ( bufferedData == null )
                        return;
                }
                catch ( Exception )
                {
                    return;
                }
            }

            bool axesMoved = false;

            //Accumulate all axis movements for one axesMove message..
            //Buttons are fired off as they are found
            foreach ( BufferedData<MDI.MouseState> packet in bufferedData )
            {
                for ( int i = 0; i < packet.Data.GetButtons().Length; i++ )
                {
                    if ( packet.Data.IsPressed( i ) )
                        if ( !_doMouseClick( 0, packet ) )
                            return;
                }

                if ( packet.Data.X != 0 )
                {
                    MouseState.X.Relative = packet.Data.X;
                    axesMoved = true;
                }

                if ( packet.Data.X != 0 )
                {
                    MouseState.Y.Relative = packet.Data.Y;
                    axesMoved = true;
                }

                if ( packet.Data.X != 0 )
                {
                    MouseState.Z.Relative = packet.Data.Z;
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

        internal override void initialize()
        {
            MouseState.Clear();

            _mouse = new MDI.Device<MDI.MouseState>( _directInput, MDI.SystemGuid.Mouse );

            _mouse.Properties.AxisMode = DeviceAxisMode.Absolute;

            //_mouse.SetDataFormat( MDI.DeviceDataFormat.Mouse );

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
                throw new Exception( "Failed to aquire mouse using DirectInput.", e );
            }
        }

        #endregion Mouse Implementation

    }
}
