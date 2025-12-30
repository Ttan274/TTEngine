using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models;

namespace TTEngine.Editor.Services
{
    public class EntityDefinitionService
    {
        public static List<EntityDefinitionModel> Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<EntityDefinitionModel>>(json);
        }
    }
}
