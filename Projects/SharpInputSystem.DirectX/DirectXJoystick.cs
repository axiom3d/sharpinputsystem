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
        
        // debugging stuff
        // private int[] hatsw = new int[4];
        // private int[] hatsw = { 4, 42, 420, 4200 };
        // private bool[] povMoved = { false, false, false, false };
        // end debugging stuff

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

        private void _read()
        {
            if (_joystick.Acquire().IsFailure)
                return;
            if (_joystick.Poll().IsFailure)
                return;

            MDI.JoystickState state = _joystick.GetCurrentState();

            int axis = 0;
            if (state.X != 0)
            {
                axis = 0;
                JoystickState.Axis[axis].Absolute = state.X;
            }
            if (state.Y != 0)
            {
                axis = 1;
                JoystickState.Axis[axis].Absolute = state.Y;
            }
            if (state.Z != 0)
            {
                axis = 2;
                JoystickState.Axis[axis].Absolute = state.Z;
            }
            if (state.RotationZ != 0)
            {
                axis = 3;
                JoystickState.Axis[axis].Absolute = state.RotationZ;
            }

            int[] hatsw = state.GetPointOfViewControllers();

            #region hatswDebugginStuff

            // debugging stuff
            if (hatsw == null)
            {
                Console.Write(String.Format("hatsw is null \n"));
                Console.ReadLine();
            }

            // Console.Write(String.Format("hatsw value:{0} length:{1} \n", hatsw[0], hatsw.Length));
            // this works, but only outputs the 1st hat/povswitch(" [0] ")

            // foreach (int hatIndex in hatsw)
            // this loop doesn't work
            // for (int hatIndex = 0; hatIndex < hatsw.Length; hatIndex++)
            // but THIS loop works?! WTH? (and the hatswitch values reported in the console are correct, and promptly updated on input)
            //{
            //    Console.Write(String.Format("hatsw index:{0}, value:{1} length:{2} \n", hatIndex, hatsw[hatIndex], hatsw.Length));
            //}
            //

            #endregion

            // trying to convert the DirectX  hat/pov values into Axiom Directions
            // foreach (int hatIndex in hatsw)
            //{
                //switch (hatsw[0])
                //{
                //    case -1:
                //        JoystickState.Povs[0].Direction = Pov.Position.Centered;
                //        break;
                //    case 0:
                //        JoystickState.Povs[0].Direction = Pov.Position.North;
                //        break;
                //    case 4500:
                //        JoystickState.Povs[0].Direction = Pov.Position.NorthEast;
                //        break;
                //    case 9000:
                //        JoystickState.Povs[0].Direction = Pov.Position.East;
                //        break;
                //    case 13500:
                //        JoystickState.Povs[0].Direction = Pov.Position.SouthEast;
                //        break;
                //    case 18000:
                //        JoystickState.Povs[0].Direction = Pov.Position.South;
                //        break;
                //    case 22500:
                //        JoystickState.Povs[0].Direction = Pov.Position.SouthWest;
                //        break;
                //    case 27000:
                //        JoystickState.Povs[0].Direction = Pov.Position.West;
                //        break;
                //    case 31500:
                //        JoystickState.Povs[0].Direction = Pov.Position.NorthWest;
                //        break;

                //}
            //} 
        }

        private void _readBuffered()
        {
            IEnumerable<MDI.JoystickState> bufferedData = null;

            if (_joystick.Acquire().IsFailure)
                return;
            if (_joystick.Poll().IsFailure)
                return;

            bufferedData = _joystick.GetBufferedData();

            bool[] axisMoved = {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
								  false,false,false,false,false,false,false,false};
            bool[] buttonsPressed = {false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
								  false,false,false,false,false,false,false,false};
            bool[] sliderMoved = { false, false, false, false };

            bool[] povMoved = { false, false, false, false };

            // original parsing routine
            //foreach (MDI.JoystickState data in bufferedData)
            //{
            //    int axis = 0;
            //}

            // begin parsing stickStates 
            foreach (MDI.JoystickState data in bufferedData)
            {
                int axis = 0;

                if (data.X != 0)
                {
                    axis = 0;
                    JoystickState.Axis[axis].Absolute = data.X;
                    axisMoved[axis] = true;
                }
                if (data.Y != 0)
                {
                    axis = 1;
                    JoystickState.Axis[axis].Absolute = data.Y;
                    axisMoved[axis] = true;
                }
                if (data.Z != 0)
                {
                    axis = 2;
                    JoystickState.Axis[axis].Absolute = data.Z;
                    axisMoved[axis] = true;
                }
                if (data.RotationZ != 0)
                {
                    axis = 3;
                    JoystickState.Axis[axis].Absolute = data.RotationZ;
                    axisMoved[axis] = true;
                }

                buttonsPressed = data.GetButtons();


                int[] hatsw = data.GetPointOfViewControllers();

                #region hatswDebugginStuff

                // debugging stuff
                if (hatsw == null)
                {
                    Console.Write(String.Format("hatsw is null \n"));
                    Console.ReadLine();
                }

                // Console.Write(String.Format("hatsw value:{0} length:{1} \n", hatsw[0], hatsw.Length));
                // this works, but only outputs the 1st hat/povswitch(" [0] ")

                // foreach (int hatIndex in hatsw)
                // this loop doesn't work
                // for (int hatIndex = 0; hatIndex < hatsw.Length; hatIndex++)
                // but THIS loop works?! WTH? (and the hatswitch values reported in the console are correct, and promptly updated on input)
                //{
                //    Console.Write(String.Format("hatsw index:{0}, value:{1} length:{2} \n", hatIndex, hatsw[hatIndex], hatsw.Length));
                //}
                //

                #endregion

                // converting DriverValues to AxiomDirections
                // neither of these loops work
                // foreach (int hatIndex in hatsw)
                // for (int hatIndex = 0; hatIndex < hatsw.Length; hatIndex++)
                //{
                //    switch (hatsw[hatIndex])
                //    {
                //        case -1:
                //            povMoved[hatIndex] = false;
                //            break;
                //        case 0:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.North;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 4500:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.NorthEast;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 9000:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.East;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 13500:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.SouthEast;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 18000:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.South;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 22500:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.SouthWest;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 27000:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.West;
                //            povMoved[hatIndex] = true;
                //            break;
                //        case 31500:
                //            JoystickState.Povs[hatIndex].Direction = Pov.Position.NorthWest;
                //            povMoved[hatIndex] = true;
                //            break;
                //    }
                //} 

            } // end parsing stickStates

            //Check to see if any of the axes values have changed.. if so send events
            if ((IsBuffered == true) && (EventListener != null))
            {
                JoystickEventArgs temp = new JoystickEventArgs(this, JoystickState);

                //Update axes
                for (int i = 0; i < axisMoved.Length; i++)
                    if (axisMoved[i])
                        if (EventListener.AxisMoved(temp, i) == false)
                            return;

                //Update buttons
                for (int i = 0; i < buttonsPressed.Length; i++)
                    if (buttonsPressed[i])
                        if (EventListener.ButtonPressed(temp, i) == false)
                            return;

                //Now update sliders
                //for ( int i = 0; i < 4; i++ )
                //    if ( sliderMoved[ i ] )
                //        if ( EventListener.SliderMoved( temp, i ) == false )
                //            return;

                //Now update POV
                //for (int i = 0; i < 4; i++)
                //    if (povMoved[i])
                //        if (EventListener.PovMoved(temp, i) == false)
                //            return;

            } //end event sending
        }

		#endregion Methods

		#region SharpInputSystem.Joystick Implementation
		
		public override void Capture()
		{
            if (this.IsBuffered)
                _readBuffered();
            else
                _read();

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
