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

        public ObservableCollection<ComponentBase> Components { get; set; } = new();
    }
}
