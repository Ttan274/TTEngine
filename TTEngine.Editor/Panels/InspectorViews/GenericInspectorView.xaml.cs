using System.Windows;
using System.Windows.Controls;
using TTEngine.Editor.Models.Selection;

namespace TTEngine.Editor.Panels.InspectorViews
{
    /// <summary>
    /// Interaction logic for GenericInspectorView.xaml
    /// </summary>
    public partial class GenericInspectorView : UserControl
    {
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
    }
}
