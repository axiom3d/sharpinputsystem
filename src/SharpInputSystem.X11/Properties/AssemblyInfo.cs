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

#region SVN Version Information
// <file>
//     <license see="license.txt"/>
//     <id value="$Id$"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion Namespace Declarations

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyDescription( "Portable Input System in C#" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "SharpInputSystem" )]
[assembly: AssemblyCopyright( "Copyright © 2007-2011 Michael Cummings" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: AssemblyTitle( "SharpInputSystem X11 Implementation" )]
[assembly: AssemblyConfiguration( "" )]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
//[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
//[assembly: Guid( "746f6979-9dee-4461-82bf-5634bf329602" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

#if DEBUG
[assembly: AssemblyVersion( "0.4.0.*" )]
#else
[assembly: AssemblyVersion( "0.4.0.0" )]
#endif

#if !XBOX360
[assembly: AssemblyFileVersion( "0.4.0.0" )]
#endif

#if !XBOX360
// Configure Common.Logging using the .config file
//[assembly: Common.Logging.Configuration.XmlConfigurator( Watch = true )]
#endif
