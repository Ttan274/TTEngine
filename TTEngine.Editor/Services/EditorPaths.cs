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
        public static string Data => Combine("Assets", "Data");
        public static string Maps => Combine("Assets", "Maps");
        public static string Textures => Combine("Assets", "Textures");
        public static string Animation => Combine("Assets", "Animation");

        //Data Files
        public static string EntityDefs => Path.Combine(Data, "entity_def.json");
        public static string TileDefs => Path.Combine(Data, "tile_def.json");
        public static string InteractableDefs => Path.Combine(Data, "Interactables.json");
        public static string TrapDefs => Path.Combine(Data, "TrapDef.json");
        public static string LevelDefs => Path.Combine(Data, "Levels.json");

        //Helper
        private static string Combine(params string[] parts)
            => Path.GetFullPath(Path.Combine(new[] { Root }.Concat(parts).ToArray()));

        //Map Helper
        public static string GetMapPath(string mapId)
            => Path.Combine(Maps, $"{mapId}.json");

        //will be deleted 
        public static string GetTextureFolder()
        {
            return Path.GetFullPath(Path.Combine(Root, "Assets", "Textures"));
        }

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
