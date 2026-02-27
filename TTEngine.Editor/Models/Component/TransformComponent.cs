namespace TTEngine.Editor.Models.Component
{
    public class TransformComponent : ComponentBase
    {
        public TransformComponent()
        {
            Type = "Transform";
        }

        private float _x;
        public float X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private float _y;
        public float Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }
    }
}
