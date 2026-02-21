using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services.Asset;

namespace TTEngine.Editor.Services
{
    public static class EditorLaunchService
    {
        public static void Launch(ProjectSession session)
        {
            var services = new ServiceCollection();

            ConfigureServices(services, session);

            var provider = services.BuildServiceProvider();

            var editor = provider.GetRequiredService<MainWindow>();

            editor.Show();

            Application.Current.MainWindow = editor;
        }

        private static void ConfigureServices(IServiceCollection services, ProjectSession session)
        {
            services.AddSingleton(session); 

            //Console
            services.AddSingleton<EditorConsole>();

            //Services
            services.AddSingleton<MapService>();
           // services.AddSingleton<LevelService>();
            services.AddSingleton<AnimationService>(
                _ => new AnimationService(session.AnimPath));

            //Repositories
            services.AddSingleton<JsonRepository<EntityDefinitionModel>>(
                _ => new JsonRepository<EntityDefinitionModel>(session.EntityDefsPath));

            services.AddSingleton<JsonRepository<TileDefinition>>(
                _ => new JsonRepository<TileDefinition>(session.TileDefsPath));

            services.AddSingleton<JsonRepository<InteractableDefinition>>(
                _ => new JsonRepository<InteractableDefinition>(session.InteractableDefsPath));

            services.AddSingleton<JsonRepository<TrapDefinition>>(
                _ => new JsonRepository<TrapDefinition>(session.TrapDefsPath));

            //Catalog
            services.AddSingleton<DefinitionCatalog>();

            //States
            services.AddSingleton<ToolState>();
            services.AddSingleton<PlacementState>();
            services.AddSingleton<LayerState>();
            services.AddSingleton<MapSessionState>();

            //Assets
            services.AddSingleton<AssetFileService>();
            services.AddSingleton<AssetPanel>();
            //services.AddSingleton<InspectorPanel>();

            //Root State
            services.AddSingleton<EditorState>();

            //Window
            services.AddSingleton<MainWindow>();
        }
    }
}
