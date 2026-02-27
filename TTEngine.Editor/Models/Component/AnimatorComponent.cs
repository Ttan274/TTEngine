namespace TTEngine.Editor.Models.Component
{
    public class AnimatorComponent : ComponentBase
    {
        public AnimatorComponent()
        {
            Type = "Animator";
        }

        public Dictionary<string, string> Animations { get; set; } = new();
    }
}
