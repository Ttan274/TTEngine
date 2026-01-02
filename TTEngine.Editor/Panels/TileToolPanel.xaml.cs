using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TileToolPanel.xaml
    /// </summary>
    public partial class TileToolPanel : UserControl
    {
        public event Action<ToolMode> ToolModeChanged;
        public event Action<int> BrushSizechanged;

        public event Action SaveClicked;
        public event Action LoadClicked;
        public event Action StartGameClicked;

        private EditorState Editor => DataContext as EditorState;

        public TileToolPanel()
        {
            InitializeComponent();
        }

        //Tile Types
        private void OnTileClicked(object sender, RoutedEventArgs e)
        {
            if(sender is Border border && border.Tag is int tileId && Editor != null)
            {
                Editor.SelectedTile = Editor.TileDefinitions.First(t => t.Id == tileId);
            }
        }

        private void TileButtonLoaded(object sender, RoutedEventArgs e)
        {
            if(sender is Border border && border.Tag is int tileId && Editor != null)
            {
                UpdateBtnVisual(border);
                Editor.PropertyChanged += (_, __) => UpdateBtnVisual(border);
            }
        }

        private void UpdateBtnVisual(Border border)
        {
            if(border.Tag is int tileId && Editor != null)
            {
                bool isSelected = Editor.SelectedTile?.Id == tileId;

                border.Background = isSelected 
                               ? new SolidColorBrush(Color.FromArgb(255, 70, 130, 180))
                               : new SolidColorBrush(Color.FromArgb(255, 45, 45, 45));

                border.BorderBrush = isSelected
                                ? Brushes.Gold
                                : Brushes.DimGray;

                border.BorderThickness = isSelected 
                                    ? new Thickness(2) 
                                    : new Thickness(1);
            }
        }

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
