#region MIT/X11 License

/*
Sharp Input System Library
Copyright � 2007-2019 Michael Cummings

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Views.InputMethods;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common.Logging;

namespace SharpInputSystem
{
    public class AndroidKeyboard : Keyboard
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AndroidKeyboard));

        private View _view;

        #region Construction and Destruction

        public AndroidKeyboard(InputManager creator, View device, bool buffered)
        {
            Creator = creator;
            this._view = device;
            IsBuffered = buffered;
            Type = InputType.Mouse;
            EventListener = null;

            _view.SetOnKeyListener(new KeyboardListener(this));

            Log.Debug("AndroidKeyboard device created.");
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

                Log.Debug("AndroidKeyboard device disposed.");
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.Dispose(disposeManagedResources);
        }

        #endregion Construction and Destruction

        public override int[] KeyStates
        {
            get
            {
                return new int[0];
            }
        }

        public override string AsString(KeyCode key)
        {
            return String.Empty;
        }

        public override void Capture()
        {
        }

        public override bool IsKeyDown(KeyCode key)
        {
            return false;
        }

        protected override void Initialize()
        {
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            return true;
        }

        public void ShowSoftInput()
        {
            ((InputMethodManager)((AndroidInputManager)Creator).Context.GetSystemService(Context.InputMethodService)).ShowSoftInput(this._view, ShowFlags.Forced);
        }
    }
}