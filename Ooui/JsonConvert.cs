using System;

namespace Ooui
{
    class JsonConvert
    {
        public static void WriteJsonString (System.IO.TextWriter w, string s)
        {
            w.Write ('\"');
            for (var i = 0; i < s.Length; i++) {
                var c = s[i];
                if (c == '\"') {
                    w.Write ("\\\"");
                }
                else if (c == '\r') {
                    w.Write ("\\r");
                }
                else if (c == '\n') {
                    w.Write ("\\n");
                }
                else if (c == '\t') {
                    w.Write ("\\t");
                }
                else if (c == '\b') {
                    w.Write ("\\b");
                }
                else if (c == '\\') {
                    w.Write ("\\");
                }
                else {
                    w.Write (c);
                }
            }
            w.Write ('\"');
        }

        public static void WriteJsonValue (System.IO.TextWriter w, object value)
        {
            if (value == null) {
                w.Write ("null");
                return;
            }
            var s = value as string;
            if (s != null) {
                WriteJsonString (w, s);
                return;
            }

            var a = value as Array;
            if (a != null) {
                w.Write ('[');
                var head = "";
                foreach (var o in a) {
                    w.Write (head);
                    WriteJsonValue (w, o);
                    head = ",";
                }
                w.Write (']');
                return;
            }

            var e = value as EventTarget;
            if (e != null) {
                w.Write ('\"');
                w.Write (e.Id);
                w.Write ('\"');
                return;
            }

            if (value is Color) {
                WriteJsonString (w, ((Color)value).ToString ());
                return;
            }

            var icult = System.Globalization.CultureInfo.InvariantCulture;

            if (value is double) {
                w.Write (((double)value).ToString (icult));
                return;
            }

            if (value is int) {
                w.Write (((int)value).ToString (icult));
                return;
            }

            if (value is float) {
                w.Write (((float)value).ToString (icult));
                return;
            }

            w.Write (Newtonsoft.Json.JsonConvert.SerializeObject (value));
        }

        public static string SerializeObject (object value)
        {
            using (var sw = new System.IO.StringWriter ()) {
                WriteJsonValue (sw, value);
                return sw.ToString ();
            }
        }

        static object ReadJsonArray (string j, ref int i)
        {
            throw new NotImplementedException ();
        }

        static object ReadJsonObject (string json, ref int i)
        {
            var e = json.Length;
            while (i < e) {
                while (i < e && char.IsWhiteSpace (json[i]))
                    i++;
                if (i >= e)
                    throw new Exception ("JSON Unexpected end");
                var n = e - i;
                i++;
            }
            throw new NotImplementedException ();
        }

        static object ReadJsonString (string j, ref int i)
        {
            throw new NotImplementedException ();
        }

        static object ReadJsonNumber (string j, ref int i)
        {
            throw new NotImplementedException ();
        }

        static object ReadJsonValue (string json, ref int i)
        {
            var e = json.Length;
            while (i < e && char.IsWhiteSpace (json[i]))
                i++;
            if (i >= e)
                throw new Exception ("JSON Unexpected end");
            var n = e - i;
            switch (json[i]) {
                case '[':
                    return ReadJsonArray (json, ref i);
                case '{':
                    return ReadJsonObject (json, ref i);
                case '\"':
                    return ReadJsonString (json, ref i);
                case 'f':
                    i += 5;
                    return false;
                case 't':
                    i += 4;
                    return true;
                default:
                    return ReadJsonNumber (json, ref i);
            }
        }

        public static object ReadJsonValue (string json, int startIndex)
        {
            var i = startIndex;
            return ReadJsonValue (json, ref i);
        }
    }
}
