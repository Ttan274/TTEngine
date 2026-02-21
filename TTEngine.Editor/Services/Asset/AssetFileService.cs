using System.IO;

namespace TTEngine.Editor.Services.Asset
{
    public class AssetFileService
    {
        public bool Exists(string path) => Directory.Exists(path) || File.Exists(path);

        public void CreateFolder(string parentPath, string folderName)
        {
            string fullPath = Path.Combine(parentPath, folderName);

            if(!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
        }

        public void Delete(string path)
        {
            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return;
            }

            if(File.Exists(path))
                File.Delete(path);
        }

        public void Rename(string path, string newName)
        {
            string parent = Path.GetDirectoryName(path);
            string newPath = Path.Combine(parent, newName);

            if (Directory.Exists(path))
            {
                Directory.Move(path, newPath);
                return;
            }

            if (File.Exists(path))
                File.Move(path, newPath);
        }

        public void Move(string source, string target)
        {
            string fileName = Path.GetFileName(source);
            string newPath = Path.Combine(target, fileName);

            if (Directory.Exists(source))
            {
                Directory.Move(source, newPath);
                return;
            }

            if (File.Exists(source))
                File.Move(source, newPath);
        }
    }
}
