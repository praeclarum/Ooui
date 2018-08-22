using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class ImageRenderer : ViewRenderer<Xamarin.Forms.Image, Ooui.Html.Image>
    {
        bool _isDisposed;

        protected override void Dispose (bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing) {
            }

            _isDisposed = true;

            base.Dispose (disposing);
        }

        protected override async void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            if (Control == null) {
                var imageView = new Ooui.Html.Image ();
                SetNativeControl (imageView);
                this.Style.Overflow = "hidden";
            }

            if (e.NewElement != null) {
                SetAspect ();
                await TrySetImage (e.OldElement);
                SetOpacity ();
            }

            base.OnElementChanged (e);
        }

        protected override async void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);
            if (e.PropertyName == Xamarin.Forms.Image.SourceProperty.PropertyName)
                await TrySetImage ();
            else if (e.PropertyName == Xamarin.Forms.Image.IsOpaqueProperty.PropertyName)
                SetOpacity ();
            else if (e.PropertyName == Xamarin.Forms.Image.AspectProperty.PropertyName)
                SetAspect ();
        }

        void SetAspect ()
        {
            if (_isDisposed || Element == null || Control == null) {
                return;
            }
        }

        protected virtual async Task TrySetImage (Xamarin.Forms.Image previous = null)
        {
            // By default we'll just catch and log any exceptions thrown by SetImage so they don't bring down
            // the application; a custom renderer can override this method and handle exceptions from
            // SetImage differently if it wants to

            try {
                await SetImage (previous).ConfigureAwait (false);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine ("Error loading image: {0}", ex);
            }
            finally {
                ((IImageController)Element)?.SetIsLoading (false);
            }
        }

        protected async Task SetImage (Xamarin.Forms.Image oldElement = null)
        {
            if (_isDisposed || Element == null || Control == null) {
                return;
            }

            var source = Element.Source;

            if (oldElement != null) {
                var oldSource = oldElement.Source;
                if (Equals (oldSource, source))
                    return;

                if (oldSource is FileImageSource && source is FileImageSource && ((FileImageSource)oldSource).File == ((FileImageSource)source).File)
                    return;

                Control.Source = "";
            }

            IImageSourceHandler handler;

            Element.SetIsLoading (true);

            if (source != null &&
                (handler = Xamarin.Forms.Internals.Registrar.Registered.GetHandler<IImageSourceHandler> (source.GetType ())) != null) {
                string uiimage;
                try {
                    uiimage = await handler.LoadImageAsync (source, scale: 1.0f);
                }
                catch (OperationCanceledException) {
                    uiimage = null;
                }

                if (_isDisposed)
                    return;

                var imageView = Control;
                if (imageView != null)
                    imageView.Source = uiimage;

                ((IVisualElementController)Element).NativeSizeChanged ();
            }
            else {
                Control.Source = "";
            }

            Element.SetIsLoading (false);
        }

        void SetOpacity ()
        {
            if (_isDisposed || Element == null || Control == null) {
                return;
            }
        }
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<string> LoadImageAsync (ImageSource imagesource, CancellationToken cancelationToken = default (CancellationToken), float scale = 1);
    }

    public sealed class FileImageSourceHandler : IImageSourceHandler
    {
#pragma warning disable 1998
        public async Task<string> LoadImageAsync (ImageSource imagesource, CancellationToken cancelationToken = default (CancellationToken), float scale = 1f)
        {
            string image = null;
            var filesource = imagesource as FileImageSource;
            var file = filesource?.File;
            if (!string.IsNullOrEmpty (file)) {
                var name = System.IO.Path.GetFileName (file);
                image = "/images/" + name;
                if (Ooui.UI.TryGetFileContentAtPath (image, out var f)) {
                    // Already published
                }
                else {
                    await Task.Run (() => Ooui.UI.PublishFile (image, file), cancelationToken);
                }
            }
            return image;
        }
    }

    public sealed class StreamImagesourceHandler : IImageSourceHandler
    {
        public async Task<string> LoadImageAsync (ImageSource imagesource, CancellationToken cancelationToken = default (CancellationToken), float scale = 1f)
        {
            string image = null;
            var streamsource = imagesource as StreamImageSource;
            if (streamsource?.Stream != null) {
                using (var streamImage = await ((IStreamImageSource)streamsource).GetStreamAsync (cancelationToken).ConfigureAwait (false)) {
                    if (streamImage != null) {
                        var data = new byte[streamImage.Length];
                        using (var outputStream = new System.IO.MemoryStream (data)) {
                            await streamImage.CopyToAsync (outputStream, 4096, cancelationToken).ConfigureAwait (false);
                        }
                        var hash = Ooui.Utilities.Hash (data);
                        var etag = "\"" + hash + "\"";
                        image = "/images/" + hash;
                        if (Ooui.UI.TryGetFileContentAtPath (image, out var file) && file.Etag == etag) {
                            // Already published
                        }
                        else {
                            Ooui.UI.PublishFile (image, data, etag, "image");
                        }
                    }
                }
            }

            if (image == null) {
                System.Diagnostics.Debug.WriteLine ("Could not load image: {0}", streamsource);
            }
            return image;
        }
    }

    public sealed class ImageLoaderSourceHandler : IImageSourceHandler
    {
        public Task<string> LoadImageAsync (ImageSource imagesource, CancellationToken cancelationToken = default (CancellationToken), float scale = 1f)
        {
            var imageLoader = imagesource as UriImageSource;
            return Task.FromResult (imageLoader?.Uri.ToString () ?? "");
        }
    }
}
