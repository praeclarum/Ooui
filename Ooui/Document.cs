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

        void Proxy_MessageSent (Message message)
        {
            Send (message);
        }
    }
}
