using Ooui;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Samples
{
    public class MonkeysSample : ISample
    {
        public string Title => "Xamarin.Forms Monkeys";
        public string Path => "/monkeys";

        public Ooui.Element CreateElement()
        {
            var page = new Monkeys.Views.MonkeysView();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
