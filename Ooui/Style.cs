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
            get => GetProperty ("background-color");
            set => SetProperty ("background-color", value);
        }

        public Value BackgroundImage {
            get => GetProperty ("background-image");
            set => SetProperty ("background-image", value);
        }

        public Value BorderTopColor {
            get => GetProperty ("border-top-color");
            set => SetProperty ("border-top-color", value);
        }

        public Value BorderRightColor {
            get => GetProperty ("border-right-color");
            set => SetProperty ("border-right-color", value);
        }

        public Value BorderBottomColor {
            get => GetProperty ("border-bottom-color");
            set => SetProperty ("border-bottom-color", value);
        }

        public Value BorderLeftColor {
            get => GetProperty ("border-left-color");
            set => SetProperty ("border-left-color", value);
        }

        public Value BorderColor {
            get => GetProperty ("border-top-color");
            set {
                SetProperty ("border-top-color", value);
                SetProperty ("border-right-color", value);
                SetProperty ("border-bottom-color", value);
                SetProperty ("border-left-color", value);
            }
        }

        public Value BorderTopStyle {
            get => GetProperty ("border-top-style");
            set => SetProperty ("border-top-style", value);
        }

        public Value BorderRightStyle {
            get => GetProperty ("border-right-style");
            set => SetProperty ("border-right-style", value);
        }

        public Value BorderBottomStyle {
            get => GetProperty ("border-bottom-style");
            set => SetProperty ("border-bottom-style", value);
        }

        public Value BorderLeftStyle {
            get => GetProperty ("border-left-style");
            set => SetProperty ("border-left-style", value);
        }

        public Value BorderStyle {
            get => GetProperty ("border-top-style");
            set {
                SetProperty ("border-top-style", value);
                SetProperty ("border-right-style", value);
                SetProperty ("border-bottom-style", value);
                SetProperty ("border-left-style", value);
            }
        }

        public Value BorderTopWidth {
            get => GetProperty ("border-top-width");
            set => SetProperty ("border-top-width", value);
        }

        public Value BorderRightWidth {
            get => GetProperty ("border-right-width");
            set => SetProperty ("border-right-width", value);
        }

        public Value BorderBottomWidth {
            get => GetProperty ("border-bottom-width");
            set => SetProperty ("border-bottom-width", value);
        }

        public Value BorderLeftWidth {
            get => GetProperty ("border-left-width");
            set => SetProperty ("border-left-width", value);
        }

        public Value BorderWidth {
            get => GetProperty ("border-top-width");
            set {
                SetProperty ("border-top-width", value);
                SetProperty ("border-right-width", value);
                SetProperty ("border-bottom-width", value);
                SetProperty ("border-left-width", value);
            }
        }

        public Value Bottom {
            get => GetProperty ("bottom");
            set => SetProperty ("bottom", value);
        }

        public Value Clear {
            get => GetProperty ("clear");
            set => SetProperty ("clear", value);
        }

        public Value Color {
            get => GetProperty ("color");
            set => SetProperty ("color", value);
        }

        public Value Cursor {
            get => GetProperty ("cursor");
            set => SetProperty ("cursor", value);
        }

        public Value Float {
            get => GetProperty ("float");
            set => SetProperty ("float", value);
        }

        public Value FontFamily {
            get => GetProperty ("font-family");
            set => SetProperty ("font-family", value);
        }

        public Value FontSize {
            get => GetProperty ("font-size");
            set => SetProperty ("font-size", value);
        }

        public Value FontStyle {
            get => GetProperty ("font-style");
            set => SetProperty ("font-style", value);
        }

        public Value FontVariant {
            get => GetProperty ("font-variant");
            set => SetProperty ("font-variant", value);
        }

        public Value FontWeight {
            get => GetProperty ("font-weight");
            set => SetProperty ("font-weight", value);
        }

        public Value Height {
            get => GetProperty ("height");
            set => SetProperty ("height", value);
        }

        public Value Left {
            get => GetProperty ("left");
            set => SetProperty ("left", value);
        }

        public Value LineHeight {
            get => GetProperty ("line-height");
            set => SetProperty ("line-height", value);
        }

        public Value MarginTop {
            get => GetProperty ("margin-top");
            set => SetProperty ("margin-top", value);
        }

        public Value MarginRight {
            get => GetProperty ("margin-right");
            set => SetProperty ("margin-right", value);
        }

        public Value MarginBottom {
            get => GetProperty ("margin-bottom");
            set => SetProperty ("margin-bottom", value);
        }

        public Value MarginLeft {
            get => GetProperty ("margin-left");
            set => SetProperty ("margin-left", value);
        }

        public Value Margin {
            get => GetProperty ("margin-top");
            set {
                SetProperty ("margin-top", value);
                SetProperty ("margin-right", value);
                SetProperty ("margin-bottom", value);
                SetProperty ("margin-left", value);
            }
        }

        public Value PaddingTop {
            get => GetProperty ("padding-top");
            set => SetProperty ("padding-top", value);
        }

        public Value PaddingRight {
            get => GetProperty ("padding-right");
            set => SetProperty ("padding-right", value);
        }

        public Value PaddingBottom {
            get => GetProperty ("padding-bottom");
            set => SetProperty ("padding-bottom", value);
        }

        public Value PaddingLeft {
            get => GetProperty ("padding-left");
            set => SetProperty ("padding-left", value);
        }

        public Value Padding {
            get => GetProperty ("padding-top");
            set {
                SetProperty ("padding-top", value);
                SetProperty ("padding-right", value);
                SetProperty ("padding-bottom", value);
                SetProperty ("padding-left", value);
            }
        }

        public Value Right {
            get => GetProperty ("right");
            set => SetProperty ("right", value);
        }

        public Value Top {
            get => GetProperty ("top");
            set => SetProperty ("top", value);
        }

        public Value Visibility {
            get => GetProperty ("visibility");
            set => SetProperty ("visibility", value);
        }

        public Value Width {
            get => GetProperty ("width");
            set => SetProperty ("width", value);
        }

        public Value GetProperty (string propertyName)
        {
            lock (properties) {
                Value p;
                if (!properties.TryGetValue (propertyName, out p)) {
                    p = "inherit";
                }
                return p;
            }
        }

        public void SetProperty (string propertyName, Value value)
        {
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
