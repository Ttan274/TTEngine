namespace TTEngine.Editor.Models.Component
{
    public class AnimatorComponent : ComponentBase
    {
        public AnimatorComponent()
            : base("Animator")
        {
        }

        public Dictionary<string, string> Animations { get; set; } = new();
    }
}
