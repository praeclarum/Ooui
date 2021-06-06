using System;
using Microsoft.Maui;

namespace Ooui.Maui
{
	public class ActivationState : IActivationState
	{
		public ActivationState(IMauiContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public IMauiContext Context { get; }
	}
}
