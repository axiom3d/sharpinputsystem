using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyDescription( "Portable Input System in C#" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "SharpInputSystem.DirectX" )]
[assembly: AssemblyCopyright( "Copyright © 2007-2012 Michael Cummings" )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: AssemblyTitle( "SharpInputSystem.DirectX" )]
[assembly: AssemblyConfiguration( "" )]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid( "bbc146e1-c620-440f-b3ed-255bcab47450" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

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
