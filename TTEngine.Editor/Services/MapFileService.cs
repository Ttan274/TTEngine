using System.IO;
using System.Text.Json;
using TTEngine.Editor.Dtos;

namespace TTEngine.Editor.Services
{
    public static class MapFileService
    {
        private const string LIVE_MAP_PATH = @"..\..\..\..\Maps\active_map.json";
        public static string GetMapPath() { return LIVE_MAP_PATH; }

        public static void Save(TileMapData data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(LIVE_MAP_PATH, json); 
        }

        public static TileMapData Load()
        {
            string json = File.ReadAllText(LIVE_MAP_PATH);
            return JsonSerializer.Deserialize<TileMapData>(json);
        }
    }
}
