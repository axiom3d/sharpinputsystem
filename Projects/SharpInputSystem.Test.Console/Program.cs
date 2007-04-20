using System;
using System.Collections.Generic;
using System.Text;
using SWF = System.Windows.Forms;

using log4net;

using SharpInputSystem;

namespace SharpInputSystem.Test.Console
{
    class EventHandler : IKeyboardListener, IMouseListener
    {
		private static readonly ILog log = LogManager.GetLogger( typeof( Program ) );

        public bool appRunning = true;

        #region IKeyboardListener Members

        public bool KeyPressed( KeyEventArgs e )
        {
            log.Info( String.Format(  "KeyPressed : {0} {1}", e.Key, e.Text ));
            return true;
        }

        public bool KeyReleased( KeyEventArgs e )
        {
            if ( e.Key == KeyCode.Key_ESCAPE || e.Key == KeyCode.Key_Q )
                appRunning = false;
            return true;
        }

        #endregion

        #region IMouseListener Members

        public bool MouseMoved( MouseEventArgs arg )
        {
            log.Info( String.Format(  "MouseMoved : R( {0} , {1} ) A( {2} , {3} )", arg.State.X.Relative, arg.State.Y.Relative, arg.State.X.Absolute, arg.State.Y.Absolute ));
            return true;
        }

        public bool MousePressed( MouseEventArgs arg, MouseButtonID id )
        {
            log.Info( String.Format(  "MousePressed : {0}", arg.State.Buttons ));
            return true;
        }

        public bool MouseReleased( MouseEventArgs arg, MouseButtonID id )
        {
            log.Info( String.Format(  "MouseReleased : {0}", arg.State.Buttons ));
            return true;
        }

        #endregion
    }

    class Program
    {
        private static EventHandler _handler = new EventHandler();

        private static InputManager _inputManager;
        private static Keyboard _kb;
        private static Mouse _m;

		private static readonly ILog log = LogManager.GetLogger( typeof( Program ) );

		static void DoStartup()
        {
	        ParameterList pl = new ParameterList();
            Main frm = new Main();
            pl.Add( new Parameter( "WINDOW", frm) );

            //Default mode is foreground exclusive..but, we want to show mouse - so nonexclusive
            //pl.Add( new Parameter( "w32_mouse", "CLF_FOREGROUND" ) );
            //pl.Add( new Parameter( "w32_mouse", "CLF_NONEXCLUSIVE" ) );

	        //This never returns null.. it will raise an exception on errors
	        _inputManager = InputManager.CreateInputSystem(pl);

			log.Info( String.Format( "SIS Version : {0}", _inputManager.Version ) );
			log.Info( String.Format( "Platform : {0}", _inputManager.InputSystemName ) );
			log.Info( String.Format( "Number of Mice : {0}", _inputManager.MiceCount ) );
			log.Info( String.Format( "Number of Keyboards : {0}", _inputManager.KeyboardCount ) );
			log.Info( String.Format( "Number of Joys/Pads: {0}", _inputManager.JoystickCount ) );

            bool buffered = true;

            if ( _inputManager.KeyboardCount > 0 )
            {
                _kb = _inputManager.CreateInputObject<Keyboard>( buffered );
                log.Info( String.Format( "Created {0}buffered keyboard", buffered ? "" : "un" ) );
                _kb.EventListener = _handler;
            }

            if( _inputManager.MiceCount > 0 )
            {
                _m = _inputManager.CreateInputObject<Mouse>( buffered );
                log.Info( String.Format( "Created {0}buffered mouse", buffered ? "" : "un" ) );
                _m.EventListener = _handler;

                MouseState ms = _m.MouseState;
                ms.Width = 100;
                ms.Height = 100;
            }

            ////This demo only uses at max 4 joys
            //int numSticks = g_InputManager->numJoysticks();
            //if( numSticks > 4 )	numSticks = 4;

            //for( int i = 0; i < numSticks; ++i )
            //{
            //    g_joys[i] = (JoyStick*)g_InputManager->createInputObject( OISJoyStick, true );
            //    g_joys[i]->setEventCallback( &handler );

            //    g_ff[i] = (ForceFeedback*)g_joys[i]->queryInterface( Interface::ForceFeedback );
            //    if( g_ff[i] )
            //    {
            //        std::cout << "\nCreated buffered joystick with ForceFeedback support.\n";
            //        //Dump out all the supported effects:
            //        const ForceFeedback::SupportedEffectList &list = g_ff[i]->getSupportedEffects();
            //        ForceFeedback::SupportedEffectList::const_iterator i = list.begin(),
            //            e = list.end();
            //        for( ; i != e; ++i)
            //            std::cout << "Force =  " << i->first << " Type = " << i->second << std::endl;
            //    }
            //    else
            //        std::cout << "\nCreated buffered joystick. **without** FF support";
            //}
        }

        static void Main( string[] args )
        {
            log.Info( "SharpInputSystem Console Application" );
            try
            {
                DoStartup();
 
                while ( _handler.appRunning )
                {
                    //Throttle down CPU usage

                    if ( _kb != null )
                    {
                        _kb.Capture();
                        if ( !_kb.IsBuffered )
                            handleNonBufferedKeys();
                    }

                    if ( _m != null )
                    {
                        _m.Capture();
                        //if ( !_m.IsBuffered() )
                        //    handleNonBufferedMouse();
                    }

                    //for( int i = 0; i < 4 ; ++i )
                    //{
                    //    if( _joys[i] )
                    //    {
                    //        _joys[i].Capture();
                    //        if( !_joys[i].IsBuffered() )
                    //            handleNonBufferedJoy( _joys[i] );
                    //    }
                    //}
                }
            }
            catch ( Exception e )
            {
				log.Error( "SIS Exception Caught!", e );
				log.Info( "Press any key to exit." );
                System.Console.ReadKey();
            }

	        if( _inputManager !=null )
	        {
		        _inputManager.DestroyInputObject( _kb );
		        //_inputManager.DestroyInputObject( _m );

                //for ( int i = 0; i < 4; ++i )
                //    _inputManager.DestroyInputObject( _joys[ i ] );
	        }

			log.Info( "Goodbye" );
	        return;

        }

        private static void handleNonBufferedKeys()
        {
            if ( _kb.IsKeyDown( KeyCode.Key_ESCAPE ) || _kb.IsKeyDown( KeyCode.Key_Q ) )
                _handler.appRunning = false;
            if ( _kb.IsShiftState( Keyboard.ShiftState.Alt ) ) System.Console.Write( " ALT " );
            if ( _kb.IsShiftState( Keyboard.ShiftState.Shift ) ) System.Console.Write( " SHIFT " );
            if ( _kb.IsShiftState( Keyboard.ShiftState.Ctrl ) ) System.Console.Write( " CTRL " );

            int [] ks = _kb.KeyStates;
            for ( int i = 0; i < ks.Length; i++ )
            {
                if ( ks[ i ] != 0 )
                {
                    log.Info( String.Format( "KeyPressed : {0} {1}", (KeyCode)i, i ));
                }
            }
        }


    }
}
