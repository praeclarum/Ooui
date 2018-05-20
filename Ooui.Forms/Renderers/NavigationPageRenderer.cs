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
        //private Stack<(Page page, string hash)> backHashStack = new Stack<(Page page, string hash)>();
        //private Stack<(Page page, string hash)> forwardHashStack = new Stack<(Page page, string hash)>();
        private Stack<DefaultRenderer> backElementStack = new Stack<DefaultRenderer>();
        private Stack<DefaultRenderer> forwardElementStack = new Stack<DefaultRenderer>();
        bool ignoreNavEventFlag = false;
        int pageCountTrackerForPopToRootAsync = 0;
        //private Queue<DefaultRenderer> ignoreNavEventFlag = new Queue<DefaultRenderer>();
        //private Stack<Page> forwardPageStack = new Stack<Page>();

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

        //signaling doesn't seem to work until child has been inserted into document, so wait for this and then adjust the hash for direct navigation
        protected override void OnChildInsertedBefore(Node newChild, Node referenceChild)
        {
            base.OnChildInsertedBefore(newChild, referenceChild);
            var index = this.Children.IndexOf(newChild);
            if (index - 1 >= 0)
                (this.Children[index - 1] as Element).Style.Display = "none";

            
            this.backElementStack.Push(newChild as DefaultRenderer);
            //this.forwardHashStack.Clear();

            //this runs every time an element is inserted... only needed for very first element, so kind of hacky but works
            //this.NativeView.Document.Window.Call("history.replaceState", null, null, GenerateFullHash());

        }

        protected override void OnChildRemoved(Node child)
        {
            base.OnChildRemoved(child);
            // Need to check if a PopToRootAsync was performed... all that happens is that the second from the beginning child is removed.
            // Either use a hack by comparing id numbers using order... or handle it in OnPopToRootRequested

            if (this.Children.Count > 0)
                (this.Children.Last() as Element).Style.Display = "block";

            // If this is from Form's PopAsync, then we need to pop from the backstack.  If this is coming from a browser hash change, we've already handled the pop
            //if (this.backHashStack.Peek() == child)
            //{
                DefaultRenderer popped = this.backElementStack.Pop();
                this.forwardElementStack.Push(child as DefaultRenderer);

            pageCountTrackerForPopToRootAsync++;
            //}
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
                    // get index of popped element
                    //var index = this.Children.IndexOf(popped);
                    // remove it
                    //this.Element.Navigation.RemovePage(peeked.Element as Page);
                    //this.RemoveChild(peeked);
                    this.ignoreNavEventFlag = true;
                    this.Element.PopAsync();    

                    // hide the popped element
                    //(this.Children[index] as Element).Style.Display = "none";
                    // put it in the forward stack
                    //this.forwardHashStack.Push(popped);
                    // show the element just beneath
                    //(this.Children[index - 1] as Element).Style.Display = "block";
                }
                // case for clicking forward
                else if (lastPageFromForwardStack != null)
                {
                    
                    var popped = this.forwardElementStack.Pop();
                    this.ignoreNavEventFlag = true;
                    //var pageTypeName = popped.Element.GetType().Name;
                    //Page pageInstance = (Page)Activator.CreateInstance(Type.GetType($"{this.ns}.{pageTypeName}, {this.assemblyName}"));
                    this.Element.PushAsync(popped.Element as Page);
                    //            await this.Element.Navigation.PushAsync(pageInstance as Page);
                    //this.InsertBefore(popped, null);


                    // get index of popped element
                    //var index = this.Children.IndexOf(popped);
                    // show the popped element
                    //(this.Children[index] as Element).Style.Display = "block";
                    // put it in the backward stack
                    //this.backHashStack.Push(popped);
                    // hide the element just beneath
                    //(this.Children[index - 1] as Element).Style.Display = "none";
                }
                // case for someone typing in url from scratch with hash
                else
                {

                }
            }
            //    {
            //        // edit the backHashStack (and optionally stick extra in the forward stack)
            //        //(Page page, string hash)? lastPageHashPopped = null;
            //        (string pageId, string hash) lastPageHashPeeked;
            //        while ((lastPageHashPeeked = backHashStack.Peek()).hash != lastPageHashFromBackStack.Value.hash)
            //        {
            //            var lastPageHashPopped = backHashStack.Pop();
            //            //put it in the forward stack
            //            forwardHashStack.Push(lastPageHashPopped);
            //        }
            //        // present page that is current
            //        //HideElement(lastPageHashPeeked.page);

            //    }
            //    // case for clicking forward
            //    else if (lastPageHashFromForwardStack != null)
            //    {
            //        //edit the forwardHashStack
            //        (string pageId, string hash) lastPageHashPopped;
            //        do
            //        {
            //            lastPageHashPopped = forwardHashStack.Pop();
            //            //put it in the backstack
            //            backHashStack.Push(lastPageHashPopped);
            //        }
            //        while (lastPageHashPopped.hash != lastPageHashFromForwardStack.Value.hash && forwardHashStack.Count > 0);
            //        // present page that is current
            //        //HideElement(lastPageHashPopped.page);
            //    }
            //    // case for someone typing in url from scratch with hash!
            //    else if (fullHash.Length > 0)
            //    {
            //        foreach (var hash in splitHash.Where(x => !string.IsNullOrWhiteSpace(x)))
            //        {
            //            var pageTypeName = WebUtility.HtmlDecode(hash);
            //            var pageInstance = Activator.CreateInstance(Type.GetType($"{this.ns}.{pageTypeName}, {this.assemblyName}"));
            //            await this.Element.Navigation.PushAsync(pageInstance as Page);
            //        }
            //    }

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

            //stick first page in navigation stack with # path (not to be displayed)
            //this.backHashStack.Push((reversedNavStack.First(), "#"));
            
            // Add any pages except the first one.  The first page will always be root (and preloaded) and have no special path.
            foreach (var page in reversedNavStack.Except(new Page[] { reversedNavStack.First() }))
            {
                this.ProcessHashStateForwardNav(page);
            }           

            // If there are more pages than root, show the last page in the stack instead of root.
            if (reversedNavStack.Count() > 1)
            {
                HideElement(reversedNavStack.Last());
            }
        }

        // For navigation Via Xamarin.Forms
        private void ProcessHashStateForwardNav(Page page)
        {
            this.forwardElementStack.Clear();
            //this.backHashStack.Push((page, WebUtility.HtmlEncode(page.GetType().Name)));
            this.NativeView.Document.Window.Call("history.pushState", null, null,  GenerateFullHash());
        }

        private void HideElement(Page page)
        {
            if (this.NativeView.Children.Count > 1)
            {
                // Remove the first child of the div representing our navigation page.
                var child = this.NativeView.Children.First();
                (child as Element).Style.Display = "none";
            }
            // Element has already been added as a result of the pushasync from xamarin.forms and how the default Node handles children

            // Generate the Ooui element and add the new page
            //this.NativeView.AppendChild(page.GetOouiElement());
        }

        // This is where you would normally set back button state based on what's in the stack.
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Not needed for browser... back button is always available.
        }

        private void OnPopToRootRequested(object sender, NavigationRequestedEventArgs e)
        {
            
            //remove any other children that weren't removed (only 2nd child from root is removed)
            //var count = 1;
            //for (var i=this.Children.Count -1; i>0; i--)
            //{
            //    count++;
            //    this.RemoveChild(this.Children[i]);
            //}
            e.Realize = true;
            this.NativeView.Document.Window.Call("history.go", $"-{this.pageCountTrackerForPopToRootAsync}");
            this.pageCountTrackerForPopToRootAsync = 0;
        }

        private void OnPopRequested(object sender, NavigationRequestedEventArgs e)
        {
            //var element = e.Page.GetOouiElement();
            this.pageCountTrackerForPopToRootAsync = 0;
            if (this.ignoreNavEventFlag)
            {
                this.ignoreNavEventFlag = false;
            }
            else
            {
                this.NativeView.Document.Window.Call("history.back");
            }
            e.Realize = true;
        }

        // This is where you would draw the new contents.
        private void OnPushRequested(object sender, NavigationRequestedEventArgs e)
        {
            if (this.ignoreNavEventFlag)
            {
                this.ignoreNavEventFlag = false;
            }
            else
            {
                this.forwardElementStack.Clear();
                this.NativeView.Document.Window.Call("history.pushState", null, null, GenerateFullHash());
            }
            e.Realize = true;
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
