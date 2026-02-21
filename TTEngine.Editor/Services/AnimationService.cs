using System.IO;
using TTEngine.Editor.Models.Animation;

namespace TTEngine.Editor.Services
{
    public class AnimationService
    {
        private readonly Dictionary<string, AnimationDefinition> _cache = new();
        private readonly string _folder;
        public IReadOnlyCollection<AnimationDefinition> All
            => _cache.Values;

        public AnimationService(string folder)
        {
            _folder = folder;
        }

        public void LoadAll()
        {
            _cache.Clear();
            if (!Directory.Exists(_folder))
                return;

            foreach (var file in Directory.GetFiles(_folder, "*.json"))
            {
                var repo = new JsonRepository<AnimationDefinition>(file);
                AnimationDefinition anim = null; //repo.Get();

                if (anim != null && !string.IsNullOrEmpty(anim.Id))
                    _cache[anim.Id] = anim;
            }
        }

        public void Save(AnimationDefinition def)
        {
            if (string.IsNullOrWhiteSpace(def.Id))
                throw new InvalidOperationException("Animation id is empty");

            var repo = new JsonRepository<AnimationDefinition>(GetPath(def.Id));
        
            //repo.Save(def);
            _cache[def.Id] = def;
        }

        public AnimationDefinition Get(string id)
            => _cache.TryGetValue(id, out var anim) ? anim : null;
    
        //Helper
        public string GetPath(string id) => Path.Combine(_folder, $"{id}.json");
    
        public string GenerateUniqueId(string baseId = "new_anim")
        {
            string id = baseId;
            int counter = 1;
            while (_cache.ContainsKey(id))
            {
                id = $"{baseId}_{counter}";
                counter++;
            }
            return id;
        }
    }
}
