using System;

namespace System.ComponentModel
{
    public interface INotifyPropertyChanged
    {

    }

    public class PropertyChangedEventArgs : EventArgs
    {

    }

    public delegate void PropertyChangedEventHandler (object sender, PropertyChangedEventArgs e);
}