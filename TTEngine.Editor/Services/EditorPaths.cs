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


        //will be deleted from here 
        public static string GetMapsFolder()
        {
            return Path.GetFullPath(Path.Combine(Root, "Assets", "Maps"));
        }

        public static string GetTextureFolder()
        {
            return Path.GetFullPath(Path.Combine(Root, "Assets", "Textures"));
        }

        public static string GetDataFolder()
        {
            return Path.GetFullPath(Path.Combine(Root, "Assets", "Data"));
        }
        //will be deleted from here to here

        //Engine Exe
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
            return Path.Combine(Root, "x64");
        }
    }
}
