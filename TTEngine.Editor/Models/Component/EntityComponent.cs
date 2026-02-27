namespace TTEngine.Editor.Models.Component
{
    public class EntityComponent : ComponentBase
    {
        public EntityComponent()
        {
            Type = "EntityStats";
        }

        //Stats
        private float _speed;
        public float Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        private float _attackDamage;
        public float AttackDamage
        {
            get => _attackDamage;
            set => SetProperty(ref _attackDamage, value);
        }

        private float _attackInterval;
        public float AttackInterval
        {
            get => _attackInterval;
            set => SetProperty(ref _attackInterval, value);
        }

        private float _maxHP;
        public float MaxHP
        {
            get => _maxHP;
            set => SetProperty(ref _maxHP, value);
        }

        //Buraya daha eklenebilir
    }
}
