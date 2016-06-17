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