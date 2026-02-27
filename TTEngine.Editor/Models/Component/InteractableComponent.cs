namespace TTEngine.Editor.Models.Component
{
    public class InteractableComponent : ComponentBase
    {
        public InteractableComponent()
        {
            Type = "Interactable";
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
    }
}
