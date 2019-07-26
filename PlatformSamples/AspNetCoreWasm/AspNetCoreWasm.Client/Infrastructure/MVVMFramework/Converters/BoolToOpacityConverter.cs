using System;
using System.Globalization;
using Xamarin.Forms;

//
//  2018-05-01  Mark Stega
//              Created
//

namespace OouiWXF.Infrastructure.MVVMFramework.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0.4;
            }
            if (value.GetType() != typeof(bool))
            {
                return 0.4;
            }
            return ((bool)value) ? 1.0 : 0.4;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            return value;
        }
    }
}
