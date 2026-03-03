namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public enum ToolMode
    {
        Brush,
        Fill
    }

    public class ToolState : ObservableObject
    {
        private ToolMode _currentToolmode;
        public ToolMode CurrentToolMode
        {
            get => _currentToolmode;
            set => SetProperty(ref _currentToolmode, value);
        }
    }
}
