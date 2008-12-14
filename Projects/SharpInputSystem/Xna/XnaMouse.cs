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
using System.Text;

using Xna = Microsoft.Xna.Framework;
using XInput = Microsoft.Xna.Framework.Input;
using log4net;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    /// <summary>
    /// Xna specialization of the Mouse class
    /// </summary>
    class XnaMouse : Mouse
    {
        #region Fields and Properties
        private static readonly ILog log = LogManager.GetLogger( typeof( XnaMouse ) );

        // Variables for XnaKeyboard
        private MouseInfo _mInfo;

        #endregion Fields and Properties

        #region Construction and Destruction

        public XnaMouse( InputManager creator, bool buffered )
        {
            Creator = creator;
            IsBuffered = buffered;
            Type = InputType.Mouse;
            EventListener = null;

            _mInfo = ((XnaInputManager)Creator).CaptureDevice<Mouse,MouseInfo>();

            if ( _mInfo == null )
            {
                throw new Exception( "No devices match requested type." );
            }

            log.Debug( "XnaKeyboard device created." );

        }

        protected override void _dispose( bool disposeManagedResources )
        {
            if ( !isDisposed )
            {
                if ( disposeManagedResources )
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.

                ( (XnaInputManager)Creator ).ReleaseDevice<Mouse>( _mInfo );

                log.Debug( "XnaMouse device disposed." );

            }
            isDisposed = true;

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base._dispose( disposeManagedResources );
        }

        #endregion Construction and Destruction

        #region InputObject Implementation

        public override void Capture()
        {
            throw new NotImplementedException();
        }

        internal override void initialize()
        {
            throw new NotImplementedException();
        }

        #endregion InputObject Implementation
    }
}
