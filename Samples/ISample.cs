using System;
using Ooui;
using Ooui.Html;

namespace Samples
{
    public interface ISample
    {
        string Title { get; }
        Element CreateElement ();
    }
}
