using System.IO;
using System.Text.Json;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Level;

namespace TTEngine.Editor.Services
{
    public static class LevelFileService
    {
        private const string LEVEL_FILE_NAME = "Levels.json";

        public static void Save(IEnumerable<LevelDefinition> levels)
        {
            var dto = new LevelFileDto
            {
                Levels = levels.Select(l => new LevelDto
                {
                    Id = l.Id,
                    MapId = l.MapId,
                    IsActive = l.IsActive
                }).ToList()
            };

            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetPath(), json);
        }

        public static List<LevelDefinition> Load()
        {
            var path = GetPath();
            if(!File.Exists(path))
                return new List<LevelDefinition>();

            var json = File.ReadAllText(path);
            var dto = JsonSerializer.Deserialize<LevelFileDto>(json);

            return dto.Levels.Select(l => new LevelDefinition
            {
                Id = l.Id,
                MapId = l.MapId,
                IsActive = l.IsActive
            }).ToList();
        }

        private static string GetPath()
        {
            var path = EditorPaths.GetDataFolder();
            return Path.Combine(path, LEVEL_FILE_NAME);
        }
    }
}
