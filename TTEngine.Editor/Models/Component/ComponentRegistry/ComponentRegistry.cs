namespace TTEngine.Editor.Models.Component.ComponentRegistry
{
    public static class ComponentRegistry
    {
        private static readonly List<ComponentTypeInfo> _types = new()
        {
            new ComponentTypeInfo(
                "Transform",
                typeof(TransformComponent),
                allowMultiple: false,
                isRemovable: false),

            new ComponentTypeInfo(
                "Animator",
                typeof(AnimatorComponent),
                allowMultiple: false,
                isRemovable: true),

            new ComponentTypeInfo(
                "EntityStats",
                typeof(EntityComponent),
                allowMultiple: false,
                isRemovable: true),

            new ComponentTypeInfo(
                "Trap",
                typeof(TrapComponent),
                allowMultiple: false,
                isRemovable: true),

            new ComponentTypeInfo(
                "Interactable",
                typeof(InteractableComponent),
                allowMultiple: false,
                isRemovable: true)
        };

        public static IReadOnlyList<ComponentTypeInfo> Types => _types;

        public static ComponentTypeInfo Get(string typeName)
            => _types.FirstOrDefault(t => t.TypeName == typeName);
    }
}
