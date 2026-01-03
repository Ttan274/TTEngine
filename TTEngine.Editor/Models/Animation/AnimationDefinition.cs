namespace TTEngine.Editor.Models.Animation
{
    public class AnimationDefinition
    {
        //Definition
        public string Id { get; set; }
        public string SpriteSheetPath { get; set; }

        //Animation Stats
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameCount { get; set; }
        public float FrameTime { get; set; }
        public bool Loop { get; set; }

        //Animation Events
        public HashSet<int> EventFrames { get; set; }
    }
}
