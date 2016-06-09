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

namespace SharpInputSystem
{
    class TouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private AndroidMouse _mouse;

        public TouchListener( AndroidMouse mouse )
        {
            this._mouse = mouse;
        }

        public bool OnTouch( View v, MotionEvent e )
        {
            return _mouse.OnTouch( v, e );
        }
    }

    class KeyboardListener : Java.Lang.Object, View.IOnKeyListener
    {
        private AndroidKeyboard _kb;

        public KeyboardListener(AndroidKeyboard keyboard)
        {
            this._kb = keyboard;
        }

        public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            return _kb.OnKey(v, keyCode, e);
        }
    }
}