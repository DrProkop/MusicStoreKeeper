using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vmv.View
{
    public static class AttachedProps
    {
        #region [  Expanding property  ]

        public static readonly DependencyProperty ExpandingPropertyProperty = DependencyProperty.RegisterAttached(
            "ExpandingProperty",
            typeof(ICommand),
            typeof(AttachedProps),
            new PropertyMetadata(OnExpandingPropertyChanged));

        public static void SetExpandingProperty(DependencyObject o, ICommand value)
        {
            o.SetValue(ExpandingPropertyProperty, value);
        }

        public static ICommand GetExpandingProperty(DependencyObject o)
        {
            return (ICommand) o.GetValue(ExpandingPropertyProperty);
        }

        private static void OnExpandingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tvi = d as TreeViewItem;
            if (tvi == null) return;
            var command = e.NewValue as ICommand;
            if (command==null)return;
            tvi.Expanded += (s, a) =>
            {
                if (command.CanExecute(a))
                {
                    command.Execute(a);
                }
            };

        }

        #endregion
    }
}
