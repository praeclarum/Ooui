using Monkeys.ViewModels;
using Xamarin.Forms;

namespace Monkeys.Views
{
    public partial class MonkeysView : ContentPage
    {
        public MonkeysView()
        {
            InitializeComponent();

            BindingContext = new MonkeysViewModel();
        }
    }
}
