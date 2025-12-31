using System.Windows;

namespace TTEngine.Editor.Models
{
    public class EnemySpawnModel
    {
        public Point Position { get; set; }
        public string DefinitionId { get; set; }
    }

    public class PlayerSpawnModel
    {
        public Point Position { get; set; }
        public string DefinitionId { get; set; } = "Player";
    }
}
