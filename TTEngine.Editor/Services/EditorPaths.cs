using System.IO;

namespace TTEngine.Editor.Services
{
    public static class EditorPaths
    {
        private const string ENGINE_NAME = "TTEngine.exe";

        //Root
        private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        public static string Root => ProjectRoot;

        //Asset Folders
        public static string Assets => Combine("Assets");

        //Helper
        private static string Combine(params string[] parts)
            => Path.GetFullPath(Path.Combine(new[] { Root }.Concat(parts).ToArray()));

        //Engine Exe
        public static string GetEngineExe()
        {
#if DEBUG
            return Path.GetFullPath(Path.Combine(Root, "x64", "Debug", ENGINE_NAME));
#else
            return Path.GetFullPath(Path.Combine(Root, "x64", "Release",  ENGINE_NAME));
#endif
        }
    }
}
