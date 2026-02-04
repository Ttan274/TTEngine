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

            return Path.GetFullPath(Path.Combine(root, "Assets"));
        }

        public static string GetTextureFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Textures"));
        }

        public static string GetDataFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Data"));
        }

        public static string GetAnimationFolder()
        {
            var root = GetProjectRoot();

            return Path.GetFullPath(Path.Combine(root, "Assets", "Animation"));
        }

        public static string GetEngineExe()
        {
#if DEBUG
            return Path.GetFullPath(Path.Combine(GetEngineExeBase(), "Debug", ENGINE_NAME));
#else
            return Path.GetFullPath(Path.Combine(GetEngineExeBase(), "Release",  ENGINE_NAME));
#endif
        }

        private static string GetEngineExeBase()
        {
            var root = GetProjectRoot();
            return Path.Combine(root, "x64");
        }
    }
}
