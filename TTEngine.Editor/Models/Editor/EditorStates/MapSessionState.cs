using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public class MapSessionState : ObservableObject
    {
        public const string DEFAULT_MAP_ID = "Map_Default";

        private readonly MapService _mapService;
        public MapService MapService => _mapService;

        public MapSessionState(MapService mapService)
        {
            _mapService = mapService;
        }

        private TileMapModel _activeMap;
        public TileMapModel ActiveMap
        {
            get => _activeMap;
            set => SetProperty(ref _activeMap, value);
        }

        private string _activeMapId;
        public string ActiveMapId
        {
            get => _activeMapId;
            set => SetProperty(ref _activeMapId, value);
        }

        public bool IsDefaultMap 
            => ActiveMapId == DEFAULT_MAP_ID;

        public void Save()
        {
            if (IsDefaultMap || ActiveMap == null || string.IsNullOrEmpty(ActiveMapId))
                return;

            _mapService.Save(ActiveMapId, ActiveMap);
        }
    }
}
