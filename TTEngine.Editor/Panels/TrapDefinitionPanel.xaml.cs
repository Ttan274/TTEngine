using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for TrapDefinitionPanel.xaml
    /// </summary>
    public partial class TrapDefinitionPanel : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<TrapDefinition> Traps { get; }
            = new ObservableCollection<TrapDefinition>();

        private TrapDefinition _selectedTrap;
        public TrapDefinition SelectedTrap
        {
            get => _selectedTrap;
            set
            {
                _selectedTrap = value;
                DataContext = null;
                DataContext = this;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFireTrap)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSawTrap)));

                UpdatePreview();
            }
        }

        public bool IsFireTrap => SelectedTrap?.Id == "Fire";
        public bool IsSawTrap => SelectedTrap?.Id == "Saw";

        public event PropertyChangedEventHandler PropertyChanged;

        public TrapDefinitionPanel()
        {
            InitializeComponent();
            LoadTraps();
            DataContext = this;
        }

        private void LoadTraps()
        {
            Traps.Clear();

            var list = TrapFileService.Load();
            foreach (var t in list)
                Traps.Add(t);

            SelectedTrap = Traps.FirstOrDefault();
        }

        private void UpdatePreview()
        {
            try
            {
                if(SelectedTrap == null || string.IsNullOrEmpty(SelectedTrap.ImagePath))
                {
                    PreviewImage.Source = null;
                    return;
                }

                string targetPath = Path.Combine(EditorPaths.GetTextureFolder(), SelectedTrap.ImagePath);

                if(!File.Exists(targetPath))
                {
                    PreviewImage.Source = null;
                    return;
                }

                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(targetPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                PreviewImage.Source = bmp;
            }
            catch 
            {
                PreviewImage.Source = null;
            }
        }

        //UI Buttons
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var trap = new TrapDefinition
            {
                Id = "new_trap",
                ImagePath = "",
                ActiveDuration = 1f,
                InactiveDuration = 1f,
                DamagePerSecond = 10f,
                Speed = 60f,
                Damage = 20f,
                DamageCooldown = 0.5f
            };

            Traps.Add(trap);
            SelectedTrap = trap;
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTrap == null)
                return;

            if (MessageBox.Show(
               $"Delete trap '{SelectedTrap.Id}'?",
               "Confirm",
               MessageBoxButton.YesNo,
               MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            Traps.Remove(SelectedTrap);
            SelectedTrap = Traps.FirstOrDefault();
        }

        private void BrowseImageClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTrap == null)
                return;

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = EditorPaths.GetTextureFolder()
            };

            if (dialog.ShowDialog() != true)
                return;

            SelectedTrap.ImagePath = Path.GetFileName(dialog.FileName);
            UpdatePreview();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if(Traps.Any(t => string.IsNullOrWhiteSpace(t.Id)))
            {
                MessageBox.Show("Trap id cannot be empty");
                return;
            }

            if(Traps.GroupBy(t => t.Id).Any(g => g.Count() > 1))
            {
                MessageBox.Show("Trap id must be unique");
                return;
            }

            TrapFileService.Save(Traps.ToList());
        }
    }
}
