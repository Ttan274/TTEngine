using System.IO;
using System.Text.Json;
using TTEngine.Editor.Models.Animation;

namespace TTEngine.Editor.Services
{
    public static class AnimationDefinitionService
    {
        private static readonly Dictionary<string, AnimationDefinition> _cache = new();

        private static readonly string FilePath = EditorPaths.GetAnimationFolder();

        public static IReadOnlyCollection<AnimationDefinition> All
            => _cache.Values;

        public static AnimationDefinition Get(string id)
            => _cache.TryGetValue(id, out var anim) ? anim : null;

        public static void LoadAll()
        {
            _cache.Clear();

            foreach (var file in Directory.GetFiles(FilePath, "*.json"))
            {
                var json = File.ReadAllText(file);
                var anim = JsonSerializer.Deserialize<AnimationDefinition>(json);

                if (anim != null && !string.IsNullOrEmpty(anim.Id))
                    _cache[anim.Id] = anim;
            }
        }

        public static void Save(AnimationDefinition anim)
        {
            if (string.IsNullOrWhiteSpace(anim.Id))
                throw new InvalidOperationException("Animation id is empty");

            string path = GetPath(anim.Id);

            var json = JsonSerializer.Serialize(anim, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
            _cache[anim.Id] = anim;
        }

        public static AnimationDefinition Create(string baseId = "new_anim")
        {
            string id = GenerateUniqueId(baseId);

            var anim = new AnimationDefinition
            {
                Id = id,
                FrameWidth = 128,
                FrameHeight = 128,
                FrameCount = 1,
                FrameTime = 0.1f,
                Loop = true,
                EventFrames = new()
            };

            _cache[id] = anim;
            return anim;
        }

        public static void Delete(string id)
        {
            if (!_cache.ContainsKey(id))
                return;

            string path = GetPath(id);
            if(File.Exists(path))
                File.Delete(path);

            _cache.Remove(id);
        }

        //Helpers

        private static string GetPath(string id)
                => Path.Combine(FilePath, $"{id}.json");

        private static string GenerateUniqueId(string baseId)
        {
            string id = baseId;
            int i = 1;

            while (_cache.ContainsKey(id))
                id = $"{baseId}_{i++}";

            return id;
        }
    }
}
