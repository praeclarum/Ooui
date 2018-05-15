using OouiWXF.Infrastructure.MVVMFramework.ViewModel;
using System.Windows.Input;
using Xamarin.Forms;

//
//  2018-05-01  Mark Stega
//              Created
//

namespace OouiWXF.ViewModels.MainPage
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            pChangeColorsButtonClickCount = 0;
            pEnableChangeColorsCommand = true;
            pChangeColorsNarrative = "pEnableChangeColorCommand is " + pEnableChangeColorsCommand.ToString() + " before any user interaction.";
            ToggleChangeColorsButtonCommand = new Command(ToggleChangeColorsButtonClicked);
            ChangeColorsButtonCommand = new Command(ChangeColorsButtonClicked);
            pColor0 = Color.Red;
            pColor1 = Color.Green;
            pColor2 = Color.Blue;
        }

        public void ToggleChangeColorsButtonClicked(object parameter)
        {
            pChangeColorsButtonClickCount++;
            pEnableChangeColorsCommand = !pEnableChangeColorsCommand;
            pChangeColorsNarrative = "pEnableChangeColorCommand is " + pEnableChangeColorsCommand.ToString() + " after " + pChangeColorsButtonClickCount.ToString() + " clicks.";
        }

        public ICommand ToggleChangeColorsButtonCommand
        {
            get;
            private set;
        }

        public void ChangeColorsButtonClicked(object parameter)
        {
            if (pColor0 == Color.Red)
            {
                pColor0 = Color.Green;
                pColor1 = Color.Blue;
                pColor2 = Color.Red;
            }
            else if (pColor0 == Color.Green)
            {
                pColor0 = Color.Blue;
                pColor1 = Color.Red;
                pColor2 = Color.Green;
            }
            else
            {
                pColor0 = Color.Red;
                pColor1 = Color.Green;
                pColor2 = Color.Blue;
            }
        }

        public ICommand ChangeColorsButtonCommand
        {
            get;
            private set;
        }

        public int pChangeColorsButtonClickCount
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public bool pEnableChangeColorsCommand
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        public Color pColor0
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }

        public Color pColor1
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }

        public Color pColor2
        {
            get { return GetValue<Color>(); }
            set { SetValue(value); }
        }

        public string pChangeColorsNarrative
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

    }
}
