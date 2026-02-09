using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
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
                Editor.SelectedTile = Editor.TileDefinitions.First(t => t.Id == tileId);
            }
        }
        private void TileButtonLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is int tileId && Editor != null)
            {
                UpdateTileBtnVisual(border);
                Editor.PropertyChanged += (_, __) => UpdateTileBtnVisual(border);
            }
        }

        private void UpdateTileBtnVisual(Border border)
        {
            if (border.Tag is int tileId && Editor != null)
            {
                bool isSelected = Editor.SelectedTile?.Id == tileId;
                UpdateBtnVisual(border, isSelected);
            }
        }
        private void TileImageLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.DataContext is TileDefinition tile && !string.IsNullOrEmpty(tile.SpritePath))
            {
                LoadImage(img, tile.SpritePath);
            }
        }
        #endregion

        #region Interactable Methods

        private void OnInteractableClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is string interactableId && Editor != null)
            {
                Editor.SelectedInteractable = Editor.InteractableDefinitions.First(t => t.Id == interactableId);
            }
        }

        private void InteractableButtonLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is string interactableId && Editor != null)
            {
                UpdateInteractableBtnVisual(border);
                Editor.PropertyChanged += (_, __) => UpdateInteractableBtnVisual(border);
            }
        }

        private void UpdateInteractableBtnVisual(Border border)
        {
            if (border.Tag is string interactableId && Editor != null)
            {
                bool isSelected = Editor.SelectedInteractable?.Id == interactableId;
                UpdateBtnVisual(border, isSelected);
            }
        }

        private void InteractableImageLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.DataContext is InteractableDefinition def && !string.IsNullOrEmpty(def.ImagePath))
            {
                LoadImage(img, def.ImagePath);
            }
        }

        #endregion

        #region Trap Methods
        private void OnTrapClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is string trapId && Editor != null)
            {
                Editor.SelectedTrap = Editor.TrapDefinitions.First(t => t.Id == trapId);
            }
        }

        private void TrapButtonLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is string trapId && Editor != null)
            {
                UpdateTrapBtnVisual(border);
                Editor.PropertyChanged += (_, __) => UpdateTrapBtnVisual(border);
            }
        }

        private void UpdateTrapBtnVisual(Border border)
        {
            if (border.Tag is string trapId && Editor != null)
            {
                bool isSelected = Editor.SelectedTrap?.Id == trapId;
                UpdateBtnVisual(border, isSelected);
            }
        }

        private void TrapImageLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.DataContext is TrapDefinition def && !string.IsNullOrEmpty(def.ImagePath))
            {
                LoadImage(img, def.ImagePath);
            }
        }
        #endregion

        //Helpers
        private void UpdateBtnVisual(Border border, bool status)
        {
            border.BorderBrush = status
                                ? Brushes.Gold
                                : Brushes.DimGray;

            border.BorderThickness = status
                                ? new Thickness(2)
                                : new Thickness(1);
        }

        private void LoadImage(Image img, string fileName)
        {
            string path = Path.Combine(EditorPaths.GetTextureFolder(), fileName);

            if (!File.Exists(path))
                return;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();

            img.Source = bmp;
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
    }
}
