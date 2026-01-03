using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for InspectorPanel.xaml
    /// </summary>
    public partial class InspectorPanel : UserControl
    {
        public InspectorPanel()
        {
            InitializeComponent();
        }

        public void SetContent(UserControl content)
        {
            InspectorContent.Content = content;
        }

        public void Clear()
        {
            InspectorContent.Content = null;
        }

        public void BindEditor(EditorState editor)
        {
            ConsolePanel.DataContext = editor;
        }
    }
}
