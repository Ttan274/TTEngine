namespace TTEngine.Editor.Models.Definitions
{
    public class EditorValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; set; } = new();
    }
}
