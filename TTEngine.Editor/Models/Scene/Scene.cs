namespace TTEngine.Editor.Models.Scene
{
    public class Scene
    {
        public string Id { get; set; }
        public MapData Map { get; set; }
        public SpawnData Spawns { get; set; }
        public bool IsActive { get; set; }
    }
}
