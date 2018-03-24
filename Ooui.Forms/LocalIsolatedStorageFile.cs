using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Ooui.Forms
{
    public class LocalIsolatedStorageFile : IIsolatedStorageFile
    {
        public Task CreateDirectoryAsync (string path)
        {
            return Task.FromResult<object> (null);
        }

        public Task<bool> GetDirectoryExistsAsync (string path)
        {
            return Task.FromResult<bool> (true);
        }

        public Task<bool> GetFileExistsAsync (string path)
        {
            return Task.FromResult<bool> (false);
        }

        public Task<DateTimeOffset> GetLastWriteTimeAsync (string path)
        {
            throw new NotSupportedException ();
        }

        public Task<Stream> OpenFileAsync (string path, Xamarin.Forms.Internals.FileMode mode, Xamarin.Forms.Internals.FileAccess access)
        {
            throw new NotSupportedException ();
        }

        public Task<Stream> OpenFileAsync (string path, Xamarin.Forms.Internals.FileMode mode, Xamarin.Forms.Internals.FileAccess access, Xamarin.Forms.Internals.FileShare share)
        {
            throw new NotSupportedException ();
        }
    }
}
