using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TTEngine.Editor.Models.Project;
using TTEngine.Editor.Services;

namespace TTEngine.Editor
{
    /// <summary>
    /// Interaction logic for ProjectHubWindow.xaml
    /// </summary>
    public partial class ProjectHubWindow : Window
    {
        private readonly ProjectService _projectService = new();
        private readonly RecentProjectService _recentProjectService = new();

        private ObservableCollection<ProjectInfo> _recentProjects = 
            new ObservableCollection<ProjectInfo>();

        public ProjectHubWindow()
        {
            InitializeComponent();

            RecentList.ItemsSource = _recentProjects;

            LoadRecentProjects();
        }

        private void LoadRecentProjects()
        {
            _recentProjects.Clear();

            foreach(var project in _recentProjectService.GetRecentProjects())
                _recentProjects.Add(project);
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateProjectWindow();
            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var session = _projectService.CreateProject(dialog.ProjectName,
                                                            dialog.SelectedPath);

                _recentProjectService.AddRecentProject(session.RootPath);

                LoadRecentProjects();

                OpenEditor(session);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Project Folder"
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            try
            {
                var session = _projectService.OpenProject(dialog.FileName);

                _recentProjectService.AddRecentProject(session.RootPath);

                LoadRecentProjects();

                OpenEditor(session);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (RecentList.SelectedItem is not ProjectInfo project)
                return;

            if (MessageBox.Show("Delete recent project?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            _projectService.DeleteProject(project.Path);
            _recentProjectService.RemoveRecent(project.Path);

            _recentProjects.Remove(project);
        }

        private void RecentList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RecentList.SelectedItem is not ProjectInfo project)
                return;

            try
            {
                var session = _projectService.OpenProject(project.Path);

                _recentProjectService.AddRecentProject(project.Path);

                OpenEditor(session);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowInExplorer_Click(object sender, RoutedEventArgs e)
        {
            if (RecentList.SelectedItem is not ProjectInfo project)
                return;

            if(!Directory.Exists(project.Path))
            {
                MessageBox.Show("Project folder not found.");
                return;
            }

            System.Diagnostics.Process.Start("explorer.exe", project.Path);
        }

        private void RemoveProject_Click(object sender, RoutedEventArgs e)
        {
            if (RecentList.SelectedItem is not ProjectInfo project)
                return;

            _recentProjectService.RemoveRecent(project.Path);

            _recentProjects.Remove(project);
        }

        private void OpenEditor(ProjectSession session)
        {
            EditorLaunchService.Launch(session);
            Close();
        }
    }
}
