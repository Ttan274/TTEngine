using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Level;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for MapsPanel.xaml
    /// </summary>
    public partial class MapsPanel : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<MapDefinition> Maps { get; }
            = new ObservableCollection<MapDefinition>();

        private MapDefinition _selectedMap;
        public MapDefinition SelectedMap
        {
            get => _selectedMap;
            set
            {
                if (_selectedMap == value)
                    return;

                _selectedMap = value;
                OnPropertyChanged(nameof(SelectedMap));

                LoadSelectedMap();
            }
        }

        private bool IsDefaultMap => SelectedMap?.Id == "Map_Default";

        private readonly EditorState _editorState;

        public MapsPanel(EditorState editorState)
        {
            InitializeComponent();
            DataContext = this;
            _editorState = editorState;
            LoadMapsFromDisk();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string p)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        #region Load / Select Map
        
        private void LoadMapsFromDisk()
        {
            Maps.Clear();

            var path = EditorPaths.GetMapsFolder();

            foreach (var m in Directory.GetFiles(path, "*.json"))
            {
                string id = Path.GetFileNameWithoutExtension(m);
                Maps.Add(new MapDefinition { Id = id });
            }

            if (Maps.Count > 0)
                SelectedMap = Maps[0];
        }

        private void LoadSelectedMap()
        {
            if (SelectedMap == null)
                return;

            var data = MapFileService.Load(SelectedMap.Id);
            TileMapModel model;

            if(data == null)
            {
                model = new TileMapModel();
                model.Init();
            }
            else
            {
                model = MapFileService.FromDto(data);
            }

            _editorState.ActiveMapId = SelectedMap.Id;
            _editorState.ActiveMap = model;
        }

        #endregion

        #region UI Actions

        private void AddMapClick(object sender, RoutedEventArgs e)
        {
            string id = $"Map{Maps.Count + 1}";

            var model = new TileMapModel();
            model.Init();

            var data = MapFileService.ToDto(model);
            MapFileService.Save(id, data);

            var map = new MapDefinition { Id = id };
            Maps.Add(map);
            SelectedMap = map;
        }

        private void DeleteMapClick(object sender, RoutedEventArgs e)
        {
            if (SelectedMap == null)
                return;

            if(IsDefaultMap)
            {
                MessageBox.Show(
                "Default Map cannot be deleted",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
                return;
            }

            var result = MessageBox.Show(
                $"Delete map '{SelectedMap.Id}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            MapFileService.Delete(SelectedMap.Id);
            int index = Maps.IndexOf(SelectedMap);
            Maps.Remove(SelectedMap);

            if (Maps.Count > 0)
                SelectedMap = Maps[Math.Max(0, index - 1)];
            else if(Maps.Count == 0)
            {
                _editorState.ActiveMapId = null;
                _editorState.ActiveMap = null;
            }
        }

        #endregion
    }
}
