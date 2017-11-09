using System;
using Xamarin.Forms;
using Ooui.Forms;

namespace Samples
{
	public class XamarinFormsSample
	{
		public void Publish ()
		{
			Forms.Init ();

			var countLabel = new Label {
				Text = "0",
                BackgroundColor = Color.Gold,
			};
			var countButton = new Button {
			};
			countButton.Clicked += (sender, e) => {
				var v = int.Parse (countLabel.Text);
				countLabel.Text = (v + 1).ToString ();
			};
			var page = new ContentPage {
				Content = new StackLayout {
                    BackgroundColor = Color.Khaki,
					Children = {
						new Label { Text = "Hello World!" },
						countLabel,
						countButton,
					},
				},
			};

			page.Publish ("/xamarin-forms");
			page.PublishShared ("/xamarin-forms-shared");
		}
	}
}
