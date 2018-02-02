using System;

namespace Ooui
{
    public class Canvas : Element
    {
        CanvasRenderingContext2D context2d = new CanvasRenderingContext2D ();
        int gotContext2d = 0;

        public int Width {
            get => GetAttribute ("width", 300);
            set => SetAttributeProperty ("width", value < 0 ? 0 : value);
        }

        public int Height {
            get => GetAttribute ("height", 150);
            set => SetAttributeProperty ("height", value < 0 ? 0 : value);
        }

        public Canvas ()
            : base ("canvas")
        {
            context2d.MessageSent += Send;
        }

        public CanvasRenderingContext2D GetContext2D ()
        {
            if (System.Threading.Interlocked.CompareExchange (ref gotContext2d, 1, 0) == 0) {
                var mcall = Message.Call (Id, "getContext", "2d");
                mcall.ResultId = context2d.Id;
                Send (mcall);
            }
            return context2d;
        }
        protected override bool SaveStateMessageIfNeeded (Message message)
        {
            if (message.TargetId == Id) {
                switch (message.MessageType) {
                    case MessageType.Call when message.Key == "getContext" && message.Value is Array a && a.Length == 1 && "2d".Equals (a.GetValue (0)):
                        AddStateMessage (message);
                        break;
                }
            }
            return base.SaveStateMessageIfNeeded (message);
        }
    }
}
