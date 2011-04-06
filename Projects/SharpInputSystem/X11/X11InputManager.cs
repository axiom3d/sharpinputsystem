using System;
using System.Collections.Generic;
using System.Text;

namespace SharpInputSystem
{
	public class X11InputManager : InputManager
	{
		public bool GrabsState
		{
			get;
			set;
		}

		protected override void _initialize( ParameterList args )
		{
			throw new NotImplementedException();
		}

		public override string InputSystemName
		{
			get
			{
				return "X11";
			}
		}

	}
}
