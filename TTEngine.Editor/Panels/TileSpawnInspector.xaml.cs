using System.Windows.Controls;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TileSpawnInspector.xaml
    /// </summary>
    public partial class TileSpawnInspector : UserControl
    {
        public TileSpawnInspector(int x, int y, int tileType)
        {
            InitializeComponent();

            TileInfo.Text = $"({x},{y})\nType: {(TileType)tileType}";
        }
    }
}
