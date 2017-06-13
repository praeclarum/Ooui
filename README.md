# Ooui

Ooui is a small cross-platform UI library for .NET that uses web technologies.

It presents a classic object-oriented UI API that controls a dumb web view or browser. With Ooui, you get the full power of your favorite .NET programming language *plus* the ability to interact with your app using any device with a web browser.

## Quick Example

```csharp
using Ooui;

// Create the UI
var button = new Button($"Click me!");

// Add some logic to it
var count = 0;
button.Clicked += (s, e) => {
    count++;
    button.Text = $"Clicked {count} times";
};

// Publishing makes an object available at a given URL
// The user should be directed to http://localhost:8080/button
UI.Publish("/button", button);
```

In this example, all users would be interacting with the same button. That's right, automatic collaboration!

If you want each user to get their own button, then you will `Publish` a function to create it:

```csharp
Button MakeButton() {
    var button = new Button($"Click me!");
    var count = 0;
    button.Clicked += (s, e) => {
        count++;
        button.Text = $"Clicked {count} times";
    };
    return button;
}

UI.Publish("/button", MakeButton);
```


## How it works

When the user requests a page, Ooui will connect to the client using a Web Socket. This socket is used to keep an in-memory model of the UI (the one you work with as a programmer) in sync with the actual UI shown to the user in their browser.

When the user clicks or otherwise interacts, those events are sent back over the web socket so that your code can deal with them.


## Comparison

<table>
<thead><tr><th>UI Library</th><th>Ooui</th><th>Xamarin.Forms</th><th>ASP.NET MVC</th></tr></thead>

<tr>
<th>How big is it?</th>
<td>50 KB</td>
<td>2,000 KB</td>
<td>5,000 KB</td>
</tr>

<tr>
<th>Where does it run?</th>
<td>Everywhere</td>
<td>iOS, Android, Mac, Windows</td>
<td>Windows, Linux, Mac</td>
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
<td>How much money do you have?</td>
<td>What's the web?</td>
<td>Sure</td>
</tr>



</table>

