namespace TTEngine.Editor.Models.Scene
{
    public class MapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public List<List<int>> CollisionTiles { get; set; }
    }
}
