using System.Collections.ObjectModel;
using System.Reflection;

namespace TTEngine.Editor.ViewModels.Panel
{
    public class GenericInspectorViewModel
    {
        public ObservableCollection<PropertyFieldViewModel> Fields { get; }
            = new ObservableCollection<PropertyFieldViewModel>();

        public object Target { get; }

        public Action OnValueChanged {  get; set; }

        public GenericInspectorViewModel(object target)
        {
            Target = target;

            var props = target.GetType()
                              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanRead && p.CanWrite && p.Name != "Components");

            foreach (var prop in props)
            {
                var fieldVM = new PropertyFieldViewModel(target, prop);
                fieldVM.ValueChanged += () => OnValueChanged?.Invoke();
                Fields.Add(fieldVM);
            }
        }
    }
}
