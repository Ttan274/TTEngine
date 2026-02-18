using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //States
            services.AddSingleton<ToolState>();
            services.AddSingleton<PlacementState>();
            services.AddSingleton<LayerState>();

            //Root State
            services.AddSingleton<EditorState>();

            //Window
            services.AddSingleton<MainWindow>();
        }
    }
}
