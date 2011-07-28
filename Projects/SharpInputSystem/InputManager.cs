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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

#endregion Namespace Declarations

namespace SharpInputSystem
{
	public class Pair<K, T>
	{
		public K first;
		public T second;
	}

	public class Parameter : Pair<string, object>
	{
		public Parameter( string key, object value )
		{
			first = key;
			second = value;
		}
	}

	public class ParameterList : System.Collections.Generic.List<Parameter>
	{
	}

	abstract public class InputManager
	{
		private static readonly Common.Logging.ILog log = Common.Logging.LogManager.GetLogger( typeof( InputManager ) );

		private List<InputObjectFactory> _factories = new List<InputObjectFactory>();
		protected Dictionary<InputObject, InputObjectFactory> _createdInputObjects = new Dictionary<InputObject, InputObjectFactory>();

		/// <summary>
		/// Initializes the static instance of the class
		/// </summary>
		static InputManager()
		{
			log.Info( "Static initialization complete." );
		}

		/// <summary>
		/// Creates appropriate input system dependent on platform.
		/// </summary>
		/// <param name="windowHandle">Contains OS specific window handle (such as HWND or X11 Window)</param>
		/// <returns>A reference to the created manager, or raises an Exception</returns>
		/// <exception cref="Exception">Exception</exception>
		/// <exception cref="ArgumentException">ArgumentException</exception>
		static public InputManager CreateInputSystem( object windowHandle )
		{
			ParameterList args = new ParameterList();
			args.Add( new Parameter( "WINDOW", windowHandle ) );
			return CreateInputSystem( args );
		}

		/// <summary>
		/// Creates appropriate input system dependent on platform. 
		/// </summary>
		/// <param name="args">contains OS specific info (such as HWND and HINSTANCE for window apps), and access mode.</param>
		/// <returns>A reference to the created manager, or raises an Exception</returns>
		/// <exception cref="Exception">Exception</exception>
		/// <exception cref="ArgumentException">ArgumentException</exception>
		static public InputManager CreateInputSystem( PlatformApi api, ParameterList args )
		{
			InputManager im;

			// Since this is a required parameter for all InputManagers, check it here instead of having each 
			if ( !args.Any( delegate( Parameter p )
			{
				return p.first.ToUpperInvariant() == "WINDOW";
			} ) )
			{
				ArgumentException ae = new ArgumentException( "Cannot initialize InputManager instance, no 'WINDOW' parameter present." );
				//log.Error( "", ae );
				throw ae;
			}

			im = PlatformFactory.Create( api );

			im._initialize( args );
			return im;

		}

		/// <summary>
		/// Creates appropriate input system dependent on platform. 
		/// </summary>
		/// <param name="args">contains OS specific info (such as HWND and HINSTANCE for window apps), and access mode.</param>
		/// <returns>A reference to the created manager, or raises an Exception</returns>
		/// <exception cref="Exception">Exception</exception>
		/// <exception cref="ArgumentException">ArgumentException</exception>
		static public InputManager CreateInputSystem( ParameterList args )
		{
			//log.Info( "Detecting native platform." );

			PlatformApi api = PlatformApi.AutoDetect;

			return CreateInputSystem( api, args );
		}

		/// <summary>
		/// Gets version of the Assembly
		/// </summary>
		virtual public string Version
		{
			get
			{
#if !XBOX360
				return ( (AssemblyFileVersionAttribute)( Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyFileVersionAttribute ), false )[ 0 ] ) ).Version;
#else
				return "0.3.0.0";
#endif
			}
		}

		/// <summary>
		/// Gets the name of the current input system.. eg. "DirectX", "Sdl", "Xna", etc
		/// </summary>
		virtual public string InputSystemName
		{
			get
			{
				return ( (AssemblyConfigurationAttribute)( Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyConfigurationAttribute ), false )[ 0 ] ) ).Configuration;
			}
		}

		/// <summary>
		/// Returns the number of the specified devices discovered by OIS
		/// </summary>
		/// <typeparam name="T">Type that you are interested in</typeparam>
		/// <returns></returns>
		public int DeviceCount<T>() where T : InputObject
		{
			int deviceCount = 0;
			foreach ( InputObjectFactory factory in _factories )
			{
				deviceCount += factory.DeviceCount<T>();
			}
			return deviceCount;
		}

		/// <summary>
		/// Lists all unused devices
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<Type, string>> FreeDevices
		{
			get
			{
				List<KeyValuePair<Type, string>> freeDevices = new List<KeyValuePair<Type, string>>();
				foreach ( InputObjectFactory factory in _factories )
				{
					freeDevices.AddRange( factory.FreeDevices );
				}
				return freeDevices;
			}
		}


		/// <summary>
		/// Returns the type of input requested or raises Exception
		/// </summary>
		/// <param name="type"></param>
		/// <param name="buffermode"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public T CreateInputObject<T>( bool bufferMode, string vendor ) where T : InputObject
		{
			InputObject obj = null;

			foreach ( InputObjectFactory factory in _factories )
			{
				if ( factory.FreeDeviceCount<T>() > 0 )
				{
					if ( vendor == null || vendor == String.Empty || factory.VendorExists<T>( vendor ) )
					{
						obj = factory.CreateInputObject<T>( this, bufferMode, vendor );
						if ( obj != null )
						{
							_createdInputObjects.Add( obj, factory );
							break;
						}
					}
				}
			}

			if ( obj == null )
				throw new Exception( "No devices match requested type." );

			try
			{
				obj.initialize();
			}
			catch ( Exception e )
			{
				obj.Dispose();
				obj = null;
				throw e; //rethrow
			}

			return (T)obj;
		}

		/// <summary>
		/// Destroys Input Object
		/// </summary>
		/// <param name="inputObject"></param>
		virtual public void DestroyInputObject( InputObject inputObject )
		{
			if ( inputObject != null )
			{
				if ( _createdInputObjects.ContainsKey( inputObject ) )
				{
					( (InputObjectFactory)_createdInputObjects[ inputObject ] ).DestroyInputObject( inputObject );
					_createdInputObjects.Remove( inputObject );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		abstract protected void _initialize( ParameterList args );

		protected InputManager()
		{
		}

		public void RegisterFactory( InputObjectFactory factory )
		{
			_factories.Add( factory );
		}

		public void UnregisterFactory( InputObjectFactory factory )
		{
			_factories.Remove( factory );
		}
	}
}
