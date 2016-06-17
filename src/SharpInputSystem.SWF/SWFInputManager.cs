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
using System.Windows.Forms;
using Common.Logging;

namespace SharpInputSystem.SWF
{
    public class SWFInputManager :  InputManager, InputObjectFactory
    {
        private static readonly ILog log = LogManager.GetLogger( typeof( SWFInputManager ) );

        #region WindowHandle Property

        private Control _window;
        private IntPtr _hwnd;
        public IntPtr WindowHandle
        {
            get
            {
                return _hwnd;
            }
        }

        #endregion WindowHandle Property

        internal SWFInputManager()
            : base()
        {
            log.Info( "System.Windows.Forms InputHandler Created." );
            RegisterFactory( (InputObjectFactory) this );
        }

        #region SharpInputSystem.InputManager Implementation
        
        protected override void _initialize( ParameterList args )
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

            if( _hwnd != IntPtr.Zero )
            {
                _window = Control.FromChildHandle( _hwnd );
            }

            KeyboardInfo keyboardInfo = new KeyboardInfo();
            keyboardInfo.Vendor = this.InputSystemName;
            keyboardInfo.Id = 0;
            _unusedDevices.Add( keyboardInfo );
            
            MouseInfo mouseInfo = new MouseInfo();
            mouseInfo.Vendor = this.InputSystemName;
            mouseInfo.Id = 0;
            _unusedDevices.Add( mouseInfo );

        }

        public override string InputSystemName
        {
            get
            {
                return "SWF";
            }
        }

        #endregion

        #region SharpInputSystem.InputObjectFactory Implementation

        private List<DeviceInfo> _unusedDevices = new List<DeviceInfo>();
        private int _joystickCount = 0;

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
            foreach( DeviceInfo device in _unusedDevices )
            {
                if( devType == device.GetType().Name )
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
                                                  new object[] { this, _window, bufferMode } );
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

        #endregion

    }
}