using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models.Project;

namespace TTEngine.Editor.Services.Project
{
    public class ProjectService
    {
        private const string ProjectFileName = "Project.ttproj";
        private const string EngineVers = "1.0.0";

        public ProjectSession CreateProject(string name, string parentPath)
        {
            string projectRoot = Path.Combine(parentPath, name);

            if (Directory.Exists(projectRoot))
                throw new Exception("Project folder already exists");

            Directory.CreateDirectory(projectRoot);
            CreateTemplateFolders(projectRoot);

            var metadata = new ProjectMetadata
            {
                Name = name,
                EngineVersion = EngineVers,
                CreatedAt = DateTime.UtcNow
            };

            string projectFilePath = Path.Combine(projectRoot, ProjectFileName);
            File.WriteAllText(projectFilePath,
                JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));

            return new ProjectSession(projectRoot, metadata);
        }

        public ProjectSession OpenProject(string projectRoot)
        {
            string projectFilePath = Path.Combine(projectRoot, ProjectFileName);

            if (!File.Exists(projectFilePath))
                throw new Exception("Invalid project folder");

            var metadata = JsonSerializer.Deserialize<ProjectMetadata>(File.ReadAllText(projectFilePath));
            metadata.LastOpened = DateTime.UtcNow;

            File.WriteAllText(projectFilePath,
                JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));

            return new ProjectSession(projectRoot, metadata);
        }

        public void DeleteProject(string projectRoot)
        {
            if (Directory.Exists(projectRoot))
                Directory.Delete(projectRoot, true);
        }

        //Helper
        private void CreateTemplateFolders(string root)
        {
            //Base folders
            string assetsRoot = Path.Combine(root, "Assets");
            Directory.CreateDirectory(assetsRoot);
            Directory.CreateDirectory(Path.Combine(root, "Builds"));
            Directory.CreateDirectory(Path.Combine(root, "Config"));

            //Asset folders
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Animations"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Data"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Fonts"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Textures"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Scenes"));

            //Sub Asset folders
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Data", "Entities"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Data", "Tiles"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Data", "Interactables"));
            Directory.CreateDirectory(Path.Combine(assetsRoot, "Data", "Traps"));
        }
    }
}
