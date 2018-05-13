using System;
using System.Threading;
using System.Threading.Tasks;
using Ooui;

namespace Samples
{
    public class MapSample : ISample
    {
        public string Title => "Map Sample";

        GoogleMap MakeMap ()
        {
            var map = new GoogleMap (mapType: GoogleMap.MapType.HYBRID,firstMapControlOnPage: true) {
               APIKey = "YOUR_GOOGLE_MAPS_API_KEY_HERE",
            };


            //Add Marker from code on creating of view
            map.AddMarker(new GoogleMap.MapMarker()
            {
                lat = 51.511884,
                lng = -0.195421,
                title = "Testing Markers on Load",
                infoWindow = new GoogleMap.MapInfoWindow()
                {
                    content = "This is the first pin placement (I think it is at Kensington Palace)"
                }
            });
            
            ThreadPool.QueueUserWorkItem((state) =>
            {
                //Add Marker from code after view has been loaded
                Thread.Sleep(5000);
                map.AddMarker(new GoogleMap.MapMarker
                {
                    lat = 51.5073346,
                    lng = -0.1276831,
                    title = "Testing Markers",
                    infoWindow = new GoogleMap.MapInfoWindow
                    {
                        content = "This is a random pin placement"
                    }
                });
            });
            ThreadPool.QueueUserWorkItem((state) =>
            {
                //Change map center location after ten seconds and add a marker there
                Thread.Sleep(10000);

                var pos = new GoogleMap.Position
                {
                    latitude = 40.6892,
                    longitude = 74.0445,
                };
                map.CenterOn(pos);
                map.AddMarker(new GoogleMap.MapMarker
                {
                    lat = pos.latitude,
                    lng = pos.longitude,
                    title = "Kyrgyzstan",
                    infoWindow = new GoogleMap.MapInfoWindow
                    {
                        content = "This should be in Kyrgyzstan"
                    }
                });
                ThreadPool.QueueUserWorkItem(async (state2) =>
                {
                    await Task.Delay(5000);

                    var pos2 = await map.GetCenter();
                });
            });
            return map;
        }

        public void Publish ()
        {
            var b = MakeMap ();

            UI.Publish ("/shared-map", b);
            UI.Publish ("/map", MakeMap);
        }

        public Element CreateElement ()
        {
            return MakeMap ();
        }
    }
}


