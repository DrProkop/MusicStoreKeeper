using System;
using System.Windows;
using System.Windows.Threading;
using ControlzEx.Theming;
using Fluent;
using Fluent.Theming;

namespace MusicStoreKeeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //var source = new Uri("pack://application:,,,/ResourceLibrary;component/DarkTeal.xaml");
            //var newTheme=new Theme(new LibraryTheme(source,RibbonLibraryThemeProvider.DefaultInstance));
            //ThemeManager.Current.AddTheme(newTheme);
            //var theme = ThemeManager.Current.DetectTheme(Application.Current);
            ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.GetTheme("Dark.Teal"));
            base.OnStartup(e);
            Start.Configure();
            Start.Run();
        }

        private void MusicStoreKeeper_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception just occurred: {e.Exception.Message} ", "Something is really wrong", MessageBoxButton.OK, MessageBoxImage.Warning);
            //e.Handled = true;
        }
    }
}
