using System.Windows.Controls;
using TTEngine.Editor.Models.Interactable;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for InteractableInspector.xaml
    /// </summary>
    public partial class InteractableInspector : UserControl
    {
        public InteractableInspector(InteractableModel model, IEnumerable<InteractableDefinition> defs)
        {
            InitializeComponent();

            var def = defs.FirstOrDefault(d => d.Id == model.DefinitionId);
            if (def == null)
                return;

            DataContext = new
            {
                DefinitionId = def.Id,
                Type = def.Type
            };
        }
    }
}
