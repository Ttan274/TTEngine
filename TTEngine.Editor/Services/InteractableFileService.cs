using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models.Interactable;

namespace TTEngine.Editor.Services
{
    public static class InteractableFileService
    {
        private static string FILE_NAME = "Interactables.json";

        public static void Save(List<InteractableDefinition> interactables)
        {
            var json = JsonSerializer.Serialize(interactables, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetPath(), json);
        }

        public static List<InteractableDefinition> Load()
        {
            var path = GetPath();
            if(!File.Exists(path))
                return new List<InteractableDefinition>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<InteractableDefinition>>(json);
        }

        private static string GetPath()
        {
            var path = EditorPaths.GetDataFolder();
            return Path.Combine(path, FILE_NAME);
        }
    }
}
