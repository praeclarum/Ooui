using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Samples.Maps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MapsPage : ContentPage
	{
		public MapsPage ()
		{
			InitializeComponent ();

            PrepareMap();
		}

        private Map _map = null;
        private void PrepareMap()
        {
            _map = new Map(MapSpan.FromCenterAndRadius(new Position(40.6892494, -74.0466891), Distance.FromMeters(100)))
            {
                MapType = MapType.Hybrid,
                WidthRequest = 320,
                HeightRequest = 200,
            };
            _map.Pins.Clear();
            _map.Pins.Add(new Pin()
            {
                Position = new Position(40.6892494, -74.0466891),
                Label = "Statue of Liberty",
                Address = "This is the Statue of Liberty"
            });
            MapHolder.Children.Add(_map);
        }
    }
}
