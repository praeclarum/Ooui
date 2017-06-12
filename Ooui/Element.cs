using System;

namespace Ooui
{
    public class Element
    {
        public string Id { get; private set; }

        public Element ()
        {
            Id = GenerateId ();
        }

        protected bool SetProperty<T> (ref T backingStore, T newValue)
        {
            if (!backingStore.Equals (newValue)) {
                backingStore = newValue;
                return true;
            }
            return false;
        }

        const string IdChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        static string GenerateId ()
        {
            var rand = new Random();
            var chars = new char[8];
            for (var i = 0; i < chars.Length; i++) {
                chars[i] = IdChars[rand.Next(0, IdChars.Length)];
            }
            return new string(chars);
        }
    }
}
