using System;
using System.Collections.Generic;
using System.ComponentModel;
using Value = System.Object;

namespace Ooui
{
    public class Style : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        readonly Dictionary<string, Value> properties =
            new Dictionary<string, Value> ();
        static readonly private char[] numberChars = new char[] {
            '0','1','2','3','4','5','6','7','8','9',
            '.','-','+',
        };

        public Value AlignSelf {
            get => this["align-self"];
            set => this["align-self"] = value;
        }

        public Value BackgroundColor {
            get => this["background-color"];
            set => this["background-color"] = value;
        }

        public Value BackgroundImage {
            get => this["background-image"];
            set => this["background-image"] = value;
        }

        public Value BorderTopColor {
            get => this["border-top-color"];
            set => this["border-top-color"] = value;
        }

        public Value BorderRightColor {
            get => this["border-right-color"];
            set => this["border-right-color"] = value;
        }

        public Value BorderBottomColor {
            get => this["border-bottom-color"];
            set => this["border-bottom-color"] = value;
        }

        public Value BorderLeftColor {
            get => this["border-left-color"];
            set => this["border-left-color"] = value;
        }

        public Value BorderColor {
            get => this["border-top-color"];
            set {
                this["border-top-color"] = value;
                this["border-right-color"] = value;
                this["border-bottom-color"] = value;
                this["border-left-color"] = value;
            }
        }

        public Value BorderTopStyle {
            get => this["border-top-style"];
            set => this["border-top-style"] = value;
        }

        public Value BorderRightStyle {
            get => this["border-right-style"];
            set => this["border-right-style"] = value;
        }

        public Value BorderBottomStyle {
            get => this["border-bottom-style"];
            set => this["border-bottom-style"] = value;
        }

        public Value BorderLeftStyle {
            get => this["border-left-style"];
            set => this["border-left-style"] = value;
        }

        public Value BorderStyle {
            get => this["border-top-style"];
            set {
                this["border-top-style"] = value;
                this["border-right-style"] = value;
                this["border-bottom-style"] = value;
                this["border-left-style"] = value;
            }
        }

        public Value BorderTopWidth {
            get => this["border-top-width"];
            set => this["border-top-width"] = AddNumberUnits (value, "px");
        }

        public Value BorderRightWidth {
            get => this["border-right-width"];
            set => this["border-right-width"] = AddNumberUnits (value, "px");
        }

        public Value BorderBottomWidth {
            get => this["border-bottom-width"];
            set => this["border-bottom-width"] = AddNumberUnits (value, "px");
        }

        public Value BorderLeftWidth {
            get => this["border-left-width"];
            set => this["border-left-width"] = AddNumberUnits (value, "px");
        }

        public Value BorderRadius {
            get => this["border-radius"];
            set {
                this["border-radius"] = AddNumberUnits (value, "px");
            }
        }

        public Value BorderWidth {
            get => this["border-top-width"];
            set {
                this["border-top-width"] = AddNumberUnits (value, "px");
                this["border-right-width"] = AddNumberUnits (value, "px");
                this["border-bottom-width"] = AddNumberUnits (value, "px");
                this["border-left-width"] = AddNumberUnits (value, "px");
            }
        }

        public Value Bottom {
            get => this["bottom"];
            set => this["bottom"] = AddNumberUnits (value, "px");
        }

        public Value Clear {
            get => this["clear"];
            set => this["clear"] = value;
        }

        public Value Color {
            get => this["color"];
            set => this["color"] = value;
        }

        public Value Cursor {
            get => this["cursor"];
            set => this["cursor"] = value;
        }

        public Value Display {
            get => this["display"];
            set => this["display"] = value;
        }

        public Value FlexFlow {
            get => this["flex-flow"];
            set => this["flex-flow"] = value;
        }

        public Value FlexGrow {
            get => this["flex-grow"];
            set => this["flex-grow"] = value;
        }

        public Value FlexShrink {
            get => this["flex-shrink"];
            set => this["flex-shrink"] = value;
        }

        public Value Float {
            get => this["float"];
            set => this["float"] = value;
        }

        public Value FontFamily {
            get => this["font-family"];
            set => this["font-family"] = value;
        }

        public Value FontSize {
            get => this["font-size"];
            set => this["font-size"] = AddNumberUnits (value, "px");
        }

        public Value FontStyle {
            get => this["font-style"];
            set => this["font-style"] = value;
        }

        public Value FontVariant {
            get => this["font-variant"];
            set => this["font-variant"] = value;
        }

        public Value FontWeight {
            get => this["font-weight"];
            set => this["font-weight"] = value;
        }

        public Value Height {
            get => this["height"];
            set => this["height"] = AddNumberUnits (value, "px");
        }

        public Value Left {
            get => this["left"];
            set => this["left"] = AddNumberUnits (value, "px");
        }

        public Value LineHeight {
            get => this["line-height"];
            set => this["line-height"] = value;
        }

        public Value MarginTop {
            get => this["margin-top"];
            set => this["margin-top"] = value;
        }

        public Value MarginRight {
            get => this["margin-right"];
            set => this["margin-right"] = value;
        }

        public Value MarginBottom {
            get => this["margin-bottom"];
            set => this["margin-bottom"] = value;
        }

        public Value MarginLeft {
            get => this["margin-left"];
            set => this["margin-left"] = value;
        }

        public Value Margin {
            get => this["margin-top"];
            set {
                this["margin-top"] = value;
                this["margin-right"] = value;
                this["margin-bottom"] = value;
                this["margin-left"] = value;
            }
        }

        public Value Opacity {
            get => this["opacity"];
            set => this["opacity"] = value;
        }

        public Value Order {
            get => this["order"];
            set => this["order"] = value;
        }

        public Value Overflow {
            get => this["overflow"];
            set => this["overflow"] = value;
        }

        public Value PaddingTop {
            get => this["padding-top"];
            set => this["padding-top"] = value;
        }

        public Value PaddingRight {
            get => this["padding-right"];
            set => this["padding-right"] = value;
        }

        public Value PaddingBottom {
            get => this["padding-bottom"];
            set => this["padding-bottom"] = value;
        }

        public Value PaddingLeft {
            get => this["padding-left"];
            set => this["padding-left"] = value;
        }

        public Value Padding {
            get => this["padding-top"];
            set {
                this["padding-top"] = value;
                this["padding-right"] = value;
                this["padding-bottom"] = value;
                this["padding-left"] = value;
            }
        }

        public Value Position {
            get => this["position"];
            set => this["position"] = value;
        }

        public Value Right {
            get => this["right"];
            set => this["right"] = value;
        }

        public Value TextAlign {
            get => this["text-align"];
            set => this["text-align"] = value;
        }

        public Value TextDecoration {
            get => this["text-decoration"];
            set => this["text-decoration"] = value;
        }

        public Value Top {
            get => this["top"];
            set => this["top"] = AddNumberUnits (value, "px");
        }

        public Value Transform {
            get => this["transform"];
            set => this["transform"] = value;
        }

        public Value TransformOrigin {
            get => this["transform-origin"];
            set => this["transform-origin"] = value;
        }

        public Value VerticalAlign {
            get => this["vertical-align"];
            set => this["vertical-align"] = value;
        }

        public Value Visibility {
            get => this["visibility"];
            set => this["visibility"] = value;
        }

        public Value Width {
            get => this["width"];
            set => this["width"] = AddNumberUnits (value, "px");
        }

        public Value ZIndex {
            get => this["z-index"];
            set => this["z-index"] = value;
        }

        public Value WhiteSpace {
            get => this["white-space"];
            set => this["white-space"] = value;
        }

        public Value this[string propertyName] {
            get {
                lock (properties) {
                    Value p;
                    if (!properties.TryGetValue (propertyName, out p)) {
                        p = "inherit";
                    }
                    return p;
                }
            }
            set {
                var safeValue = value ?? "inherit";
                lock (properties) {
                    if (value == null) {
                        if (!properties.Remove (propertyName))
                            return;
                    }
                    else {
                        Value old;
                        if (properties.TryGetValue (propertyName, out old)) {
                            if (EqualityComparer<Value>.Default.Equals (old, safeValue))
                                return;
                        }
                        properties[propertyName] = safeValue;
                    }
                }
                PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
            }
        }

        public static string GetJsName (string propertyName)
        {
            var o = new System.Text.StringBuilder ();
            var needsCap = false;
            for (var i = 0; i < propertyName.Length; i++) {
                var c = propertyName[i];
                if (c == '-') {
                    needsCap = true;
                }
                else {
                    o.Append (needsCap ? Char.ToUpperInvariant (c) : c);
                    needsCap = false;
                }
            }
            return o.ToString ();
        }

        public override string ToString ()
        {
            var o = new System.Text.StringBuilder ();
            var head = "";
            lock (properties) {
                foreach (var p in properties) {
                    o.Append (head);
                    o.Append (p.Key);
                    o.Append (":");
                    o.Append (Convert.ToString (p.Value, System.Globalization.CultureInfo.InvariantCulture));
                    head = ";";
                }
            }
            return o.ToString ();
        }

        static string AddNumberUnits (object val, string units)
        {
            if (val == null)
                return null;
            if (val is string s)
                return s;
            
            if (val is int i)
                return i + units;
            if (val is double d)
                return d.ToString (System.Globalization.CultureInfo.InvariantCulture) + units;
            if (val is float f)
                return f.ToString (System.Globalization.CultureInfo.InvariantCulture) + units;

            if (val is IConvertible c)
                return c.ToString (System.Globalization.CultureInfo.InvariantCulture) + units;

            return val.ToString ();
        }

        public double GetNumberWithUnits (string key, string units, double baseValue)
        {
            var v = this[key];
            if (v == null)
                return 0;

            if (v is string s) {
                var lastIndex = s.LastIndexOfAny (numberChars);
                if (lastIndex < 0)
                    return 0;
                var num = double.Parse (s.Substring (0, lastIndex + 1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                return num;
            }

            if (v is int i)
                return i;
            if (v is double d)
                return d;
            if (v is float f)
                return f;

            if (v is IConvertible c)
                return c.ToDouble (System.Globalization.CultureInfo.InvariantCulture);

            return 0;
        }
    }
}
