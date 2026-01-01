using System.Windows.Controls;
using System.Windows.Input;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for LayerPanel.xaml
    /// </summary>
    public partial class LayerPanel : UserControl
    {
        public LayerPanel()
        {
            InitializeComponent();
        }

        private void Layer_Click(object sender, MouseButtonEventArgs e)
        {
            if(DataContext is EditorState state && sender is TextBlock tb && tb.DataContext is EditorLayer layer)
            {
                state.SetActiveLayer(layer);
            }
        }
    }
}
