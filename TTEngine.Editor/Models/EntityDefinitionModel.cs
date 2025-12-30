namespace TTEngine.Editor.Models
{
    public class EntityDefinitionModel
    {
        public string Id { get; set; } = "NewEntity";
        public float Speed { get; set; } = 5f;
        public float AttackDamage { get; set; } = 10f;
        public float AttackInterval { get; set; } = 1f;
        public float MaxHP { get; set; } = 100f;
    }
}
