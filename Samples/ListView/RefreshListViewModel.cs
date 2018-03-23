using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Samples
{
    public class RefreshListViewModel : BindableObject
    {
        public ICommand AddCmd => new Command (() => {
            Data.Add (Input);
            OnPropertyChanged (nameof (Data));
        });

        private string _input;

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged ();
            }
        }

        private ObservableCollection<string> _data = new ObservableCollection<string>();

        public ObservableCollection<string> Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged ();
            }
        }
    }
}
