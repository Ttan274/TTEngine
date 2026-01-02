using System.Windows;
using System.Windows.Controls;
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
    }
}
