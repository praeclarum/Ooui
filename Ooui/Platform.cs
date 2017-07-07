using System;

namespace Ooui
{
    static class Platform
    {
#if __IOS32423__
        public static void Present (string url, object presenter)
        {

        }
#else
        public static void Present (string url, object presenter)
        {

        }
#endif
    }
}
