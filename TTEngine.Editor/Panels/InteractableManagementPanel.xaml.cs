using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for InteractableManagementPanel.xaml
    /// </summary>
    public partial class InteractableManagementPanel : UserControl
    {
        public ObservableCollection<InteractableDefinition> Interactables { get; }
            = new ObservableCollection<InteractableDefinition>();

        private InteractableDefinition _selectedInteractable;
        public InteractableDefinition SelectedInteractable
        {
            get => _selectedInteractable;
            set
            {
                _selectedInteractable = value;
                DataContext = null;
                DataContext = this;
                UpdatePreview();
            }
        }

        public InteractableManagementPanel()
        {
            InitializeComponent();
            LoadInteractables();
            DataContext = this;
        }

        private void LoadInteractables()
        {
            Interactables.Clear();

            var list = InteractableFileService.Load();
            foreach (var i in list)
                Interactables.Add(i);

            SelectedInteractable = Interactables.FirstOrDefault();
        }

        private void UpdatePreview()
        {
            try
            {
                if (SelectedInteractable == null)
                    return;

                string targetPath = Path.Combine(EditorPaths.GetTextureFolder(), SelectedInteractable.ImagePath);
            
                if(!File.Exists(targetPath))
                {
                    PreviewImage.Source = null;
                    SelectedInteractable.ImagePath = "";
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

        //Button Events
        private void AddClick(object sender, RoutedEventArgs e)
        {
            var newInteractable = new InteractableDefinition
            {
                Id = "newId",
                Type = "newType",
                ImagePath = ""
            };

            Interactables.Add(newInteractable);
            SelectedInteractable = newInteractable;
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (SelectedInteractable == null)
                return;

            if (MessageBox.Show(
                $"Delete interactable '{SelectedInteractable.Id}'?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            Interactables.Remove(SelectedInteractable);
            SelectedInteractable = Interactables.FirstOrDefault();
        }

        private void BrowseImageClick(object sender, RoutedEventArgs e)
        {
            if (SelectedInteractable == null)
                return;

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg",
                InitialDirectory = EditorPaths.GetTextureFolder()
            };

            if (dialog.ShowDialog() != true)
                return;

            SelectedInteractable.ImagePath = Path.GetFileName(dialog.FileName);
            UpdatePreview();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if(Interactables.Any(i => string.IsNullOrWhiteSpace(i.Id)))
            {
                MessageBox.Show("Interactable Id cannot be empty");
                return;
            }

            if(Interactables.GroupBy(i => i.Id).Any(g => g.Count() > 1))
            {
                MessageBox.Show("Interactable Id must be unique");
                return;
            }

            InteractableFileService.Save(Interactables.ToList());
        }
    }
}
