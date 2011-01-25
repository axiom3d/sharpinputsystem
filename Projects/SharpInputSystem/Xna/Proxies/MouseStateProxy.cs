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

using log4net;
using System.Reflection;
using System.Security.Policy;

#endregion Namespace Declarations

namespace SharpInputSystem.Proxies.Xna
{
	/// <summary>
	/// Proxy Class to the <see>Microsoft.Xna.Framework.Input.MouseState</see> Class
	/// </summary>
	class MouseStateProxy
	{
		public enum ButtonState
		{
			Released,
			Pressed,
		}

		#region Fields and Properties

		private const string XnaAssembly = "Microsoft.Xna.Framework, Version=3.1.0.0, Culture=Neutral, PublicKeyToken=6d5c3888ef60e27d";
		private const string XnaType = "Microsoft.Xna.Framework.Input.MouseState";

		private static Assembly xfg;
		private static Type xnaMouseState;
		private static PropertyInfo X_get, Y_get, ScrollWheelValue_get;
		private static PropertyInfo LeftButton_get, MiddleButton_get, RightButton_get, XButton1_get, XButton2_get;

		private static readonly ILog log = LogManager.GetLogger( typeof( MouseStateProxy ) );

		#endregion Fields and Properties

		#region Construction and Destruction

		static MouseStateProxy()
		{
			// Initialize refelection proxies.
			xfg = Assembly.Load( XnaAssembly );
			xnaMouseState = xfg.GetType( XnaType );

			X_get = xnaMouseState.GetProperty( "X" );
			Y_get = xnaMouseState.GetProperty( "Y" );
			ScrollWheelValue_get = xnaMouseState.GetProperty( "ScrollWheelValue" );
			LeftButton_get = xnaMouseState.GetProperty( "LeftButton" );
			MiddleButton_get = xnaMouseState.GetProperty( "MiddleButton" );
			RightButton_get = xnaMouseState.GetProperty( "RightButton" );
			XButton1_get = xnaMouseState.GetProperty( "XButton1" );
			XButton2_get = xnaMouseState.GetProperty( "XButton2" );
		}

		public MouseStateProxy()
		{
			X = Y = ScrollWheelValue = 0;
			LeftButton = MiddleButton = RightButton = XButton1 = XButton2 = ButtonState.Released;
		}

		public MouseStateProxy( object xnaState )
			: this()
		{
			X = (int)X_get.GetValue( xnaState, null );
			Y = (int)Y_get.GetValue( xnaState, null );
			ScrollWheelValue = (int)ScrollWheelValue_get.GetValue( xnaState, null );
			LeftButton = (ButtonState)LeftButton_get.GetValue( xnaState, null );
			MiddleButton = (ButtonState)MiddleButton_get.GetValue( xnaState, null );
			RightButton = (ButtonState)RightButton_get.GetValue( xnaState, null );
			XButton1 = (ButtonState)XButton1_get.GetValue( xnaState, null );
			XButton2 = (ButtonState)XButton2_get.GetValue( xnaState, null );
		}

		#endregion Construction and Destruction

		#region Microsoft.Xna.Framework.Input.MouseState Proxies

		public int X
		{
			get;
			protected set;
		}
		public int Y
		{
			get;
			protected set;
		}
		public int ScrollWheelValue
		{
			get;
			protected set;
		}

		public ButtonState LeftButton
		{
			get;
			protected set;
		}
		public ButtonState MiddleButton
		{
			get;
			protected set;
		}
		public ButtonState RightButton
		{
			get;
			protected set;
		}
		public ButtonState XButton1
		{
			get;
			protected set;
		}
		public ButtonState XButton2
		{
			get;
			protected set;
		}

		#endregion Microsoft.Xna.Framework.Input.MouseState Proxies
	}
}