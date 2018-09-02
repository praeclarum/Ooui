using Ooui;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Samples
{
    public class Light : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        private string _comment;
        private Xamarin.Forms.Color _color;
        private bool _isOn;

        public string Name { get { return _name; } set { OnPropertyChanged(); _name = value; } }
        public string Comment { get { return _comment; } set { OnPropertyChanged(); _comment = value; } }
        public Xamarin.Forms.Color Color { get { return _color; } set { OnPropertyChanged(); _color = value; } }
        public bool IsOn { get { return _isOn; } set { OnPropertyChanged(); OnPropertyChanged("isNotOn"); _isOn = value; } }
        public bool IsNotOn { get { return !_isOn; } }

        public Light()
        {
            this.IsOn = false;
            this.Name = "My first light!";
            this.Color = Xamarin.Forms.Color.Blue;
            this.Comment = "Bedroom";
        }
        public Light(bool isOn, string name, Xamarin.Forms.Color color, string comment)
        {
            this.IsOn = isOn;
            this.Name = name;
            this.Color = color;
            this.Comment = comment;
        }
        void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static class DataSource
    {
        public static ObservableCollection<Light> GetLights()
        {
            ObservableCollection<Light> lights = new ObservableCollection<Light>()
            {
                new Light(false, "Bedside", Xamarin.Forms.Color.Blue, "Mel's Bedroom"),
                new Light(false, "Desk", Xamarin.Forms.Color.Red, "Mel's Bedroom"),
                new Light(true, "Flood Lamp", Xamarin.Forms.Color.Olive, "Outside"),
                new Light(false, "hallway1", Xamarin.Forms.Color.Teal, "Entry Hallway"),
                new Light(false, "hallway2", Xamarin.Forms.Color.Purple, "Entry Hallway")
            };

            return lights;
        }
    }

    class EntryListViewSample : ISample
    {
        public string Title => "Xamarin.Forms Basic Entry ListView Sample";
        public string Path => "/entry-listview";

        public Ooui.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "ListView",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };

            panel.Children.Add(titleLabel);

            ListView listView = new ListView() { ItemsSource = DataSource.GetLights() };

            listView.ItemTemplate = new DataTemplate(typeof(EntryCell));
            listView.ItemTemplate.SetBinding(EntryCell.LabelProperty, "Comment");
            listView.ItemTemplate.SetBinding(EntryCell.TextProperty, "Name");

            panel.Children.Add(listView);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }

    class SwitchListViewSample : ISample
    {
        public string Title => "Xamarin.Forms Basic Switch ListView Sample";
        public string Path => "/switch-listview";

        public Ooui.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "ListView",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold
            };

            panel.Children.Add(titleLabel);

            ListView listView = new ListView() { ItemsSource = DataSource.GetLights() };

            listView.ItemTemplate = new DataTemplate(typeof(SwitchCell));
            listView.ItemTemplate.SetBinding(SwitchCell.TextProperty, "Name");
            listView.ItemTemplate.SetBinding(SwitchCell.OnProperty, "IsOn");

            panel.Children.Add(listView);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish(Path, CreateElement);
        }
    }
}
