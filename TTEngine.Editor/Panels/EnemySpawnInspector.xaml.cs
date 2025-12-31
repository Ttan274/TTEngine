using System.Windows.Controls;
using TTEngine.Editor.Models;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for EnemySpawnInspector.xaml
    /// </summary>
    public partial class EnemySpawnInspector : UserControl
    {
        private EnemySpawnModel _model;
        private List<EntityDefinitionModel> _definitions;

        public EnemySpawnInspector(EnemySpawnModel model, List<EntityDefinitionModel> definitions)
        {
            InitializeComponent();

            _model = model;
            _definitions = definitions;
            UpdateStats();
        }

        private void UpdateStats()
        {
            var def = _definitions.FirstOrDefault(d => d.Id == _model.DefinitionId);
            if (def == null) return;

            TypeText.Text = $"Type: {def.Id}";

            StatsText.Text = $@"Speed: { def.Speed}
                                AttackDamage: {def.AttackDamage}
                                AttackInterval: {def.AttackInterval}
                                MaxHP: {def.MaxHP}";
        }
    }
}
