using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using GTEventMaker.Database;

using Microsoft.Extensions.DependencyInjection;

namespace GTEventMaker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public static GameDB GameDatabase { get; set; }
        public static Random Random = new Random();

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            if (!ConfigureServices(services))
                return;

            serviceProvider = services.BuildServiceProvider();
        }

        private bool ConfigureServices(ServiceCollection services)
        {
            GameDatabase = new GameDB(Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.db"));
            if (!GameDatabase.CreateConnection())
            {
                MessageBox.Show("Could not connect to local database (data.db).");
                return false;
            }

            services.AddSingleton<GameMakerWindow>();
            return true;
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = serviceProvider.GetService<GameMakerWindow>();
            mainWindow.Show();
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An error occured in the editor, please report to the creator:\n {e.Exception?.InnerException ?? e.Exception}");
            e.Handled = true;
        }
    }
}
