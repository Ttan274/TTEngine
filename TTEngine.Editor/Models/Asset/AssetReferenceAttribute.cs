namespace TTEngine.Editor.Models.Asset
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AssetReferenceAttribute : Attribute
    {
        public AssetType AssetType { get; }

        public AssetReferenceAttribute(AssetType assetType)
        {
            AssetType = assetType;
        }
    }
}
