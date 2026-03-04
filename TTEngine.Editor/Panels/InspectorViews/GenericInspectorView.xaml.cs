using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Editor;
using TTEngine.Editor.Models.Selection;
using TTEngine.Editor.ViewModels.Panel;
using TTEngine.Editor.WindowPanels;

namespace TTEngine.Editor.Panels.InspectorViews
{
    /// <summary>
    /// Interaction logic for GenericInspectorView.xaml
    /// </summary>
    public partial class GenericInspectorView : UserControl
    {
        private EditorState Editor => Application.Current.MainWindow.DataContext as EditorState;

        public GenericInspectorView()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AssetSelectionViewModel selection)
            {
                selection.Save();
                MessageBox.Show("Saved Succesfully");
            }
        }

        private void AssetPicker_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is PropertyFieldViewModel field && field.AssetReference != null)
            {
                var picker = new AssetPickerWindow(Editor.Project, field.AssetReference.AssetType);

                if (picker.ShowDialog() == true)
                    field.StringValue = picker.SelectedAsset;
            }
        }
    }
}
