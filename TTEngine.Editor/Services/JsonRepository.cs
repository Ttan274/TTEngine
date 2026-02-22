using System.IO;

namespace TTEngine.Editor.Services
{
    public class JsonRepository<T>
    {
        private readonly string _path;

        public JsonRepository(string path)
        {
            _path = path;

            //if(!Directory.Exists(_path))
            //    Directory.CreateDirectory(_path);
        }

        public List<T> GetAll()
        {
            var files = Directory.GetFiles(_path, "*.json");

            var result = new List<T>();

            foreach(var file in files)
            {
                var item = JsonFileService.Load<T>(file);
                result.Add(item);
            }

            return result;
        }

        public T Get(string fileName)
        {
            string targetPath = Path.Combine(_path, $"{fileName}.json");
            return JsonFileService.Load<T>(targetPath);
        }

        public void Delete(string fileName)
        {
            string targetPath = Path.Combine(_path, $"{fileName}.json");
            if(File.Exists(targetPath))
                File.Delete(targetPath);
        }

        public void Save(T item, string fileName)
        {
            string targetPath = Path.Combine(_path, $"{fileName}.json");
            JsonFileService.Save(targetPath, item);
        }
    }
}
