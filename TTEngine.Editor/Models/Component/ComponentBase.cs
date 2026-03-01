using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Models.Component
{
    public class ComponentBase : ObservableObject
    {
        public string Type { get; }

        protected ComponentBase(string type)
        {
            Type = type;
        }
    }
}
