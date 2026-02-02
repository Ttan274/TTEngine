using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;

namespace TTEngine.Editor.Models.Tile
{
    public class TileMapModel
    {
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 30;
        public int TileSize { get; set; } = 50;
        public Dictionary<MapLayerType, int[]> Layers { get; set; }
        public PlayerSpawnModel PlayerSpawn { get; set; }
        public List<EnemySpawnModel> EnemySpawns { get; set; } = new();
        public List<InteractableModel> Interactables { get; set; } = new();

        public void Init()
        {
            Layers = new Dictionary<MapLayerType, int[]>
            {
                {MapLayerType.Background, CreateEmptyLayer() },
                {MapLayerType.Collision, CreateEmptyLayer() },
                {MapLayerType.Decoration, CreateEmptyLayer() },
                {MapLayerType.Interactable, CreateEmptyLayer() }
            };
        }

        private int[] CreateEmptyLayer() => new int[Width * Height];

        public int GetIndex(int x, int y) => (y * Width + x);
    }
}
