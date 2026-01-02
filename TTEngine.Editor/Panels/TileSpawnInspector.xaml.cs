using System.Windows.Controls;
using TTEngine.Editor.Enums;

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

            TileInfo.Text = $"({x},{y})\nType: {(CollisionType)tileType}";
        }
    }
}
