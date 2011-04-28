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
	/// 
	/// </summary>
	public enum InputType
	{
		Unknown,
		Keyboard,
		Mouse,
		Joystick,
		Tablet
	}

	public interface IInputObjectInterface
	{
	}

	abstract public class InputObjectEventArgs
	{
		private InputObject _device;
		public InputObject Device
		{
			get
			{
				return _device;
			}
			protected set
			{
				_device = value;
			}
		}

		public InputObjectEventArgs( InputObject obj )
		{
			_device = obj;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	abstract public class InputObject : IDisposable
	{

		private InputType _type;
		/// <summary>
		/// Get the type of device
		/// </summary>
		public InputType Type
		{
			get
			{
				return _type;
			}
			protected set
			{
				_type = value;
			}
		}

		private string _vendor;
		/// <summary>
		/// Get the vender string name
		/// </summary>
		public String Vendor
		{
			get
			{
				return _vendor;
			}
			protected set
			{
				_vendor = value;
			}
		}

		private InputManager _creator;
		/// <summary>
		/// Returns this input object's creator
		/// </summary>
		public InputManager Creator
		{
			get
			{
				return _creator;
			}
			protected set
			{
				_creator = value;
			}
		}

		private bool _isBuffered;
		/// <summary>
		/// Get buffered mode - true is buffered, false otherwise
		/// </summary>
		public virtual bool IsBuffered
		{
			get
			{
				return _isBuffered;
			}
			protected set
			{
				_isBuffered = value;
			}
		}

		private string _deviceID;
		/// <summary>
		/// Not fully implemented yet
		/// </summary>
		public string DeviceID
		{
			get
			{
				return _deviceID;
			}
			protected set
			{
				_deviceID = value;
			}
		}

		/// <summary>
		/// Used for updating call once per frame before checking state or to update events
		/// </summary>
		abstract public void Capture();

		/// <summary>
		/// 
		/// </summary>
		abstract public void initialize();

		virtual public IInputObjectInterface QueryInterface<T>() where T : IInputObjectInterface
		{
			return default( T );
		}

		#region IDisposable Implementation

		private bool _disposed = false;
		protected bool isDisposed
		{
			get
			{
				return _disposed;
			}
			set
			{
				_disposed = value;
			}
		}

		protected virtual void _dispose( bool disposeManagedResources )
		{
			if ( !isDisposed )
			{
				if ( disposeManagedResources )
				{
					// Dispose managed resources.
				}

				// There are no unmanaged resources to release, but
				// if we add them, they need to be released here.
			}
			isDisposed = true;

			// If it is available, make the call to the
			// base class's Dispose(Boolean) method
			//base._dispose( disposeManagedResources );
		}

		public void Dispose()
		{
			_dispose( true );
			GC.SuppressFinalize( this );
		}

		~InputObject()
		{
			_dispose( false );
		}

		#endregion IDisposable Implementation
	}
}
