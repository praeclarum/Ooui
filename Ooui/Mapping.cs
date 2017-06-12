using System;
using System.Collections.Generic;

namespace Ooui
{
    public class Mapping
    {
        readonly Type type;
        public string TagName { get; private set; }

        public Mapping (Type type)
        {
            this.type = type;
            TagName = type.Name.ToLowerInvariant ();
        }

        public string GetMemberPath (string propertyName)
        {
            return propertyName;
        }

        static readonly Dictionary<string, Mapping> mappings =
            new Dictionary<string, Mapping> ();

        public static Mapping Get (Type type)
        {
            var key = type.FullName;
            Mapping m;
            if (!mappings.TryGetValue (key, out m)) {
                m = new Mapping (type);
                mappings[key] = m;
            }
            return m;
        }
    }
}
