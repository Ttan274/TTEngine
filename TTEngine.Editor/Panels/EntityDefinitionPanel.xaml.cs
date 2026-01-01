using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        #region OnChange Methods

        private void OnDefinitionChanged(object sender, SelectionChangedEventArgs e)
        {
            _current = DefinitionCombo.SelectedItem as EntityDefinitionModel;
            if (_current == null) return;

            //Stats
            SpeedBox.Text = _current.Speed.ToString(CultureInfo.InvariantCulture);
            AttackDamageBox.Text = _current.AttackDamage.ToString(CultureInfo.InvariantCulture);
            AttackIntervalBox.Text = _current.AttackInterval.ToString(CultureInfo.InvariantCulture);
            MaxHpBox.Text = _current.MaxHP.ToString(CultureInfo.InvariantCulture);

            //Textures
            IdleTextureBox.Text = _current.IdleTexture;
            WalkTextureBox.Text = _current.WalkTexture;
            HurtTextureBox.Text = _current.HurtTexture;
            DeathTextureBox.Text = _current.DeathTexture;

            UpdatePreview(_current.IdleTexture, IdlePreview);
            UpdatePreview(_current.WalkTexture, WalkPreview);
            UpdatePreview(_current.HurtTexture, HurtPreview);
            UpdatePreview(_current.DeathTexture, DeathPreview);
            RefreshAttackTexturePreview();
        }

        private void OnIdleTextureChanged(object sender, TextChangedEventArgs e)
        {
            if (_current == null)
                return;

            _current.IdleTexture = IdleTextureBox.Text;
            UpdatePreview(_current.IdleTexture, IdlePreview);
        }

        private void OnWalkTextureChanged(object sender, TextChangedEventArgs e)
        {
            if (_current == null)
                return;

            _current.WalkTexture = WalkTextureBox.Text;
            UpdatePreview(_current.WalkTexture, WalkPreview);
        }

        private void OnHurtTextureChanged(object sender, TextChangedEventArgs e)
        {
            if (_current == null)
                return;

            _current.HurtTexture = HurtTextureBox.Text;
            UpdatePreview(_current.HurtTexture, HurtPreview);
        }

        private void OnDeathTextureChanged(object sender, TextChangedEventArgs e)
        {
            if (_current == null)
                return;

            _current.DeathTexture = DeathTextureBox.Text;
            UpdatePreview(_current.DeathTexture, DeathPreview);
        }

        #endregion

        #region Button Clicks

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            //Stats
            _current.Speed = float.Parse(SpeedBox.Text, CultureInfo.InvariantCulture);
            _current.AttackDamage = float.Parse(AttackDamageBox.Text, CultureInfo.InvariantCulture);
            _current.AttackInterval = float.Parse(AttackIntervalBox.Text, CultureInfo.InvariantCulture);
            _current.MaxHP = float.Parse(MaxHpBox.Text, CultureInfo.InvariantCulture);

            //Textures
            _current.IdleTexture = IdleTextureBox.Text;
            _current.WalkTexture = WalkTextureBox.Text;
            _current.HurtTexture = HurtTextureBox.Text;
            _current.DeathTexture = DeathTextureBox.Text;

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
                IdleTexture = "",
                WalkTexture = "",
                HurtTexture = "",
                DeathTexture = "",
                AttackTextures = new List<string>()
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

        private void BrowseIdleTexture(object sender, RoutedEventArgs e)
        {
            var file = BrowseTextureFile();
            if (file == null)
                return;

            IdleTextureBox.Text = file;
            UpdatePreview(file, IdlePreview);
        }

        private void BrowseWalkTexture(object sender, RoutedEventArgs e)
        {
            var file = BrowseTextureFile();
            if (file == null)
                return;

            WalkTextureBox.Text = file;
            UpdatePreview(file, WalkPreview);
        }

        private void BrowseHurtTexture(object sender, RoutedEventArgs e)
        {
            var file = BrowseTextureFile();
            if (file == null)
                return;

            HurtTextureBox.Text = file;
            UpdatePreview(file, HurtPreview);
        }

        private void BrowseDeathTexture(object sender, RoutedEventArgs e)
        {
            var file = BrowseTextureFile();
            if (file == null)
                return;

            DeathTextureBox.Text = file;
            UpdatePreview(file, DeathPreview);
        }

        private void AddAttackTexture(object sender, RoutedEventArgs e)
        {
            if (_current == null)
                return;

            string file = BrowseTextureFile();

            _current.AttackTextures.Add(file);
            RefreshAttackTexturePreview();
        }

        private void RemoveAttackTexture(object sender, RoutedEventArgs e)
        {
            if (_current == null || _current.AttackTextures.Count == 0)
                return;

            _current.AttackTextures.RemoveAt(_current.AttackTextures.Count - 1);
            RefreshAttackTexturePreview();
        }

        #endregion

        #region Helpers

        private string BrowseTextureFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PNG Files (*.png)|*.png",
                InitialDirectory = EditorPaths.GetAssetsFolder()
            };

            if (dialog.ShowDialog() != true)
                return null;

            return Path.GetFileName(dialog.FileName);
        }

        private void UpdatePreview(string fileName, Image target)
        {
            try
            {
                string assetPath = EditorPaths.GetAssetsFolder();

                string targetPath = Path.Combine(assetPath, fileName);

                if (!File.Exists(targetPath))
                {
                    target.Source = null;
                    return;
                }

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(targetPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                target.Source = bmp;
            }
            catch 
            {
                target.Source = null;
            }
        }

        private void RefreshAttackTexturePreview()
        {
            if (_current == null)
                return;

            var images = _current.AttackTextures.Select(t => LoadAttackPreview(t)).ToList();

            AttackTextureList.ItemsSource = images;
        }

        private BitmapImage LoadAttackPreview(string fileName)
        {
            try
            {
                string assetPath = EditorPaths.GetAssetsFolder();

                string targetPath = Path.Combine(assetPath, fileName);

                if (!File.Exists(targetPath))
                    return null;

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(targetPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                return bmp;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
