# SharpInputSystem

> A cross platform object oriented input system meant to be very robust and compatable with many systems and operating systems. Written in C# on .NET.

## Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/scy7wjq7ppwvll7s?svg=true)](https://ci.appveyor.com/project/borrillis/sharpinputsystem)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors-)<!-- ALL-CONTRIBUTORS-BADGE:END -->

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
- Pre-requisites

> - [Visual Studio Code](https://code.visualstudio.com/download)
> - [DotNet Core 2.2 SDK](https://dotnet.microsoft.com/download)
> - [.Net Framework Developer Pack 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49978)
> - [Mono 5.2.0](https://download.mono-project.com/archive/5.2.0/)

- Restore DotNet Tools

    ```sh
    dotnet tool restore
    ```

- Initialize the build tooling

    ```sh
    dotnet cake --bootstrap
    ```

- Restore packages and dependencies

    ```sh
    dotnet restore src/SharpInputSystem.sln
    ```

- Build SharpInputSystem

    ```sh
    dotnet cake
    ```

## Contributing

1. Fork it (<https://github.com/yourname/yourproject/fork>)
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="http://michaelcummings.net"><img src="https://avatars2.githubusercontent.com/u/757221?v=4" width="100px;" alt="Michael Cummings"/><br /><sub><b>Michael Cummings</b></sub></a><br /><a href="https://github.com/axiom3d/sharpinputsystem/commits?author=borrillis" title="Code">💻</a> <a href="https://github.com/axiom3d/sharpinputsystem/commits?author=borrillis" title="Documentation">📖</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!

### Who do I talk to?

- [![Contact us on Slack!](https://img.shields.io/badge/chat-slack-ff69b4.svg)](https://axiom3d.slack.com/messages/CF7TEK2KW)

## License

Copyright © Axiom3D, Michael Cummings and contributors.

SharpInputSystem is provided as-is under the MIT license. For more information see [LICENSE](https://github.com/axiom3d/sharpinputsystem/blob/master/LICENSE.txt).

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/)
to clarify expected behavior in our community.
For more information see the [Axiom3D Code of Conduct](http://axiom3d.github.io/code-of-conduct).
