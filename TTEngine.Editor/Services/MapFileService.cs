using System.IO;
using System.Text.Json;
using System.Windows;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Services
{
    public static class MapFileService
    {
        public static void Save(string mapId, TileMapData data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetMapPath(mapId), json); 
        }

        public static TileMapData Load(string mapId)
        {
            var path = GetMapPath(mapId);

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<TileMapData>(json);
        }

        public static void Delete(string mapId)
        {
            var path = GetMapPath(mapId);
            if (File.Exists(path))
                File.Delete(path);
        }

        public static bool Exists(string mapId) => File.Exists(GetMapPath(mapId));

        public static string GetMapPath(string mapId)
        {
            var path = EditorPaths.GetMapsFolder();
            return Path.Combine(path, $"{mapId}.json");
        }

        //Helpers
        public static TileMapData ToDto(TileMapModel model)
        {
            TileMapData data = new TileMapData
            {
                Width = model.Width,
                Height = model.Height,
                TileSize = model.TileSize,
                Layers = model.Layers,
                PlayerSpawn = model.PlayerSpawn == null
                ? null
                : new SpawnDto
                {
                    X = model.PlayerSpawn.Position.X,
                    Y = model.PlayerSpawn.Position.Y,
                    DefinitionId = "Player"
                },
                EnemySpawns = model.EnemySpawns == null
                ? new List<SpawnDto>()
                : model.EnemySpawns.Select(p => new SpawnDto
                {
                    X = p.Position.X,
                    Y = p.Position.Y,
                    DefinitionId = p.DefinitionId,
                }).ToList()
            };

            return data;
        }

        public static TileMapModel FromDto(TileMapData data)
        {
            TileMapModel model = new TileMapModel
            {
                Width = data.Width,
                Height = data.Height,
                TileSize = data.TileSize,
                Layers = data.Layers,
                PlayerSpawn = null
            };

            if(data.PlayerSpawn != null)
            {
                model.PlayerSpawn = new PlayerSpawnModel
                {
                    Position = new Point(data.PlayerSpawn.X, data.PlayerSpawn.Y),
                    DefinitionId = data.PlayerSpawn.DefinitionId
                };
            }

            model.EnemySpawns.Clear();

            if(data.EnemySpawns != null)
            {
                foreach (var sp in data.EnemySpawns)
                {
                    model.EnemySpawns.Add(new EnemySpawnModel
                    {
                        Position = new Point(sp.X, sp.Y),
                        DefinitionId = sp.DefinitionId,
                    });
                }
            }

            return model;
        }
    }
}
