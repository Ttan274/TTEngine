using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Entity;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for PlayerSpawnInspector.xaml
    /// </summary>
    public partial class PlayerSpawnInspector : UserControl
    {
        private PlayerSpawnModel _model;
        private List<EntityDefinitionModel> _definitions;

        public PlayerSpawnInspector(PlayerSpawnModel model, List<EntityDefinitionModel> definitions)
        {
            InitializeComponent();

            _model = model;
            _definitions = definitions;

            PositionText.Text = $"Position: X={_model.Position.X}, Y={_model.Position.Y}";
            UpdateStats();
        }

        private void UpdateStats()
        {
            var def = _definitions.FirstOrDefault(d => d.Id == _model.DefinitionId);
            if (def == null) return;

            TypeText.Text = $"Type: {def.Id}";

            StatsText.Text = $@"Speed: {def.Speed}
                                AttackDamage: {def.AttackDamage}
                                AttackInterval: {def.AttackInterval}
                                MaxHP: {def.MaxHP}";
        }
    }
}
