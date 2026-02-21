using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
