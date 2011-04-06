﻿#region MIT/X11 License
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
	class UnixKernel
	{
		//private static readonly Common.Logging.ILog log = Common.Logging.LogManager.GetLogger( typeof( Environment ) );

		private static utsname _uts = new utsname();

		public string SysName
		{
			get
			{
				return _uts.sysname ?? string.Empty;
			}
		}

		public string NodeName
		{
			get
			{
				return _uts.nodename ?? string.Empty;
			}
		}

		public string Release
		{
			get
			{
				return _uts.release ?? string.Empty;
			}
		}

		public string Version
		{
			get
			{
				return _uts.version ?? string.Empty;
			}
		}

		public string Machine
		{
			get
			{
				return _uts.machine ?? string.Empty;
			}
		}

		static UnixKernel()
		{
		}

		/// <summary>
		/// Detects the unix kernel by p/invoking uname (libc).
		/// </summary>
		/// <returns></returns>
		public UnixKernel()
		{
			try
			{
				uname( out _uts );
			}
			catch ( Exception ex )
			{
				//log.Error( m => m( "Failed to call uname()." ), ex );
			}
		}

		[DllImport( "libc" )]
		private static extern void uname( out utsname uname_struct );

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
		struct utsname
		{
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
			public string sysname;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
			public string nodename;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
			public string release;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
			public string version;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
			public string machine;

			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 1024 )]
			public string extraJustInCase;

		}
	}
}
