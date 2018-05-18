using System;
using System.Collections.Generic;
using System.Text;

namespace Ooui
{
    public class Location:EventTarget
    {
        string href = "";

        public string HRef
        {
            get => href;
            set
            {
                if (string.IsNullOrEmpty(value) || href == value)
                    return;
                href = value;
                Send(Message.Set("window", "location['href']", value));
                OnPropertyChanged("Location");
            }
        }

        string hash = "";

        public string Hash
        {
            get => hash;
            set
            {
                if (string.IsNullOrEmpty(value) || hash == value)
                    return;
                hash = value;
                Send(Message.Set("window", "location['hash']", value));
                OnPropertyChanged("Hash");
            }
        }

        public void PushState(string hash)
        {
            Call("pushState", null, null, "#" + "testing");
        }

        public Location()
            : base("location")
        {
            Id = "location";
        }
    }
}
