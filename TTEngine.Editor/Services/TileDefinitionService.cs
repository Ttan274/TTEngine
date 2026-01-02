using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Services
{
    public static class TileDefinitionService
    {
        private static string FilePath = Path.Combine(EditorPaths.GetDataFolder(), "tile_def.json");
        private static List<TileDefinition> _definitions;
        
        public static IReadOnlyList<TileDefinition> Load()
        {
            if(_definitions != null)
                return _definitions;

            string json = File.ReadAllText(FilePath);

            _definitions = JsonSerializer.Deserialize<List<TileDefinition>>(json,
                new JsonSerializerOptions
                {
                    Converters = {new JsonStringEnumConverter()}
                });
            return _definitions;
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(_definitions,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            File.WriteAllText(FilePath, json);
        }

        public static TileDefinition GetById(int id)
            => _definitions.FirstOrDefault(d => d.Id == id);
    
        public static TileDefinition AddTile()
        {
            int nextId = _definitions.Count == 0
                ? 1
                : _definitions.Max(t => t.Id) + 1;

            var tile = new TileDefinition
            {
                Id = nextId,
                Name = $"NewTile_{nextId}",
                SpritePath = "",
                CollisionType = Enums.CollisionType.None
            };

            _definitions.Add(tile);
            Save();

            return tile;
        }

        public static void RemoveTile(TileDefinition tile)
        {
            _definitions.Remove(tile);
            Save();
        }
    }
}
