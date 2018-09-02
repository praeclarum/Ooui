using System;
using Ooui;

namespace Samples
{
    public interface ISample
    {
        string Title { get; }
        string Path { get; }
        Element CreateElement ();
        void Publish();
    }
}
