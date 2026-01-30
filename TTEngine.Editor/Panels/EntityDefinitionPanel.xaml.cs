using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Animation;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for EntityDefinitionPanel.xaml
    /// </summary>
    public partial class EntityDefinitionPanel : UserControl
    {
        private List<EntityDefinitionModel> _definitions;
        private List<AnimationDefinition> _animations;
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
            _animations = AnimationDefinitionService.All.ToList();

            _current = DefinitionCombo.SelectedItem as EntityDefinitionModel;
            if (_current == null) return;

            //Stats
            SpeedBox.Text = _current.Speed.ToString(CultureInfo.InvariantCulture);
            AttackDamageBox.Text = _current.AttackDamage.ToString(CultureInfo.InvariantCulture);
            AttackIntervalBox.Text = _current.AttackInterval.ToString(CultureInfo.InvariantCulture);
            MaxHpBox.Text = _current.MaxHP.ToString(CultureInfo.InvariantCulture);

            //Animations
            SetCombobox(IdleAnimCombo, _current.IdleAnimation);
            SetCombobox(WalkAnimCombo, _current.WalkAnimation);
            SetCombobox(HurtAnimCombo, _current.HurtAnimation);
            SetCombobox(DeathAnimCombo, _current.DeathAnimation);
            AttackAnimCombo.ItemsSource = _animations;
            RefreshAttackList();
        }

        private void RefreshAttackList()
        {
            AttackAnimList.ItemsSource = null;
            AttackAnimList.ItemsSource = _current.AttackAnimations;
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            //Stats
            _current.Speed = float.Parse(SpeedBox.Text, CultureInfo.InvariantCulture);
            _current.AttackDamage = float.Parse(AttackDamageBox.Text, CultureInfo.InvariantCulture);
            _current.AttackInterval = float.Parse(AttackIntervalBox.Text, CultureInfo.InvariantCulture);
            _current.MaxHP = float.Parse(MaxHpBox.Text, CultureInfo.InvariantCulture);

            //Animations
            _current.IdleAnimation = (IdleAnimCombo.SelectedItem as AnimationDefinition)?.Id;
            _current.WalkAnimation = (WalkAnimCombo.SelectedItem as AnimationDefinition)?.Id;
            _current.HurtAnimation = (HurtAnimCombo.SelectedItem as AnimationDefinition)?.Id;
            _current.DeathAnimation = (DeathAnimCombo.SelectedItem as AnimationDefinition)?.Id;

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
                MaxHP = 50f,
                IdleAnimation = "",
                WalkAnimation = "",
                HurtAnimation = "",
                DeathAnimation = "",
                AttackAnimations = new List<string>()
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


        private void SetCombobox(ComboBox b, string selectedAnim)
        {
            b.ItemsSource = _animations;
            b.DisplayMemberPath = "Id";

            if(!string.IsNullOrEmpty(selectedAnim))
            {
                b.SelectedItem = _animations.FirstOrDefault(a => a.Id == selectedAnim);
            }
            else
            {
                b.SelectedIndex = -1;
            }
        }

        #region AttackRelatedBtns

        private void AddAttackAnim(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            var anim = AttackAnimCombo.SelectedItem as AnimationDefinition;
            if (anim == null) return;

            if(!_current.AttackAnimations.Contains(anim.Id))
            {
                _current.AttackAnimations.Add(anim.Id);
                RefreshAttackList();
            }
        }

        private void RemoveAttackAnim(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            var anim = AttackAnimList.SelectedItem as string;
            if (anim == null) return;

            _current.AttackAnimations.Remove(anim);
            RefreshAttackList();
        }

        private void MoveAttackUp(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            int i = AttackAnimList.SelectedIndex;
            if (i <= 0) return;

            (_current.AttackAnimations[i - 1], _current.AttackAnimations[i]) =
                (_current.AttackAnimations[i], _current.AttackAnimations[i - 1]);

            RefreshAttackList();
            AttackAnimList.SelectedIndex = i - 1;
        }

        private void MoveAttackDown(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            int i= AttackAnimList.SelectedIndex;
            if (i < 0 || i >= _current.AttackAnimations.Count - 1) return;

            (_current.AttackAnimations[i + 1], _current.AttackAnimations[i]) =
                (_current.AttackAnimations[i], _current.AttackAnimations[i + 1]);

            RefreshAttackList();
            AttackAnimList.SelectedIndex = i + 1;
        }
            
        #endregion
    }
}
