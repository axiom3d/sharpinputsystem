using System;
using System.Collections.Generic;
using System.Text;
//using Common.Logging;
using MXF = Microsoft.Xna.Framework;
using SIS = SharpInputSystem;

namespace SharpInputSystem.Test.Console
{
	public interface IInputManagerService
	{ 
		SIS.InputManager Manager { get; }
		SIS.Keyboard Keyboard { get; }
		SIS.Mouse Mouse { get; }
		IList<SIS.Joystick> GamePads { get; }
	}

	class NullMouse : SIS.Mouse
	{

		public override void Capture()
		{
		}

		protected override void Initialize()
		{
		}
	}

	class NullKeyboard : SIS.Keyboard
	{

		public override int[] KeyStates
		{
			get { return new int[0]; }
		}

		public override bool IsKeyDown(KeyCode key)
		{
			return false;
		}

		public override string AsString(KeyCode key)
		{
			return String.Empty;
		}

		public override void Capture()
		{
		}

		protected override void Initialize()
		{
		}
	}

	class InputManager: MXF.GameComponent, IInputManagerService
	{
		#region Fields and Properties
		
		//private static readonly ILog log = LogManager.GetLogger(typeof(InputManager));

		SIS.InputManager _inputManager;
		public SIS.InputManager Manager { get { return _inputManager; } }

		SIS.Keyboard _kb;
		public SIS.Keyboard Keyboard { get { return _kb; } }

		SIS.Mouse _m;
		public SIS.Mouse Mouse { get { return _m; } }

		List<SIS.Joystick> _joys = new List<SIS.Joystick>();
		public IList<SIS.Joystick> GamePads { get { return _joys; } }
		List<SIS.ForceFeedback> _ff = new List<SIS.ForceFeedback>();

		#endregion Fields and Properties

		public InputManager( MXF.Game game )
			: base( game )
		{
		}

		public override void Initialize()
		{
			ParameterList args = new ParameterList();
			args.Add(new Parameter("WINDOW", this.Game.Window.Handle));
#if !( WINDOWS_PHONE )
			args.Add( new Parameter( "w32_mouse_hide", true ) );
#endif
			_inputManager = SIS.InputManager.CreateInputSystem(typeof(SIS.Xna.XnaInputManagerFactory), args);

			//log.Info( String.Format( "SIS Version : {0}", _inputManager.Version ) );
			//log.Info( String.Format( "Platform : {0}", _inputManager.InputSystemName ) );
			//log.Info( String.Format( "Number of Mice : {0}", _inputManager.DeviceCount<SIS.Mouse>() ) );
			//log.Info( String.Format( "Number of Keyboards : {0}", _inputManager.DeviceCount<SIS.Keyboard>() ) );
			//log.Info( String.Format( "Number of GamePads: {0}", _inputManager.DeviceCount<SIS.Joystick>() ) );

			bool buffered = false;

			if ( _inputManager.DeviceCount<SIS.Keyboard>() > 0 )
			{
				_kb = _inputManager.CreateInputObject<SIS.Keyboard>( buffered, "" );
				//log.Info( String.Format( "Created {0}buffered keyboard", buffered ? "" : "un" ) );
			}
			else
				_kb = new NullKeyboard();

			if ( _inputManager.DeviceCount<SIS.Mouse>() > 0 )
			{
				_m = _inputManager.CreateInputObject<SIS.Mouse>( buffered, "" );
				//log.Info( String.Format( "Created {0}buffered mouse", buffered ? "" : "un" ) );
				////_m.EventListener = _handler;

				//MouseState ms = _m.MouseState;
				//ms.Width = 100;
				//ms.Height = 100;
			}
			else
				_m = new NullMouse();

			////This demo only uses at max 4 joys
			int numSticks = _inputManager.DeviceCount<SIS.Joystick>();
			if( numSticks > 4 )	numSticks = 4;

			for ( int i = 0; i < numSticks; ++i )
			{
				_joys.Insert( i, _inputManager.CreateInputObject<SIS.Joystick>( true, "" ) );
				//_joys[ i ].EventListener = _handler;

				_ff.Insert( i, (SIS.ForceFeedback)_joys[ i ].QueryInterface<SIS.ForceFeedback>() );
				if ( _ff[ i ] != null )
				{
					//log.Info( String.Format( "Created buffered joystick with ForceFeedback support." ) );
					//Dump out all the supported effects:
					//        const ForceFeedback::SupportedEffectList &list = g_ff[i]->getSupportedEffects();
					//        ForceFeedback::SupportedEffectList::const_iterator i = list.begin(),
					//            e = list.end();
					//        for( ; i != e; ++i)
					//            std::cout << "Force =  " << i->first << " Type = " << i->second << std::endl;
				}
				//else
					//log.Info( String.Format( "Created buffered joystick. **without** FF support" ) );
			}
			base.Initialize();
		}

		public override void Update( MXF.GameTime gameTime )
		{
			base.Update( gameTime );
		}
	}
}
