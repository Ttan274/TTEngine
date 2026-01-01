using System.IO;
using System.Text.Json;
using TTEngine.Editor.Dtos;

namespace TTEngine.Editor.Services
{
    public static class MapFileService
    {
        private const string MAP_NAME = "active_map.json";

        public static void Save(TileMapData data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var path = EditorPaths.GetMapsFolder();
            var mapLocation = Path.Combine(path, MAP_NAME);

            File.WriteAllText(mapLocation, json); 
        }

        public static TileMapData Load()
        {
            var path = EditorPaths.GetMapsFolder();
            var mapLocation = Path.Combine(path, MAP_NAME);

            string json = File.ReadAllText(mapLocation);
            return JsonSerializer.Deserialize<TileMapData>(json);
        }
    }
}
