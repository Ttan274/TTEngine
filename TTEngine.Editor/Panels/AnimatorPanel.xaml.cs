using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Asset;
using TTEngine.Editor.Models.Definitions;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Services.IO;
using TTEngine.Editor.WindowPanels;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for AnimatorPanel.xaml
    /// </summary>
    public partial class AnimatorPanel : UserControl
    {
        private readonly ProjectSession _session;
        private AnimatorDefinition _model;
        private string _filePath;

        public AnimatorPanel(ProjectSession session)
        {
            InitializeComponent();
            _session = session;
        }

        public void LoadFile(string path)
        {
            _filePath = path;
            _model = JsonFileService.Load<AnimatorDefinition>(path);
            DataContext = _model;
        }

        private void AddState_Click(object sender, RoutedEventArgs e)
        {
            _model.States.Add(new AnimatorState{
                Name = "NewState"
            });
        }

        private void RemoveState_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is AnimatorState state)
                _model.States.Remove(state);
        }

        private void PickAnimation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AnimatorState state)
            {
                var picker = new AssetPickerWindow(_session, AssetType.Animation);

                if (picker.ShowDialog() == true)
                    state.Animation = picker.SelectedAsset;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_model == null || string.IsNullOrEmpty(_filePath))
                return;

            JsonFileService.Save(_filePath, _model);
        }
    }
}
