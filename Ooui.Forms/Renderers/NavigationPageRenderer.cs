using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Ooui.Forms.Renderers
{
    public class NavigationPageRenderer : VisualElementRenderer<NavigationPage>
    {
        private Stack<DefaultRenderer> backElementStack = new Stack<DefaultRenderer>();
        private Stack<DefaultRenderer> forwardElementStack = new Stack<DefaultRenderer>();
        // required hack to make sure a browser nav event (back and forward buttons) don't trigger the commands to pushState or go back on the browser AGAIN
        bool ignoreNavEventFlag = false;

        private string ns;
        private string assemblyName;

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PushRequested -= OnPushRequested;
                e.OldElement.PopRequested -= OnPopRequested;
                e.OldElement.PopToRootRequested -= OnPopToRootRequested;
                e.OldElement.InternalChildren.CollectionChanged -= OnChildrenChanged;
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;
            }
            if (e.NewElement != null)
            {
                e.NewElement.PushRequested += OnPushRequested;
                e.NewElement.PopRequested += OnPopRequested;
                e.NewElement.PopToRootRequested += OnPopToRootRequested;
                e.NewElement.InternalChildren.CollectionChanged += OnChildrenChanged;
                e.NewElement.PropertyChanged += OnElementPropertyChanged;

                GetAssemblyInfoForRootPage();
            }                        
        }
               
        protected override bool TriggerEventFromMessage(Message message)
        {
            if (message.TargetId == "window" && message.Key == "hashchange" && message.Value is Newtonsoft.Json.Linq.JObject k)
            {
                ProcessHash((string)k["hash"]);
                return true;
            }
            else
                return base.TriggerEventFromMessage(message);
        }

        //signaling doesn't seem to work until child has been inserted into document, so wait for this and then adjust the hash for direct navigation
        protected override void OnChildInsertedBefore(Node newChild, Node referenceChild)
        {
            base.OnChildInsertedBefore(newChild, referenceChild);

            var index = this.Children.IndexOf(newChild);
            if (index - 1 >= 0)
                (this.Children[index - 1] as Element).Style.Display = "none";
            
            this.backElementStack.Push(newChild as DefaultRenderer);

            if (this.ignoreNavEventFlag)
                this.ignoreNavEventFlag = false;
            else
            {
                this.forwardElementStack.Clear();
                this.NativeView.Document.Window.Call("history.pushState", null, null, GenerateFullHash());
            }
        }

        protected override void OnChildRemoved(Node child)
        {
            base.OnChildRemoved(child);

            if (this.Children.Count > 0)
                (this.Children.Last() as Element).Style.Display = "block";

            DefaultRenderer popped = this.backElementStack.Pop();
            this.forwardElementStack.Push(child as DefaultRenderer);

            if (this.ignoreNavEventFlag)
                this.ignoreNavEventFlag = false;
            else
                this.NativeView.Document.Window.Call("history.back");
        }

        //only called when user types url manually OR clicks forward or back in the browser OR very beginning of navigation (backStack has first element already though)
        private void ProcessHash(string fullHash)
        {
            IEnumerable<string> browserPageArray;
            if (fullHash.Length > 0)
                browserPageArray = fullHash.Split('/').Select(x => x == "#" ? this.backElementStack.Last().Element.GetType().Name : x);
            else
                browserPageArray = new string[] { this.backElementStack.Last().Element.GetType().Name };

            // if current hash doesn't match up with the current page displayed (which is the first/most recent one in the backHashStack)
            if (backElementStack.First().Element.GetType().Name != browserPageArray.Last())
            {
                // see if the last hash item is in the backStack (nav backwards) OR in the forward stack (nav forwards) OR not there at all (regenerate stack)
                var lastPageFromBackStack = backElementStack.FirstOrDefault(x => x.Element.GetType().Name == browserPageArray.Last());//.Cast<(Page page, string hash)?>().FirstOrDefault();
                var lastPageFromForwardStack = forwardElementStack.FirstOrDefault(x => x.Element.GetType().Name == browserPageArray.Last());//.Cast<(Page page, string hash)?>().FirstOrDefault();

                // just need to adjust internal stack and page view
                // case clicked back button
                if (lastPageFromBackStack != null)
                {
                    var peeked = this.backElementStack.Peek();
                    this.ignoreNavEventFlag = true;
                    this.Element.PopAsync();
                }
                // case for clicking forward
                else if (lastPageFromForwardStack != null)
                {
                    var popped = this.forwardElementStack.Pop();
                    this.ignoreNavEventFlag = true;

                    // *** This should work, but the result is a page that doesn't format correctly.  Instead, we'll have to create new pages from scratch.
                    //this.Element.PushAsync(popped.Element as Page);
                    this.Element.PushAsync((Page)Activator.CreateInstance(Type.GetType($"{this.ns}.{popped.Element.GetType().Name}, {this.assemblyName}")));
                }
                // case for someone typing in url from scratch with hash
                else
                {
                    //assume someone put the url into the browser manually from scratch
                    //first replace the current state with # only.
                    this.Document.Window.Call("history.replaceState", null, null, GenerateFullHash());
                    foreach (var pageName in browserPageArray.Where(x=> x != backElementStack.Last().Element.GetType().Name))
                    {
                        var pageTypeName = WebUtility.HtmlDecode(pageName);
                        var pageInstance = Activator.CreateInstance(Type.GetType($"{this.ns}.{pageTypeName}, {this.assemblyName}"));
                        this.Element.PushAsync(pageInstance as Page);
                    }
                }
            }
        }

        // Reflection needs more than just the name to create an instance from a string.
        private void GetAssemblyInfoForRootPage()
        {
            if (this.Element.Navigation.NavigationStack.Count > 0)
            {
                var page = this.Element.Navigation.NavigationStack.Last();
                var type = page.GetType();
                this.ns = type.Namespace;
                this.assemblyName = type.Assembly.FullName;
            }
        }
                
        // This is where you would normally set back button state based on what's in the stack.
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Not needed for browser... back button is always available.
        }

        private void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            e.Realize = true;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void OnPopRequested(object sender, NavigationRequestedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            e.Realize = true;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        // This is where you would draw the new contents.
        private void OnPushRequested(object sender, NavigationRequestedEventArgs e)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            e.Realize = true;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private string GenerateFullHash()
        {
            string hashString = "";
            bool started = false;
            foreach (var i in this.backElementStack.Reverse())
            {
                if (started)
                    hashString += "/" + i.Element.GetType().Name;
                else
                {
                    started = true;
                    hashString += "#";
                }
            }
            return hashString;
        }
        
    }
}
