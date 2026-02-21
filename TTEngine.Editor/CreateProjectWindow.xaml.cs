using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public string ProjectName => NameBox.Text;
        public string SelectedPath => PathBox.Text;

        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Project Location"
            };

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
                PathBox.Text = dialog.FileName;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ProjectName) ||
               string.IsNullOrWhiteSpace(SelectedPath))
            {
                MessageBox.Show("Please enter name and select folder location");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
