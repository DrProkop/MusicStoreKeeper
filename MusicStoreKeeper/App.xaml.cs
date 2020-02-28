using System;
using System.Windows;
using Fluent;

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
            ThemeManager.AddTheme(new Uri("pack://application:,,,/ResourceLibrary;component/DarkTeal.xaml"));
            var theme = ThemeManager.DetectTheme(Application.Current);
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.GetTheme("Dark.Teal"));
            base.OnStartup(e);;
            Start.Configure();
            Start.Run();
        }
    }
}
