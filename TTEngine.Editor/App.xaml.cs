using System.Windows;

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
            base.OnStartup(e);

            var projectHub = new ProjectHubWindow();
            MainWindow = projectHub;
            projectHub.Show();
        }
    }
}
