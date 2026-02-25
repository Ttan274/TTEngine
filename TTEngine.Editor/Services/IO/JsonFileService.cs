using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TTEngine.Editor.Services.IO
{
    public static class JsonFileService
    {
        private static readonly JsonSerializerOptions _options =
            new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };

        public static T Load<T>(string path)
        {
            if(!File.Exists(path))
                throw new FileNotFoundException($"The file at path '{path}' was not found.");

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public static void Save<T>(string path, T data)
        {
            var json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }
    }
}
