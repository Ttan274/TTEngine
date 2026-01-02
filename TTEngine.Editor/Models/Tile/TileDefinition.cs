using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Models.Tile
{
    public class TileDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SpritePath { get; set; }
        public CollisionType CollisionType { get; set; }
    }
}
