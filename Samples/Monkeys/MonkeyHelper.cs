using System.Collections.ObjectModel;
using Monkeys.Models;
using Xamarin.Forms;

namespace Monkeys.Helpers
{
    public static class MonkeyHelper
    {
        public static ObservableCollection<Monkey> Monkeys { get; set; }

        static MonkeyHelper()
        {
            Monkeys = new ObservableCollection<Monkey>
            {
                new Monkey
                {
                    Name = "Baboon",
                    Location = "Africa & Asia",
                    Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.Baboon.jpg", System.Reflection.Assembly.GetCallingAssembly())
                },
                new Monkey
                {
                    Name = "Capuchin Monkey",
                    Location = "Central & South America",
                    Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.Capuchin.jpg", System.Reflection.Assembly.GetCallingAssembly())
                },
                new Monkey
                {
                    Name = "Blue Monkey",
                    Location = "Central and East Africa",
                    Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.BlueMonkey.jpg", System.Reflection.Assembly.GetCallingAssembly())
                },
                new Monkey
                {
                    Name = "Squirrel Monkey",
                    Location = "Central & South America",
                    Details = "The squirrel monkeys are the New World monkeys of the genus Saimiri. They are the only genus in the subfamily Saimirinae. The name of the genus Saimiri is of Tupi origin, and was also used as an English name by early researchers.",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.Squirrel.jpg", System.Reflection.Assembly.GetCallingAssembly())
                },
                new Monkey
                {
                    Name = "Golden Lion Tamarin",
                    Location = "Brazil",
                    Details = "The golden lion tamarin also known as the golden marmoset, is a small New World monkey of the family Callitrichidae.",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.GoldenLionTamarin.jpg", System.Reflection.Assembly.GetCallingAssembly())
                },
                new Monkey
                {
                    Name = "Howler Monkey",
                    Location = "South America",
                    Details = "Howler monkeys are among the largest of the New World monkeys. Fifteen species are currently recognised. Previously classified in the family Cebidae, they are now placed in the family Atelidae.",
                    Image = ImageSource.FromResource("Samples.Monkeys.Images.Howler.jpg", System.Reflection.Assembly.GetCallingAssembly())
                }
            };
        }
    }
}
