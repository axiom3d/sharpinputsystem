#region MIT/X11 License
/*
Sharp Input System Library
Copyright � 2007-2011 Michael Cummings

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
using System.Linq;

using XInput = Microsoft.Xna.Framework.Input;
using MXF = Microsoft.Xna.Framework;

//using Common.Logging;

#endregion Namespace Declarations

namespace SharpInputSystem.Xna
{
	/// <summary>
	/// Xna 2.0 Input Manager Specialization
	/// </summary>
	class XnaInputManager : InputManager, InputObjectFactory
	{
		#region Fields and Properties

		//private static readonly ILog log = LogManager.GetLogger( typeof( XnaInputManager ) );

		private IntPtr _hwnd;
		private List<DeviceInfo> _unusedDevices = new List<DeviceInfo>();
		private int _gamePadCount = 0;

		#region keyboardInUse Property
		private bool _keyboardInUse = false;
		internal bool keyboardInUse
		{
			get
			{
				return _keyboardInUse;
			}
			set
			{
				_keyboardInUse = value;
			}
		}
		#endregion keyboardInUse Property

		#region mouseInUse Property
		private bool _mouseInUse = false;
		internal bool mouseInUse
		{
			get
			{
				return _mouseInUse;
			}
			set
			{
				_mouseInUse = value;
			}
		}
		#endregion keyboardInUse Property

        public bool HideMouse { get; set; }

		#endregion Fields and Properties

		#region Construction and Destruction

		internal XnaInputManager()
			: base()
		{
			RegisterFactory( this );
		}

		#endregion Construction and Destruction

		#region Methods

		private void _enumerateDevices()
		{
#if !( XBOX || XBOX360 )
			KeyboardInfo keyboardInfo = new KeyboardInfo();
			keyboardInfo.Vendor = this.InputSystemName;
			keyboardInfo.Id = 0;
			_unusedDevices.Add( keyboardInfo );

			MouseInfo mouseInfo = new MouseInfo();
			mouseInfo.Vendor = this.InputSystemName;
			mouseInfo.Id = 0;
			_unusedDevices.Add( mouseInfo );
#endif
			int maxPlayers = /* (int)Xna.PlayerIndex.Four */ 4;
			for ( int player = 0; player < maxPlayers; player++ )
			{
                XInput.GamePadCapabilities gpCaps = XInput.GamePad.GetCapabilities((MXF.PlayerIndex)player);
                if (gpCaps.IsConnected)
                {
                    JoystickInfo joystickInfo = new JoystickInfo();
                    joystickInfo.DeviceId = new Guid();
                    joystickInfo.Vendor = this.InputSystemName;
                    joystickInfo.Id = _gamePadCount++;

                    this._unusedDevices.Add(joystickInfo);
                }
			}
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

		internal R CaptureDevice<T, R>() where T : InputObject
		{
			foreach ( DeviceInfo device in _unusedDevices )
			{
				if ( typeof( R ) == device.GetType() )
				{
					_unusedDevices.Remove( device );
					return (R)device;
				}
			}
			return default( R );
		}

		internal void ReleaseDevice<T>( DeviceInfo device ) where T : InputObject
		{
			_unusedDevices.Add( device );
		}

		#endregion Methods

		#region InputManager Implementation

		protected override void _initialize( ParameterList args )
		{
			// Find the first Parameter entry with WINDOW
            var parameter = args.First( p => p.first.ToLower() == "window" );
            if (parameter != null)
            {
                _hwnd = (IntPtr)parameter.second;
            }

            parameter = args.FirstOrDefault( p => p.first.ToLower() == "xna_mouse_hide" );
            if (parameter != null)
            {
                if (parameter.second is Boolean)
                {
                    HideMouse = (bool)parameter.second;
                }
            }

		    _parseConfigSettings( args );
			_enumerateDevices();
		}

		public override string InputSystemName
		{
			get
			{
				return "Xna";
			}
		}

		#endregion InputManager Implementation

		#region InputObjectFactory Members

		IEnumerable<KeyValuePair<Type, string>> InputObjectFactory.FreeDevices
		{
			get
			{
				List<KeyValuePair<Type, string>> freeDevices = new List<KeyValuePair<Type, string>>();
				foreach ( DeviceInfo dev in _unusedDevices )
				{
					if ( dev.GetType() == typeof( KeyboardInfo ) && !keyboardInUse )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Keyboard ), this.InputSystemName ) );

					if ( dev.GetType() == typeof( KeyboardInfo ) && !_mouseInUse )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Mouse ), this.InputSystemName ) );

					if ( dev.GetType() == typeof( JoystickInfo ) )
						freeDevices.Add( new KeyValuePair<Type, string>( typeof( Joystick ), dev.Vendor ) );
				}
				return freeDevices;
			}
		}

		int InputObjectFactory.DeviceCount<T>()
		{
#if !(XBOX || XBOX360)
			if ( typeof( T ) == typeof( Keyboard ) )
				return 1;
			if ( typeof( T ) == typeof( Mouse ) )
				return 1;
#else
			if ( typeof( T ) == typeof( Keyboard ) )
				return 0;
			if ( typeof( T ) == typeof( Mouse ) )
				return 0;
#endif
			if ( typeof( T ) == typeof( Joystick ) )
				return _gamePadCount;
			return 0;
		}

		public int FreeDeviceCount<T>() where T : InputObject
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

		public bool VendorExists<T>( string vendor ) where T : InputObject
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
            string name = this.GetType().FullName.Remove( this.GetType().FullName.LastIndexOf( "." ) + 1 );
            Type objectType = System.Reflection.Assembly.GetExecutingAssembly().GetType( name + typeName );
            T obj = null;

			System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.CreateInstance;
			try
			{
				obj = (T)objectType.InvokeMember( typeName,
												  bindingFlags,
												  null,
												  null,
												  new object[] { this, bufferMode } );
			}
			catch ( Exception ex )
			{
				//log.Error( "Cannot create requested device.", ex );
			}
			return obj;
		}

		void InputObjectFactory.DestroyInputObject( InputObject obj )
		{
			obj.Dispose();
		}

		#endregion
	}
}
