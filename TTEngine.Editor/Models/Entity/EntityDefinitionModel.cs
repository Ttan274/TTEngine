namespace TTEngine.Editor.Models.Entity
{
    public class EntityDefinitionModel
    {
        //Type
        public string Id { get; set; }    

        //Stats
        public float Speed { get; set; } 
        public float AttackDamage { get; set; } 
        public float AttackInterval { get; set; } 
        public float MaxHP { get; set; }

        //Animation Storage (New) -- Later will dropped into a dictionary
        public string IdleAnimation { get; set; }
        public string WalkAnimation { get; set; }
        public string HurtAnimation { get; set; }
        public string DeathAnimation { get; set; }
        public List<string> AttackAnimations { get; set; }
    }
}
