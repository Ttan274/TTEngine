using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.GameObject;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Panels;
using TTEngine.Editor.Services.Asset;
using TTEngine.Editor.Services.IO;

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
            //Project Session
            services.AddSingleton(session); 

            //Console
            services.AddSingleton<EditorConsole>();

            //Services
            services.AddSingleton<SceneService>(
                _ => new SceneService(session.ScenePath));

            //Repositories
            services.AddSingleton<JsonRepository<TileDefinition>>(
                _ => new JsonRepository<TileDefinition>(session.TileDefsPath));

            services.AddSingleton<JsonRepository<GameObject>>(
                _ => new JsonRepository<GameObject>(session.GameObjectDefs));

            //Catalog
            services.AddSingleton<DefinitionCatalog>();

            //States
            services.AddSingleton<ToolState>();
            services.AddSingleton<PlacementState>();
            services.AddSingleton<SceneSessionState>();

            //Animation
            services.AddTransient<AnimationPanel>();

            //Assets
            services.AddSingleton<AssetFileService>();
            services.AddSingleton<AssetPanel>();

            //Root State
            services.AddSingleton<EditorState>();

            //Window
            services.AddSingleton<MainWindow>();
        }
    }
}
