# Ooui Web Framework <img src="https://raw.githubusercontent.com/praeclarum/Ooui/master/Documentation/Icon.png" height="32"> [![Build Status](https://www.bitrise.io/app/86585e168136767d/status.svg?token=G9Svvnv_NvG40gcqu48RNQ)](https://www.bitrise.io/app/86585e168136767d)

| Version | Package | Description |
| ------- | ------- | ----------- |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.svg)](https://www.nuget.org/packages/Ooui) | [Ooui](https://www.nuget.org/packages/Ooui) | Core library with HTML elements and a server. |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.Forms.svg)](https://www.nuget.org/packages/Ooui.Forms) | [Ooui.Forms](https://www.nuget.org/packages/Ooui.Forms) | Xamarin.Forms backend using Ooui |
| [![NuGet Package](https://img.shields.io/nuget/v/Ooui.AspNetCore.svg)](https://www.nuget.org/packages/Ooui.AspNetCore) | [Ooui.AspNetCore](https://www.nuget.org/packages/Ooui.AspNetCore) | Integration with ASP.NET Core MVC |

Ooui (pronounced *weeee!*) is a small cross-platform UI library for .NET that uses web technologies.

It presents a classic object-oriented UI API that controls a dumb browser. With Ooui, you get the full power of your favorite .NET programming language *plus* the ability to interact with your app using any device.


## Try it Online

Head on over to [http://ooui.mecha.parts](http://ooui.mecha.parts) to tryout the samples.


## Try the Samples Locally

```bash
git clone git@github.com:praeclarum/Ooui.git
cd Ooui

dotnet restore
msbuild
dotnet run --project Samples/Samples.csproj --no-build
```

*(There is currently an issue with Xamarin.Forms and building from the dotnet cli, so for now we use the msbuild command and then set the --no-build flag on dotnet run but this will eventually change when the issue is resolved.)*

This will open the default starting page for the Samples. Now point your browser at [http://localhost:8080/shared-button](http://localhost:8080/shared-button)

You should see a button that tracks the number of times it was clicked.
The source code for that button is shown in the example below.


## Example Use

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
        // The user should be directed to http://localhost:8080/button
        UI.Publish ("/shared-button", button);

        // Don't exit the app until someone hits return
        Console.ReadLine ();
    }
}
```

Make sure to add a reference to Ooui before you try running!

```bash
dotnet add package Ooui
```

With just that code, the user will be presented with a silly counting button.

In fact, any number of users can hit that URL and start interacting with the same button. That's right, automatic collaboration!

If you want each user to get their own button, then you will instead `Publish` a **function** to create it:

```csharp
Button MakeButton()
{
    var button = new Button("Click me!");
    var count = 0;
    button.Click += (s, e) => {
        count++;
        button.Text = $"Clicked {count} times";
    };
    return button;
}

UI.Publish("/button", MakeButton);
```

Now every user (well, every load of the page) will get their own button.


## How it works

When the user requests a page, Ooui will connect to the client using a Web Socket. This socket is used to keep an in-memory model of the UI (the one you work with as a programmer) in sync with the actual UI shown to the user in their browser. This is done using a simple messaging protocol with JSON packets.

When the user clicks or otherwise interacts with the UI, those events are sent back over the web socket so that your code can deal with them.


## Comparison

<table>
<thead><tr><th>UI Library</th><th>Ooui</th><th>Xamarin.Forms</th><th>ASP.NET MVC</th></tr></thead>

<tr>
<th>How big is it?</th>
<td>80 KB</td>
<td>850 KB</td>
<td>1,300 KB</td>
</tr>

<tr>
<th>Where does it run?</th>
<td>Everywhere</td>
<td>iOS, Android, Mac, Windows</td>
<td>Everywhere</td>
</tr>

<tr>
<th>How do I make a button?</th>
<td><pre>new Button()</pre></td>
<td><pre>new Button()</pre></td>
<td><pre>&lt;button /&gt;</pre></td>
</tr>

<tr>
<th>Does it use native controls?</th>
<td>No, HTML5 controls</td>
<td>Yes!</td>
<td>HTML5 controls</td>
</tr>

<tr>
<th>What controls are available?</th>
<td>All of those in HTML5</td>
<td>Xamarin.Forms controls</td>
<td>All of those in HTML5</td>
</tr>

<tr>
<th>Which architecture will you force me to use?</th>
<td>None, you're free</td>
<td>MVVM</td>
<td>MVC/MVVM</td>
</tr>

<tr>
<th>What's the templating language?</th>
<td>C#</td>
<td>XAML</td>
<td>Razor</td>
</tr>

<tr>
<th>How do I style things?</th>
<td>CSS baby!</td>
<td>XAML resources</td>
<td>CSS</td>
</tr>

<tr>
<th>Is there databinding?</th>
<td>No :-(</td>
<td>Yes!</td>
<td>Debatable</td>
</tr>

<tr>
<th>Do I need to run a server?</th>
<td>Nope</td>
<td>Heck no</td>
<td>Yes</td>
</tr>

<tr>
<th>Is it web scale?</th>
<td>Yes?</td>
<td>What's the web?</td>
<td>Yes!</td>
</tr>



</table>

