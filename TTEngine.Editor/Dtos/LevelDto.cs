namespace TTEngine.Editor.Dtos
{
    public class LevelDto
    {
        public string Id { get; set; }
        public string MapId { get; set; }
        public bool IsActive { get; set; }
    }

    public class LevelFileDto
    {
        public List<LevelDto> Levels { get; set; } = new();
    }
}
