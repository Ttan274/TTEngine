using System.IO;
using System.IO.Compression;

namespace TTEngine.Editor.Services
{
    public static class BuildService
    {
        private static readonly string[] RuntimeDLLs =
        {
            "SDL3.dll",
            "SDL3_image.dll",
            "SDL3_ttf.dll"
        };

        public static void BuildGame(string targetFolder, string buildName = "TTGame")
        {
            string engineExe = EditorPaths.GetEngineExe();
            string engineDir = Path.GetDirectoryName(engineExe);
            string assetsDir = EditorPaths.GetAssetsFolder();

            if (!File.Exists(engineExe))
                throw new FileNotFoundException("Engine exe not found", engineExe);

            if (!Directory.Exists(assetsDir))
                throw new DirectoryNotFoundException("Assets folder not found");

            //Build Folder Struture
            string buildRoot = Path.Combine(targetFolder, buildName);
            string buildAssets = Path.Combine(buildRoot, "Assets");

            if(Directory.Exists(buildRoot))
                Directory.Delete(buildRoot, true);

            Directory.CreateDirectory(buildRoot);
            Directory.CreateDirectory(buildAssets);

            //Copy Exe
            File.Copy(engineExe, Path.Combine(buildRoot, $"{buildName}.exe"));

            //Copy DLLs
            CopyRuntimeDLLs(engineDir, buildRoot);

            //Copy Assets
            CopyDirectory(assetsDir, buildAssets);

            //MetaData
            string version = "1.0.0";
            WriteReadme(buildRoot, version);

            //Zip
            ZipBuild(buildRoot, version);
        }

        private static void CopyRuntimeDLLs(string engineDir, string buildRoot)
        {
            foreach (var dll in RuntimeDLLs)
            {
                string src = Path.Combine(engineDir, dll);
                string dst = Path.Combine(buildRoot, dll);

                if (!File.Exists(src))
                    throw new FileNotFoundException($"Missing runtime dll: {dll}");

                File.Copy(src, dst, true);
            }
        }

        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);

            foreach (var f in Directory.GetFiles(source))
                File.Copy(f, Path.Combine(target, Path.GetFileName(f)));

            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(target, Path.GetFileName(dir)));
        }

        private static void WriteReadme(string target, string version)
        {
            string path = Path.Combine(target, "README.txt");

            string content =    $@"TTGame Version: {version}
                                Build Date: {DateTime.Now:yyyy-MM-dd}

                                Controls:
                                - A / D   : Move
                                - W   : Jump
                                - F5       : Interact
                                - Esc     : Quit

                                Notes:
                                - Built with custom C++ engine
                                - SDL3 based
                                ";

            File.WriteAllText(path, content);
        }

        private static void ZipBuild(string buildRoot, string version)
        {
            string parentDir = Directory.GetParent(buildRoot)!.FullName;
            string zipName = $"TTGAME_{version}.zip";
            string zipPath = Path.Combine(parentDir, zipName);

            if(File.Exists(zipPath))
                File.Delete(zipPath);

            ZipFile.CreateFromDirectory(buildRoot, zipPath, CompressionLevel.Optimal, includeBaseDirectory: true);
        }
    }
}
