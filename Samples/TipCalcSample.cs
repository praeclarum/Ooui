using Ooui;
using Xamarin.Forms;

namespace Samples
{
    public class TipCalcSample : ISample
    {
        public string Title => "Xamarin.Forms TipCalc";

        public Ooui.Element CreateElement()
        {
            var page = new TipCalc.TipCalcPage();
            return page.GetOouiElement();
        }

        public void Publish()
        {
            UI.Publish("/tipcalc", CreateElement);
        }
    }
}
