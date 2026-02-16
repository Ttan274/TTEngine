namespace TTEngine.Editor.Services
{
    public class JsonRepository<T>
    {
        private readonly string _path;

        public JsonRepository(string path)
        {
            _path = path;
        }

        public List<T> GetAll()
        {
            return JsonFileService.Load<List<T>>(_path);
        }

        public void SaveAll(List<T> items)
        {
            JsonFileService.Save(_path, items);
        }
    }
}
