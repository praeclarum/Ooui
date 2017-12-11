using System;
using Xamarin.Forms;

namespace Ooui.Forms
{
	public class PlatformRenderer : Ooui.Div
	{
		readonly Platform platform;

		public Platform Platform => platform;

        public override bool WantsFullScreen => true;

		public PlatformRenderer (Platform platform)
		{
			this.platform = platform;
		}

        protected override bool TriggerEventFromMessage (Message message)
        {
            if (message.TargetId == "window" && message.Key == "resize" && message.Value is Newtonsoft.Json.Linq.JObject j) {
                var width = (double)j["width"];
                var height = (double)j["height"];
                Platform.Element.Style.Width = width;
                Platform.Element.Style.Height = height;
                return true;
            }
            else {
                return base.TriggerEventFromMessage (message);
            }
        }
	}
}
