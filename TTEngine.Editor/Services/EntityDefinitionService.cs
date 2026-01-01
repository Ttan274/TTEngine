using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models;

namespace TTEngine.Editor.Services
{
    public class EntityDefinitionService
    {
        private const string DEF_NAME = "entity_def.json";

        public static List<EntityDefinitionModel> Load()
        {
            var path = EditorPaths.GetDataFolder();
            var defLocation = Path.Combine(path, DEF_NAME);

            string json = File.ReadAllText(defLocation);
            return JsonSerializer.Deserialize<List<EntityDefinitionModel>>(json);
        }

        public static void Save(List<EntityDefinitionModel> data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var path = EditorPaths.GetDataFolder();
            var defLocation = Path.Combine(path, DEF_NAME);

            File.WriteAllText(defLocation, json);
        }
    }
}
