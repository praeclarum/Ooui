using Ooui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Samples
{
    public class XamMapsSample : ISample
    {
        public string Title => "Xamarin.Forms Maps";

        public Ooui.Element CreateElement()
        {
            var page = new Samples.Maps.MapsPage();
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.Sleep(25000);
                var map = ((page.Content as StackLayout).Children[1] as Map);
                var eiffelTower = new Position(48.858093, 2.294694);
                Pin pin = new Pin()
                {
                    Position = eiffelTower,
                    Label = "New Pin",
                    Address = "Somewhere",
                };
                map.Pins.Add(pin);

                Thread.Sleep(10000);

                map.MoveToRegion(MapSpan.FromCenterAndRadius(eiffelTower, Distance.FromMeters(100)));

                Thread.Sleep(10000);

                map.Pins.Remove(pin);

                Thread.Sleep(10000);

                map.Pins.Add(pin);

                Thread.Sleep(10000);

                map.Pins.Clear();

                Thread.Sleep(10000);

                map.MapType = MapType.Street;

            });
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/xammaps", CreateElement);
        }
    }
}
