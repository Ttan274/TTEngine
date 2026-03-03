using System.IO;
using TTEngine.Editor.Models.Asset;

namespace TTEngine.Editor.Services.Asset
{
    public static class AssetTreeBuilder
    {
        public static List<AssetNode> Build(string rootPath)
        {
            var result = new List<AssetNode>();

            if(!Directory.Exists(rootPath))
                return result;

            var rootDir = new DirectoryInfo(rootPath);

            foreach (var dir in rootDir.GetDirectories())
                result.Add(BuildDirectory(dir));

            foreach (var file in rootDir.GetFiles())
            {
                result.Add(new AssetNode
                {
                    Name = file.Name,
                    FullPath = file.FullName,
                    IsFolder = false,
                    Type = ResolveType(file),
                    IsSystemFile = CheckSystemFile(file.Name)
                });
            }

            return result;
        }

        //Helpers
        private static AssetNode BuildDirectory(DirectoryInfo dir)
        {
            var node = new AssetNode
            {
                Name = dir.Name,
                FullPath = dir.FullName,
                IsFolder = true,
                Type = FileExtension.Folder
            };

            foreach (var subdir in dir.GetDirectories())
                node.Children.Add(BuildDirectory(subdir));

            foreach (var file in dir.GetFiles())
            {
                node.Children.Add(new AssetNode
                {
                    Name = file.Name,
                    FullPath = file.FullName,
                    IsFolder = false,
                    Type = ResolveType(file),
                    IsSystemFile = CheckSystemFile(file.Name)
                });
            }

            return node;
        }

        private static FileExtension ResolveType(FileInfo file)
        {
            return file.Extension.ToLower() switch
            {
                ".json" => FileExtension.Json,
                ".png" => FileExtension.Texture,
                ".ttf" => FileExtension.Font,
                _ => FileExtension.Unknown
            };
        }

        private static bool CheckSystemFile(string fileName)
        {
            return fileName.ToLower() switch
            {
                "map_default.json" => true,
                "player.json" => true,
                "empty_tile.json" => true,
                _ => false
            };
        }
    }
}
