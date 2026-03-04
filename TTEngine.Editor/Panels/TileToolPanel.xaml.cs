using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Editor.EditorStates;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TileToolPanel.xaml
    /// </summary>
    public partial class TileToolPanel : UserControl
    {
        public event Action<ToolMode> ToolModeChanged;
        public event Action<int> BrushSizechanged;
        public event Action StartGameClicked;

        private EditorState Editor => DataContext as EditorState;

        public TileToolPanel()
        {
            InitializeComponent();
        }

        #region Tile Methods
        private void OnTileClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is int tileId && Editor != null)
            {
                Editor.Placement.SelectedTile = Editor.Definition.TileDefinitions.First(t => t.Id == tileId);
            }
        }
        private void TileButtonLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is int tileId && Editor != null)
            {
                UpdateTileBtnVisual(border);
                Editor.Placement.PropertyChanged += (_, __) => UpdateTileBtnVisual(border);
            }
        }

        private void UpdateTileBtnVisual(Border border)
        {
            if (border.Tag is int tileId && Editor != null)
            {
                bool isSelected = Editor.Placement.SelectedTile?.Id == tileId;

                border.BorderBrush = isSelected
                                ? Brushes.Gold
                                : Brushes.DimGray;

                border.BorderThickness = isSelected
                                    ? new Thickness(2)
                                    : new Thickness(1);
            }
        }

        private void TileImageLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.DataContext is TileDefinition tile && !string.IsNullOrEmpty(tile.SpritePath))
            {
                string path = Path.Combine(EditorPaths.GetTextureFolder(), tile.SpritePath);

                if (!File.Exists(path))
                    return;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                img.Source = bmp;
            }
        }
        #endregion

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

        //Button Clicks
        private void StartGame_Click(object sender, RoutedEventArgs e)
            => StartGameClicked?.Invoke();

        private void BuildGame_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select build output folder",
                CheckFileExists = false,
                FileName = "Select Folder"
            };

            if (dialog.ShowDialog() != true)
                return;

            string selectedFolder = Path.GetDirectoryName(dialog.FileName);

            try
            {
                BuildService.BuildGame(selectedFolder, "TTGame");
                MessageBox.Show("Build completed succesfully! \nZIP package created.", "Build", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Build Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    
        public void SetStartButtonTxt(string text) => RunBtn.Content = text;
    }
}
