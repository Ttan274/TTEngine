using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TileToolPanel.xaml
    /// </summary>
    public partial class TileToolPanel : UserControl
    {
        public event Action<ToolMode> ToolModeChanged;
        public event Action<TileType> TileTypeChanged;
        public event Action<int> BrushSizechanged;

        public event Action SaveClicked;
        public event Action LoadClicked;
        public event Action StartGameClicked;

        public TileToolPanel()
        {
            InitializeComponent();
        }

        //Tile Types
        private void GroundTile_Checked(object sender, RoutedEventArgs e)
            => TileTypeChanged?.Invoke(TileType.Ground);
        private void WallTile_Checked(object sender, RoutedEventArgs e)
            => TileTypeChanged?.Invoke(TileType.Wall);

        //Brush Size
        private void BrushSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int size = (int)e.NewValue;

            BrushSizeLabel.Text = $"Size: {size}x{size}"; ;

            BrushSizechanged?.Invoke(size);
        }

        //Tool Modes
        private void Brush_Checked(object sender, RoutedEventArgs e)
            => ToolModeChanged?.Invoke(ToolMode.Brush);
        private void Fill_Checked(object sender, RoutedEventArgs e)
            => ToolModeChanged?.Invoke(ToolMode.Fill);
        private void PlayerSpawn_Checked(object sender, RoutedEventArgs e)
            => ToolModeChanged?.Invoke(ToolMode.PlayerSpawn);
        private void EnemySpawn_Checked(object sender, RoutedEventArgs e)
            => ToolModeChanged?.Invoke(ToolMode.EnemySpawn);

        //Button Clicks
        private void SaveMap_Click(object sender, RoutedEventArgs e)
            => SaveClicked?.Invoke();

        private void LoadMap_Click(object sender, RoutedEventArgs e)
            => LoadClicked?.Invoke();

        private void StartGame_Click(object sender, RoutedEventArgs e)
            => StartGameClicked?.Invoke();
    }
}
