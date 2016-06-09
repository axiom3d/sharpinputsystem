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

#endregion Namespace Declaration

namespace SharpInputSystem
{
	/// <summary>
	/// Win32 InputManager specialization
	/// </summary>
	class Win32InputManager : InputManager, InputObjectFactory
	{
		#region Fields and Properties

		//private static readonly Common.Logging.ILog log = Common.Logging.LogManager.GetLogger( typeof( Win32InputManager ) );

		private List<DeviceInfo> _unusedDevices = new List<DeviceInfo>();
		private int _joystickCount = 0;

		private IntPtr _hwnd;

		#endregion Fields and Properties

		internal Win32InputManager()
			: base()
		{
			RegisterFactory( this );
		}

		public override string InputSystemName
		{
			get
			{
				return "Win32";
			}
		}

		protected override void _initialize( ParameterList args )
		{
			// Find the WINDOW parameter
			object window = args.Find( delegate( Parameter p )
										{
											return p.first.ToUpper() == "WINDOW";
										}
									 ).second;
			if ( window is IntPtr )
			{
				_hwnd = (IntPtr)window;
			}
			else
			{
				//throw new Exception( "SharpInputSystem.Win32InputManger requires a Handle to a window." );
			}

			_parseConfigSettings( args );
			_enumerateDevices();
		}

		private void _enumerateDevices()
		{
			KeyboardInfo keyboardInfo = new KeyboardInfo();
			keyboardInfo.Vendor = this.InputSystemName;
			keyboardInfo.Id = 0;
			_unusedDevices.Add( keyboardInfo );

			MouseInfo mouseInfo = new MouseInfo();
			mouseInfo.Vendor = this.InputSystemName;
			mouseInfo.Id = 0;
			_unusedDevices.Add( mouseInfo );

		}

		private void _parseConfigSettings( ParameterList args )
		{

		}

		internal DeviceInfo PeekDevice<T>() where T : InputObject
		{
			string devType = typeof( T ).Name + "Info";

			foreach ( DeviceInfo device in _unusedDevices )
			{
				if ( devType == device.GetType().Name )
					return device;
			}

			return null;
		}

		internal DeviceInfo CaptureDevice<T>() where T : InputObject
		{
			string devType = typeof( T ).Name + "Info";

			foreach ( DeviceInfo device in _unusedDevices )
			{
				if ( devType == device.GetType().Name )
				{
					_unusedDevices.Remove( device );
					return device;
				}
			}

			return null;
		}

		internal void ReleaseDevice<T>( DeviceInfo device ) where T : InputObject
		{
			_unusedDevices.Add( device );
		}

		#region InputObjectFactory Implementation

		IEnumerable<KeyValuePair<Type, string>> InputObjectFactory.FreeDevices
		{
			get
			{
				List<KeyValuePair<Type, string>> freeDevices = new List<KeyValuePair<Type, string>>();
				foreach ( DeviceInfo dev in _unusedDevices )
				{
					if ( dev.GetType() == typeof( KeyboardInfo ) )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Keyboard ), this.InputSystemName ) );

					if ( dev.GetType() == typeof( KeyboardInfo ) )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Mouse ), this.InputSystemName ) );

					if ( dev.GetType() == typeof( JoystickInfo ) )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Joystick ), dev.Vendor ) );
				}
				return freeDevices;
			}
		}

		int InputObjectFactory.DeviceCount<T>()
		{
			if ( typeof( T ) == typeof( Keyboard ) )
				return 1;
			if ( typeof( T ) == typeof( Mouse ) )
				return 1;
			if ( typeof( T ) == typeof( Joystick ) )
				return _joystickCount;
			return 0;
		}

		int InputObjectFactory.FreeDeviceCount<T>()
		{
			string devType = typeof( T ).Name + "Info";
			int deviceCount = 0;
			foreach ( DeviceInfo device in _unusedDevices )
			{
				if ( devType == device.GetType().Name )
					deviceCount++;
			}
			return deviceCount;
		}

		bool InputObjectFactory.VendorExists<T>( string vendor )
		{
			if ( typeof( T ) == typeof( Keyboard ) || typeof( T ) == typeof( Mouse ) || vendor.ToLower() == InputSystemName.ToLower() )
			{
				return true;
			}
			else
			{
				if ( typeof( T ) == typeof( Joystick ) )
				{
					foreach ( DeviceInfo dev in _unusedDevices )
					{
						if ( dev.GetType() == typeof( JoystickInfo ) )
						{
							JoystickInfo joy = (JoystickInfo)dev;
							if ( joy.Vendor.ToLower() == vendor.ToLower() )
								return true;
						}
					}
				}
			}
			return false;
		}

		InputObject InputObjectFactory.CreateInputObject<T>( InputManager creator, bool bufferMode, string vendor )
		{
			string typeName = this.InputSystemName + typeof( T ).Name;
			Type objectType = System.Reflection.Assembly.GetExecutingAssembly().GetType( "SharpInputSystem." + typeName );
			T obj;

			System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.CreateInstance;

			obj = (T)objectType.InvokeMember( typeName,
											  bindingFlags,
											  null,
											  null,
											  new object[] { this, bufferMode } );
			return obj;
		}

		void InputObjectFactory.DestroyInputObject( InputObject obj )
		{
			obj.Dispose();
		}

		#endregion InputObjectFactory Implementation
	}
}
