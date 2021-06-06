using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Compatibility.Platform.Ooui
{
    [Preserve(AllMembers = true)]
    class ResourcesProvider : ISystemResourcesProvider
    {
        ResourceDictionary? _dictionary;

        public ResourcesProvider()
        {
        }

        public IResourceDictionary GetSystemResources()
        {
            _dictionary = new ResourceDictionary();
            UpdateStyles();

            return _dictionary;
        }

        Style GenerateStyle(string elementType)
        {
            var result = new Style(typeof(Label));

            // result.Setters.Add(new Setter { Property = Label.FontSizeProperty, Value = (double)font.PointSize });

            // result.Setters.Add(new Setter { Property = Label.FontFamilyProperty, Value = font.Name });

            return result;
        }

        void UpdateStyles()
        {
            if (_dictionary == null)
                return;
            _dictionary[Device.Styles.TitleStyleKey] = GenerateStyle("h1");
            _dictionary[Device.Styles.SubtitleStyleKey] = GenerateStyle("h2");
            _dictionary[Device.Styles.BodyStyleKey] = GenerateStyle("body");
            _dictionary[Device.Styles.CaptionStyleKey] = GenerateStyle("small");

            _dictionary[Device.Styles.ListItemTextStyleKey] = GenerateStyle("body");
            _dictionary[Device.Styles.ListItemDetailTextStyleKey] = GenerateStyle("body");
        }
    }
}
