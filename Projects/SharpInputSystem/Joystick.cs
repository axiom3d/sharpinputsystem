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

#endregion Namespace Declarations

namespace SharpInputSystem
{
	/// <summary>
	/// POV / HAT Joystick component
	/// </summary>
	public class Pov : Component
	{
		#region Enumerations and Constants

		[Flags()]
		public enum Position
		{
			Centered = 0x00000000,
			North = 0x00000001,
			South = 0x00000010,
			East = 0x00000100,
			West = 0x00001000,
			NorthEast = 0x00000101,
			SouthEast = 0x00000110,
			NorthWest = 0x00001001,
			SouthWest = 0x00001010
		}

		#endregion Enumerations and Constants

		#region Fields and Properties

		#region Direction Property
		private Position _direction;
		public Position Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
		#endregion Direction Property

		#endregion Fields and Properties

		#region Constructors
		public Pov()
			: base( ComponentType.Pov )
		{
		}
		#endregion Constructors
	}

	/// <summary>
	/// A sliding axis
	/// </summary>
	/// <remarks>
	/// only used in Win32 Right Now
	/// </remarks>
	public class Slider : Component
	{
		#region Fields and Properties

		#region X Property
		private int _x;
		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}
		#endregion X Property

		#region Y Property
		private int _y;
		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}
		#endregion Y Property

		#endregion Fields and Properties

		#region Constructors
		public Slider()
			: base( ComponentType.Slider )
		{
		}
		#endregion Constructors
	}

	/// <summary>
	/// Represents the state of the joystick
	/// <remarks>
	/// All members are valid for both buffered and non buffered mode
	/// Sticks with zero values are not present on the device
	/// </remarks>
	public class JoystickState
	{
		#region Fields and Properties

		#region Axes Property

		private List<Axis> _axis = new List<Axis>();
		public List<Axis> Axis
		{
			get
			{
				return _axis;
			}
		}

		#endregion Axes Property

		#region Povs Property
		private Pov[] _povs = new Pov[ 4 ];
		public Pov[] Povs
		{
			get
			{
				return _povs;
			}
		}
		#endregion Povs Property

		#region Sliders Property
		private Slider[] _sliders = new Slider[ 4 ];
		public Slider[] Sliders
		{
			get
			{
				return _sliders;
			}
		}
		#endregion Sliders Property

		#region Buttons Property
		private int _buttons;
		public int Buttons
		{
			get
			{
				return _buttons;
			}
			set
			{
				_buttons = value;
			}
		}
		#endregion Buttons Property

		#endregion Fields and Properties

		#region Constructors

		public JoystickState()
		{
			Clear();
		}

		#endregion Constructors

		#region Methods

		public bool IsButtonDown( int button )
		{
			return ( ( _buttons & ( 1L << button ) ) == 0 ) ? false : true;
		}

		public void Clear()
		{
			_buttons = 0;
			foreach ( Axis ax in _axis )
			{
				ax.AbsoluteOnly = true;
				ax.Clear();
			}
		}

		#endregion Methods
	}

	/// <summary>
	/// Specialized for joystick events 
	/// </summary>
	public class JoystickEventArgs : InputObjectEventArgs
	{
		#region Fields and Properties

		#region State Property

		private JoystickState _state;
		public JoystickState State
		{
			get
			{
				return _state;
			}
		}
		#endregion State Property

		#endregion Fields and Properties

		#region Constructors

		public JoystickEventArgs( InputObject obj, JoystickState state )
			: base( obj )
		{
			_state = state;
		}

		#endregion Constructors

	}


	public interface IJoystickListener
	{
		bool ButtonPressed( JoystickEventArgs arg, int button );
		bool ButtonReleased( JoystickEventArgs arg, int button );

		bool AxisMoved( JoystickEventArgs arg, int axis );

		//Joystick Event, amd sliderID
		bool SliderMoved( JoystickEventArgs arg, int slider );
		//Joystick Event, amd povID
		bool PovMoved( JoystickEventArgs arg, int pov );
	}

	/// <summary>
	/// To recieve buffered joystick input, derive a class from this, and implement the
	/// methods here. Then set the call back to your JoyStick instance with JoyStick::setEventCallback
	/// Each JoyStick instance can use the same callback class, as a devID number will be provide
	/// to differentiate between connected joysticks. Of course, each can have a seperate
	/// callback instead.
	/// </summary>
	public abstract class BaseJoystickListener : IJoystickListener
	{
		#region IJoystickListener Members

		public abstract bool ButtonPressed( JoystickEventArgs arg, int button );

		public abstract bool ButtonReleased( JoystickEventArgs arg, int button );

		public abstract bool AxisMoved( JoystickEventArgs arg, int axis );

		public virtual bool SliderMoved( JoystickEventArgs arg, int slider )
		{
			return true;
		}

		public virtual bool PovMoved( JoystickEventArgs arg, int pov )
		{
			return true;
		}

		#endregion
	}

	public abstract class Joystick : InputObject
	{
		#region Enumerations and Constants

		const int Max_Axis = 32767;
		const int Min_Axis = -32768;

		#endregion Enumerations and Constants

		#region Fields and Properties

		#region AxisCount Property

		private short _axisCount;
		public short AxisCount
		{
			get
			{
				return _axisCount;
			}
			protected set
			{
				_axisCount = value;
			}
		}

		#endregion AxisCount Property

		#region ButtonCount Property

		private short _buttonCount;
		public short ButtonCount
		{
			get
			{
				return _buttonCount;
			}
			protected set
			{
				_buttonCount = value;
			}
		}

		#endregion ButtonCount Property

		#region HatCount Property

		private short _hatCount;
		public short HatCount
		{
			get
			{
				return _hatCount;
			}
			protected set
			{
				_hatCount = value;
			}
		}

		#endregion HatCount Property

		#region JoystickState Property

		private JoystickState _joystickState = new JoystickState();
		public JoystickState JoystickState
		{
			get
			{
				return _joystickState;
			}
		}

		#endregion JoystickState Property

		#region EventListener Property

		private IJoystickListener _listener;
		/// <summary>
		/// Resisters an object to recieve the Joystick events
		/// </summary>
		public IJoystickListener EventListener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		#endregion EventListener Property

		#endregion Fields and Properties

	}
}
