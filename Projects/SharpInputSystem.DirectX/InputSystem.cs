?using System;
using SharpInputSystem;
namespace SharpInputSystem.DirectX
{
	public class InputSystem : IInputSystemFactory
	{
		public PlatformApi Api
		{
			get { return PlatformApi.DirectX; }
		}

		public InputManager Create()
		{
			return new DirectXInputManager();
		}
	}
}
