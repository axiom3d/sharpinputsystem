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
    class OnTouchListener : Java.Lang.Object, View.IOnTouchListener    
    {
        private AndroidMouse _mouse;

        public OnTouchListener( AndroidMouse mouse )
        {
            this._mouse = mouse;
        }

        public bool OnTouch( View v, MotionEvent e )
        {
            return _mouse.OnTouch( v, e );
        }
    }
}