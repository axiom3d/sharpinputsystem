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
    public class SWFMouse : Mouse
    {
        private Control _window;
        private int _lastX, _lastY;

        public SWFMouse( InputManager creator, Control window, bool buffered )
        {
            this.Creator = creator;
            this.IsBuffered = buffered;

            _window = window;
        }

        protected override void Dispose (bool disposeManagedResources)
        {
            if ( disposeManagedResources )
            {
                _window.MouseMove -= Handle_windowMouseMove;
                _window.MouseUp -= Handle_windowMouseUp;
                _window.MouseDown -= Handle_windowMouseDown;
            }
            base.Dispose (disposeManagedResources);
        }

        protected override void Initialize()
        {
            _window.MouseMove += Handle_windowMouseMove;
            _window.MouseUp += Handle_windowMouseUp;
            _window.MouseDown += Handle_windowMouseDown;
        }

        void Handle_windowMouseDown (object sender,  System.Windows.Forms.MouseEventArgs e)
        {
            if ( EventListener != null && IsBuffered )
                EventListener.MousePressed( new SharpInputSystem.MouseEventArgs( this, MouseState ), MouseButtonID.Left );
        }

        void Handle_windowMouseUp (object sender,  System.Windows.Forms.MouseEventArgs e)
        {
            if ( EventListener != null && IsBuffered )
                EventListener.MouseReleased( new SharpInputSystem.MouseEventArgs( this, MouseState ), MouseButtonID.Left );
        }

        void Handle_windowMouseMove (object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseState.X.Absolute = e.X;
            MouseState.Y.Absolute = e.Y;
            MouseState.X.Relative = _lastX - e.X;
            MouseState.Y.Relative = _lastY - e.Y;
            _lastX = e.X;
            _lastY = e.Y;
            
            if ( EventListener != null && IsBuffered )
                EventListener.MouseMoved( new SharpInputSystem.MouseEventArgs( this, MouseState ) );
        }

        public override void Capture ()
        {
            System.Windows.Forms.Application.DoEvents();
        }

    }
}

