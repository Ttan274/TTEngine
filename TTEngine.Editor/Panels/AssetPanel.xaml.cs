using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TTEngine.Editor.Models.Asset;
using TTEngine.Editor.Models.Component;
using TTEngine.Editor.Models.Definitions;
using TTEngine.Editor.Models.GameObject;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services.Asset;
using TTEngine.Editor.Services.IO;

namespace TTEngine.Editor.Panels
{
    /// <summary>
    /// Interaction logic for AssetPanel.xaml
    /// </summary>
    public partial class AssetPanel : UserControl
    {
        private readonly ProjectSession _session;
        private readonly AssetFileService _fileService;

        private ObservableCollection<AssetNode> _assetTree
            = new ObservableCollection<AssetNode>();

        public event Action<string> AssetCreated;
        public event Action<string> AssetOpened;

        public AssetPanel(ProjectSession session, AssetFileService fileService)
        {
            InitializeComponent();

            _session = session;
            _fileService = fileService;

            AssetTree.ItemsSource = _assetTree;
            AssetTree.PreviewMouseRightButtonDown += RightClicked;

            LoadTree();
        }

        private void LoadTree()
        {
            _assetTree.Clear();

            var nodes = AssetTreeBuilder.Build(_session.AssetsPath);

            foreach(var n in nodes)
                _assetTree.Add(n);
        }

        private void Refresh()
        {
            LoadTree();
        }

        //Double Click
        private void DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            if (node.IsFolder)
                return;

            if (!node.FullPath.EndsWith(".json"))   //Only open json files for now
                return;

            AssetOpened?.Invoke(node.FullPath);
        }

        //Right Click
        private void RightClicked(object sender, MouseButtonEventArgs e)
        {
            var item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if(item != null)
                item.IsSelected = true;
        }

        private static T VisualUpwardSearch<T>(DependencyObject? source) where T : DependencyObject
        {
            while(source != null && source is not T)
                source = VisualTreeHelper.GetParent(source);

            return source as T;
        }

        //Context Menu Actions      
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            if(menuItem.DataContext is not AssetNode node)
                return;

            if(node.IsSystemFile)
            {
                MessageBox.Show("Cannot delete system files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Delete '{node.Name}'?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            _fileService.Delete(node.FullPath);

            Refresh();
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem)
                return;

            if (menuItem.DataContext is not AssetNode node)
                return;

            if (node.IsSystemFile)
            {
                MessageBox.Show("Cannot rename system files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new InputDialog(node.Name);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() != true)
                return;

            string newName = dialog.Result;

            if (string.IsNullOrWhiteSpace(newName))
                return;

            _fileService.Rename(node.FullPath, newName);

            Refresh();
        }

        private void ShowInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            System.Diagnostics.Process.Start("explorer.exe", node.FullPath);
        }

        //Create Related Actions
        private void NewFolder_Click(object sender, RoutedEventArgs e)
        {
            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            _fileService.CreateFolder(GetTargetFolder(node), "NewFolder");

            Refresh();
        }

        private void CreateAnimation_Click(object sender, RoutedEventArgs e)
        {
            CreateJsonAsset<AnimationDefinition>("NewAnimation");
        }

        private void CreateTile_Click(object sender, RoutedEventArgs e)
        {
            CreateJsonAsset<TileDefinition>("NewTile");
        }

        private void CreateGameObject_Click(object sender, RoutedEventArgs e)
        {
            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            string parentPath = GetTargetFolder(node);

            string fileName = "NewGameObject.json";
            string fullPath = Path.Combine(parentPath, fileName);

            if (File.Exists(fullPath))
            {
                MessageBox.Show("File already exists");
                return;
            }

            var go = new GameObject
            {
                Id = "NewGameObject"
            };

            //Zorunlu olarak transform ekliyoruz
            go.Components.Add(new TransformComponent());

            JsonFileService.Save(fullPath, go);
            
            Refresh();

            AssetCreated?.Invoke(fullPath);
        }

        //Helpers
        private string GetTargetFolder(AssetNode node)
        {
            if(node.IsFolder)
                return node.FullPath;

            return Path.GetDirectoryName(node.FullPath);
        }

        private void CreateJsonAsset<T>(string defaultName) where T : new()
        {
            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            string parentPath = GetTargetFolder(node);

            string fileName = defaultName + ".json";
            string fullPath = Path.Combine(parentPath, fileName);

            if(File.Exists(fullPath))
            {
                MessageBox.Show("File already exists");
                return;
            }

            var newObject = new T();

            JsonFileService.Save(fullPath, newObject);

            Refresh();

            AssetCreated?.Invoke(fullPath);
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select file to add",
                IsFolderPicker = false,
                Multiselect = true
            };

            dialog.Filters.Add(new CommonFileDialogFilter("Json Files", "*.json"));

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            if (AssetTree.SelectedItem is not AssetNode node)
                return;

            string targetFolder = GetTargetFolder(node);
            int importedCount = 0;

            foreach (var file in dialog.FileNames)
            {
                string destPath = Path.Combine(targetFolder, Path.GetFileName(file));

                if (File.Exists(destPath))
                    continue;

                File.Copy(file, destPath);
                importedCount++;
            }

            Refresh();
            MessageBox.Show($"{importedCount} file(s) imported succesfully.");
        }
    }
}
