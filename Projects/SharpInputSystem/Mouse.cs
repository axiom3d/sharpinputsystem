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

#endregion Namespace Declarations

namespace SharpInputSystem
{
	/// <summary>
	/// Button ID for mouse devices
	/// </summary>
	public enum MouseButtonID
	{
		Left = 0,
		Right,
		Middle,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7
	};


	/// <summary>
	/// Represents the state of the mouse
	///	All members are valid for both buffered and non buffered mode
	/// </summary>
	public class MouseState
	{
		#region Fields and Properties

		#region Width Property

		private int _width;
		/// <summary>
		/// Represents the width of your display area.. used if mouse clipping
		/// or mouse grabbed in case of X11 - defaults to 50.. Make sure to set this
		/// and change when your size changes.. */
		/// </summary>
		public int Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		#endregion Width Property

		#region Height Property

		private int _height;
		/// <summary>
		/// Represents the height of your display area.. used if mouse clipping
		/// or mouse grabbed in case of X11 - defaults to 50.. Make sure to set this
		/// and change when your size changes.. */
		/// </summary>
		public int Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		#endregion Height Property

		#region X Property

		private Axis _x;
		/// <summary>
		/// X Axis Component
		/// </summary>
		public Axis X
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

		private Axis _y;
		/// <summary>
		/// Y Axis Component
		/// </summary>
		public Axis Y
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

		#region Z Property

		private Axis _z;
		/// <summary>
		/// Z Axis Component
		/// </summary>
		public Axis Z
		{
			get
			{
				return _z;
			}
			set
			{
				_z = value;
			}
		}

		#endregion Z Property

		#region Buttons Property

		private int _buttons;
		/// <summary>
		/// represents all buttons - bit position indicates button down
		/// </summary>
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

		#region Constructor

		public MouseState()
		{
			_height = _width = 50;
			_buttons = 0;

			_x = new Axis();
			_y = new Axis();
			_z = new Axis();
		}

		#endregion Constructor

		#region Methods

		/// <summary>
		/// Button down test
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public bool IsButtonDown( MouseButtonID button )
		{
			return ( _buttons & ( 1 << (int)button ) ) == 0 ? false : true;
		}

		/// <summary>
		/// Clear all the values
		/// </summary>
		public void Clear()
		{
			_x.Clear();
			_y.Clear();
			_z.Clear();
			_buttons = 0;
		}

		#endregion Methods
	};

	/// <summary>
	/// Specialized for mouse events 
	/// </summary>
	public sealed class MouseEventArgs : InputObjectEventArgs
	{

		#region Fields and Properties

		#region State Property

		private MouseState _state;
		/// <summary>
		/// The state of the mouse - including buttons and axes
		/// </summary>
		public MouseState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		#endregion State Property

		#endregion Fields and Properties

		#region Constructors

		public MouseEventArgs( InputObject obj, MouseState ms )
			: base( obj )
		{
			_state = ms;
		}

		#endregion Constructors

	};

	/// <summary>
	/// To recieve buffered mouse input, derive a class from this, and implement the
	///	methods here. Then set the call back to your Mouse instance with Mouse::setEventCallback
	/// </summary>
	public interface IMouseListener
	{
		bool MouseMoved( MouseEventArgs arg );
		bool MousePressed( MouseEventArgs arg, MouseButtonID id );
		bool MouseReleased( MouseEventArgs arg, MouseButtonID id );
	};

	/// <summary>
	/// Mouse base class. To be implemented by specific system (ie. DirectX Mouse)
	/// This class is useful as you remain OS independent using this common interface.
	/// </summary>
	public abstract class Mouse : InputObject
	{
		#region Fields and Properties

		#region EventListener Property

		/// <summary>
		/// Used for buffered/actionmapping callback
		/// </summary>
		private IMouseListener _listener;
		/// <summary>
		/// Register/unregister a Mouse Listener - Only one allowed for simplicity. If broadcasting
		/// is neccessary, just broadcast from the callback you registered.
		/// </summary>
		public IMouseListener EventListener
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

		#region MouseState Property

		/// <summary>
		/// The state of the mouse.
		/// </summary>
		private MouseState _state = new MouseState();
		/// <summary>
		/// Returns the state of the mouse - is valid for both buffered and non buffered mode
		/// </summary>
		public MouseState MouseState
		{
			get
			{
				return _state;
			}
			protected set
			{
				_state = value;
			}
		}

		#endregion MouseState Property

		#endregion Fields and Properties
	};
}
