using Autofac;
using Common;
using Discogs;
using FileManager;
using FileAnalyzer;
using MusicStoreKeeper.CollectionManager;
using MusicStoreKeeper.DataModel;
using MusicStoreKeeper.Vmv.View;
using MusicStoreKeeper.Vmv.ViewModel;

namespace AppConfiguration
{
    public class Start
    {
        private static IContainer _container;

        public static void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LoggerManager>().As<ILoggerManager>().SingleInstance();
            builder.RegisterType<FileManager.FileManager>().As<IFileManager>().SingleInstance();
            builder.RegisterType<MusicFileAnalyzer>().As<IMusicFileAnalyzer>();
            builder.RegisterType<MusicDirAnalyzer>().As<IMusicDirAnalyzer>();
            builder.RegisterType<CollectionManager>().As<ICollectionManager>();
            builder.RegisterType<DiscogsClient>();
            builder.RegisterType<Repository>().As<IRepository>();
            builder.RegisterType<PreviewFactory>();
            //Screens
            builder.RegisterType<MainWindowVm>().SingleInstance();
            builder.RegisterType<MusicCollectionScreenVm>();
            builder.RegisterType<MusicSearchScreenVm>();
            builder.RegisterType<SettingsScreenVm>();

            builder.RegisterType<MainWindow>().OnActivated(arg =>
            {
                var context = arg.Context;
                var instance = arg.Instance;
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