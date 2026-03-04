using System.Collections.ObjectModel;
using TTEngine.Editor.Models.Asset;

namespace TTEngine.Editor.Models.Definitions
{
    public class AnimatorDefinition
    {
        public string Id { get; set; }
        public string DefaultState { get; set; }
        public ObservableCollection<AnimatorState> States { get; set; } = new();
    }

    public class AnimatorState
    {
        public string Name { get; set; }

        [AssetReference(AssetType.Animation)]
        public string Animation { get; set; }
    }
}
