namespace TTEngine.Editor.Models.Scene
{
    public class Scene
    {
        public string Id { get; set; }
        public MapData Map { get; set; }
        public List<SceneObjectData> SceneObjects { get; set; } = new();
        public bool IsActive { get; set; }
    }

    public class SceneObjectData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string PrefabId { get; set; }
        public string InstanceId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
