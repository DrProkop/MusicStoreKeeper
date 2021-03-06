﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MusicStoreKeeper.Vmv.View.Converters
{
    internal class GenresAndStylesConverter : IValueConverter
    {
        /// <summary>Converts lists of album genres and styles to a formatted string. </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is bool inCollection)
            {
                if (!inCollection)
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            if (value is List<string> stylesList)
            {
                if (!stylesList.Any()) return DependencyProperty.UnsetValue;

                StringBuilder builder = new StringBuilder();
                foreach (var style in stylesList)
                {
                    builder.AppendFormat("[{0}] ", style);
                }
                return builder.ToString();
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>Converts a value. </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}