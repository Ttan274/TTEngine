namespace TTEngine.Editor.Models.Asset
{
    public enum AssetType
    {
        Texture,
        Animator,
        Animation 
    }

    public class AssetItem
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }
}
