# SharpInputSystem
> A cross platform object oriented input system meant to be very robust and compatable with many systems and operating systems. Written in C# on .NET.

## Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/scy7wjq7ppwvll7s?svg=true)](https://ci.appveyor.com/project/borrillis/sharpinputsystem)

## About SharpInputSystem

![](doc/assets/img/SharpInputSystem-Icon.png)

The SharpInputSystem is an effort to build an easy to use, cross platform input library capable of handling most input devices. Currently planned devices include Keyboard, Mouse, Joysticks and Feedback devices.
The system is written using C# and supports the following API's :

- DirectX9/XInput using SharpDX on Windows
- X11 on Linux using custom P/Invokes on Linux
- Windows Forms on Windows
- OpenGL on Android

## Getting Started

### Installing

[Always Be NuGetting](https://nuget.org/packages/SharpInputSystem/). Package contains binaries for:

- OpenGL
- DirectX
- Windows Forms 
- UWP
- Xamarin (Android, iOS and Mac)
- .NET Standard 2.0

### Building from source

- Windows
> ./build.cmd

- Mac/Linux
> ./build.sh

## Contributing

1. Fork it (<https://github.com/yourname/yourproject/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

### Who do I talk to?

- [![Contact us on Slack!](https://img.shields.io/badge/chat-slack-ff69b4.svg)](https://axiom3d.slack.com/messages/CF7TEK2KW)

## License

Copyright © Axiom3D, Michael Cummings and contributors.

SharpInputSystem is provided as-is under the MIT license. For more information see [LICENSE](https://github.com/axiom3d/sharpinputsystem/blob/master/LICENSE.txt).

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/)
to clarify expected behavior in our community.
For more information see the [Axiom3D Code of Conduct](http://axiom3d.github.io/code-of-conduct).
