using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Component.ComponentRegistry;

namespace TTEngine.Editor.ViewModels.Panel
{
    public class ComponentViewModel
    {
        public ComponentBase Model { get; }
        public GenericInspectorViewModel Inspector { get; }
        public string Type => Model.Type;
        public bool IsRemovable => ComponentRegistry.Get(Type)?.IsRemovable ?? true;

        public ComponentViewModel(ComponentBase model)
        {
            Model = model;
            Inspector = new GenericInspectorViewModel(model);
        }
    }
}
