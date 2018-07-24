using Ooui.Html;
using System;

namespace Ooui
{
    public class Document : EventTarget
    {
        public Window Window { get; } = new Window ();
        public Body Body { get; } = new Body ();

        public Document ()
            : base ("document")
        {
            Id = "document";
            Window.MessageSent += Proxy_MessageSent;
            Body.MessageSent += Proxy_MessageSent;
        }

        public void ReleaseCapture ()
        {
            Call ("releaseCapture");
        }

        public void ExecCommand (string commandName, bool showDefaultUI, string valueArgument = null)
        {
            Call ("execCommand", commandName, showDefaultUI, valueArgument);
        }

        void Proxy_MessageSent (Message message)
        {
            Send (message);
        }
    }
}
