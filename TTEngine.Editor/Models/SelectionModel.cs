using System.Windows;
using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Models
{
    public class SelectionModel
    {
        public SelectionType Type { get; set; } = SelectionType.None;

        public int TileX { get; set; }
        public int TileY { get; set; }

        public Point Spawn {  get; set; }
    }
}
