using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models.Project;

namespace TTEngine.Editor.Services
{
    public class RecentProjectService
    {
        private readonly string _filePath;

        public RecentProjectService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "TTEngine");

            Directory.CreateDirectory(folder);

            _filePath = Path.Combine(folder, "RecentProjects.json");
        }

        public List<ProjectInfo> GetRecentProjects()
        {
            if(!File.Exists(_filePath))
                return new List<ProjectInfo>();

            var paths = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_filePath));

            var result = new List<ProjectInfo>();

            foreach (var path in paths)
            {
                string projectFile = Path.Combine(path, "Project.ttproj");

                if (!File.Exists(projectFile))
                    continue;

                try
                {
                    var metadata = JsonSerializer.Deserialize<ProjectMetadata>(File.ReadAllText(projectFile));

                    result.Add(new ProjectInfo
                    {
                        Name = metadata.Name,
                        Path = path
                    });
                }
                catch 
                {
                    //Ignoring broken projects
                }
            }

            return result;
        }

        private List<string> GetRecentPaths()
        {
            if(!File.Exists(_filePath))
                return new List<string>();

            return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_filePath)) ?? new List<string>();
        }

        public void AddRecentProject(string path)
        {
            var paths = GetRecentPaths();

            paths.Remove(path);
            paths.Insert(0, path);

            File.WriteAllText(_filePath, JsonSerializer.Serialize(paths, new JsonSerializerOptions { WriteIndented = true }));
        }

        public void RemoveRecent(string path)
        {
            var paths = GetRecentPaths();

            if(paths.Remove(path))
                File.WriteAllText(_filePath, JsonSerializer.Serialize(paths, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
