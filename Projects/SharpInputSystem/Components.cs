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
	/// type specifier for all components (button, axis, etc)
	/// </summary>
	public enum ComponentType
	{
		Unknown,
		Button,
		Axis,
		Slider,
		Pov,
		Vector3
	}

	/// <summary>
	/// Base type for all device components (button, axis, etc)
	/// </summary>
	public class Component
	{
		public Component()
		{
			this.Type = ComponentType.Unknown;
		}

		public Component( ComponentType type )
		{
			this.Type = type;
		}

		private ComponentType _type;
		public ComponentType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
	}

	public class Button : Component
	{
		public Button()
			: base( ComponentType.Button )
		{
		}

		public Button( bool isPushed )
			: base( ComponentType.Button )
		{
			this.IsPushed = isPushed;
		}

		private bool _isPushed;
		public bool IsPushed
		{
			get
			{
				return _isPushed;
			}
			protected set
			{
				_isPushed = value;
			}
		}

	}

	public class Axis : Component
	{
		public Axis()
			: base( ComponentType.Axis )
		{
		}

		private int _absolute;
		public int Absolute
		{
			get
			{
				return _absolute;
			}
			set
			{
				_absolute = value;
			}
		}

		private int _relative;
		public int Relative
		{
			get
			{
				return _relative;
			}
			set
			{
				_relative = value;
			}
		}

		private bool _absoluteOnly;
		public bool AbsoluteOnly
		{
			get
			{
				return _absoluteOnly;
			}
			set
			{
				_absoluteOnly = value;
			}
		}

		public void Clear()
		{
			_absolute = _relative = 0;
		}
	}

	/// <summary>
	/// A 3D Vector component (perhaps an orientation, as in the WiiMote)
	/// </summary>
	public class Vector3 : Component
	{
		private float _x, _y, _z;

		public Vector3()
		{
		}

		public Vector3( float x, float y, float z )
			: base( ComponentType.Vector3 )
		{
			_x = x;
			_y = y;
			_z = z;
		}

		public void Clear()
		{
			_x = _y = _z = 0;
		}

	}
}
