using System;
using NUnit.Framework;
using Ooui;

namespace Tests
{
	[TestFixture]
	public class PresentTests
	{
		[TestCase]
		public void Present ()
		{
			var b = new Button ("Click Me");
			UI.Publish ("/b", b);
			UI.Present ("/b", AndroidTests.MainActivity.Shared.Browser);
		}
	}
}
