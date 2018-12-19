#region MIT/X11 License

/*
Sharp Input System Library
Copyright © 2007-2019 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to Phillip Castaneda for maintaining such a high quality project.

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
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Common.Logging;

#endregion Namespace Declarations

namespace SharpInputSystem
{
    public class AndroidMouse : Mouse
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AndroidMouse));

        private View _view;

        #region Construction and Destruction

        public AndroidMouse(InputManager creator, View device, bool buffered)
        {
            Creator = creator;
            this._view = device;
            IsBuffered = buffered;
            Type = InputType.Mouse;
            EventListener = null;

            _view.SetOnTouchListener(new TouchListener(this));

            Log.Debug("AndroidMouse device created.");
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // Dispose managed resources.
                }
                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.

                Log.Debug("AndroidMouse device disposed.");
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.Dispose(disposeManagedResources);
        }

        #endregion Construction and Destruction

        public override void Capture()
        {
            
        }

        protected override void Initialize()
        {
            
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            MouseState.X.Absolute = (int)e.GetX(e.ActionIndex);
            MouseState.Y.Absolute = (int)e.GetY(e.ActionIndex);

            int mouseButton = e.GetPointerId(e.ActionIndex);

            switch (e.ActionMasked)
            {
                // DOWN                
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    if (EventListener != null && IsBuffered)
                        return EventListener.MousePressed(new MouseEventArgs(this, MouseState), (MouseButtonID)mouseButton);
                    break;
                // UP                
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                    if (EventListener != null && IsBuffered)
                        return EventListener.MouseReleased(new MouseEventArgs(this, MouseState), (MouseButtonID)mouseButton);
                    break;
                // MOVE                
                case MotionEventActions.Move:
                    for (int i = 0; i < e.PointerCount; i++)
                    {
                        MouseState.X.Absolute = (int)e.GetX(e.ActionIndex);
                        MouseState.Y.Absolute = (int)e.GetY(e.ActionIndex);
                        if (EventListener != null && IsBuffered)
                            return EventListener.MouseMoved(new MouseEventArgs(this, MouseState));
                    }
                    break;

                // CANCEL, OUTSIDE                
                case MotionEventActions.Cancel:
                case MotionEventActions.Outside:
                    if (EventListener != null && IsBuffered)
                        return EventListener.MouseReleased(new MouseEventArgs(this, MouseState), (MouseButtonID)mouseButton);
                    break;
            }
            return true;
        }
    }
}