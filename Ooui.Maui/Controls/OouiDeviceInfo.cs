using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;

namespace Ooui.Maui.Controls
{
    public class OouiDeviceInfo : DeviceInfo
    {
        // TODO: fak: return correct screen size
        public override Size PixelScreenSize => new Size(1024, 768);

        public override Size ScaledScreenSize => new Size(1024, 768);

        public override double ScalingFactor => 1.0;
    }
}
