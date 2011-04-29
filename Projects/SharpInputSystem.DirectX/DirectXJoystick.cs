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
using System.Drawing;

using SWF = System.Windows.Forms;

using MDI = SlimDX.DirectInput;
using System.Collections.Generic;
using SharpInputSystem;
#endregion Namespace Declarations

namespace SharpInputSystem.DirectX
{
    ///<summary>
    ///
    /// </summary>
	class DirectXJoystick : Joystick
	{
	    #region Fields and Properties

	    private const int _BUFFER_SIZE = 124;

	    private MDI.CooperativeLevel _coopSettings;
	    private MDI.DirectInput _directInput;
	    private MDI.Joystick _joystick;
	    private DirectXForceFeedback _forceFeedback;
	    private JoystickInfo _joyInfo;

	    private Guid _deviceGuid;

	    private IntPtr _window;

	    private int _axisNumber;
	    private Dictionary<int, int> _axisMapping = new Dictionary<int, int>();

	    #endregion Fields and Properties

	    #region Construction and Destruction

	    public DirectXJoystick( InputManager creator, MDI.DirectInput device, bool buffered, MDI.CooperativeLevel coopSettings )
	    {
	        Creator = creator;
	        _directInput = device;
	        IsBuffered = buffered;
	        _coopSettings = coopSettings;
	        Type = InputType.Joystick;
	        EventListener = null;

	        _joyInfo = (JoystickInfo)( (DirectXInputManager)Creator ).CaptureDevice<Joystick>();

	        if ( _joyInfo == null )
	        {
	            throw new Exception( "No devices match requested type." );
	        }

	        _deviceGuid = _joyInfo.DeviceId;
	        Vendor = _joyInfo.Vendor;
	        DeviceID = _joyInfo.Id.ToString();

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
	            if ( _joystick != null )
	            {
	                try
	                {
	                    _joystick.Unacquire();
	                }
	                finally
	                {
	                    _joystick.Dispose();
	                    _joystick = null;
	                    _directInput = null;
	                    _forceFeedback = null;
	                }

	                ( (DirectXInputManager)Creator ).ReleaseDevice<Joystick>( _joyInfo );

	            }
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
	        MDI.Capabilities joystickCapabilities;

	        joystickCapabilities = _joystick.Capabilities;
	        this.AxisCount = (short)joystickCapabilities.AxesCount;
	        this.ButtonCount = (short)joystickCapabilities.ButtonCount;
	        this.HatCount = (short)joystickCapabilities.PovCount;
            
	        _axisNumber = 0;
	        _axisMapping.Clear();

	        ////Enumerate Force Feedback (if any)
	        if ( ( joystickCapabilities.Flags & MDI.DeviceFlags.ForceFeedback ) != 0 )
	        {
	            _forceFeedback = new DirectXForceFeedback( this );
	        }

	        ////Enumerate and set axis constraints (and check FF Axes)
	        foreach ( MDI.DeviceObjectInstance doi in _joystick.GetObjects() )
	        {
	            if ( ( doi.ObjectType & MDI.ObjectDeviceType.Axis ) != 0 )
	            _axisMapping.Add( doi.Offset, _axisNumber++ );

	            if ( ( doi.ObjectType & MDI.ObjectDeviceType.ForceFeedbackActuator ) != 0 )
	            {
	                if ( _forceFeedback == null )
	                {
	                    throw new Exception( "ForceFeedback Axis found but reported no ForceFeedback Effects!" );
	                }
	            }
	        }

	    }

	    #endregion Methods

	    #region SharpInputSystem.Joystick Implementation
		
	    public override void Capture()
	    {
	        IEnumerable<MDI.JoystickState> bufferedData = null;

	        if ( _joystick.Acquire().IsFailure )
	            return;
	        if ( _joystick.Poll().IsFailure )
	            return;

	        bufferedData = _joystick.GetBufferedData();

	        bool[] axisMoved = {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
	                              false,false,false,false,false,false,false,false};
	        bool[] sliderMoved = { false, false, false, false };

	        foreach ( MDI.JoystickState data in bufferedData )
	        {
	            int axis = 0;
	        }

	        //Check to see if any of the axes values have changed.. if so send events
	        if ( ( IsBuffered == true ) && ( EventListener != null ) )
	        {
	            JoystickEventArgs temp = new JoystickEventArgs( this, JoystickState );

	            //Update axes
	            for ( int i = 0; i < 24; i++ )
	                if ( axisMoved[ i ] )
	                    if ( EventListener.AxisMoved( temp, i ) == false )
	                        return;

	            //Now update sliders
	            for ( int i = 0; i < 4; i++ )
	                if ( sliderMoved[ i ] )
	                    if ( EventListener.SliderMoved( temp, i ) == false )
	                        return;
	        }

	    }

	    protected override void initialize()
	    {
	        JoystickState.Axis.Clear();

	        _joystick = new MDI.Joystick( _directInput, _deviceGuid );

	        //_joystick.SetDataFormat( MDI.DeviceDataFormat.Joystick );

	        _window = ( (DirectXInputManager)Creator ).WindowHandle;

	        _joystick.SetCooperativeLevel( _window, _coopSettings );

	        if ( IsBuffered )
	        {
	            _joystick.Properties.BufferSize = _BUFFER_SIZE;
	        }

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

	    public override IInputObjectInterface QueryInterface<T>()
	    {
	        if ( typeof( T ) == typeof( ForceFeedback ) )
	        {
	            return _forceFeedback;
	        }
	        return base.QueryInterface<T>();
	    }
	    #endregion SharpInputSystem.Joystick Implementation
	}
}
