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
using Tao.Sdl;
using log4net;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    class SdlJoystick : Joystick
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SdlJoystick));

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="buffered">true for buffered keyboard input</param>
		public SdlJoystick( InputManager creator, bool buffered )
		{
			Creator = creator;
			IsBuffered = buffered;
			Type = InputType.Joystick;
			EventListener = null;
		}

        #region SharpInputSystem.Joystick Implementation

        /// <summary>
        /// Used for updating call once per frame before checking state or to update events
        /// </summary>
        public override void Capture()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void initialize()
        {
            throw new NotImplementedException();
        }

        #endregion SharpInputSystem.Joystick Implementation
    }
}
