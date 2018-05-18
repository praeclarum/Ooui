namespace Ooui
{
    public class Window : EventTarget
    {
        public Location Location { get; } = new Location();

        //string location = "";

        //public string Location {
        //    get => location;
        //    set {
        //        if (string.IsNullOrEmpty (value) || location == value)
        //            return;
        //        location = value;
        //        Send (Message.Set ("window", "location", value));
        //        OnPropertyChanged ("Location");
        //    }
        //}

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
