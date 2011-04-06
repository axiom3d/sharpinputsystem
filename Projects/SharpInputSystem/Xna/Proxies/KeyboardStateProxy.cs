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

//using Common.Logging;
using System.Reflection;

#endregion Namespace Declarations

namespace SharpInputSystem.Proxies.Xna
{
	/// <summary>
	/// Proxy Class to the <see>Microsoft.Xna.Framework.Input.MouseState</see> Class
	/// </summary>
	class KeyboardStateProxy
	{
		public enum ButtonState
		{
			Released,
			Pressed,
		}

		#region Fields and Properties

		private const string XnaAssembly = "Microsoft.Xna.Framework, Version=4.0.0.0, Culture=Neutral, PublicKeyToken=6d5c3888ef60e27d";
		private const string XnaAssemblyPath = @"\Microsoft XNA\XNA Game Studio\v4.0\References\Windows\x86\Microsoft.Xna.Framework.dll";
		private const string XnaType = "Microsoft.Xna.Framework.Input.KeyboardState";

		private static Assembly xfg;
		private static Type xnaKeyboardState;
		private static MethodInfo _getPressedKeys;
		private static MethodInfo _isKeyDown;
		private object instance;

		//private static readonly ILog log = LogManager.GetLogger( typeof( MouseStateProxy ) );

		#endregion Fields and Properties

		#region Construction and Destruction

		static KeyboardStateProxy()
		{
			// Initialize refelection proxies.
			var programFilesPath = System.Environment.GetFolderPath( System.Environment.SpecialFolder.ProgramFilesX86 );
			xfg = System.Reflection.Assembly.Load( System.IO.File.ReadAllBytes( programFilesPath + XnaAssemblyPath ) );
			xnaKeyboardState = xfg.GetType( XnaType );

			_getPressedKeys = xnaKeyboardState.GetMethod( "GetPressedKeys" );
			_isKeyDown = xnaKeyboardState.GetMethod( "IsKeyDown" );
		}

		public KeyboardStateProxy()
		{
		}

		public KeyboardStateProxy( object xnaState )
			: this()
		{
			instance = xnaState;
		}

		#endregion Construction and Destruction

		#region Microsoft.Xna.Framework.Input.KeyboardState Proxies

		public int[] GetPressedKeys()
		{
			return (int[])_getPressedKeys.Invoke( instance, new object[] { } );
		}

		public bool IsKeyDown( int key )
		{
			return (bool)_isKeyDown.Invoke( instance, new object[] { key } );
		}

		#endregion Microsoft.Xna.Framework.Input.KeyboardState Proxies
	}
}