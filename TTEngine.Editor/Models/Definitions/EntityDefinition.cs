using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.Models.Definitions
{
    public class EntityDefinition : ObservableObject
    {
        //Type
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
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

        //Animations
        public Dictionary<string, string> Animations { get; set; } = new();

        public EntityDefinition()
        {
            Animations["Idle"] = "test1";
            Animations["Walk"] = "test2";
        }
    }
}
