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
        private const string BaseId = "NewEntity";

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

        private void NewDefClicked(object sender, RoutedEventArgs e)
        {
            string newId = BaseId;
            int index = 1;

            while(_definitions.Any(d => d.Id == newId))
            {
                newId = BaseId + index;
                index++;
            }

            var def = new EntityDefinitionModel
            {
                Id = newId,
                Speed = 60f,
                AttackDamage = 10f,
                AttackInterval = 1.2f,
                MaxHP = 50f
            };

            _definitions.Add(def);

            DefinitionCombo.Items.Refresh();
            DefinitionCombo.SelectedItem = def;
        }

        private void DeleteDefClicked(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            if(_current.Id == "Player")
            {
                MessageBox.Show("Player definition cannot be deleted");
                return;
            }

            var result = MessageBox.Show($"Delete definition '{_current.Id}'?",
                                                             "Confirm",
                                                             MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return;

            _definitions.Remove(_current);

            DefinitionCombo.Items.Refresh();
            DefinitionCombo.SelectedIndex = 0;
        }
    }
}
