using System;
using Xamarin.Forms;
using Ooui.Forms;

namespace Samples
{
    public class XamarinFormsSample : ISample
	{
        public string Title => "Xamarin.Forms Button Counter";

		Page MakePage ()
		{
			var countLabel = new Label {
				Text = "0",
                BackgroundColor = Color.Gold,
                HorizontalOptions = LayoutOptions.Center,
			};
			var countButton = new Button {
                Text = "Increase",
                HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			countButton.Clicked += (sender, e) => {
				var v = int.Parse (countLabel.Text);
				countLabel.Text = (v + 1).ToString ();
			};
            return new ContentPage {
                Content = new StackLayout {
                    BackgroundColor = Color.Khaki,
                    Children = {
                        new Label {
                            Text = "Hello World!",
                            FontSize = 32,
                            HorizontalOptions = LayoutOptions.Center,
                        },
						countLabel,
						countButton,
					},
				},
			};
        }

        public void Publish ()
        {
            var page = MakePage ();
            page.Publish ("/xamarin-forms-shared");

            Ooui.UI.Publish ("/xamarin-forms", () => MakePage ().GetOouiElement ());
        }

        public Ooui.Element CreateElement ()
        {
            return MakePage ().GetOouiElement ();
        }
    }
}
