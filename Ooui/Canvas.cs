using System;

namespace Ooui
{
    public class Canvas : Element
    {
        CanvasRenderingContext2D context2d = new CanvasRenderingContext2D ();
        int gotContext2d = 0;

        int width = 300;
        public int Width {
            get => width;
            set => SetProperty (ref width, value <= 0 ? 150 : value, "width");
        }

        int height = 150;
        public int Height {
            get => height;
            set => SetProperty (ref height, value <= 0 ? 150 : value, "height");
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
