namespace Ooui
{
    public class Window : EventTarget
    {
        public Location Location { get; } = new Location();

        public Window ()
            : base ("window")
        {
            Id = "window";
            Location.MessageSent += Proxy_MessageSent;
        }

        void Proxy_MessageSent(Message message)
        {
            Send(message);
        }
    }
}
