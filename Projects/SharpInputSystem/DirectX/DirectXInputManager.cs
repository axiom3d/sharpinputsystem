#region LGPL License
/*
Sharp Input System Library
Copyright (C) 2007 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to the Phillip Castaneda for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#region Namespace Declarations

using System;
using System.Collections.Generic;
using SWF = System.Windows.Forms;

using MDI = Microsoft.DirectX.DirectInput;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    /// <summary>
    /// DirectX 9.0c InputManager specialization
    /// </summary>
    class DirectXInputManager : InputManager
    {
        private Dictionary<String, MDI.CooperativeLevelFlags> _settings = new Dictionary<string,Microsoft.DirectX.DirectInput.CooperativeLevelFlags>();
        private JoystickInfoList _unusedJoysticks = new JoystickInfoList();

        private SWF.Control _hwnd;
        public SWF.Control WindowHandle
        {
            get
            {
                return _hwnd;
            }
        }

        private int _joystickCount;
        public override int JoystickCount
        {
            get
            {
                return _joystickCount;
            }
        }

        public override int MiceCount
        {
            get
            {
                return 1;
            }
        }

        public override int KeyboardCount
        {
            get
            {
                return 1;
            }
        }
        public override T CreateInputObject<T>( bool bufferMode )
        {
            string typeName = "DirectX" + typeof( T ).Name;
            Type objectType;
            T obj;

            objectType = System.Reflection.Assembly.GetExecutingAssembly().GetType( "SharpInputSystem." + typeName);

            System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.CreateInstance;

            obj = (T)objectType.InvokeMember( typeName,
                                              bindingFlags,
                                              null, 
                                              null,
                                              new object[] { this, null, bufferMode, _settings[ typeof( T ).Name ] } );     

            try
            {
                obj.initialize();
            }
            catch ( Exception e )
            {
                obj.Dispose();
                obj = null;
                throw e; //rethrow
            }

	        return obj;
        }

        public override void DestroyInputObject( InputObject inputObject )
        {
            if ( inputObject != null )
            {
                inputObject.Dispose();
                inputObject = null;
            }
        }

        protected override void _initialize( ParameterList args )
        {
            // Find the first Parameter entry with WINDOW
            _hwnd = (SWF.Control)args.Find( 
                                            delegate( Parameter p ) 
                                            { 
                                                return p.first.ToUpper() == "WINDOW"; 
                                            } 
                                          ).second;

            if ( _hwnd is SWF.Form )
            {
                //_hwnd = _hwnd;
            }
            else if ( _hwnd is SWF.PictureBox )
            {
                // if the control is a picturebox, we need to grab its parent form
                while ( !( _hwnd is SWF.Form ) && _hwnd != null )
                {
                    _hwnd = _hwnd.Parent;
                }
            }
            else
            {
                throw new Exception( "SharpInputSystem.DirectXInputManger requires the handle of either a PictureBox or a Form." );
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
            foreach ( MDI.DeviceInstance device in MDI.Manager.Devices )
            {
                if ( device.DeviceType == MDI.DeviceType.Joystick || device.DeviceType == MDI.DeviceType.Gamepad ||
                     device.DeviceType == MDI.DeviceType.FirstPerson || device.DeviceType == MDI.DeviceType.Driving ||
                     device.DeviceType == MDI.DeviceType.Flight )
                {
                    JoystickInfo joystickInfo = new JoystickInfo();
                    joystickInfo.DeviceID = device.InstanceGuid;
                    joystickInfo.Vendor = device.ProductName;
                    joystickInfo.ID = this.JoystickCount;

                    this._joystickCount++;

                    this._unusedJoysticks.Add( joystickInfo );
                }
            }
        }

        private void _parseConfigSettings( ParameterList args )
        {
            System.Collections.Generic.Dictionary<String, MDI.CooperativeLevelFlags> valueMap = new System.Collections.Generic.Dictionary<string, MDI.CooperativeLevelFlags>();

            valueMap.Add( "CLF_BACKGROUND", MDI.CooperativeLevelFlags.Background );
            valueMap.Add( "CLF_FOREGROUND", MDI.CooperativeLevelFlags.Foreground );
            valueMap.Add( "CLF_EXCLUSIVE", MDI.CooperativeLevelFlags.Exclusive );
            valueMap.Add( "CLF_NONEXCLUSIVE", MDI.CooperativeLevelFlags.NonExclusive );
            valueMap.Add( "CLF_NOWINDOWSKEY", MDI.CooperativeLevelFlags.NoWindowsKey );

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
                _settings[ typeof( Mouse ).Name ] = MDI.CooperativeLevelFlags.NonExclusive | MDI.CooperativeLevelFlags.Background;
            if ( _settings[ typeof( Keyboard ).Name ] == 0 )
                _settings[ typeof( Keyboard ).Name ] = MDI.CooperativeLevelFlags.NonExclusive | MDI.CooperativeLevelFlags.Background;
            if ( _settings[ typeof( Joystick ).Name ] == 0 )
                _settings[ typeof( Joystick ).Name ] = MDI.CooperativeLevelFlags.Exclusive | MDI.CooperativeLevelFlags.Foreground;

        }
    }
}
