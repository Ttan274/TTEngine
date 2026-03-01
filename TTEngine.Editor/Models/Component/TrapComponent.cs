namespace TTEngine.Editor.Models.Component
{
    public class TrapComponent : ComponentBase
    {
        public TrapComponent()
            : base("Trap")
        {
        }

        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private float _val1;
        public float Val1
        {
            get => _val1;
            set => SetProperty(ref _val1, value);
        }
       
        private float _val2;
        public float Val2
        {
            get => _val2;
            set => SetProperty(ref _val2, value);
        }

        private float _val3;
        public float Val3
        {
            get => _val3;
            set => SetProperty(ref _val3, value);
        }
    }
}
