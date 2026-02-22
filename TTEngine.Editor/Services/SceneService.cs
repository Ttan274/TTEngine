using System.IO;
using TTEngine.Editor.Models.Scene;

namespace TTEngine.Editor.Services
{
    public class SceneService
    {
        public const string DEFAULT_SCENE_NAME = "DefaultScene";

        private readonly string _sceneFolder;

        public SceneService(string sceneFolder)
        {
            _sceneFolder = sceneFolder;

            string defPath = GetScenePath(DEFAULT_SCENE_NAME);

            if (!File.Exists(defPath))
                CreateScene(DEFAULT_SCENE_NAME, 50, 30, 50);
        }

        private string GetScenePath(string sceneId)
            => Path.Combine(_sceneFolder, $"{sceneId}.json");

        public void Save(string sceneId, Scene scene)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            JsonFileService.Save(GetScenePath(sceneId), scene);
        }

        public Scene Load(string sceneId)
        {
            string path = GetScenePath(sceneId);

            if (!File.Exists(path))
                throw new FileNotFoundException($"Scene {sceneId} not found in this loacation");

            return JsonFileService.Load<Scene>(path);
        }

        public void Delete(string sceneId)
        {
            string path = GetScenePath(sceneId);

            if (File.Exists(path))
                File.Delete(path);
        }

        public List<string> GetAllScenes()
        {
            return Directory.GetFiles(_sceneFolder, "*.json")
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToList();
        }

        //Default Scene Creation
        public Scene CreateScene(string id, int w, int h, int t)
        {
            var scene = new Scene
            {
                Id = id,
                Map = new MapData
                {
                    Width = w,
                    Height = h,
                    TileSize = t,
                    CollisionTiles = CreateEmptyGrid(w, h)
                },
                Spawns = new SpawnData(),
                IsActive = true
            };

            Save(id, scene);
            return scene;
        }

        private List<List<int>> CreateEmptyGrid(int width, int height)
        {
            var grid = new List<List<int>>();
            for (int y = 0; y < height; y++)
            {
                var row = new List<int>();
                for (int x = 0; x < width; x++)
                {
                    row.Add(0); // 0 represents an empty tile
                }
                grid.Add(row);
            }
            return grid;
        }

        public void Rename(string oldId, string newId)
        {
            string oldPath = GetScenePath(oldId);
            string newPath = GetScenePath(newId);

            if (!File.Exists(oldPath))
                return;

            if (File.Exists(newPath))
                throw new Exception("Scene already exists");

            File.Move(oldPath, newPath);
        }
    }
}
