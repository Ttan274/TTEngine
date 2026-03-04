using TTEngine.Editor.Models.Asset;

namespace TTEngine.Editor.Models.Component
{
    public class AnimatorComponent : ComponentBase
    {
        public AnimatorComponent()
            : base("Animator")
        {
        }

        [AssetReference(AssetType.Animator)]
        public string Animator { get; set; }
    }
}
