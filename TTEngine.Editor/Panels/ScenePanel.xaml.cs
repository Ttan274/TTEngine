using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for ScenePanel.xaml
    /// </summary>
    public partial class ScenePanel : UserControl
    {
        private EditorState _state;

        public ScenePanel()
        {
            InitializeComponent();
        }

        public void Bind(EditorState state)
        {
            _state = state;

            SceneList.ItemsSource = _state.SceneSession.GetAllScenes();

            SceneList.SetBinding(ListBox.SelectedItemProperty,
                                 new Binding("SceneSession.ActiveSceneId")
                                 {
                                     Source = _state,
                                     Mode = BindingMode.TwoWay
                                 });
        }

        private void RefreshList()
        {
            SceneList.ItemsSource = null;
            SceneList.ItemsSource = _state.SceneSession.GetAllScenes();
        }

        private void NewScene_Click(object sender, RoutedEventArgs e)
        {
            string newId = $"Scene_{DateTime.Now.Ticks}";

            _state.SceneSession.Create(newId);
            RefreshList();
            SceneList.SelectedItem = newId;
        }

        private void SceneList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SceneList.SelectedItem == null)
                return;

            string sceneId = SceneList.SelectedItem.ToString();
            _state.SceneSession.Load(sceneId);
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (SceneList.SelectedItem == null)
                return;

            string oldId = SceneList.SelectedItem.ToString();

            var input = Microsoft.VisualBasic.Interaction.InputBox("Enter new scene name", ">Rename scene", oldId);

            if (string.IsNullOrWhiteSpace(input))
                return;

            _state.SceneSession.Rename(oldId, input);

            RefreshList();
            SceneList.SelectedItem = input;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SceneList.SelectedItem == null)
                return;

            string id = SceneList.SelectedItem.ToString();
            if (MessageBox.Show($"Delete scene {id} ?", 
                "Confirm", 
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _state.SceneSession.Delete(id);

            RefreshList();
        }
    }
}
