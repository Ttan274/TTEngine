using System.Collections.ObjectModel;

namespace TTEngine.Editor.Models.Asset
{
    public enum AssetType
    {
        Unknown,
        Folder,
        Json,
        Texture,
        Font
    }

    public class AssetNode
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsFolder { get; set; }
        public AssetType Type { get; set; }
        public bool IsSystemFile { get; set; }

        public ObservableCollection<AssetNode> Children { get; set; }
            = new ObservableCollection<AssetNode>();
    }
}
