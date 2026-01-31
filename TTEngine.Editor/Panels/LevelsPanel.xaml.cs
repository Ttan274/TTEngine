using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Level;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for LevelsPanel.xaml
    /// </summary>
    public partial class LevelsPanel : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<LevelDefinition> Levels { get; }
            = new ObservableCollection<LevelDefinition>();
        public ObservableCollection<string> AvailableMaps { get; }
            = new ObservableCollection<string>();

        private LevelDefinition _selectedLevel;
        public LevelDefinition SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
            }
        }

        private readonly EditorState _editorState;

        public LevelsPanel(EditorState editorState)
        {
            InitializeComponent();
            DataContext = this;
            
            _editorState = editorState;
            LoadMaps();
            LoadLevels();
            SelectedLevel = Levels[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string p)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        private void LoadMaps()
        {
            AvailableMaps.Clear();

            var path = EditorPaths.GetMapsFolder();

            foreach (var f in Directory.GetFiles(path, "*.json"))
            {
                string mapId = Path.GetFileNameWithoutExtension(f);
                AvailableMaps.Add(mapId);
            }
        }

        private void LoadLevels()
        {
            Levels.Clear();

            var loaded = LevelFileService.Load();

            if(loaded.Count == 0)
            {
                Levels.Add(new LevelDefinition
                {
                    Id = "Level1",
                    MapId = "Map_Default",
                    IsActive = true
                });
            }
            else
            {
                foreach (var lvl in loaded)
                    Levels.Add(lvl);
            }

            SelectedLevel = Levels.FirstOrDefault();

            foreach (var item in Levels)
                item.PropertyChanged += (_, _) => SaveLevels();
                
        }

        private void SaveLevels()
        {
            LevelFileService.Save(Levels);
        }

        #region UI region

        private void AddLevelClick(object sender, RoutedEventArgs e)
        {
            var l = new LevelDefinition
            {
                Id = $"Level{Levels.Count + 1}",
                IsActive = true,
                MapId = AvailableMaps.FirstOrDefault()
            };

            Levels.Add(l);
            SelectedLevel = l;
            SaveLevels();
        }

        private void DeleteLevelClick(object sender, RoutedEventArgs e)
        {
            if (SelectedLevel == null)
                return;

            int index = Levels.IndexOf(SelectedLevel);
            Levels.Remove(SelectedLevel);

            if (Levels.Count > 0)
                SelectedLevel = Levels[Math.Max(0, index - 1)];

            SaveLevels();
        }

        private void MoveUpClick(object sender, RoutedEventArgs e)
        {
            int index = Levels.IndexOf(SelectedLevel);
            if (index <= 0) return;

            Levels.Move(index, index - 1);
            SaveLevels();
        }

        private void MoveDownClick(object sender, RoutedEventArgs e)
        {
            int index = Levels.IndexOf(SelectedLevel);
            if (index < 0 || index >= Levels.Count - 1) return;

            Levels.Move(index, index + 1);
            SaveLevels();
        }

        #endregion
    }
}
