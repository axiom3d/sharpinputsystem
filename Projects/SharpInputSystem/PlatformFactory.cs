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
using System.IO;
using System.Reflection;

#endregion Namespace Declarations

namespace SharpInputSystem
{
	internal static class PlatformFactory
	{
		public static InputManager Create( PlatformApi api )
		{
			IList<IInputManagerFactory> systems = CollectSystems();
			foreach ( IInputManagerFactory system in systems )
				if ( (system.Api & api) == api || ( api == PlatformApi.AutoDetect ))
					return system.Create();

			throw new Exception( "No Supported Input system found." );
		}

		private static IList<IInputManagerFactory> CollectSystems()
		{
			IList<IInputManagerFactory> system = new List<IInputManagerFactory>();
			string[] files = Directory.GetFiles( "." );
			string assemblyName = Assembly.GetExecutingAssembly().GetName().Name + ".dll";

			foreach ( string file in files )
			{
                string currentFile = Path.GetFileName(file);

                if (Path.GetExtension(file) != ".dll" || currentFile == assemblyName)
                    continue;

				string fullPath = Path.GetFullPath( file );
                try
                {
				    Assembly assemembly = Assembly.LoadFrom( fullPath );
				    if ( assemblyName != null )
				    {
					    Type[] types = assemembly.GetTypes();
					    foreach ( Type t in types )
					    {
						    if ( typeof( IInputManagerFactory ).IsAssignableFrom( t ) )
						    {

							    system.Add( (IInputManagerFactory)Activator.CreateInstance( t ) );
							    break;
						    }
					    }
				    }
                }
                finally
                {
                	
                }
			}

			return system;
		}

	}
}
