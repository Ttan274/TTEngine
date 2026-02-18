using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TileManagementPanel.xaml
    /// </summary>
    public partial class TileManagementPanel : UserControl
    {
        private EditorState Editor;

        public TileManagementPanel(EditorState editor)
        {
            InitializeComponent();
            Editor = editor;
            DataContext = editor;

            Loaded += (_, __) =>
            {
                if (Editor != null)
                {
                    Editor.PropertyChanged += (_, e) =>
                    {
                        if (e.PropertyName == nameof(Editor.Placement.SelectedTile))
                        {
                            UpdatePreview(Editor.Placement.SelectedTile.SpritePath);
                        }
                    };
                }
            };
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int nextId = Editor.Definition.TileDefinitions.Count == 0
                 ? 1
                 : Editor.Definition.TileDefinitions.Max(t => t.Id) + 1;

            var tile = new TileDefinition
            {
                Id = nextId,
                Name = $"NewTile_{nextId}",
                SpritePath = "",
                CollisionType = Enums.CollisionType.None
            };

            Editor.Definition.TileDefinitions.Add(tile);
            Editor.Placement.SelectedTile = tile;

            SaveAll();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.Placement.SelectedTile == null)
                return;

            if(Editor.Placement.SelectedTile.Name == "Empty")
            {
                MessageBox.Show("You cannot delete default tile");
                return;
            }

            Editor.Definition.TileDefinitions.Remove(Editor.Placement.SelectedTile);
            Editor.Placement.SelectedTile = null;

            SaveAll();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveAll();
        }

        private void SaveAll()
            => Editor.Definition.TileRepository.SaveAll(Editor.Definition.TileDefinitions.ToList());

        private void BrowseSprite_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.Placement.SelectedTile == null)
                return;

            var file = BrowseTextureFile();
            if(file != null)
            {
                Editor.Placement.SelectedTile.SpritePath = file;
                UpdatePreview(file);
            }
        }

        private string BrowseTextureFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                InitialDirectory = EditorPaths.GetTextureFolder()
            };

            if (dialog.ShowDialog() != true)
                return null;

            return Path.GetFileName(dialog.FileName);
        }

        private void UpdatePreview(string fileName)
        {
            try
            {
                string targetPath = Path.Combine(EditorPaths.GetTextureFolder(), fileName);
            
                if(!File.Exists(targetPath))
                {
                    SpritePreview.Source = null;
                    SpritePathTextBox.Text = "";
                    return;
                }

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(targetPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                SpritePathTextBox.Text = fileName;
                SpritePreview.Source = bmp;
            }
            catch 
            {
                SpritePreview.Source = null;
            }
        }
    }
}
