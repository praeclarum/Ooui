using System;
using Ooui;

namespace Samples
{
    public interface ISample
    {
        string Title { get; }
        Element CreateElement ();
    }
}
