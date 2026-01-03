using System.ComponentModel;
using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorLayer : INotifyPropertyChanged
    {
        public MapLayerType LayerType { get; }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
                VisibilityChanged?.Invoke(this);
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }

        }

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                _isLocked = value;
                OnPropertyChanged(nameof(IsLocked));
            }
        }

        public EditorLayer(MapLayerType type)
        {
            LayerType = type;
            IsLocked = false;
        }


        public event Action<EditorLayer> VisibilityChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
