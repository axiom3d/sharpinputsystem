#region LGPL License
/*
Sharp Input System Library
Copyright (C) 2007 Michael Cummings

The overall design, and a majority of the core code contained within 
this library is a derivative of the open source Open Input System ( OIS ) , 
which can be found at http://www.sourceforge.net/projects/wgois.  
Many thanks to the Phillip Castaneda for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

using Tao.Sdl;

#endregion Namespace Declarations
			
namespace SharpInputSystem
{
	/// <summary>
	/// Sdl Inout Manager wrapper
	/// </summary>
	class SdlInputManager : InputManager
	{
		private bool _grabbed;
		public bool GrabMode
		{
			get
			{
				return _grabbed;
			}
			set
			{
				_grabbed = value;
			}
		}

		private void _enumerateDevices()
		{
		}

		private void _parseConfigSettings( ParameterList args )
		{
		}


		#region InputManager Implementation

		#region InputManager Methods

		public override T CreateInputObject<T>( bool bufferMode, string vendor )
		{
			string typeName = this.InputSystemName + typeof( T ).Name;
			Type objectType;
			T obj;

			objectType = System.Reflection.Assembly.GetExecutingAssembly().GetType( "SharpInputSystem." + typeName );
			if ( objectType == null )
			{
				throw new Exception( String.Format( "Device type [{0}] not supported.", typeof( T ).Name ) );
			}

			System.Reflection.BindingFlags bindingFlags = System.Reflection.BindingFlags.CreateInstance;

			obj = (T)objectType.InvokeMember( typeName,
											  bindingFlags,
											  null,
											  null,
											  new object[] { this, bufferMode } );

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

			return obj;
		}

		protected override void _initialize( ParameterList args )
		{
			if ( Sdl.SDL_WasInit( 0 ) == 0 )
			{
				throw new Exception( "SDL not initialized already." );
			}

			// SDL initialized, finish

			_parseConfigSettings( args );
			_enumerateDevices();
		}

		#endregion InputManager Methods

		#endregion InputManager Implementation
	}
}
