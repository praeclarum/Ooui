using Mono.WebAssembly;
using System;

class Hello
{
    static int Factorial(int n)
    {
        if (n == 0) {
            return 1;
        }
        return n * Factorial(n - 1);
    }

    // This function is called from the browser by JavaScript.
    // Here we calculate the factorial of the given number then use the
    // Mono.WebAssembly API to retrieve the element from the DOM and set its
    // innerText property to the factorial result.
    static void FactorialInElement(int n, string element_id)
    {
        Console.WriteLine(
                "Calculating factorial of {0} into DOM element {1}",
                n, element_id);

        int f = Factorial(n);

        var elem = HtmlPage.Document.GetElementById(element_id);
        elem.InnerText = f.ToString();
    }

    static void PrintHtmlElements(HtmlElement elem, int level)
    {
        string str = "";
        for (int i = 0; i < level; i++) {
            str += "  ";
        }

        str += $"<{elem.TagName}";

        foreach (var name in elem.AttributeNames) {
            var value = elem.GetAttribute(name);
            str += $" {name}='{value}'";
        }

        str += ">";

        Console.WriteLine(str);

        var list = elem.Children;
        for (int i = 0; i < list.Count; i++) {
            var child = list[i];
            PrintHtmlElements(child, level + 1);
        }
    }

    static int Main(string[] args)
    {
        Ooui.Div odiv = new Ooui.Div ();
        int f = Factorial(6);
        HtmlPage.Window.Alert($"Hello world! factorial(6) -> {odiv}");

        var bi = HtmlPage.BrowserInformation;
        Console.WriteLine($"BrowserInformation: Name {bi.Name} BrowserVersion {bi.BrowserVersion} UserAgent {bi.UserAgent} Platform {bi.Platform} CookiesEnabled {bi.CookiesEnabled} ProductName {bi.ProductName}");

        var d = HtmlPage.Document;
        Console.WriteLine($"Document Location: {d.Location}");

        PrintHtmlElements(d.DocumentElement, 0);

        var p = d.CreateElement("p");
        p.InnerText = "This text was added at runtime.";
        d.Body.AppendChild(p);

        if (args.Length > 0) FactorialInElement(0, ""); // this is a hack so that the linker does not remove the FactorialInElement() method

        return f;
    }
}
