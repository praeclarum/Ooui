using System;
using System.Collections.Generic;

namespace Ooui
{
    public class HtmlMapping
    {
        readonly Type type;
        public string TagName { get; private set; }

        public HtmlMapping (Type type)
        {
            this.type = type;
            TagName = type.Name.ToLowerInvariant ();
        }

        public string GetMemberPath (string propertyName)
        {
            return propertyName;
        }

        static readonly Dictionary<string, HtmlMapping> mappings =
            new Dictionary<string, HtmlMapping> ();

        public static HtmlMapping Get (Type type)
        {
            var key = type.FullName;
            HtmlMapping m;
            if (!mappings.TryGetValue (key, out m)) {
                m = new HtmlMapping (type);
                mappings[key] = m;
            }
            return m;
        }
    }
}
