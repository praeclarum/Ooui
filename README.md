# Ooui Web Framework <img src="https://raw.githubusercontent.com/praeclarum/Ooui/master/Documentation/Icon.png" height="32"> [![Build Status](https://app.bitrise.io/app/86585e168136767d/status.svg?token=G9Svvnv_NvG40gcqu48RNQ&branch=master)](https://app.bitrise.io/app/86585e168136767d)

| Version | Package | Description |
| ------- | ------- | ----------- |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.svg)](https://www.nuget.org/packages/Ooui) | [Ooui](https://www.nuget.org/packages/Ooui) | Core library with HTML elements and a server |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.AspNetCore.svg)](https://www.nuget.org/packages/Ooui.AspNetCore) | [Ooui.AspNetCore](https://www.nuget.org/packages/Ooui.AspNetCore) | Integration with ASP.NET Core |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.Forms.svg)](https://www.nuget.org/packages/Ooui.Forms) | [Ooui.Forms](https://www.nuget.org/packages/Ooui.Forms) | Xamarin.Forms backend using Ooui ([Status](Documentation/OouiFormsStatus.md)) |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.Wasm.svg)](https://www.nuget.org/packages/Ooui.Wasm) | [Ooui.Wasm](https://www.nuget.org/packages/Ooui.Wasm) | Package your app into a web assembly |

Ooui (pronounced *weee!*) is a small cross-platform UI library for .NET that uses web technologies.

It presents a classic object-oriented UI API that controls a dumb browser. With Ooui, you get the full power of your favorite .NET programming language *plus* the ability to interact with your app using any device.


## Try it Online

Head on over to [http://ooui.mecha.parts](http://ooui.mecha.parts) to tryout the samples.

You can also load [https://s3.amazonaws.com/praeclarum.org/wasm/index.html](https://s3.amazonaws.com/praeclarum.org/wasm/index.html) to try the WebAssembly mode of Ooui running Xamarin.Forms. (That's Xamarin.Forms running right in your browser!)


## Try the Samples Locally

```bash
git clone git@github.com:praeclarum/Ooui.git
cd Ooui

dotnet restore
msbuild
dotnet run --project Samples/Samples.csproj --no-build
```

This will open the default starting page for the Samples. Now point your browser at [http://localhost:8080/shared-button](http://localhost:8080/shared-button)

You should see a button that tracks the number of times it was clicked. The source code for that button is shown in the example below.


## Example App

Here is the complete source code to a fully collaborative button clicking app.

```csharp
using System;
using Ooui;

class Program
{
    static void Main(string[] args)
    {
        // Create the UI
        var button = new Button("Click me!");

        // Add some logic to it
        var count = 0;
        button.Click += (s, e) => {
            count++;
            button.Text = $"Clicked {count} times";
        };

        // Publishing makes an object available at a given URL
        // The user should be directed to http://localhost:8080/shared-button
        UI.Publish ("/shared-button", button);

        // Don't exit the app until someone hits return
        Console.ReadLine ();
    }
}
```

Make sure to add a reference to Ooui before you start running!

```bash
dotnet add package Ooui
dotnet run
```

With just that code, a web server that serves the HTML and web socket logic necessary for an interactive button will start.



## The Many Ways to Ooui

Ooui has been broken up into several packages to increase the variety of ways that it can be used. Here are some combinations to help you decide which way is best for you.

<table>
<thead><tr><th>Ooui</th><th>Ooui.AspNetCore</th><th>Ooui.Forms</th><th>Ooui.Wasm</th><th></th></tr></thead>

<tr>
<td>&check;</td><td></td><td></td><td></td><td><a href="https://github.com/praeclarum/Ooui/wiki/Web-DOM-with-the-Built-in-Web-Server">Web DOM with the Built-in Web Server</a></td>
</tr>

<tr>
<td>&check;</td><td>&check;</td><td></td><td></td><td>Web DOM with ASP.NET Core</td>
</tr>

<tr>
<td>&check;</td><td>&check;</td><td>&check;</td><td></td><td>Xamarin.Forms with ASP.NET Core</td>
</tr>

<tr>
<td>&check;</td><td></td><td>&check;</td><td></td><td>Xamarin.Forms with the built-in web server</td>
</tr>

<tr>
<td>&check;</td><td></td><td></td><td>&check;</td><td><a href="https://github.com/praeclarum/Ooui/wiki/Web DOM-with-Web-Assembly">Web DOM with Web Assembly</a></td>
</tr>

<tr>
<td>&check;</td><td></td><td>&check;</td><td>&check;</td><td><a href="https://github.com/praeclarum/Ooui/wiki/Xamarin.Forms-with-Web-Assembly">Xamarin.Forms with Web Assembly</a></td>
</tr>

</table>


## How it works

When the user requests a page, the page will connect to the server using a web socket. This socket is used to keep the server's in-memory model of the UI (the one you work with as a programmer) in sync with the actual UI shown to the user in their browser. This is done using a simple messaging protocol with JSON packets.

When the user clicks or otherwise interacts with the UI, those events are sent back over the web socket so that your code can deal with them.

In the case of web assembly, this same dataflow takes place. However, sockets are not used as all communication is done locally in the browser process.


## Contributing

Ooui is open source and I love merging PRs. Please fork away, and please obey the .editorconfig file. :-) Try to file issues for things that you want to work on *before* you start the work so that there's no duplicated effort. If you just want to help out, check out the issues and dive in!
