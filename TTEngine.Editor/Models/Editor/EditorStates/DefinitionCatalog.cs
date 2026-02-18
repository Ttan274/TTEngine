using System.Collections.ObjectModel;
using TTEngine.Editor.Models.Entity;
using TTEngine.Editor.Models.Interactable;
using TTEngine.Editor.Models.Tile;
using TTEngine.Editor.Models.Trap;
using TTEngine.Editor.Services;

namespace TTEngine.Editor.Models.Editor.EditorStates
{
    public class DefinitionCatalog
    {
        public JsonRepository<EntityDefinitionModel> EntityRepository { get; }
        public JsonRepository<TileDefinition> TileRepository { get; }
        public JsonRepository<InteractableDefinition> InteractableRepository { get; }
        public JsonRepository<TrapDefinition> TrapRepository { get; }

        public ObservableCollection<TileDefinition> TileDefinitions { get; }
        public ObservableCollection<InteractableDefinition> InteractableDefinitions { get; }
        public ObservableCollection<TrapDefinition> TrapDefinitions { get; }
        public ObservableCollection<EntityDefinitionModel> EntityDefinitions { get; }

        public DefinitionCatalog(
            JsonRepository<EntityDefinitionModel> entityRepo,
            JsonRepository<TileDefinition> tileRepo,
            JsonRepository<InteractableDefinition> intRepo,
            JsonRepository<TrapDefinition> trapRepo
            )
        {
            //Repository assignment
            EntityRepository = entityRepo;
            TileRepository = tileRepo;
            InteractableRepository = intRepo;
            TrapRepository = trapRepo;

            //Load Definitions
            EntityDefinitions = new ObservableCollection<EntityDefinitionModel>(entityRepo.GetAll());
            TileDefinitions = new ObservableCollection<TileDefinition>(tileRepo.GetAll());
            InteractableDefinitions = new ObservableCollection<InteractableDefinition>(intRepo.GetAll());
            TrapDefinitions = new ObservableCollection<TrapDefinition>(trapRepo.GetAll());
        }
    }
}
