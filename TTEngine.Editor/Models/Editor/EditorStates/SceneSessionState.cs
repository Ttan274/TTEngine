using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public class SceneSessionState : ObservableObject
    {
        private readonly SceneService _sceneService;
        public SceneService SceneService => _sceneService;

        public SceneSessionState(SceneService sceneService)
        {
            _sceneService = sceneService;
        }

        private TTEngine.Editor.Models.Scene.Scene _activeScene;
        public TTEngine.Editor.Models.Scene.Scene ActiveScene
        {
            get => _activeScene;
            set => SetProperty(ref _activeScene, value);
        }

        private string _activeSceneId;
        public string ActiveSceneId
        {
            get => _activeSceneId;
            set => SetProperty(ref _activeSceneId, value);
        }

        //Save & Load
        public void Save()
        {
            if (ActiveScene == null || string.IsNullOrEmpty(ActiveSceneId))
                return;

            _sceneService.Save(_activeSceneId, _activeScene);
        }

        public void Load(string sceneId)
        {
            ActiveScene = _sceneService.Load(sceneId);
            ActiveSceneId = sceneId;
        }

        //Create-Delete
        public void Create(string sceneId, int w = 50, int h = 30, int t = 50)
        {
            var scene = _sceneService.CreateScene(sceneId, w, h, t);

            ActiveScene = scene;
            ActiveSceneId = sceneId;
        }

        public void Delete(string sceneId)
        {
            if (GetAllScenes().Count <= 1)
                return;

            _sceneService.Delete(sceneId);

            if(ActiveSceneId == sceneId)
            {
                var next = GetAllScenes().FirstOrDefault();
                if (next != null)
                    Load(next);
            }
        }

        //Rename
        public void Rename(string oldId, string newId)
        {
            _sceneService.Rename(oldId, newId);

            if(ActiveSceneId == oldId)
            {
                ActiveSceneId = newId;
                ActiveScene.Id = newId;
            }
        }

        //List
        public List<string> GetAllScenes()
            => _sceneService.GetAllScenes();
    }
}
