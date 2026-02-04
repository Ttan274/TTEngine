using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Enums;
using TTEngine.Editor.Models.Editor;
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
                        if (e.PropertyName == nameof(Editor.SelectedTile))
                        {
                            UpdatePreview(Editor.SelectedTile.SpritePath);
                        }
                    };
                }
            };
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var tile = TileDefinitionService.AddTile();
            Editor.TileDefinitions.Add(tile);
            Editor.SelectedTile = tile;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedTile == null)
                return;

            if(Editor.SelectedTile.Name == "Empty")
            {
                MessageBox.Show("You cannot delete default tile");
                return;
            }

            TileDefinitionService.RemoveTile(Editor.SelectedTile);
            Editor.TileDefinitions.Remove(Editor.SelectedTile);
            Editor.SelectedTile = null;
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedTile == null)
                return;

            TileDefinitionService.Save();
        }

        private void BrowseSprite_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.SelectedTile == null)
                return;

            var file = BrowseTextureFile();
            if(file != null)
            {
                Editor.SelectedTile.SpritePath = file;
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
