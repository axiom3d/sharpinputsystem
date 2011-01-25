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

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

#endregion Namespace Declarations

namespace SharpInputSystem
{
	/// <summary>
	/// Part of the libX11 library on unix platforms.
	/// </summary>
	internal sealed class LibX11
	{

		/// <summary>
		/// 
		/// </summary>
		internal enum XEventName
		{
			KeyPress = 2,
			KeyRelease = 3,
			ButtonPress = 4,
			ButtonRelease = 5,
			MotionNotify = 6,
			EnterNotify = 7,
			LeaveNotify = 8,
			FocusIn = 9,
			FocusOut = 10,
			KeymapNotify = 11,
			Expose = 12,
			GraphicsExpose = 13,
			NoExpose = 14,
			VisibilityNotify = 15,
			CreateNotify = 16,
			DestroyNotify = 17,
			UnmapNotify = 18,
			MapNotify = 19,
			MapRequest = 20,
			ReparentNotify = 21,
			ConfigureNotify = 22,
			ConfigureRequest = 23,
			GravityNotify = 24,
			ResizeRequest = 25,
			CirculateNotify = 26,
			CirculateRequest = 27,
			PropertyNotify = 28,
			SelectionClear = 29,
			SelectionRequest = 30,
			SelectionNotify = 31,
			ColormapNotify = 32,
			ClientMessage = 33,
			MappingNotify = 34,

			LASTEvent
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct XAnyEvent
		{
			public XEventName type;
			public IntPtr serial;
			public bool send_event;
			public IntPtr display;
			public IntPtr window;
		}

		[StructLayout( LayoutKind.Explicit )]
		public struct XEvent
		{
			[FieldOffset( 0 )]
			public XEventName type;
			[FieldOffset( 0 )]
			public XAnyEvent AnyEvent;
			[FieldOffset( 0 )]
			public XKeyEvent KeyEvent;
			[FieldOffset( 0 )]
			public XButtonEvent ButtonEvent;
			[FieldOffset( 0 )]
			public XMotionEvent MotionEvent;
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct XKeyEvent
		{
			public XEventName type;
			public IntPtr serial;
			public bool send_event;
			public IntPtr display;
			public IntPtr window;
			public IntPtr root;
			public IntPtr subwindow;
			public IntPtr time;
			public int x;
			public int y;
			public int x_root;
			public int y_root;
			public int state;
			public int keycode;
			public bool same_screen;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct XMotionEvent
		{
			public XEventName type;
			public IntPtr serial;
			public bool send_event;
			public IntPtr display;
			public IntPtr window;
			public IntPtr root;
			public IntPtr subwindow;
			public IntPtr time;
			public int x;
			public int y;
			public int x_root;
			public int y_root;
			public int state;
			public byte is_hint;
			public bool same_screen;
		}

		[StructLayout( LayoutKind.Sequential )]
		public struct XButtonEvent
		{
			public XEventName type;
			public IntPtr serial;
			public bool send_event;
			public IntPtr display;
			public IntPtr window;
			public IntPtr root;
			public IntPtr subwindow;
			public IntPtr time;
			public int x;
			public int y;
			public int x_root;
			public int y_root;
			public int state;
			public int button;
			public bool same_screen;
		}

		/// <summary>
		/// Name of the library we grab the input functions from
		/// </summary>
		internal const string LibraryName = "libX11";
		/// <summary>
		/// 
		/// </summary>
		internal const int GrabModeAsync = 1;
		/// <summary>
		/// 
		/// </summary>
		internal const long CurrentTime = 0;
		public const int ButtonPressMask = 1 << 2;
		public const int ButtonReleaseMask = 1 << 3;
		public const int EnterWindowMask = 1 << 4;
		public const int LeaveWindowMask = 1 << 5;
		public const int PointerMotionMask = 1 << 6;
		public const int PointerMotionHintMask = 1 << 7;
		public const int Mod4Mask = ( 1 << 6 );
		public const int MouseMovedPressedReleased = Mod4Mask | ButtonReleaseMask | ButtonPressMask;

		[DllImport( LibraryName )]
		internal static extern int XGrabPointer( IntPtr display, IntPtr grabWindow,
		bool ownerMvents, int eventMask, int pointerMode, int keyboardMode,
		IntPtr confineTo, IntPtr cursor, long time );
		[DllImport( LibraryName )]
		internal static extern int XUngrabPointer( IntPtr display, long time );
		/// <summary>
		/// To open a connection to the X server that controls a display
		/// </summary>
		/// <param name="displayName">
		/// Specifies the hardware display name, 
		/// which determines the display and communications domain to be used.
		/// On a POSIX-conformant system, if the display_name is NULL, 
		/// it defaults to the value of the DISPLAY environment variable. 
		/// </param>
		/// <returns>
		/// returns a Display structure that serves as the connection to the X server and 
		/// that contains all the information about that X server.
		/// </returns>
		[DllImportAttribute( LibraryName )]
		internal static extern IntPtr XOpenDisplay( IntPtr displayName );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="display">
		/// connection to X server
		/// </param>
		/// <returns>
		/// A <see cref="IntPtr"/>
		/// </returns>
		[DllImportAttribute( LibraryName )]
		internal static extern IntPtr XCloseDisplay( IntPtr display );
		/// <summary>
		/// Get's the root window for the default screen.
		/// </summary>
		/// <param name="display">Specifies the connection to the X server. </param>
		/// <returns>returns the root window for the default screen.</returns>
		[DllImportAttribute( LibraryName )]
		internal static extern IntPtr XDefaultRootWindow( IntPtr display );
		/// <summary>
		/// The XUngrabKeyboard() function releases the keyboard and any queued events if this client
		/// has it actively grabbed from either XGrabKeyboard() or XGrabKey(). 
		/// XUngrabKeyboard() does not release the keyboard and any queued events if the 
		/// specified time is earlier than the last-keyboard-grab time or is later than the current X server time.
		/// It also generates FocusIn and FocusOut events. 
		/// The X server automatically performs an UngrabKeyboard request if the event window for 
		/// an active keyboard grab becomes not viewable. 
		/// </summary>
		/// <param name="display">Specifies the connection to the X server. </param>
		/// <param name="currentTime">Specifies the time. You can pass either a timestamp or CurrentTime. </param>
		/// <returns></returns>
		[DllImportAttribute( LibraryName )]
		internal static extern void XUngrabKeyboard( IntPtr display, long currentTime );
		/// <summary>
		/// The XGrabKeyboard() function actively grabs control of the keyboard and generates 
		/// FocusIn and FocusOut events. Further key events are reported only to the grabbing client.
		/// XGrabKeyboard() overrides any active keyboard grab by this client.
		/// If owner_events is False, all generated key events are reported with respect to grab_window.
		/// If owner_events is True and if a generated key event would normally be reported to this client,
		/// it is reported normally; otherwise, the event is reported with respect to the grab_window.
		/// Both KeyPress and KeyRelease events are always reported, independent of any event selection made by the client. 
		/// </summary>
		/// <param name="display">Specifies the connection to the X server. </param>
		/// <param name="window">Specifies the grab window. </param>
		/// <param name="own">Specifies a Boolean value that indicates whether the keyboard events are to be reported as usual. </param>
		/// <param name="pointerMode">Specifies further processing of pointer events. You can pass GrabModeSync (0) or GrabModeAsync (1) .</param>
		/// <param name="keyboardMode">Specifies further processing of keyboard events. You can pass GrabModeSync (0) or GrabModeAsync (1). </param>
		/// <param name="currentTime">Specifies the time. You can pass either a timestamp or CurrentTime. </param>
		/// <returns></returns>
		[DllImportAttribute( LibraryName )]
		internal static extern int XGrabKeyboard( IntPtr display, IntPtr window, bool own,
			int pointerMode, int keyboardMode, long currentTime );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="display"></param>
		/// <param name="xevent"></param>
		[DllImportAttribute( LibraryName )]
		internal static extern void XNextEvent( IntPtr display, ref XEvent xevent );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key_event"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		[DllImportAttribute( LibraryName )]
		internal static extern IntPtr XLookupKeysym( ref XKeyEvent key_event, int index );
		/// <summary>
		/// Sets DetectableAutorepeat
		/// </summary>
		/// <param name="display">
		/// connection to X server
		/// </param>
		/// <param name="detectable">
		/// True => set DetectableAutorepeat
		/// </param>
		/// <param name="supported">
		/// backfilled True if DetectableAutorepeat supported
		/// </param>
		[DllImportAttribute( LibraryName )]
		internal static extern bool XkbSetDetectableAutoRepeat( IntPtr display, bool detectable, ref bool supported );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="display">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="keycode">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <param name="index">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.UInt32"/>
		/// </returns>
		[DllImportAttribute( LibraryName )]
		internal static extern uint XKeycodeToKeysym( IntPtr display, int keycode, int index );

		/// <summary>
		/// Convert's X11KeyCode to standard System.Windows.Forms.Keys;
		/// </summary>
		/// <param name="x11keycode">
		///  <see cref="System.UInt32"/>
		/// </param>
		/// <returns>
		///  <see cref="System.Windows.Forms.Keys"/>
		/// </returns>
		internal static Keys ToKeys( uint x11keycode )
		{
			switch ( x11keycode )
			{
				case 32:
					return Keys.Space;
				case 65509:
					return Keys.Capital;
				case 65289:
					return Keys.Tab;
				case 65106:
					return Keys.Oem6;// == ^ ° (ger)
				case 65366:
					return Keys.PageDown;
				case 65365:
					return Keys.PageUp;
				case 65363:
					return Keys.Right;
				case 65361:
					return Keys.Left;
				case 65364:
					return Keys.Down;
				case 65362:
					return Keys.Up;
				case 65293:
				//fall through, 65293 is return, 65421 is numpad-return.
				case 65421:
					return Keys.Return;
				case 65451:
					return Keys.Add;
				case 65300:
					return Keys.Scroll;
				case 65299:
					return Keys.Pause;
				case 65367:
					return Keys.End;
				case 65360:
					return Keys.Home;
				case 65535:
					return Keys.Delete;
				case 65379:
					return Keys.Insert;
				case 65377:
					return Keys.Print;
				case 65453:
					return Keys.Subtract;
				case 65450:
					return Keys.Multiply;
				case 65455:
					return Keys.Divide;
				case 65407:
					return Keys.NumLock;
				case 65434:
					return Keys.NumPad9;
				case 65431:
					return Keys.NumPad8;
				case 65429:
					return Keys.NumPad7;
				case 65432:
					return Keys.NumPad6;
				case 65437:
					return Keys.NumPad5;
				case 65430:
					return Keys.NumPad4;
				case 65435:
					return Keys.NumPad3;
				case 65433:
					return Keys.NumPad2;
				case 65438:
					return Keys.NumPad0;
				case 65436:
					return Keys.NumPad1;
				case 65470:
					return Keys.F1;
				case 65471:
					return Keys.F2;
				case 65472:
					return Keys.F3;
				case 65473:
					return Keys.F4;
				case 65474:
					return Keys.F5;
				case 65475:
					return Keys.F6;
				case 65476:
					return Keys.F7;
				case 65477:
					return Keys.F8;
				case 65478:
					return Keys.F9;
				case 65479:
					return Keys.F10;
				case 65480:
					return Keys.F11;
				case 65481:
					return Keys.F12;
				case 65105:
					return Keys.Oem6;// == ` ' (ger)
				case 223:
					return Keys.OemOpenBrackets;// == ? ß \ (ger)
				case 48:
					return Keys.D0;
				case 49:
					return Keys.D1;
				case 50:
					return Keys.D2;
				case 51:
					return Keys.D3;
				case 52:
					return Keys.D4;
				case 53:
					return Keys.D5;
				case 54:
					return Keys.D6;
				case 55:
					return Keys.D7;
				case 56:
					return Keys.D8;
				case 57:
					return Keys.D9;
				case 65513:
					return Keys.Alt;
				case 65383:
					return Keys.RWin;
				case 65515:
					return Keys.LWin;
				case 65507:
					return Keys.LControlKey;
				case 65506:
					return Keys.RShiftKey;
				case 65505:
					return Keys.LShiftKey;
				case 65307:
					return Keys.Escape;
				case 119:
					return Keys.W;
				case 101:
					return Keys.E;
				case 114:
					return Keys.R;
				case 116:
					return Keys.T;
				case 122:
					return Keys.Z;
				case 117:
					return Keys.U;
				case 105:
					return Keys.I;
				case 111:
					return Keys.O;
				case 112:
					return Keys.P;
				case 97:
					return Keys.A;
				case 115:
					return Keys.S;
				case 100:
					return Keys.D;
				case 102:
					return Keys.F;
				case 103:
					return Keys.G;
				case 104:
					return Keys.H;
				case 106:
					return Keys.J;
				case 107:
					return Keys.K;
				case 108:
					return Keys.L;
				case 121:
					return Keys.Y;
				case 120:
					return Keys.X;
				case 99:
					return Keys.C;
				case 118:
					return Keys.V;
				case 98:
					return Keys.B;
				case 110:
					return Keys.N;
				case 109:
					return Keys.M;
				case 113:
					return Keys.Q;
				default:
					return Keys.None;
			}
		}
	}
}
