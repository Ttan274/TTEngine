namespace TTEngine.Editor.Models.Asset
{
    public enum AssetType
    {
        Texture,
        Animation       //Audio, ...
    }

    public class AssetItem
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }
}
