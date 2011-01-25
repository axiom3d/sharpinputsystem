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
	/// Proxy Class to the <see>Microsoft.Xna.Framework.Input.Keyboard</see> Class
	/// </summary>
	class KeyboardProxy
	{
		#region Fields and Properties

		private const string XnaAssembly = "Microsoft.Xna.Framework, Version=3.1.0.0, Culture=Neutral, PublicKeyToken=6d5c3888ef60e27d";
		private const string XnaType = "Microsoft.Xna.Framework.Input.Keyboard";

		private static Assembly xfg;
		private static Type xnaKeyboard;
		private static MethodInfo getStateMethod;

		private static readonly ILog log = LogManager.GetLogger( typeof( KeyboardProxy ) );

		#endregion Fields and Properties

		#region Construction and Destruction

		static KeyboardProxy()
		{
			// Initialize refelection proxies.
			xfg = Assembly.Load( XnaAssembly );
			xnaKeyboard = xfg.GetType( XnaType );
			getStateMethod = xnaKeyboard.GetMethod( "GetState", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, new ParameterModifier[] { } );
		}

		#endregion Construction and Destruction

		#region Microsoft.Xna.Framework.Input.MouseState Proxies

		static public KeyboardStateProxy GetState()
		{
			if ( getStateMethod != null )
			{
				object state = getStateMethod.Invoke( null, null );
				return new KeyboardStateProxy( state );
			}
			return new KeyboardStateProxy();
		}

		#endregion Microsoft.Xna.Framework.Input.MouseState Proxies
	}
}