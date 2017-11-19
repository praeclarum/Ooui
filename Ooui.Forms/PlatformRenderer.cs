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
	}
}
