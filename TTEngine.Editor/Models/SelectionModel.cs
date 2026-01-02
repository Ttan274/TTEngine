using System.Windows;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Entity;

namespace TTEngine.Editor.Models
{
    public class SelectionModel
    {
        public SelectionType Type { get; set; } = SelectionType.None;

        public int TileX { get; set; }
        public int TileY { get; set; }
        public PlayerSpawnModel PlayerSpawnModel { get; set; }
        public EnemySpawnModel EnemySpawnModel { get; set; }
    }
}
