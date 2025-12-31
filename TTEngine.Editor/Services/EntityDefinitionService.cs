using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models;

namespace TTEngine.Editor.Services
{
    public class EntityDefinitionService
    {
        private const string ENTITY_DEF_PATH = @"..\..\..\..\Data\entity_def.json";

        public static List<EntityDefinitionModel> Load()
        {
            string json = File.ReadAllText(ENTITY_DEF_PATH);
            return JsonSerializer.Deserialize<List<EntityDefinitionModel>>(json);
        }

        public static void Save(List<EntityDefinitionModel> data)
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(ENTITY_DEF_PATH, json);
        }
    }
}
