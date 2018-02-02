namespace Ooui
{
    public class Iframe : Element
    {
        public string Source
        {
            get => GetStringAttribute ("src", null);
            set => SetAttributeProperty ("src", value);
        }

        public Iframe ()
            : base ("iframe")
        {
        }
    }
}
