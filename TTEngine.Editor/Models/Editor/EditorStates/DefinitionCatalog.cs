using System.Collections.ObjectModel;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Services.IO;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public class DefinitionCatalog
    {
        public JsonRepository<TileDefinition> TileRepository { get; }
        public JsonRepository<GameObject.GameObject> GameObjectRepository { get; }

        public ObservableCollection<TileDefinition> TileDefinitions { get; }
        public ObservableCollection<GameObject.GameObject> GameObjects { get; }

        public DefinitionCatalog(
            JsonRepository<TileDefinition> tileRepo,
            JsonRepository<GameObject.GameObject> gameRepo
            )
        {
            //Repository assignment
            TileRepository = tileRepo;
            GameObjectRepository = gameRepo;

            //Load Definitions
            TileDefinitions = new ObservableCollection<TileDefinition>(tileRepo.GetAll());
            GameObjects = new ObservableCollection<GameObject.GameObject>(gameRepo.GetAll());
        }
    }
}
