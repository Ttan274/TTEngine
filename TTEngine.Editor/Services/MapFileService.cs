using System.IO;
using System.Text.Json;
using TTEngine.Editor.Dtos;

namespace TTEngine.Editor.Services
{
    public static class MapFileService
    {
        public static void Save(string path, TileMapData data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json); 
        }

        public static TileMapData Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<TileMapData>(json);
        }
    }
}
