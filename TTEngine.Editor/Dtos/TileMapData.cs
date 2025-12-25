namespace TTEngine.Editor.Dtos
{
    public class TileMapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public int[] Tiles { get; set; }
        public double PlayerSpawnX { get; set; }
        public double PlayerSpawnY { get; set; }
    }
}
