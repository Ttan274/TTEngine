using System.Collections.ObjectModel;
using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public class LayerState : ObservableObject
    {
        public ObservableCollection<EditorLayer> Layers { get; }

        public LayerState()
        {
            Layers = new ObservableCollection<EditorLayer>
            {
                new EditorLayer(MapLayerType.Background),
                new EditorLayer(MapLayerType.Collision) {IsActive = true},
                new EditorLayer(MapLayerType.Decoration),
                new EditorLayer(MapLayerType.Interactable)
            };
        }

        public EditorLayer ActiveLayer =>
            Layers.First(l => l.IsActive);

        public void SetActiveLayer(EditorLayer layer)
        {
            foreach (var l in Layers)
                l.IsActive = false;

            layer.IsActive = true;

            OnPropertyChanged(nameof(ActiveLayer));
        }

        public bool IsActiveLayerLocked =>
             ActiveLayer?.IsLocked ?? false;
    }
}
