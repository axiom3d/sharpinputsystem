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
		internal const int BadWindow = 3;
		internal const int BadAccess = 10;
		#region - Structures -
		
		[StructLayout(LayoutKind.Sequential, Pack = 2)]
	    internal struct XColor
	    {
	        internal IntPtr pixel;
	        internal ushort red;
	        internal ushort green;
	        internal ushort blue;
	        internal byte flags;
	        internal byte pad;
	    }		
		#region - XEventName -
		/// <summary>
		/// 
		/// </summary>
		internal enum XEventName : int
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
		#endregion

		#region - XAnyEvent -
		[StructLayout(LayoutKind.Sequential)]
		internal struct XAnyEvent
		{
			public XEventName type;
			public IntPtr serial;
			public bool send_event;
			public IntPtr display;
			public IntPtr window;
		}
		#endregion

		#region - XEvent -
		[StructLayout(LayoutKind.Explicit)]
		public struct XEvent
		{
			[FieldOffset(0)]
			public XEventName type;
			[FieldOffset(0)]
			public XAnyEvent AnyEvent;
			[FieldOffset(0)]
			public XKeyEvent KeyEvent;
			[FieldOffset(0)]
			public XButtonEvent ButtonEvent;
			[FieldOffset(0)]
			public XMotionEvent MotionEvent;
			[FieldOffset(0)]
			public IntPtr xcrossing;
			[FieldOffset(0)]
			public IntPtr xfocus;
			[FieldOffset(0)]
			public IntPtr xexpose;
			[FieldOffset(0)]
			public IntPtr xgraphicsexpose;
			[FieldOffset(0)]
			public IntPtr xnoexpose;
			[FieldOffset(0)]
			public IntPtr xvisibility;
			[FieldOffset(0)]
			public IntPtr xcreatewindow;
			[FieldOffset(0)]
			public IntPtr xdestroywindow;
			[FieldOffset(0)]
			public IntPtr xunmap;
			[FieldOffset(0)]
			public IntPtr xmap;
			[FieldOffset(0)]
			public IntPtr xmaprequest;
			[FieldOffset(0)]
			public IntPtr xreparent;
			[FieldOffset(0)]
			public IntPtr xconfigure;
			[FieldOffset(0)]
			public IntPtr xgravity;
			[FieldOffset(0)]
			public IntPtr xresizerequest;
			[FieldOffset(0)]
			public IntPtr xconfigurerequest;
			[FieldOffset(0)]
			public IntPtr xcirculate;
			[FieldOffset(0)]
			public IntPtr xcirculaterequest;
			[FieldOffset(0)]
			public IntPtr xproperty;
			[FieldOffset(0)]
			public IntPtr xselectionclear;
			[FieldOffset(0)]
			public IntPtr xselectionrequest;
			[FieldOffset(0)]
			public IntPtr xselection;
			[FieldOffset(0)]
			public IntPtr xcolormap;
			[FieldOffset(0)]
			public IntPtr xclient;
			[FieldOffset(0)]
			public IntPtr xmapping;
			[FieldOffset(0)]
			public IntPtr xerror;
			[FieldOffset(0)]
			public IntPtr xkeymap;
			[FieldOffset(0)]
			public IntPtr pad;
		}	
		#endregion

		#region - XKeyEvent -
		[StructLayout(LayoutKind.Sequential)]
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
		#endregion

		#region - XMotionEvent -
		[StructLayout(LayoutKind.Sequential)]
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
		#endregion

		#region - XButtonEvent -
		[StructLayout(LayoutKind.Sequential)]
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
		#endregion
		
		#region - XKeyboardState -
		[StructLayout(LayoutKind.Sequential)]
		internal struct XKeyboardState 
		{
			public int key_click_percent;
			public int bell_percent;
			public uint bell_pitch, bell_duration;
			public uint led_mask;
			public int global_auto_repeat;
			public AutoRepeats auto_repeats;

			[StructLayout(LayoutKind.Explicit)]
			public struct AutoRepeats 
			{
				[FieldOffset (0)]
				public byte first;				
				[FieldOffset (31)]
				public byte last;
			}
		}
		#endregion
		
		#endregion

		#region - Enumerations - 
		/// <summary>
		/// 
		/// </summary>
		public enum KeySym
		{
			XK_0 = 0x0030,  /* U+0030 DIGIT ZERO */
			XK_1 = 0x0031, /* U+0031 DIGIT ONE */
			XK_2 = 0x0032, /* U+0032 DIGIT TWO */
			XK_3 = 0x0033, /* U+0033 DIGIT THREE */
			XK_4 = 0x0034, /* U+0034 DIGIT FOUR */
			XK_5 = 0x0035, /* U+0035 DIGIT FIVE */
			XK_6 = 0x0036, /* U+0036 DIGIT SIX */
			XK_7 = 0x0037, /* U+0037 DIGIT SEVEN */
			XK_8 = 0x0038, /* U+0038 DIGIT EIGHT */
			XK_9 = 0x0039, /* U+0039 DIGIT NINE */
			XK_backSpace = 0xff08, /* Back space, back char */
			XK_minus = 0x002d,  /* U+002D HYPHEN-MINUS */
			XK_equal = 0x003d, /* U+003D EQUALS SIGN */
			XK_space = 0x0020, /* U+0020 SPACE */
			XK_comma = 0x002c, /* U+002C COMMA */
			XK_period = 0x002e, /* U+002E FULL STOP */
			XK_backslash = 0x005c, /* U+005C REVERSE SOLIDUS */
			XK_slash = 0x002f, /* U+002F SOLIDUS */
			XK_bracketleft = 0x005b, /* U+005B LEFT SQUARE BRACKET */
			XK_bracketright = 0x005d, /* U+005D RIGHT SQUARE BRACKET */
			XK_Escape = 0xff1b,
			XK_Caps_Lock = 0xffe5, /* Caps lock */
			XK_Tab = 0xff09,
			XK_Return = 0xff0d, /* Return, enter */
			XK_Control_L = 0xffe3, /* Left control */
			XK_Control_R = 0xffe4, /* Right control */
			XK_colon = 0x003a, /* U+003A COLON */
			XK_semicolon = 0x003b, /* U+003B SEMICOLON */
			XK_apostrophe = 0x0027, /* U+0027 APOSTROPHE */
			XK_grave = 0x0060, /* U+0060 GRAVE ACCENT */
			XK_a = 0x0061, /* U+0061 LATIN SMALL LETTER A */
			XK_b = 0x0062, /* U+0062 LATIN SMALL LETTER B */
			XK_c = 0x0063, /* U+0063 LATIN SMALL LETTER C */
			XK_d = 0x0064, /* U+0064 LATIN SMALL LETTER D */
			XK_e = 0x0065, /* U+0065 LATIN SMALL LETTER E */
			XK_f = 0x0066, /* U+0066 LATIN SMALL LETTER F */
			XK_g = 0x0067, /* U+0067 LATIN SMALL LETTER G */
			XK_h = 0x0068, /* U+0068 LATIN SMALL LETTER H */
			XK_i = 0x0069, /* U+0069 LATIN SMALL LETTER I */
			XK_j = 0x006a, /* U+006A LATIN SMALL LETTER J */
			XK_k = 0x006b, /* U+006B LATIN SMALL LETTER K */
			XK_l = 0x006c, /* U+006C LATIN SMALL LETTER L */
			XK_m = 0x006d, /* U+006D LATIN SMALL LETTER M */
			XK_n = 0x006e, /* U+006E LATIN SMALL LETTER N */
			XK_o = 0x006f, /* U+006F LATIN SMALL LETTER O */
			XK_p = 0x0070, /* U+0070 LATIN SMALL LETTER P */
			XK_q = 0x0071, /* U+0071 LATIN SMALL LETTER Q */
			XK_r = 0x0072, /* U+0072 LATIN SMALL LETTER R */
			XK_s = 0x0073, /* U+0073 LATIN SMALL LETTER S */
			XK_t = 0x0074, /* U+0074 LATIN SMALL LETTER T */
			XK_u = 0x0075, /* U+0075 LATIN SMALL LETTER U */
			XK_v = 0x0076, /* U+0076 LATIN SMALL LETTER V */
			XK_w = 0x0077, /* U+0077 LATIN SMALL LETTER W */
			XK_x = 0x0078, /* U+0078 LATIN SMALL LETTER X */
			XK_y = 0x0079, /* U+0079 LATIN SMALL LETTER Y */
			XK_z = 0x007a, /* U+007A LATIN SMALL LETTER Z */
			XK_F1 = 0xffbe,
			XK_F2 = 0xffbf,
			XK_F3 = 0xffc0,
			XK_F4 = 0xffc1,
			XK_F5 = 0xffc2,
			XK_F6 = 0xffc3,
			XK_F7 = 0xffc4,
			XK_F8 = 0xffc5,
			XK_F9 = 0xffc6,
			XK_F10 = 0xffc7,
			XK_F11 = 0xffc8,
			XK_F12 = 0xffc9,
			XK_F13 = 0xffca,
			XK_F14 = 0xffcb,
			XK_F15 = 0xffcc,
			XK_KP_0 = 0xffb0,
			XK_KP_1 = 0xffb1,
			XK_KP_2 = 0xffb2,
			XK_KP_3 = 0xffb3,
			XK_KP_4 = 0xffb4,
			XK_KP_5 = 0xffb5,
			XK_KP_6 = 0xffb6,
			XK_KP_7 = 0xffb7,
			XK_KP_8 = 0xffb8,
			XK_KP_9 = 0xffb9,
			XK_KP_Add = 0xffab,
			XK_KP_Subtract = 0xffad,
			XK_KP_Decimal = 0xffae,
			XK_KP_Equal = 0xffbd, /* Equals */
			XK_KP_Divide = 0xffaf,
			XK_KP_Multiply = 0xffaa,
			XK_KP_Enter = 0xff8d, /* Enter */
			XK_KP_Home = 0xff95,
			XK_KP_Up = 0xff97,
			XK_KP_Page_Up = 0xff9a,
			XK_KP_Left = 0xff96,
			XK_KP_Begin = 0xff9d,
			XK_KP_Right = 0xff98,
			XK_KP_End = 0xff9c,
			XK_KP_Down = 0xff99,
			XK_KP_Page_Down = 0xff9b,
			XK_KP_Insert = 0xff9e,
			XK_KP_Delete = 0xff9f,
			XK_Left = 0xff51, /* Move left, left arrow */
			XK_Up = 0xff52, /* Move up, up arrow */
			XK_Right = 0xff53, /* Move right, right arrow */
			XK_Down = 0xff54, /* Move down, down arrow */
			XK_Page_Up = 0xff55,
			XK_Page_Down = 0xff56,
			XK_Home = 0xff50,
			XK_End = 0xff57, /* EOL */
			XK_Num_Lock = 0xff7f,
			XK_Print = 0xff61,
			XK_Scroll_Lock = 0xff14,
			XK_Pause = 0xff13, /* Pause, hold */
			XK_Shift_L = 0xffe1, /* Left shift */
			XK_Shift_R = 0xffe2, /* Right shift */
			XK_Alt_R = 0xffea, /* Right alt */
			XK_Alt_L = 0xffe9, /* Left alt */
			XK_Insert = 0xff63, /* Insert, insert here */
			XK_Delete = 0xffff, /* Delete, rubout */
			XK_Super_L = 0xffeb, /* Left super */
			XK_Super_R = 0xffec, /* Right super */
			XK_Menu = 0xff67,
		}
		
		[Flags]
		public enum Buttons
		{		
			Button1Mask =	(1<<8),
			Button2Mask =	(1<<9),
			Button3Mask =	(1<<10),
			Button4Mask =	(1<<11),
			Button5Mask =	(1<<12),
			Button1MotionMask =	(1<<8),
			Button2MotionMask =	(1<<9),
			Button3MotionMask =	(1<<10),
			Button4MotionMask =	(1<<11),
			Button5MotionMask =	(1<<12),
			ShiftMask =	(1<<0),
			LockMask =	(1<<1),
			ControlMask =	(1<<2),
			Mod1Mask =	(1<<3),
			Mod2Mask =	(1<<4),
			Mod3Mask =	(1<<5),
			Mod4Mask =	(1<<6),
			Mod5Mask =	(1<<7),
			Button1 =	1,
			Button2 =	2,
			Button3 =	3,
			Button4 =	4,
			Button5 =	5,
		}
		
		#endregion
		
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
		internal const int AutoRepeatModeOff = 0;
		internal const int AutoRepeatModeOn = 1;
		internal const int AutoRepeatModeDefault = 2;
		
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
		public const int Mod4Mask = (1 << 6);
		public const int Mod1Mask = ( 1 << 3 );
		public const int MouseMovedPressedReleased = Mod4Mask | ButtonReleaseMask | ButtonPressMask;
		public const long KeyPressMask = ( 1L << 0 );
		public const long KeyReleaseMask = ( 1L << 1 );
		public const int ShiftMask = ( 1 << 0 );
		public const int LockMask = ( 1 << 1 );

		[DllImport(LibraryName)]
		internal static extern int XGrabPointer(IntPtr display, IntPtr grabWindow, bool ownerMvents, int eventMask, int pointerMode, int keyboardMode, IntPtr confineTo, IntPtr cursor, long time);
		[DllImport(LibraryName)]
		internal static extern int XUngrabPointer(IntPtr display, long time);
		[DllImport( LibraryName )]
		internal static extern int XLookupString( ref XKeyEvent even, StringBuilder bufferReturn, int bytesBuffer, out int keysymReturn, IntPtr xComposeStatus );
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
		[DllImportAttribute(LibraryName)]
		internal static extern IntPtr XOpenDisplay(IntPtr displayName);
		[DllImportAttribute(LibraryName)]
		internal static extern int XPending(IntPtr displayName);
		[DllImportAttribute(LibraryName)]
		internal static extern void XWarpPointer(IntPtr display, IntPtr sourceWindow, IntPtr destWindow, int srcX, int srcY,int srcWidth, int srcHeight,int destX, int destY);
		// XQueryPointer( Display *display,
		//			      Window w,
		//			      Window *root_return, Window *child_return,
		//			      int *root_x_return, int *root_y_return,
		//			      int *win_x_return, int *win_y_return,
		//			      unsigned int *mask_return )
		[DllImportAttribute(LibraryName)]
		internal static extern bool XQueryPointer( IntPtr display, IntPtr window, out IntPtr rootWindow, out IntPtr childWindow, out int root_x_return, out int root_y_return, out int win_x_return, out int win_y_return, out uint mask_return );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="display">
		/// connection to X server
		/// </param>
		/// <returns>
		/// A <see cref="IntPtr"/>
		/// </returns>
		[DllImportAttribute(LibraryName)]
		internal static extern IntPtr XCloseDisplay(IntPtr display);
		[DllImport(LibraryName)]
        internal static extern int XDefaultScreen(IntPtr display);
		/// <summary>
		/// Get's the root window for the default screen.
		/// </summary>
		/// <param name="display">Specifies the connection to the X server. </param>
		/// <returns>returns the root window for the default screen.</returns>
		[DllImportAttribute(LibraryName)]
		internal static extern IntPtr XDefaultRootWindow(IntPtr display);
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
		[DllImportAttribute(LibraryName)]
		internal static extern void XUngrabKeyboard(IntPtr display, long currentTime);
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
		[DllImportAttribute(LibraryName)]
		internal static extern int XGrabKeyboard(IntPtr display, IntPtr window, bool own, int pointerMode, int keyboardMode, long currentTime);
		[DllImportAttribute(LibraryName)]
		internal static extern int XSelectInput(IntPtr display, IntPtr window, long mask);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="display"></param>
		/// <param name="xevent"></param>
		[DllImportAttribute(LibraryName)]
		internal static extern void XNextEvent(IntPtr display, ref XEvent even);
		[DllImportAttribute(LibraryName)]
		internal static extern bool XCheckMaskEvent(IntPtr display, int even_mask, ref XEvent even);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key_event"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		[DllImportAttribute(LibraryName)]
		internal static extern IntPtr XLookupKeysym(ref XKeyEvent key_event, int index);
		
		[DllImport(LibraryName)]
		internal static extern string XKeysymToString( KeySym keysym );
		
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
		[DllImportAttribute(LibraryName)]
		internal static extern bool XkbSetDetectableAutoRepeat(IntPtr display, bool detectable, ref bool supported);
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
		[DllImportAttribute(LibraryName)]
		internal static extern uint XKeycodeToKeysym(IntPtr display, int keycode, int index);
		[DllImportAttribute(LibraryName)]
		internal static extern bool XTranslateCoordinates(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x, int src_y, out int dest_x_return, out int dest_y_return, out IntPtr child_return);
		
		[DllImportAttribute(LibraryName)]
		internal static extern int XAutoRepeatOn( IntPtr display );

		[DllImportAttribute(LibraryName)]
		internal static extern int XAutoRepeatOff( IntPtr display );
		
		[DllImportAttribute(LibraryName)]
		internal static extern int XGetKeyboardControl( IntPtr display, out XKeyboardState old );
		
		[DllImport(LibraryName)]
        internal extern static IntPtr XCreatePixmapCursor(IntPtr display, IntPtr source, IntPtr mask, ref XColor foreground_color, ref XColor background_color, int x_hot, int y_hot);		
		[DllImport(LibraryName)]
        internal extern static IntPtr XDefaultColormap(IntPtr display, int screen_number);	
		[DllImport(LibraryName)]
        internal extern static int XAllocColor(IntPtr display, IntPtr Colormap, ref XColor colorcell_def);		
		[DllImport(LibraryName)]
        internal extern static int XAllocNamedColor(IntPtr display, IntPtr colormap, string color_name, ref XColor screen_def_return, ref XColor exact_def_return);
		[DllImport(LibraryName)]
		internal extern static IntPtr XCreatePixmapFromBitmapData(IntPtr display, IntPtr d, byte[] data, int width, int height, long fg, int bg, int depth);
		[DllImport(LibraryName)]
		internal extern static IntPtr XCreateBitmapFromData(IntPtr display, IntPtr d, byte[] data, int width, int height);
		
		[DllImport(LibraryName)]
        internal extern static int XDefineCursor(IntPtr display, IntPtr window, IntPtr cursor);

		[DllImport(LibraryName)]
        internal extern static int XUndefineCursor(IntPtr display, IntPtr window);
		
		/// <summary>
		/// Convert's X11KeyCode to standard System.Windows.Forms.Keys;
		/// </summary>
		/// <param name="x11keycode">
		///  <see cref="System.UInt32"/>
		/// </param>
		/// <returns>
		///  <see cref="System.Windows.Forms.Keys"/>
		/// </returns>
		internal static KeyCode ToKeys( uint x11keycode )
		{
			switch (x11keycode)
			{
				case 32:
					return KeyCode.Key_SPACE;
				case 65509:
					return KeyCode.Key_CAPITAL;
				case 65289:
					return KeyCode.Key_TAB;
				case 65106:
					return KeyCode.Key_OEM_102;// == ^ ° (ger)
				case 65366:
					return KeyCode.Key_PGDOWN;
				case 65365:
					return KeyCode.Key_PGUP;
				case 65363:
					return KeyCode.Key_RIGHT;
				case 65361:
					return KeyCode.Key_LEFT;
				case 65364:
					return KeyCode.Key_DOWN;
				case 65362:
					return KeyCode.Key_UP;
				case 65293:
				//fall through, 65293 is return, 65421 is numpad-return.
				case 65421:
					return KeyCode.Key_RETURN;
				case 65451:
					return KeyCode.Key_ADD;
				case 65300:
					return KeyCode.Key_SCROLL;
				case 65299:
					return KeyCode.Key_PAUSE;
				case 65367:
					return KeyCode.Key_END;
				case 65360:
					return KeyCode.Key_HOME;
				case 65535:
					return KeyCode.Key_DELETE;
				case 65379:
					return KeyCode.Key_INSERT;
				case 65377:
					return KeyCode.Key_UNASSIGNED;
				case 65453:
					return KeyCode.Key_SUBTRACT;
				case 65450:
					return KeyCode.Key_MULTIPLY;
				case 65455:
					return KeyCode.Key_DIVIDE;
				case 65407:
					return KeyCode.Key_NUMLOCK;
				case 65434:
					return KeyCode.Key_NUMPAD9;
				case 65431:
					return KeyCode.Key_NUMPAD8;
				case 65429:
					return KeyCode.Key_NUMPAD7;
				case 65432:
					return KeyCode.Key_NUMPAD6;
				case 65437:
					return KeyCode.Key_NUMPAD5;
				case 65430:
					return KeyCode.Key_NUMPAD4;
				case 65435:
					return KeyCode.Key_NUMPAD3;
				case 65433:
					return KeyCode.Key_NUMPAD2;
				case 65438:
					return KeyCode.Key_NUMPAD0;
				case 65436:
					return KeyCode.Key_NUMPAD1;
				case 65470:
					return KeyCode.Key_F1;
				case 65471:
					return KeyCode.Key_F2;
				case 65472:
					return KeyCode.Key_F3;
				case 65473:
					return KeyCode.Key_F4;
				case 65474:
					return KeyCode.Key_F5;
				case 65475:
					return KeyCode.Key_F6;
				case 65476:
					return KeyCode.Key_F7;
				case 65477:
					return KeyCode.Key_F8;
				case 65478:
					return KeyCode.Key_F9;
				case 65479:
					return KeyCode.Key_F10;
				case 65480:
					return KeyCode.Key_F11;
				case 65481:
					return KeyCode.Key_F12;
				case 65105:
					return KeyCode.Key_OEM_102;// == ` ' (ger)
				case 223:
					return KeyCode.Key_LBRACKET;// == ? ß \ (ger)
				case 48:
					return KeyCode.Key_0;
				case 49:
					return KeyCode.Key_1;
				case 50:
					return KeyCode.Key_2;
				case 51:
					return KeyCode.Key_3;
				case 52:
					return KeyCode.Key_4;
				case 53:
					return KeyCode.Key_5;
				case 54:
					return KeyCode.Key_6;
				case 55:
					return KeyCode.Key_7;
				case 56:
					return KeyCode.Key_8;
				case 57:
					return KeyCode.Key_9;
				case 65513:
					return KeyCode.Key_LMENU;
				case 65383:
					return KeyCode.Key_RWIN;
				case 65515:
					return KeyCode.Key_LWIN;
				case 65507:
					return KeyCode.Key_LCONTROL;
				case 65506:
					return KeyCode.Key_RSHIFT;
				case 65505:
					return KeyCode.Key_LSHIFT;
				case 65307:
					return KeyCode.Key_ESCAPE;
				case 119:
					return KeyCode.Key_W;
				case 101:
					return KeyCode.Key_E;
				case 114:
					return KeyCode.Key_R;
				case 116:
					return KeyCode.Key_T;
				case 122:
					return KeyCode.Key_Z;
				case 117:
					return KeyCode.Key_U;
				case 105:
					return KeyCode.Key_I;
				case 111:
					return KeyCode.Key_O;
				case 112:
					return KeyCode.Key_P;
				case 97:
					return KeyCode.Key_A;
				case 115:
					return KeyCode.Key_S;
				case 100:
					return KeyCode.Key_D;
				case 102:
					return KeyCode.Key_F;
				case 103:
					return KeyCode.Key_G;
				case 104:
					return KeyCode.Key_H;
				case 106:
					return KeyCode.Key_J;
				case 107:
					return KeyCode.Key_K;
				case 108:
					return KeyCode.Key_L;
				case 121:
					return KeyCode.Key_Y;
				case 120:
					return KeyCode.Key_X;
				case 99:
					return KeyCode.Key_C;
				case 118:
					return KeyCode.Key_V;
				case 98:
					return KeyCode.Key_B;
				case 110:
					return KeyCode.Key_N;
				case 109:
					return KeyCode.Key_M;
				case 113:
					return KeyCode.Key_Q;
				default:
					return KeyCode.Key_UNASSIGNED;
			}
		}
		///// <summary>
		///// Convert's X11KeyCode to standard System.Windows.Forms.Keys;
		///// </summary>
		///// <param name="x11keycode">
		/////  <see cref="System.UInt32"/>
		///// </param>
		///// <returns>
		/////  <see cref="System.Windows.Forms.Keys"/>
		///// </returns>
		//internal static KeyCode ToKeys( uint x11keycode )
		//{
		//    switch ( x11keycode )
		//    {
		//        case 32:
		//            return KeyCode.Key_SPACE;
		//        case 65509:
		//            return KeyCode.Key_CAPITAL;
		//        case 65289:
		//            return KeyCode.Key_TAB;
		//        case 65106:
		//            //return Keys.Oem6;// == ^ ° (ger)
		//            return KeyCode.Key_OEM_102;
		//        case 65366:
		//            return KeyCode.Key_PGDOWN;
		//        case 65365:
		//            return KeyCode.Key_PGUP;
		//        case 65363:
		//            return KeyCode.Key_RIGHT;
		//        case 65361:
		//            return KeyCode.Key_LEFT;
		//        case 65364:
		//            return KeyCode.Key_DOWN;
		//        case 65362:
		//            return KeyCode.Key_UP;
		//        case 65293:
		//        //fall through, 65293 is return, 65421 is numpad-return.
		//        case 65421:
		//            return Keys.Return;
		//        case 65451:
		//            return Keys.Add;
		//        case 65300:
		//            return Keys.Scroll;
		//        case 65299:
		//            return Keys.Pause;
		//        case 65367:
		//            return Keys.End;
		//        case 65360:
		//            return Keys.Home;
		//        case 65535:
		//            return Keys.Delete;
		//        case 65379:
		//            return Keys.Insert;
		//        case 65377:
		//            return Keys.Print;
		//        case 65453:
		//            return Keys.Subtract;
		//        case 65450:
		//            return Keys.Multiply;
		//        case 65455:
		//            return Keys.Divide;
		//        case 65407:
		//            return Keys.NumLock;
		//        case 65434:
		//            return Keys.NumPad9;
		//        case 65431:
		//            return Keys.NumPad8;
		//        case 65429:
		//            return Keys.NumPad7;
		//        case 65432:
		//            return Keys.NumPad6;
		//        case 65437:
		//            return Keys.NumPad5;
		//        case 65430:
		//            return Keys.NumPad4;
		//        case 65435:
		//            return Keys.NumPad3;
		//        case 65433:
		//            return Keys.NumPad2;
		//        case 65438:
		//            return Keys.NumPad0;
		//        case 65436:
		//            return Keys.NumPad1;
		//        case 65470:
		//            return Keys.F1;
		//        case 65471:
		//            return Keys.F2;
		//        case 65472:
		//            return Keys.F3;
		//        case 65473:
		//            return Keys.F4;
		//        case 65474:
		//            return Keys.F5;
		//        case 65475:
		//            return Keys.F6;
		//        case 65476:
		//            return Keys.F7;
		//        case 65477:
		//            return Keys.F8;
		//        case 65478:
		//            return Keys.F9;
		//        case 65479:
		//            return Keys.F10;
		//        case 65480:
		//            return Keys.F11;
		//        case 65481:
		//            return Keys.F12;
		//        case 65105:
		//            return Keys.Oem6;// == ` ' (ger)
		//        case 223:
		//            return Keys.OemOpenBrackets;// == ? ß \ (ger)
		//        case 48:
		//            return Keys.D0;
		//        case 49:
		//            return Keys.D1;
		//        case 50:
		//            return Keys.D2;
		//        case 51:
		//            return Keys.D3;
		//        case 52:
		//            return Keys.D4;
		//        case 53:
		//            return Keys.D5;
		//        case 54:
		//            return Keys.D6;
		//        case 55:
		//            return Keys.D7;
		//        case 56:
		//            return Keys.D8;
		//        case 57:
		//            return Keys.D9;
		//        case 65513:
		//            return Keys.Alt;
		//        case 65383:
		//            return Keys.RWin;
		//        case 65515:
		//            return Keys.LWin;
		//        case 65507:
		//            return Keys.LControlKey;
		//        case 65506:
		//            return Keys.RShiftKey;
		//        case 65505:
		//            return Keys.LShiftKey;
		//        case 65307:
		//            return Keys.Escape;
		//        case 119:
		//            return Keys.W;
		//        case 101:
		//            return Keys.E;
		//        case 114:
		//            return Keys.R;
		//        case 116:
		//            return Keys.T;
		//        case 122:
		//            return Keys.Z;
		//        case 117:
		//            return Keys.U;
		//        case 105:
		//            return Keys.I;
		//        case 111:
		//            return Keys.O;
		//        case 112:
		//            return Keys.P;
		//        case 97:
		//            return Keys.A;
		//        case 115:
		//            return Keys.S;
		//        case 100:
		//            return Keys.D;
		//        case 102:
		//            return Keys.F;
		//        case 103:
		//            return Keys.G;
		//        case 104:
		//            return Keys.H;
		//        case 106:
		//            return Keys.J;
		//        case 107:
		//            return Keys.K;
		//        case 108:
		//            return Keys.L;
		//        case 121:
		//            return Keys.Y;
		//        case 120:
		//            return Keys.X;
		//        case 99:
		//            return Keys.C;
		//        case 118:
		//            return Keys.V;
		//        case 98:
		//            return Keys.B;
		//        case 110:
		//            return Keys.N;
		//        case 109:
		//            return Keys.M;
		//        case 113:
		//            return Keys.Q;
		//        default:
		//            return Keys.None;
		//    }
		//}
	}
}

