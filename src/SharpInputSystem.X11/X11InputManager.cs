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

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.Logging;

namespace SharpInputSystem
{
	internal sealed class X11InputManager : InputManager, InputObjectFactory
	{
        private static readonly ILog log = LogManager.GetLogger( typeof( X11InputManager ) );
		
		private List<DeviceInfo> _unusedDevices = new List<DeviceInfo>();
        private int _joystickCount = 0;
		
		public bool GrabKeyboard { get; set; }
		public bool UseKeyboardXRepeat { get; set; }
		public bool GrabMouse { get; set; }
		public bool HideMouse { get; set; }
		public bool GrabState { get; set; }
		
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
		
		internal X11InputManager() 
			: base()
        {
			GrabKeyboard = true;
			GrabMouse = true;
			HideMouse = true;
			GrabState = true;
			
            RegisterFactory( (InputObjectFactory) this );
        }		
		
		protected override void _initialize( ParameterList args )
		{
			_parseConfigSettings( args );
			_enumerateDevices();
		}
		
		private void _parseConfigSettings( ParameterList args )
		{
            // Find the WINDOW parameter
            var parameter = args.Find( ( p ) => { return p.first.ToLower() == "window"; } );
			if ( parameter != null )
			{
				var window = parameter.second;
	            if ( window is IntPtr )
	            {
	                _hwnd = (IntPtr)window;
	            }
			}
			
			parameter = args.Find( (p) =>  { return p.first.ToLower() == "x11_keyboard_grab"; } );
			if ( parameter != null )
			{
				if ( parameter.second is Boolean )
				{
					GrabKeyboard = (bool)parameter.second;
				}
			}
			
			parameter = args.Find( (p) =>  { return p.first.ToLower() == "x11_mouse_grab"; } );
			if ( parameter != null )
			{
				if ( parameter.second is Boolean )
				{
					GrabMouse = (bool)parameter.second;
				}
			}
			parameter = args.Find( (p) =>  { return p.first.ToLower() == "x11_mouse_hide"; } );
			if ( parameter != null )
			{
				if ( parameter.second is Boolean )
				{
					HideMouse = (bool)parameter.second;
				}
			}
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
			
			var _unusedJoysticks = X11Joystick.ScanJoysticks();
			_joystickCount = _unusedJoysticks.Count;
			_unusedDevices.AddRange( _unusedJoysticks );
		}
		
		public override string InputSystemName
		{
			get
			{
				return "X11";
			}
		}

		#region InputObjectFactory implementation
		
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
		
		int InputObjectFactory.FreeDeviceCount<T> ()
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

		bool InputObjectFactory.VendorExists<T> (string vendor)
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

		InputObject InputObjectFactory.CreateInputObject<T> (InputManager creator, bool bufferMode, string vendor)
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
                                                  new object[] { this, _hwnd, bufferMode } );
            }
            catch (Exception ex)
            {
                log.Error("Cannot create requested device.", ex);
            }
            return obj;
		}

		void InputObjectFactory.DestroyInputObject (InputObject obj)
		{	
	        obj.Dispose();
		}
		
		#endregion
	}
}
