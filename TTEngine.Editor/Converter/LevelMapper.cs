using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Level;

namespace TTEngine.Editor.Converter
{
    public static class LevelMapper
    {
        public static LevelFileDto ToDto(IEnumerable<LevelDefinition> levels)
        {
            return new LevelFileDto
            {
                Levels = levels.Select(l => new LevelDto
                {
                    Id = l.Id,
                    MapId = l.MapId,
                    IsActive = l.IsActive
                }).ToList()
            };
        }

        public static List<LevelDefinition> FromDto(LevelFileDto dto)
        {
            if(dto?.Levels == null)
                return new List<LevelDefinition>();

            return dto.Levels.Select(l => new LevelDefinition
            {
                Id = l.Id,
                MapId = l.MapId,
                IsActive = l.IsActive
            }).ToList();
        }
    }
}
