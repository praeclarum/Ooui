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

        private Value GetProperty (string propertyName)
        {
            lock (properties) {
                Value p;
                if (!properties.TryGetValue (propertyName, out p)) {
                    p = "inherit";
                    properties[propertyName] = p;
                }
                return p;
            }
        }

        private void SetProperty (string propertyName, Value value)
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
