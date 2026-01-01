using System.Collections.ObjectModel;
using TTEngine.Editor.Enums;

namespace TTEngine.Editor.Models.Editor
{
    public class EditorState
    {
        public ObservableCollection<EditorLayer> Layers { get; } =
            new ObservableCollection<EditorLayer>
            {
                new EditorLayer(MapLayerType.Background),
                new EditorLayer(MapLayerType.Collision) {IsActive = true},
                new EditorLayer(MapLayerType.Decoration)
            };

        public EditorLayer ActiveLayer =>
            Layers.First(l => l.IsActive);

        public void SetActiveLayer(EditorLayer layer)
        {
            foreach (var l in Layers)
                l.IsActive = false;

            layer.IsActive = true;
        }
    }
}
