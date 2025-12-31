namespace TTEngine.Editor.Models
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

        //Visuals (Textures)
        public string IdleTexture { get; set; } = "";
        public string WalkTexture { get; set; } = "";
        public string HurtTexture { get; set; } = "";
        public string DeathTexture { get; set; } = "";
        public List<string> AttackTextures { get; set; } = new();
    }
}
