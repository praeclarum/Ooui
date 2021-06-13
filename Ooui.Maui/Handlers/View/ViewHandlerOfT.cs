#nullable enable
using System;
using System.Runtime.CompilerServices;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using NativeView = Ooui.Element;

namespace Ooui.Maui.Handlers
{
    public abstract partial class ViewHandler<TVirtualView, TNativeView> : Ooui.Maui.Handlers.ViewHandler<TVirtualView>,
        INativeViewHandler
        where TVirtualView : class, IView
        where TNativeView : NativeView
    {
        Ooui.Element? INativeViewHandler.NativeView => WrappedNativeView;
		// Ooui.Element? INativeViewHandler.ContainerView => ContainerView;

        protected new Ooui.Element? WrappedNativeView => (Ooui.Element?)base.WrappedNativeView;

        protected readonly PropertyMapper _defaultMapper;
        protected PropertyMapper _mapper;
        static bool HasSetDefaults;

        // TODO: fak: I don't know what's needed here
        // [Microsoft.Maui.HotReload.OnHotReload]
        static void OnHotReload()
        {
            HasSetDefaults = false;
        }

        protected ViewHandler(PropertyMapper mapper)
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _defaultMapper = mapper;
            _mapper = _defaultMapper;
        }

        protected abstract TNativeView CreateNativeView();

        public new TNativeView? NativeView
        {
            get => (TNativeView?)base.NativeView;
            private set => base.NativeView = value;
        }

        protected TNativeView NativeViewValidation([CallerMemberName] string callerName = "")
        {
            _ = NativeView ?? throw new InvalidOperationException($"NativeView cannot be null here: {callerName}");
            return NativeView;
        }

        public override void SetVirtualView(IView view)
        {
            _ = view ?? throw new ArgumentNullException(nameof(view));

            if (VirtualView == view)
                return;

            if (VirtualView?.Handler != null)
                VirtualView.Handler = null;

            bool setupNativeView = VirtualView == null;

            VirtualView = (TVirtualView)view;
            NativeView ??= CreateNativeView();

            if (VirtualView != null && VirtualView.Handler != this)
                VirtualView.Handler = this;

            if (setupNativeView && NativeView != null)
            {
                ConnectHandler(NativeView);
            }

            if (!HasSetDefaults)
            {
                if (NativeView != null)
                {
                    SetupDefaults(NativeView);
                }

                HasSetDefaults = true;
            }

            _mapper = _defaultMapper;

            if (VirtualView is IPropertyMapperView imv)
            {
                var map = imv.GetPropertyMapperOverrides();
                var instancePropertyMapper = map as PropertyMapper<TVirtualView>;
                if (map != null && instancePropertyMapper == null)
                {
                }
                if (instancePropertyMapper != null)
                {
                    instancePropertyMapper.Chained = _defaultMapper;
                    _mapper = instancePropertyMapper;
                }
            }

            if (_mapper is IOouiPropertyMapper omapper) {
                omapper.UpdateProperties(this, VirtualView);
            }
            else {
                throw new Exception("Expected property mapper to be a Ooui property mapper");
            }
        }

        void IViewHandler.DisconnectHandler()
        {
            if (NativeView != null && VirtualView != null)
                DisconnectHandler(NativeView);
        }

        protected virtual void ConnectHandler(TNativeView nativeView)
        {
            base.ConnectHandler(nativeView);
        }

        protected virtual void DisconnectHandler(TNativeView nativeView)
        {
            base.DisconnectHandler(nativeView);
        }

        public override void UpdateValue(string property) {
            if (_mapper == null) {
                // Do nothing
            }
            else if (_mapper is IOouiPropertyMapper omapper) {
                omapper.UpdateProperty(this, VirtualView, property);
            }
            else {
                throw new Exception("Expected property mapper to be a Ooui property mapper");
            }
        }

        protected virtual void SetupDefaults(TNativeView nativeView) { }

        public override void NativeArrange(Rectangle rect)
        {
        }

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return new Size(200, 100);
            // var nativeView = WrappedNativeView;

            // if (nativeView == null || VirtualView == null)
            // {
            // 	return new Size(widthConstraint, heightConstraint);
            // }

            // var explicitWidth = VirtualView.Width;
            // var explicitHeight = VirtualView.Height;
            // var hasExplicitWidth = explicitWidth >= 0;
            // var hasExplicitHeight = explicitHeight >= 0;

            // var sizeThatFits = nativeView.SizeThatFits(new CoreGraphics.CGSize((float)widthConstraint, (float)heightConstraint));

            // var size = new Size(
            // 	sizeThatFits.Width == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Width,
            // 	sizeThatFits.Height == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Height);

            // if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
            // {
            // 	nativeView.SizeToFit();
            // 	size = new Size(nativeView.Frame.Width, nativeView.Frame.Height);
            // }

            // return new Size(hasExplicitWidth ? explicitWidth : size.Width,
            // 	hasExplicitHeight ? explicitHeight : size.Height);
        }



        protected override void SetupContainer()
        {
            if (NativeView == null || ContainerView != null)
                return;

            // TODO: fak: Ooui doesn't support inspecting the parent
            throw new NotImplementedException();

            // var oldParent = (Element?)NativeView.Superview;

            // var oldIndex = oldParent?.IndexOfSubview(NativeView);
            // NativeView.RemoveFromSuperview();

            // ContainerView ??= new WrapperView(NativeView.Bounds);
            // ContainerView.AddSubview(NativeView);

            // if (oldIndex is int idx && idx >= 0)
            // 	oldParent?.InsertSubview(ContainerView, idx);
            // else
            // 	oldParent?.AddSubview(ContainerView);
        }

        protected override void RemoveContainer()
        {
            // TODO: fak: Ooui doesn't support inspecting the parent

            if (NativeView == null || ContainerView == null)// || NativeView.Superview != ContainerView)
                return;            

            // var oldParent = (UIView?)ContainerView.Superview;

            // var oldIndex = oldParent?.IndexOfSubview(ContainerView);
            // ContainerView.RemoveFromSuperview();

            // ContainerView = null;

            // if (oldIndex is int idx && idx >= 0)
            // 	oldParent?.InsertSubview(NativeView, idx);
            // else
            // 	oldParent?.AddSubview(NativeView);
        }
    }

    public abstract partial class ViewHandler<TVirtualView> : ViewHandler
        where TVirtualView : class, IView
    {
        internal ViewHandler()
        {
        }

        public new TVirtualView? VirtualView
        {
            get => (TVirtualView?)base.VirtualView;
            private protected set => base.VirtualView = value;
        }

        protected TVirtualView VirtualViewWithValidation([CallerMemberName] string callerName = "")
        {
            _ = VirtualView ?? throw new InvalidOperationException($"VirtualView cannot be null here: {callerName}");
            return VirtualView;
        }
    }
}