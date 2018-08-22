using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class EditorSample : ISample
    {
        public string Title => "Xamarin.Forms Editor Sample";

        public Ooui.Html.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "Editor",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
            };
            panel.Children.Add(titleLabel);

            var editor = new Editor();
            panel.Children.Add(editor);

            var labelEditor = new Xamarin.Forms.Label();
            panel.Children.Add(labelEditor);

            editor.TextChanged += (sender, args) =>
            {
                labelEditor.Text = args.NewTextValue;
            };

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/editor", CreateElement);
        }
    }
}
