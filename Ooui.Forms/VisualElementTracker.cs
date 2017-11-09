namespace Ooui.Forms
{
	internal class VisualElementTracker
	{
		private VisualElementRenderer<object> visualElementRenderer;

		public VisualElementTracker(VisualElementRenderer<object> visualElementRenderer)
		{
			this.visualElementRenderer = visualElementRenderer;
		}
	}
}