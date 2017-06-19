using System;

namespace Ooui
{
    public class Canvas : Element
    {
        Context2d context2d = new Context2d ();
        int gotContext2d = 0;

        public Canvas ()
            : base ("canvas")
        {
        }

        public Context2d GetContext2d ()
        {
            if (System.Threading.Interlocked.CompareExchange (ref gotContext2d, 1, 0) == 0) {
                var mcall = Message.Call (Id, "getContext", "2d");
                mcall.ResultId = context2d.Id;
                Send (mcall);
            }
            return context2d;
        }
        protected override void SaveStateMessageIfNeeded (Message message)
        {
            switch (message.MessageType) {
                case MessageType.Call when message.Key == "getContext" && message.Value is Array a && a.Length == 1 && "2d".Equals (a.GetValue (0)):
                    AddStateMessage (message);
                    break;
                default:
                    base.SaveStateMessageIfNeeded (message);
                    break;
            }
        }
    }
}
