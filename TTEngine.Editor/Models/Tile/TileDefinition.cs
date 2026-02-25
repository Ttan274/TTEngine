namespace TTEngine.Editor.Models.Tile
{
    public enum CollisionType
    {
        None = 0,
        Ground,
        Wall
    }

    public class TileDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpritePath { get; set; }
        public CollisionType CollisionType { get; set; }
    }
}
