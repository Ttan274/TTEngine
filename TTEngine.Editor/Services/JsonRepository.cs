namespace TTEngine.Editor.Services
{
    public class JsonRepository<T>
    {
        private readonly string _path;

        public JsonRepository(string path)
        {
            _path = path;
        }

        //List
        public List<T> GetAll()
        {
            return JsonFileService.Load<List<T>>(_path);
        }

        public void SaveAll(List<T> items)
        {
            JsonFileService.Save(_path, items);
        }

        //Single Item
        public T Get()
        {
            return JsonFileService.Load<T>(_path);
        }

        public void Save(T item)
        {
            JsonFileService.Save(_path, item);
        }
    }
}
