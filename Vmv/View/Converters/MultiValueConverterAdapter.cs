using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MusicStoreKeeper.Vmv.View.Converters
{
    [ContentProperty("Converter")]
    public class MultiValueConverterAdapter : IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        private object _lastParameter;
        private IValueConverter _lastConverter;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _lastConverter = Converter;
            if (values.Length > 1) _lastParameter = values[1];
            if (values.Length > 2) _lastConverter = (IValueConverter)values[2];
            if (Converter == null) return values[0];
            return Converter.Convert(values[0], targetType, _lastParameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_lastConverter == null) return new object[] { value };
            return new object[] { _lastConverter.ConvertBack(value, targetTypes[0], _lastParameter, culture) };
        }
    }
}