using System;

namespace Ooui
{
    public static class UI
    {
        public static void Publish (string path, object value)
        {
        }

        public static void Publish (string path, Func<object> ctor)
        {            
        }

        public static Element GetElementAtPath (string path)
        {
            throw new System.Collections.Generic.KeyNotFoundException ($"{path} does not exist");
        }
    }
}
