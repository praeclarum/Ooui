using Ooui.Forms.Renderers;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Cells
{
    public class ImageCellRenderer : TextCellRenderer
    {
        protected override CellElement CreateCellElement (Cell cell)
        {
            return new ImageCellElement ();
        }
    }
}
