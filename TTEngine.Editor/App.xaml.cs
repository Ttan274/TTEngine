using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Services;

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
            //Console
            services.AddSingleton<EditorConsole>();

            //Services
            services.AddSingleton<MapService>();
            services.AddSingleton<LevelService>();
            services.AddSingleton<AnimationService>(
                _ => new AnimationService(EditorPaths.Animation));

            //Repositories
            services.AddSingleton<JsonRepository<EntityDefinitionModel>>(
                _ => new JsonRepository<EntityDefinitionModel>(EditorPaths.EntityDefs));

            services.AddSingleton<JsonRepository<TileDefinition>>(
                _ => new JsonRepository<TileDefinition>(EditorPaths.TileDefs));

            services.AddSingleton<JsonRepository<InteractableDefinition>>(
                _ => new JsonRepository<InteractableDefinition>(EditorPaths.InteractableDefs));

            services.AddSingleton<JsonRepository<TrapDefinition>>(
                _ => new JsonRepository<TrapDefinition>(EditorPaths.TrapDefs));

            //Catalog
            services.AddSingleton<DefinitionCatalog>();

            //States
            services.AddSingleton<ToolState>();
            services.AddSingleton<PlacementState>();
            services.AddSingleton<LayerState>();
            services.AddSingleton<MapSessionState>();

            //Root State
            services.AddSingleton<EditorState>();

            //Window
            services.AddSingleton<MainWindow>();
        }
    }
}
