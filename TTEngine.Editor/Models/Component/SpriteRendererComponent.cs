namespace TTEngine.Editor.Models.Component
{
    public class SpriteRendererComponent : ComponentBase
    {
        public SpriteRendererComponent() 
            : base("SpriteRenderer")
        {
        }

        private string _spritePath;
        public string SpritePath
        {
            get => _spritePath;
            set => SetProperty(ref _spritePath, value);
        }
    }
}
