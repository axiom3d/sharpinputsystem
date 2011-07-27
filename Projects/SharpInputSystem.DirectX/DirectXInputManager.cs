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

using SlimDX.DirectInput;

using Common.Logging;

using SWF = System.Windows.Forms;

using MDI = SlimDX.DirectInput;

#endregion Namespace Declarations

namespace SharpInputSystem.DirectX
{
    /// <summary>
    /// DirectX 9.0c InputManager specialization
    /// </summary>
    class DirectXInputManager : InputManager, InputObjectFactory
    {
        #region Fields and Properties

        private static readonly ILog log = LogManager.GetLogger( typeof( DirectXInputManager ) );

        private MDI.DirectInput directInput = new DirectInput();
        private Dictionary<String, MDI.CooperativeLevel> _settings = new Dictionary<string, MDI.CooperativeLevel>();
        private List<DeviceInfo> _unusedDevices = new List<DeviceInfo>();
        private int _joystickCount = 0;

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

        #region WindowHandle Property
        private IntPtr _hwnd;
        public IntPtr WindowHandle
        {
            get
            {
                return _hwnd;
            }
        }
        #endregion WindowHandle Property

        #endregion Fields and Properties


		public override string InputSystemName
		{
			get
			{
				return "DirectX";
			}
		}
		
        internal DirectXInputManager()
            : base()
        {
            RegisterFactory( this );
        }

        protected override void _initialize( ParameterList args )
        {
            // Find the WINDOW parameter
            object window = args.Find(  delegate( Parameter p )
                                        {
                                            return p.first.ToUpper() == "WINDOW";
                                        }
                                     ).second;
            if ( window is IntPtr )
            {
                _hwnd = (IntPtr)window;
            }
            else if ( window is SWF.Control )
            {
                SWF.Control parent = (SWF.Control)window;
                // if the control is a picturebox, we need to grab its parent form
                while ( !( parent is SWF.Form ) && parent != null )
                {
                    parent = parent.Parent;
                }
                _hwnd = parent.Handle;
            }
            else
            {
                throw new Exception( "SharpInputSystem.DirectXInputManger requires either a reference to a Control or a Handle to a window." );
            }

            _settings.Add( typeof( Mouse ).Name, 0 );
            _settings.Add( typeof( Keyboard ).Name, 0 );
            _settings.Add( typeof( Joystick ).Name, 0 );

            //Ok, now we have DirectInput, parse whatever extra settings were sent to us
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

            foreach ( MDI.DeviceInstance device in directInput.GetDevices( MDI.DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly ) )
            {
                //if ( device.Type == MDI.DeviceType.Joystick || device.Type == MDI.DeviceType.Gamepad ||
                //     device.Type == MDI.DeviceType.FirstPerson || device.Type == MDI.DeviceType.Driving ||
                //     device.Type == MDI.DeviceType.Flight )
                //{
                    JoystickInfo joystickInfo = new JoystickInfo();
                    joystickInfo.DeviceId = device.InstanceGuid;
                    joystickInfo.Vendor = device.ProductName;
                    joystickInfo.Id = _joystickCount++;

                    this._unusedDevices.Add( joystickInfo );
                //}
            }
        }

        private void _parseConfigSettings( ParameterList args )
        {
            System.Collections.Generic.Dictionary<String, MDI.CooperativeLevel> valueMap = new System.Collections.Generic.Dictionary<string, MDI.CooperativeLevel>();

            valueMap.Add( "CLF_BACKGROUND", MDI.CooperativeLevel.Background );
            valueMap.Add( "CLF_FOREGROUND", MDI.CooperativeLevel.Foreground );
            valueMap.Add( "CLF_EXCLUSIVE", MDI.CooperativeLevel.Exclusive );
            valueMap.Add( "CLF_NONEXCLUSIVE", MDI.CooperativeLevel.Nonexclusive );
            valueMap.Add( "CLF_NOWINDOWSKEY", MDI.CooperativeLevel.NoWinKey );

            foreach ( Parameter p in args )
            {
                switch ( p.first.ToUpper() )
                {
                    case "W32_MOUSE":
                        _settings[ typeof( Mouse ).Name ] |= valueMap[ p.second.ToString().ToUpper() ];
                        break;
                    case "W32_KEYBOARD":
                        _settings[ typeof( Keyboard ).Name ] |= valueMap[ p.second.ToString().ToUpper() ];
                        break;
                    case "W32_JOYSTICK":
                        _settings[ typeof( Joystick ).Name ] |= valueMap[ p.second.ToString().ToUpper() ];
                        break;
                    default:
                        break;
                }
            }

            if ( _settings[ typeof( Mouse ).Name ] == 0 )
                _settings[ typeof( Mouse ).Name ] = MDI.CooperativeLevel.Exclusive | MDI.CooperativeLevel.Foreground;
            if ( _settings[ typeof( Keyboard ).Name ] == 0 )
                _settings[ typeof( Keyboard ).Name ] = MDI.CooperativeLevel.Nonexclusive | MDI.CooperativeLevel.Background;
            if ( _settings[ typeof( Joystick ).Name ] == 0 )
                _settings[ typeof( Joystick ).Name ] = MDI.CooperativeLevel.Exclusive | MDI.CooperativeLevel.Foreground;

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
			string name = this.GetType().FullName.Remove(this.GetType().FullName.LastIndexOf(".") + 1);
            Type objectType = System.Reflection.Assembly.GetExecutingAssembly().GetType( name + typeName );
            T obj = null;

			System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.CreateInstance;
			try
			{
                obj = (T)objectType.InvokeMember( typeName,
                                                  bindingFlags,
                                                  null,
                                                  null,
                                                  new object[] { this, directInput, bufferMode, _settings[ typeof( T ).Name ] } );
            }
            catch (Exception ex)
            {
                log.Error("Cannot create requested device.", ex);
            }
            return obj;
        }

        void InputObjectFactory.DestroyInputObject( InputObject obj )
        {
            obj.Dispose();
        }

        #endregion InputObjectFactory Implementation
    }
}