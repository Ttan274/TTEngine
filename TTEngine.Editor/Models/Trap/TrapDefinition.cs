namespace TTEngine.Editor.Models.Trap
{
    public class TrapDefinition
    {
        public string Id { get; set; }
        public string ImagePath { get; set; }

        //Fire
        public float ActiveDuration { get; set; }
        public float InactiveDuration { get; set; }
        public float DamagePerSecond { get; set; }

        //Saw
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float DamageCooldown { get; set; }
    }
}
