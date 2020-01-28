using Autofac;
using Vmv.View;
using Vmv.ViewModel;

namespace AppConfiguration
{
    public class Start
    {
        private static IContainer _container;
        public static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindowVm>();

            builder.RegisterType<MainWindow>().OnActivated(e =>
            {
                var context = e.Context;
                var instance = e.Instance;
                instance.DataContext = context.Resolve<MainWindowVm>();
            });

            _container = builder.Build();
        }

        public static void Run()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                scope.Resolve<MainWindow>().Show();
            }
        }
    }
}
