using System;
using Xamarin.Forms;

namespace ButtonXaml
{
    public partial class ButtonXamlPage
    {
        int count = 0;

        public ButtonXamlPage()
        {
            InitializeComponent();
        }

        public void OnButtonClicked(object sender, EventArgs args)
        {
            count++;
            LabelCount.Text = $"Click Count: {count}";
           
        }
    }
}
