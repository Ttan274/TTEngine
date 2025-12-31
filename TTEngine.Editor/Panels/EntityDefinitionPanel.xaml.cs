using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for EntityDefinitionPanel.xaml
    /// </summary>
    public partial class EntityDefinitionPanel : UserControl
    {
        private List<EntityDefinitionModel> _definitions;
        private EntityDefinitionModel _current;

        public EntityDefinitionPanel(List<EntityDefinitionModel> definitions)
        {
            InitializeComponent();

            _definitions = definitions;

            DefinitionCombo.ItemsSource = _definitions;
            DefinitionCombo.DisplayMemberPath = "Id";
            DefinitionCombo.SelectedIndex = 0;
        }

        private void OnDefinitionChanged(object sender, SelectionChangedEventArgs e)
        {
            _current = DefinitionCombo.SelectedItem as EntityDefinitionModel;
            if (_current == null) return;

            SpeedBox.Text = _current.Speed.ToString(CultureInfo.InvariantCulture);
            AttackDamageBox.Text = _current.AttackDamage.ToString(CultureInfo.InvariantCulture);
            AttackIntervalBox.Text = _current.AttackInterval.ToString(CultureInfo.InvariantCulture);
            MaxHpBox.Text = _current.MaxHP.ToString(CultureInfo.InvariantCulture);
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            _current.Speed = float.Parse(SpeedBox.Text, CultureInfo.InvariantCulture);
            _current.AttackDamage = float.Parse(AttackDamageBox.Text, CultureInfo.InvariantCulture);
            _current.AttackInterval = float.Parse(AttackIntervalBox.Text, CultureInfo.InvariantCulture);
            _current.MaxHP = float.Parse(MaxHpBox.Text, CultureInfo.InvariantCulture);

            EntityDefinitionService.Save(_definitions);
        }
    }
}
