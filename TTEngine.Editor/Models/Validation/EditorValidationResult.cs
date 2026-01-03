namespace TTEngine.Editor.Models.Validation
{
    public class EditorValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; set; } = new();
    }
}
