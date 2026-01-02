using System.IO;

namespace TTEngine.Editor.Services
{
    public static class EditorPaths
    {
        private const string ENGINE_NAME = "TTEngine.exe";

        public static string GetProjectRoot()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            return Path.GetFullPath(Path.Combine(dir, "..", "..", "..", ".."));
        }

        public static string GetMapsFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Maps"));
        }

        public static string GetAssetsFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Textures"));
        }

        public static string GetDataFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Data"));
        }

        public static string GetEngineExe()
        {
            var root = GetProjectRoot();
            var enginePath = Path.Combine(root, "x64", "Debug");

            return Path.GetFullPath(Path.Combine(enginePath, ENGINE_NAME));
        }
    }
}
