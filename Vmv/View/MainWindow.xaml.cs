using System.Windows;

namespace MusicStoreKeeper.Vmv.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var firstTab = sender as Fluent.RibbonTabItem;
            firstTab.IsSelected = true;
        }
    }
}
