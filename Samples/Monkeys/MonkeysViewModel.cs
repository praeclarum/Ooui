using Monkeys.Helpers;
using Monkeys.Models;
using System.Collections.ObjectModel;

namespace Monkeys.ViewModels
{
    public class MonkeysViewModel
    {
        public ObservableCollection<Monkey> Monkeys { get; set; }

        public MonkeysViewModel()
        {
            Monkeys = MonkeyHelper.Monkeys;
        }
    }
}
