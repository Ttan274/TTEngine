using System.Collections.ObjectModel;
using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Models.GameObject
{
    public class GameObject : ObservableObject
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _tag;
        public string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);    
        }

        private string _layer;
        public string Layer
        {
            get => _layer;
            set => SetProperty(ref _layer, value);
        }

        public ObservableCollection<ComponentBase> Components { get; set; } = new();
    }
}
