using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Motix_v2.Presentation.WinUI.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            value is bool b && b
                ? Visibility.Visible
                : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            value is Visibility v && v == Visibility.Visible;
    }
}
