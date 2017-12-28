using Ooui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Samples
{
    public class SearchBarSample : ISample
    {
        private Xamarin.Forms.Label _resultsLabel;
        
        public string Title => "Xamarin.Forms SearchBar";

        public Ooui.Element CreateElement()
        {
            var panel = new StackLayout();

            var titleLabel = new Xamarin.Forms.Label
            {
                Text = "SearchBar",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
            };
            panel.Children.Add(titleLabel);

            SearchBar searchBar = new SearchBar
            {
                Placeholder = "Xamarin.Forms Property",
            };

            searchBar.SearchButtonPressed += OnSearchBarButtonPressed;

            panel.Children.Add(searchBar);

            _resultsLabel = new Xamarin.Forms.Label();
            panel.Children.Add(_resultsLabel);

            var page = new ContentPage
            {
                Content = panel
            };

            return page.GetOouiElement();
        }

        void OnSearchBarButtonPressed(object sender, EventArgs args)
        {
            // Get the search text.
            SearchBar searchBar = (SearchBar)sender;
            string searchText = searchBar.Text;

            // Create a List and initialize the results Label.
            var list = new List<Tuple<Type, Type>>();
            _resultsLabel.Text = string.Empty;

            // Get Xamarin.Forms assembly.
            Assembly xamarinFormsAssembly = typeof(View).GetTypeInfo().Assembly;

            // Loop through all the types.
            foreach (Type type in xamarinFormsAssembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();

                // Public types only.
                if (typeInfo.IsPublic)
                {
                    // Loop through the properties.
                    foreach (PropertyInfo property in typeInfo.DeclaredProperties)
                    {
                        // Check for a match
                        if (property.Name.Equals(searchText))
                        {
                            // Add it to the list.
                            list.Add(Tuple.Create<Type, Type>(type, property.PropertyType));
                        }
                    }
                }
            }

            if (list.Count == 0)
            {
                _resultsLabel.Text =
                    String.Format("No Xamarin.Forms properties with " +
                                  "the name of {0} were found",
                                  searchText);
            }
            else
            {
                _resultsLabel.Text = "The ";

                foreach (Tuple<Type, Type> tuple in list)
                {
                    _resultsLabel.Text +=
                        String.Format("{0} type defines a property named {1} of type {2}",
                                      tuple.Item1.Name, searchText, tuple.Item2.Name);

                    if (tuple != list.Last())
                    {
                        _resultsLabel.Text += "; and the ";
                    }
                }

                _resultsLabel.Text += ".";
            }
        }

        public void Publish()
        {
            UI.Publish("/searchbar", CreateElement);
        }
    }
}
