using Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicStoreKeeper.Vmv.View.Converters
{
    /// <summary>
    /// Returns icon image sources for different file types.
    /// </summary>
    public class TreeViewIconConverter : IValueConverter
    {
        /// <summary>Converts a value. </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ISimpleFileInfo item)
            {
                if (item is DummySimpleFileInfo)
                {
                    return DependencyProperty.UnsetValue;
                }

                switch (item.Type)
                {
                    case SfiType.Directory:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/close_folder_icon.png";
                    case SfiType.AudioFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/audio_file_icon.png";
                    case SfiType.ImageFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/image_file_icon.png";
                    case SfiType.TextFile:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/document_icon.png";
                    case SfiType.Unknown:
                        return "pack://application:,,,/ResourceLibrary;component/Images/TreeViewIcons/document_blank_icon.png";
                }
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
            return DependencyProperty.UnsetValue;
        }
    }
}