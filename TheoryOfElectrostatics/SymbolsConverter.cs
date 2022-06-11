using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheoryOfElectrostatics
{
    [ValueConversion(typeof(int), typeof(string))]
    public class SymbolsConverter : IValueConverter
    {
        private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return alphabet[System.Convert.ToInt32(value)].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
