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

#endregion Namespace Declarations

namespace SharpInputSystem
{
	internal static class Environment
	{
		private static readonly Common.Logging.ILog log = Common.Logging.LogManager.GetLogger( typeof( Environment ) );

		public static bool RunningOnWindows
		{
			get;
			protected set;
		}

		public static bool RunningOnUnix
		{
			get;
			protected set;
		}

		public static bool RunningOnX11
		{
			get;
			protected set;
		}

		public static bool RunningOnMacOS
		{
			get;
			protected set;
		}

		public static bool RunningOnLinux
		{
			get;
			protected set;
		}

		public static bool RunningOnMono
		{
			get;
			protected set;
		}

		// Detects the underlying OS and runtime.
		static Environment()
		{
			if ( System.Environment.OSVersion.Platform == PlatformID.Win32NT ||
				 System.Environment.OSVersion.Platform == PlatformID.Win32S ||
				 System.Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				 System.Environment.OSVersion.Platform == PlatformID.WinCE )
			{
				RunningOnWindows = true;
			}
			else if ( System.Environment.OSVersion.Platform == PlatformID.Unix ||
					  System.Environment.OSVersion.Platform == (PlatformID)4 )
			{
				// Distinguish between Linux, Mac OS X and other Unix operating systems.
				string kernel_name = ( new UnixKernel() ).SysName;
				switch ( kernel_name )
				{
					case null:
					case "":
						throw new PlatformNotSupportedException( "Unknown platform." );

					case "Linux":
						RunningOnLinux = RunningOnUnix = true;
						break;

					case "Darwin":
						RunningOnMacOS = RunningOnUnix = true;
						break;

					default:
						RunningOnUnix = true;
						break;
				}
			}
			else
				throw new PlatformNotSupportedException( "Unknown platform." );

			// Detect whether X is present.
			// Hack: it seems that this check will cause X to initialize itself on Mac OS X Leopard and newer.
			// We don't want that (we'll be using the native interfaces anyway), so we'll avoid this check
			// when we detect Mac OS X.
			if ( !RunningOnMacOS )
			{
				try
				{
					RunningOnX11 = LibX11.XOpenDisplay(IntPtr.Zero) != IntPtr.Zero;
				}
				catch
				{
				}
			}

			// Detect the Mono runtime (code adapted from http://mono.wikia.com/wiki/Detecting_if_program_is_running_in_Mono).
			RunningOnMono = Type.GetType( "Mono.Runtime" ) != null;

			log.Debug( m => m( "Detected Runtime Environment : {0} / {1}", RunningOnWindows ? "Windows" : RunningOnLinux ? "Linux" : RunningOnMacOS ? "MacOS" : RunningOnUnix ? "Unix" : RunningOnX11 ? "X11" : "Unknown Platform",
																           RunningOnMono ? "Mono" : ".Net" ) );
		}

	}
}
