using TTEngine.Editor.Models.Definitions;

namespace TTEngine.Editor.Models.Selection
{
    public abstract class SelectionViewModel
    {
        public int X { get; }
        public int Y { get; }

        protected SelectionViewModel(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class TileSelectionViewModel : SelectionViewModel
    {
        public int TileId { get; }

        public TileSelectionViewModel(int x, int y, int tileId)
            : base(x, y)
        {
            TileId = tileId;
        }
    }

    //public class PlayerSelectionViewModel : SelectionViewModel
    //{
    //    public string Type { get; }
    //    public float Speed { get; }
    //    public float Damage { get; }
    //    public float AttackInterval { get; }
    //    public float MaxHP { get; }

    //    public PlayerSelectionViewModel(int x, int y, EntityDefinition def)
    //        : base(x, y)
    //    {
    //        Type = def.Id;
    //        Speed = def.Speed;
    //        Damage = def.AttackDamage;
    //        AttackInterval = def.AttackInterval;
    //        MaxHP = def.MaxHP;
    //    }
    //}

    //public class EnemySelectionViewModel : SelectionViewModel
    //{
    //    public string Type { get; }
    //    public float Speed { get; }
    //    public float Damage { get; }
    //    public float AttackInterval { get; }
    //    public float MaxHP { get; }

    //    public EnemySelectionViewModel(int x, int y, EntityDefinition def)
    //        : base(x, y)
    //    {
    //        Type = def.Id;
    //        Speed = def.Speed;
    //        Damage = def.AttackDamage;
    //        AttackInterval = def.AttackInterval;
    //        MaxHP = def.MaxHP;
    //    }
    //}

    public class InteractableSelectionViewModel : SelectionViewModel
    {
        public string Type { get; }
        public string Task { get; }

        public InteractableSelectionViewModel(int x, int y, string type, string task)
            : base(x, y)
        {
            Type = type;
            Task = task;
        }
    }
}
