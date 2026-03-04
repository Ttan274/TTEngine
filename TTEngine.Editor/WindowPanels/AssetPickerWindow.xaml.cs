using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TTEngine.Editor.Models.Asset;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.WindowPanels
{
    /// <summary>
    /// Interaction logic for AssetPickerWindow.xaml
    /// </summary>
    public partial class AssetPickerWindow : Window
    {
        private readonly ProjectSession _session;
        private readonly AssetType _type;
        public string SelectedAsset { get; private set; }

        public AssetPickerWindow(ProjectSession session, AssetType type)
        {
            InitializeComponent();
            _type = type;
            _session = session;
            LoadAssets();
        }

        private void LoadAssets()
        {
            //BU konumlar yanlış projeye özgü hale getirmemiz lazım
            string folder = _type switch
            {
                AssetType.Animation => _session.AnimationsPath,
                AssetType.Animator => _session.AnimatorsPath,
                AssetType.Texture => _session.TexturesPath,
                _ => null
            };

            if (folder == null || !Directory.Exists(folder))
                return;

            var files = Directory.GetFiles(folder);

            AssetList.ItemsSource = files.Select(f => new AssetItem
                                    {
                                        FullPath = f,
                                        FileName = System.IO.Path.GetFileName(f)
                                    }).ToList();
        }

        private void OnAssetClicked(object sender, MouseButtonEventArgs e)
        {
            if(sender is Border border && border.DataContext is AssetItem item)
            {
                SelectedAsset = item.FileName;
                DialogResult = true;
                Close();
            }
        }
        
        private void AssetImageLoaded(object sender, RoutedEventArgs e)
        {
            if (_type != AssetType.Texture)
                return;

            if(sender is Image img && img.DataContext is AssetItem item && File.Exists(item.FullPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(item.FullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                img.Source = bitmap;
            }
        }
    }
}
