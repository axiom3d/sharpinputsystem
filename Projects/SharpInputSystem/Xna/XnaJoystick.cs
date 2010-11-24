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
using System.Text;

using log4net;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    class XnaJoystick : Joystick
    {
        #region Fields and Properties

        private static readonly ILog log = LogManager.GetLogger( typeof( XnaJoystick ) );

        private const int _BUFFER_SIZE = 124;

        private JoystickInfo _joyInfo;

        private Guid _deviceGuid;

        private int _axisNumber;
        private Dictionary<int, int> _axisMapping = new Dictionary<int, int>();

        #endregion Fields and Properties

        #region Construction and Destruction

        public XnaJoystick( InputManager creator, bool buffered )
        {
            Creator = creator;
            IsBuffered = buffered;
            Type = InputType.Joystick;
            EventListener = null;

            _joyInfo = (JoystickInfo)( (XnaInputManager)Creator ).CaptureDevice<Joystick>();

            if ( _joyInfo == null )
            {
                throw new Exception( "No devices match requested type." );
            }

            _deviceGuid = _joyInfo.DeviceId;
            Vendor = _joyInfo.Vendor;
            DeviceID = _joyInfo.Id.ToString();

            log.Debug( "XnaJoystick device created." );

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

                ( (XnaInputManager)Creator ).ReleaseDevice<Joystick>( _joyInfo );
            }
            isDisposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base._dispose( disposeManagedResources );
        }

        #endregion Construction and Destruction

        #region Methods

        protected void _enumerate()
        {
            //We can check force feedback here too
            //XInput.GamePadCapabilities joystickCapabilities;

            //joystickCapabilities = XInput.GamePad.GetCapabilities( (Xna.PlayerIndex)Int32.Parse( this.DeviceID ) );

            //if ( joystickCapabilities.HasAButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasBButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasXButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasYButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasStartButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasBackButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasLeftStickButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasRightStickButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasLeftShoulderButton ) this.ButtonCount++;
            //if ( joystickCapabilities.HasRightShoulderButton ) this.ButtonCount++;

            //if ( joystickCapabilities.HasDPadUpButton && joystickCapabilities.HasDPadDownButton &&
            //     joystickCapabilities.HasDPadLeftButton && joystickCapabilities.HasDPadRightButton )
            //    this.AxisCount++;
            //if ( joystickCapabilities.HasLeftTrigger ) this.AxisCount++;
            //if ( joystickCapabilities.HasRightTrigger ) this.AxisCount++;
            //if ( joystickCapabilities.HasLeftXThumbStick && joystickCapabilities.HasLeftYThumbStick ) this.AxisCount++;
            //if ( joystickCapabilities.HasRightXThumbStick && joystickCapabilities.HasRightYThumbStick ) this.AxisCount++;
            
            this.HatCount++;

            _axisNumber = 0;
            _axisMapping.Clear();

            //Enumerate Force Feedback (if any)

            //Enumerate and set axis constraints (and check FF Axes)

        }

        #endregion Methods

        #region InputObject Implementation

        public override void Capture()
        {
            //XInput.GamePadState currentState = XInput.GamePad.GetState( (Xna.PlayerIndex)Int32.Parse( DeviceID ) );
        }

        internal override void initialize()
        {
            //Enumerate all axes/buttons/sliders/etc before aquiring
            _enumerate();

            JoystickState.Axis.Capacity = this.AxisCount;
            for ( int i = 0; i < this.AxisCount; i++ )
            {
                JoystickState.Axis.Add( new Axis() );
            }
            JoystickState.Clear();

            Capture();

        }

        #endregion InputObject Implementation

    }
}