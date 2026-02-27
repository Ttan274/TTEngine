using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Models.Definitions
{
    public class AnimationDefinition : ObservableObject
    {
        //Definition
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _spriteSheetPath;
        public string SpriteSheetPath
        {
            get => _spriteSheetPath;
            set => SetProperty(ref _spriteSheetPath, value);
        }

        //Animation Stats
        private int _frameWidth;
        public int FrameWidth
        {
            get => _frameWidth;
            set => SetProperty(ref _frameWidth, value);
        }

        private int _frameHeight;
        public int FrameHeight
        {
            get => _frameHeight;
            set => SetProperty(ref _frameHeight, value);
        }

        private int _frameCount;
        public int FrameCount
        {
            get => _frameCount;
            set => SetProperty(ref _frameCount, value);
        }

        private float _frameTime;
        public float FrameTime
        {
            get => _frameTime;
            set => SetProperty(ref _frameTime, value);
        }

        private bool _loop;
        public bool Loop
        {
            get => _loop;
            set => SetProperty(ref _loop, value);
        }

        //Animation Events
        public HashSet<int> EventFrames { get; set; } = new();
    }
}
