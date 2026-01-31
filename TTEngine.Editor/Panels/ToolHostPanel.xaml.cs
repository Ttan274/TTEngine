using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for ToolHostPanel.xaml
    /// </summary>
    public partial class ToolHostPanel : UserControl
    {
        private EditorState _editorState;

        public ToolHostPanel()
        {
            InitializeComponent();
        }

        public void BindEditor(EditorState editorState) => _editorState = editorState;

        public void Clear() => ContentHost.Content = null;

        private void OpenTiles(object sender, RoutedEventArgs e)
            => ContentHost.Content = new TileManagementPanel(_editorState);
        private void OpenEntities(object sender, RoutedEventArgs e)
            => ContentHost.Content = null;
        private void OpenAnimations(object sender, RoutedEventArgs e)
            => ContentHost.Content = new AnimationDefinitionPanel();
        private void OpenMaps(object sender, RoutedEventArgs e)
            => ContentHost.Content = new MapsPanel(_editorState);
        private void OpenLevels(object sender, RoutedEventArgs e)
            => ContentHost.Content = new LevelsPanel(_editorState);
    }
}
