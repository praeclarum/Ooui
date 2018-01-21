namespace Ooui
{
    public class Iframe : Element
    {
        public Iframe()
                : base("iframe")
        {

        }

        string src = null;
        public string Src
        {
            get => src;
            set => SetProperty(ref src, value, "src");
        }
    }
}
