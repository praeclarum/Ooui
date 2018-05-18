using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;

namespace Ooui.Forms.Renderers
{
    public class NavigationPageRenderer : VisualElementRenderer<NavigationPage>
    {
        // mimics Xamarin.Forms stack... constains previous page AND current page.  Empty Stack will indicate current page should be root.
        private Stack<(Page page, string hash)> backHashStack = new Stack<(Page page, string hash)>();
        private Stack<(Page page, string hash)> forwardHashStack = new Stack<(Page page, string hash)>();
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
                CopyCurrentNavigationStackToBrowserHistory();
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

        //only called when user types url manually OR clicks forward or back in the browser
        private async void ProcessHash(string fullHash)
        {
            string[] splitHash;
            if (fullHash.Length > 0)
                splitHash = fullHash.Substring(1).Split('/');
            else
                splitHash = new string[] { };

            // since only C#7.0... have to do this madness to deal with ValueTuple equality checking for null
            var lastPageHashFromBackStack = backHashStack.Where(x => x.hash == splitHash.Last()).Cast<(Page page, string hash)?>().FirstOrDefault();
            var lastPageHashFromForwardStack = forwardHashStack.Where(x => x.hash == splitHash.Last()).Cast<(Page page, string hash)?>().FirstOrDefault();

            // case for clicking back
            if (lastPageHashFromBackStack != null)
            {
                // edit the backHashStack (and optionally stick extra in the forward stack)
                //(Page page, string hash)? lastPageHashPopped = null;
                (Page page, string hash) lastPageHashPeeked;
                while ((lastPageHashPeeked = backHashStack.Peek()).hash != lastPageHashFromBackStack.Value.hash)
                {
                    var lastPageHashPopped = backHashStack.Pop();
                    forwardHashStack.Push(lastPageHashPopped);
                }
                // present page that is current
                ReplaceElement(lastPageHashPeeked.page);

            }
            // case for clicking forward
            else if (lastPageHashFromForwardStack != null)
            {
                //edit the forwardHashStack
                while (forwardHashStack.Peek().hash != lastPageHashFromForwardStack.Value.hash)
                {
                    var page = forwardHashStack.Pop();
                    //put it in the forward stack
                    forwardHashStack.Push(page);
                }
            }
            // case for someone typing in url from scratch with hash!
            else if (fullHash.Length > 0)
            {
                foreach (var hash in splitHash.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var pageTypeName = WebUtility.HtmlDecode(hash);
                    var pageInstance = Activator.CreateInstance(Type.GetType($"{this.ns}.{pageTypeName}, {this.assemblyName}"));
                    await this.Element.Navigation.PushAsync(pageInstance as Page);
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

        private void CopyCurrentNavigationStackToBrowserHistory()
        {
            var reversedNavStack = this.Element.Navigation.NavigationStack.Reverse();
            
            // Add any pages except the first one.  The first page will always be root and have no special path.
            foreach (var page in reversedNavStack.Except(new Page[] { reversedNavStack.First() }))
            {
                this.PushHashState(page);
            }           
            // If there are more pages than root, show the last page in the stack instead of root.
            if (reversedNavStack.Count() > 1)
            {
                ReplaceElement(reversedNavStack.Last());
            }
        }

        private void PushHashState(Page page)
        {
            // Pages must have titles when used in navigation
            if (string.IsNullOrWhiteSpace(page.Title))
                throw new Exception("Pages must have titles when used in navigation.");

            this.backHashStack.Push((page, WebUtility.HtmlEncode(page.GetType().Name)));
            this.NativeView.Document.Window.Call("history.pushState", null, null, "#" + GenerateFullHash());
        }

        private void ReplaceElement(Page page)
        {
            // Remove the first (and only) child of the div representing our navigation page.
            foreach (var child in this.NativeView.Children)
                this.NativeView.RemoveChild(child);

            // Generate the Ooui element and add the new page
            this.NativeView.AppendChild(page.GetOouiElement());
        }

        

        // This is where you would normally set back button state based on what's in the stack.
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Not needed for browser... back button is always available.
        }

        private void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPopRequested(object sender, NavigationRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        // This is where you would draw the new contents.
        private void OnPushRequested(object sender, NavigationRequestedEventArgs e)
        {
            //// Remove the first (and only) child of the div representing our navigation page.
            //this.NativeView.RemoveChild(this.NativeView.FirstChild);

            //// Generate the Ooui element and add the new page
            //this.NativeView.AppendChild(e.Page.GetOouiElement());

            PushHashState(e.Page);

            ReplaceElement(e.Page);

            //this.hashStack.Push((e.Page, WebUtility.UrlEncode(e.Page.Title)));
                        
            //this.NativeView.Document.Window.Call("history.pushState", null, null, "#" + e.Page.Title);

            
        }
        


        string GenerateFullHash()
        {
            string hashString = "";
            foreach (var i in this.backHashStack.Reverse())
            {
                hashString += i.hash + '/';
            }
            hashString = hashString.Substring(0, hashString.Length - 1);
            return hashString;
        }
        
    }
}
