using Ooui.Forms.Renderers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Ooui.Forms.Maps
{
    public class MapViewRenderer : ViewRenderer<Map, GoogleMap>
    {
        bool _disposed;

        static Size initialSize = Size.Zero;
        private ObservableCollection<Pin> _pins;
        private MapSpan _currentSpan;

        IElementController ElementController => Element as IElementController;

        public MapViewRenderer():base()
        {

        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {

            var size = new Size(Element.WidthRequest,Element.HeightRequest);
            var vpadding = 16;
            var hpadding = 32;
            size = new Size(size.Width + hpadding, size.Height + vpadding);
            return new SizeRequest(size, size);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                if (Element != null)
                {
                    Element.FocusChangeRequested -= Element_FocusChangeRequested;
                }
                if (Control != null)
                {
                    if (_pins != null)
                    {
                        _pins.CollectionChanged -= Pins_CollectionChanged;
                        _pins.Clear();
                        _pins = null;
                    }
                    _currentSpan = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.FocusChangeRequested -= Element_FocusChangeRequested;
            }

            if (e.NewElement == null)
                return;

            e.NewElement.FocusChangeRequested += Element_FocusChangeRequested;

            if (Control == null)
            {
                var map = new Ooui.GoogleMap(apiKey: FormsMaps.APIKey,firstMapControlOnPage: true,mapType: GetMapType(Element), startPos: GetPos(Element));
                foreach (var item in Element.Pins)
                {
                    map.AddMarker(GetMapMarkerFromPin(item));
                }

                _pins = new ObservableCollection<Pin>(Element.Pins);
                _pins.CollectionChanged += Pins_CollectionChanged;
                _currentSpan = Element.LastMoveToRegion;
                SetNativeControl(map);

                Debug.Assert(Control != null, "Control != null");

                WatchPins();
                WatchRegion();
            }
        }

        private void Pins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        Control.AddMarker(GetMapMarkerFromPin(item as Pin));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        Control.RemoveMarker(GetMapMarkerFromPin(item as Pin));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        Control.RemoveMarker(GetMapMarkerFromPin(item as Pin));
                    }
                    foreach (var item in e.NewItems)
                    {
                        Control.AddMarker(GetMapMarkerFromPin(item as Pin));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Control.ClearMarkers();
                    break;
                default:
                    break;
            }
        }
        
        private void WatchRegion()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    Thread.Sleep(900);
                    if (_disposed)
                    {
                        return;
                    }
                    try
                    {
                        if (Element.LastMoveToRegion != _currentSpan)
                        {
                            _currentSpan = Element.LastMoveToRegion;
                            Control.CenterOn(GetPos(Element));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        continue;
                    }
                }
            });
        }

        private void WatchPins()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                while (true)
                {
                    Thread.Sleep(900);
                    if (_disposed)
                    {
                        return;
                    }
                    try
                    {
                        if (Element.Pins.Count != _pins.Count)
                        {
                            if (Element.Pins.Count == 0)
                            {
                                _pins.Clear();
                                continue;
                            }

                            foreach (var item in Element.Pins)
                            {
                                if (!_pins.Contains(item))
                                {
                                    _pins.Add(item);
                                }
                            }

                            var removedItems = _pins.Where(pin => !Element.Pins.Contains(pin)).ToArray();
                            foreach (var item in removedItems)
                            {
                                _pins.Remove(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        continue;
                    }
                }
            });
        }

        private static GoogleMap.MapMarker GetMapMarkerFromPin(Pin item)
        {
            return new GoogleMap.MapMarker
            {
                lat = item.Position.Latitude,
                lng = item.Position.Longitude,
                title = item.Label,
                infoWindow = new GoogleMap.MapInfoWindow
                {
                    content = item.Address,
                },
            };
        }

        private GoogleMap.Position GetPos(Map element)
        {
            return new GoogleMap.Position()
            {
                latitude = element.LastMoveToRegion.Center.Latitude,
                longitude = element.LastMoveToRegion.Center.Longitude,
            };
        }

        private GoogleMap.MapType GetMapType(Map element)
        {
            switch (element.MapType)
            {
                case MapType.Street:
                    return GoogleMap.MapType.ROADMAP;
                case MapType.Satellite:
                    return GoogleMap.MapType.SATELLITE;
                case MapType.Hybrid:
                    return GoogleMap.MapType.HYBRID;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element.MapType));
            }
        }

        void Element_FocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            if (e.Focus && Control != null)
            {
                Control?.Focus();
                e.Result = true;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }
            else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
            {
                //TODO: Implement
                //UpdateIsShowingUser();
            }
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
            {
                //TODO: Implement
                //UpdateHasScrollEnabled();
            }
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                //TODO: Implement
                //UpdateHasZoomEnabled();
            }

            base.OnElementPropertyChanged(sender, e);
        }

        private void UpdateMapType()
        {
            Control.ChangeMapType(GetMapType(Element));
        }
    }
}
