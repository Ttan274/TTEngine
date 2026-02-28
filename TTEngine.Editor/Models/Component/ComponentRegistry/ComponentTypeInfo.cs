namespace TTEngine.Editor.Models.Component.ComponentRegistry
{
    public class ComponentTypeInfo
    {
        public string TypeName { get; }
        public Type DefType { get; }
        public bool AllowMultiple { get; }
        public bool IsRemovable { get; }

        public ComponentTypeInfo(string typeName, Type defType, bool allowMultiple, bool isRemovable)
        {
            TypeName = typeName;
            DefType = defType;
            AllowMultiple = allowMultiple;
            IsRemovable = isRemovable;
        }
    }
}
