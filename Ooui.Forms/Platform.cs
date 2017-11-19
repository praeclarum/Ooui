using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ooui.Forms.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Web;

namespace Ooui.Forms
{
    public class Platform : BindableObject, IPlatform, INavigation, IDisposable
    {
        bool _disposed;

        readonly PlatformRenderer _renderer;

        public Ooui.Element Element => _renderer;

        public Page Page { get; private set; }

        IReadOnlyList<Page> INavigation.ModalStack => throw new NotImplementedException ();

        IReadOnlyList<Page> INavigation.NavigationStack => throw new NotImplementedException ();

        public static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached ("Renderer", typeof (IVisualElementRenderer), typeof (Platform), default (IVisualElementRenderer),
            propertyChanged: (bindable, oldvalue, newvalue) => {
                var view = bindable as VisualElement;
                if (view != null)
                    view.IsPlatformEnabled = newvalue != null;
            });

        public Platform ()
        {
            _renderer = new PlatformRenderer (this);

            _renderer.Style.PropertyChanged += HandleRendererStyle_PropertyChanged;

            MessagingCenter.Subscribe (this, Page.AlertSignalName, (Page sender, AlertArguments arguments) => {
                var alert = new DisplayAlert (arguments);
                alert.Clicked += CloseAlert;

                _renderer.AppendChild (alert.Element);

                void CloseAlert (object s, EventArgs e)
                {
                    _renderer.RemoveChild (alert.Element);
                }
            });
        }

        void IDisposable.Dispose ()
        {
            if (_disposed)
                return;
            _disposed = true;

            MessagingCenter.Unsubscribe<Page, ActionSheetArguments> (this, Page.ActionSheetSignalName);
            MessagingCenter.Unsubscribe<Page, AlertArguments> (this, Page.AlertSignalName);
            MessagingCenter.Unsubscribe<Page, bool> (this, Page.BusySetSignalName);
        }

        public static IVisualElementRenderer CreateRenderer (VisualElement element)
        {
            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer> (element.GetType ()) ?? new DefaultRenderer ();
            renderer.SetElement (element);
            return renderer;
        }

        public static IVisualElementRenderer GetRenderer (VisualElement bindable)
        {
            return (IVisualElementRenderer)bindable.GetValue (RendererProperty);
        }

        public static void SetRenderer (VisualElement bindable, IVisualElementRenderer value)
        {
            bindable.SetValue (RendererProperty, value);
        }

        protected override void OnBindingContextChanged ()
        {
            SetInheritedBindingContext (Page, BindingContext);

            base.OnBindingContextChanged ();
        }

        public SizeRequest GetNativeSize (VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderView = GetRenderer (view);
            if (renderView == null || renderView.NativeView == null)
                return new SizeRequest (Size.Zero);

            return renderView.GetDesiredSize (widthConstraint, heightConstraint);
        }

        public void SetPage (Page newRoot)
        {
            if (newRoot == null)
                return;
            if (Page != null)
                throw new NotImplementedException ();
            Page = newRoot;

            Page.Platform = this;
            AddChild (Page);

            Page.DescendantRemoved += HandleChildRemoved;

            Application.Current.NavigationProxy.Inner = this;
        }

        void HandleChildRemoved (object sender, ElementEventArgs e)
        {
            throw new NotImplementedException ();
        }

        void AddChild (VisualElement view)
        {
            if (!Application.IsApplicationOrNull (view.RealParent))
                Console.Error.WriteLine ("Tried to add parented view to canvas directly");

            if (GetRenderer (view) == null) {
                var viewRenderer = CreateRenderer (view);
                SetRenderer (view, viewRenderer);

                _renderer.AppendChild (viewRenderer.NativeView);
                viewRenderer.SetElementSize (new Size (640, 480));
            }
            else
                Console.Error.WriteLine ("Potential view double add");
        }

        void HandleRendererStyle_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var pageRenderer = GetRenderer (Page);
            pageRenderer?.SetElementSize (Ooui.Forms.Extensions.ElementExtensions.GetSizeRequest (_renderer, double.PositiveInfinity, double.PositiveInfinity).Request);
        }

        void INavigation.InsertPageBefore (Page page, Page before)
        {
            throw new NotImplementedException ();
        }

        Task<Page> INavigation.PopAsync ()
        {
            throw new NotImplementedException ();
        }

        Task<Page> INavigation.PopAsync (bool animated)
        {
            throw new NotImplementedException ();
        }

        Task<Page> INavigation.PopModalAsync ()
        {
            throw new NotImplementedException ();
        }

        Task<Page> INavigation.PopModalAsync (bool animated)
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PopToRootAsync ()
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PopToRootAsync (bool animated)
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PushAsync (Page page)
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PushAsync (Page page, bool animated)
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PushModalAsync (Page page)
        {
            throw new NotImplementedException ();
        }

        Task INavigation.PushModalAsync (Page page, bool animated)
        {
            throw new NotImplementedException ();
        }

        void INavigation.RemovePage (Page page)
        {
            throw new NotImplementedException ();
        }
    }
}
