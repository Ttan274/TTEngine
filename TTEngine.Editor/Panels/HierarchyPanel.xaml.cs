using System.Windows.Controls;
using System.Windows.Input;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for HierarchyPanel.xaml
    /// </summary>
    public partial class HierarchyPanel : UserControl
    {
        public event Action RequestMapRedraw;

        public HierarchyPanel()
        {
            InitializeComponent();
        }

        private void HierarchyKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
                return;

            if (DataContext is not EditorState editor)
                return;

            var obj = editor.SceneSession.SelectedObject;

            if (obj == null)
                return;

            editor.SceneSession.ActiveScene.SceneObjects.Remove(obj);
            RequestMapRedraw?.Invoke();
        }
    }
}
