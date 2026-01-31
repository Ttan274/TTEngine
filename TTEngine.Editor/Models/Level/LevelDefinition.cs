using System.ComponentModel;

namespace TTEngine.Editor.Models.Level
{
    public class LevelDefinition : INotifyPropertyChanged
    {
        private string _id;
        public string Id
        {
            get => _id;
            set { _id = value;  OnPropertyChanged(nameof(Id)); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(nameof(IsActive)); }
        }

        private string _mapId;
        public string MapId
        {
            get => _mapId;
            set { _mapId = value; OnPropertyChanged(nameof(MapId)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string p)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
