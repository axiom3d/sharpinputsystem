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
        Pov
    }

    /// <summary>
    /// Base type for all device components (button, axis, etc)
    /// </summary>
    public class Component
    {
        public Component()
        {
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
            protected set
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
}
