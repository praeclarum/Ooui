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
            set => this["border-top-width"] = value;
        }

        public Value BorderRightWidth {
            get => this["border-right-width"];
            set => this["border-right-width"] = value;
        }

        public Value BorderBottomWidth {
            get => this["border-bottom-width"];
            set => this["border-bottom-width"] = value;
        }

        public Value BorderLeftWidth {
            get => this["border-left-width"];
            set => this["border-left-width"] = value;
        }

        public Value BorderWidth {
            get => this["border-top-width"];
            set {
                this["border-top-width"] = value;
                this["border-right-width"] = value;
                this["border-bottom-width"] = value;
                this["border-left-width"] = value;
            }
        }

        public Value Bottom {
            get => this["bottom"];
            set => this["bottom"] = value;
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
            set => this["font-size"] = value;
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
            set => this["height"] = value;
        }

        public Value Left {
            get => this["left"];
            set => this["left"] = value;
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
            set => this["top"] = value;
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
            set => this["width"] = value;
        }

        public Value this [string propertyName]
        {
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
                    Value old;
                    if (properties.TryGetValue (propertyName, out old)) {
                        if (EqualityComparer<Value>.Default.Equals (old, safeValue))
                            return;
                    }
                    properties[propertyName] = safeValue;
                }
                PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
            }
        }
    }
}
