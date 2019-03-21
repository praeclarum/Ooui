using System.Collections.Generic;
using System.Linq;
using Ooui;
using Ooui.Forms;
using Xamarin.Forms;
using Element = Ooui.Element;

namespace Samples
{
    public class SamplePicker : ISample
    {
        private readonly IEnumerable<ISample> _samplePages;

        public SamplePicker(IEnumerable<ISample> samplePages)
        {
            _samplePages = samplePages.OrderBy(s => s.Title);
        }

        public string Title => "Ooui samples - choose your demo";
        public string Path => "/sample-picker";
        public Element CreateElement()
        {
            var panel = new StackLayout();
            
            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "Choose a sample",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
            };
            panel.Children.Add(titleLabel);

            foreach (var samplePage in _samplePages)
            {
                panel.Children.Add(new LinkLabel {Text = samplePage.Title.Replace("Xamarin.Forms ", ""), HRef = samplePage.Path});
            }

            var page = new ContentPage
            {
                Title = Title,
                Padding = new Thickness(16),
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
