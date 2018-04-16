namespace Ooui
{
    public class Window : EventTarget
    {
        string location = "";

        public string Location {
            get => location;
            set {
                if (string.IsNullOrEmpty (value) || location == value)
                    return;
                location = value;
                Send (Message.Set ("window", "location", value));
                OnPropertyChanged ("Location");
            }
        }

        public Window ()
            : base ("window")
        {
            Id = "window";
        }
    }
}
