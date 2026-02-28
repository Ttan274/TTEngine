using System.IO;

namespace TTEngine.Editor.Models.Project
{
    public class ProjectSession
    {
        public string RootPath { get;}
        public ProjectMetadata MetaData { get;}

        public ProjectSession(string rootPath, ProjectMetadata metaData)
        {
            RootPath = rootPath;
            MetaData = metaData;
        }

        //Base folders
        public string AssetsPath => Path.Combine(RootPath, "Assets");
        public string BuildsPath => Path.Combine(RootPath, "Builds");
        public string ConfigsPath => Path.Combine(RootPath, "Config");
        
        //Asset folders
        public string AnimPath => Path.Combine(AssetsPath, "Animations");
        public string DataPath => Path.Combine(AssetsPath, "Data");
        public string FontsPath => Path.Combine(AssetsPath, "Fonts");
        public string TexturesPath => Path.Combine(AssetsPath, "Textures");
        public string ScenePath => Path.Combine(AssetsPath, "Scenes");

        //Sub Asset folders
        public string TileDefsPath => Path.Combine(DataPath, "Tiles");
        public string GameObjectDefs => Path.Combine(DataPath, "GameObjects");
    }
}
